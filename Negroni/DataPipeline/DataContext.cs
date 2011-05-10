/* *********************************************************************
   Copyright 2009-2010 MySpace

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
********************************************************************* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using System.Text;
using System.IO;
using Negroni.DataPipeline.Security;
using Negroni.OpenSocial.EL;
using System.Text.RegularExpressions;

namespace Negroni.DataPipeline
{

	/// <summary>
	/// Baseline implementation for OSML of the DataContext.  
	/// This acts as a complex nested dictionary object for holding and retrieving data 
	/// that will be used in processing an OSML Gadget.
	/// </summary>
	/// <remarks>
	/// To manage recursive references, this is not a thread-safe object.
	/// The internal counter currentCalculateReferenceDepth is global to the instance.
	/// If multi-threaded under heavy load, this could cause values to come up null
	/// unexpectedly by failing too early or trap the code in infinite recursion by
	/// resetting the counter too often.
	/// </remarks>
	public class DataContext : IObjectResolver, IDisposable 
	{
#if DEBUG
		public string myId = Guid.NewGuid().ToString();
#endif

		public const string VARIABLE_START = "${";
		public const string VARIABLE_END = "}";

		public const string LEGACY_MSGVARIABLE_START = "__MSG_";
		public const string LEGACY_MSGVARIABLE_END = "__";

		/// <summary>
		/// Maximum number of recursions allowed before dumping out
		/// </summary>
		const int MAX_REFERENCE_DEPTH = 250;

		/// <summary>
		/// Number of times CalculateVariableValue has been invoked in a resolution request.
		/// If this exceeds MAX_REFERENCE_DEPTH the code should return null and exit
		/// </summary>
		private int currentCalculateReferenceDepth = 0;

		/// <summary>
		/// Number of times ResolveVariables has been invoked in a resolution request.
		/// If this exceeds MAX_REFERENCE_DEPTH the code should return null and exit
		/// </summary>
		private int currentResolveReferenceDepth = 0;

		/// <summary>
		/// JSON format error message for key not found error
		/// </summary>
		const string JSONERR_KEY_NOT_FOUND = "{error: {message:'Requested key not found'}}";

		/// <summary>
		/// Key to the message bundles.
		/// </summary>
		public const string RESERVED_KEY_MESSAGE = "Msg";

		/// <summary>
		/// Key to parent objects
		/// </summary>
		public const string RESERVED_KEY_PARENT = "Parent";

		/// <summary>
		/// Key to hidden fetched markup from external sources.
		/// </summary>
		public const string RESERVED_KEY_FETCHED_MARKUP = "FetchedMarkup";


		private List<string> _inferredLocalVariableKeyStack = null;

		/// <summary>
		/// Local variable keys added to the scope.  This
		/// is used for resolving variables within templates
		/// and loops where the explicit object reference is not provided.
		/// </summary>
		internal List<string> InferredLocalVariableKeyStack
		{
			get
			{
				if (_inferredLocalVariableKeyStack == null)
				{
					_inferredLocalVariableKeyStack = new List<string>();
				}
				return _inferredLocalVariableKeyStack;
			}
		}


		/// <summary>
		/// Initializes the data context and adds the base MessageBundle catalog
		/// </summary>
		public DataContext() {
			_masterData = new Dictionary<string, DataItem>(120);
			RegisterDataItem(RESERVED_KEY_MESSAGE, new ResourceBundleCatalog());
			if(_masterData.ContainsKey(RESERVED_KEY_MESSAGE)){
				_masterData[RESERVED_KEY_MESSAGE].ExcludeFromClientContext = true;
			}
		}

		private DataContextSettings _settings = null;

		/// <summary>
		/// Settings for specific variable escaping and legacy support.
		/// </summary>
		public DataContextSettings Settings
		{
			get
			{
				if (_settings == null)
				{
					_settings = new DataContextSettings();
				}
				return _settings;
			}
		}

		/// <summary>
		/// Gets or sets the active view to utilize for resolving key values.
		/// </summary>
		/// <remarks>This is required to evaluate when the same key is used across
		/// multiple views with different declarations
		/// </remarks>
		public string ActiveViewScope { get; set; }



		private DataRequestContext _requestContext = null;
		
		/// <summary>
		/// RequestContext holds information specific to this request
		/// such as current user and current culture
		/// </summary>
		public DataRequestContext RequestContext
		{
			get {
				if (null == _requestContext)
				{
					_requestContext = new DataRequestContext();
				}				
				return _requestContext; 
			}
			set { _requestContext = value; }
		}

		#region MessageBundle resource support

		private string _cultureString = null;
		/// <summary>
		/// Culture to render under.  Defaults to US engrish
		/// </summary>
		public string Culture
		{
			get
			{
				if (_cultureString == null)
				{
					_cultureString = "en-US";
				}
				return _cultureString;
			}
			set { 
				_cultureString = value;
				ResourceStringCatalog.PreferredCulture = value;
			}
		}


		private ResourceBundleCatalog _resourceStringCatalog = null;

		/// <summary>
		/// Accessor for resourceStringCatalog.
		/// Performs lazy load upon first request
		/// </summary>
		public ResourceBundleCatalog ResourceStringCatalog
		{
			get
			{
				if (_resourceStringCatalog == null)
				{
					object obj = GetVariableObject(RESERVED_KEY_MESSAGE);
					_resourceStringCatalog  = (ResourceBundleCatalog) obj;
				}
				return _resourceStringCatalog;
			}
			private set
			{
				_resourceStringCatalog = value;
			}
		}

		/// <summary>
		/// Adds a new IResourceBundle to the catalog
		/// </summary>
		/// <param name="bundle"></param>
        public void AddResourceBundle(IResourceBundle bundle)
        {
            ResourceStringCatalog.AddBundle(bundle);
		}


		#endregion

		/// <summary>
		/// Resolve all EL variables in the source string with the given dataContext
		/// </summary>
		/// <param name="source"></param>
		/// <param name="dataContext"></param>
		/// <returns></returns>
		public string ResolveVariables(string source)
		{
			return ResolveVariables(source, null);
		}

		/// <summary>
		/// Resolve any EL variables that are included in <paramref name="variableKeys"/>
		/// </summary>
		/// <param name="source"></param>
		/// <param name="variableKeys">An array of root-level keys. Use "*" or null to denote all keys</param>
		/// <returns></returns>
		public string ResolveVariables(string source, string[] variableKeys)
		{
			if (String.IsNullOrEmpty(source)) return String.Empty;
			if (this.MasterData.Count == 0) return source;


			if (source.IndexOf(DataContext.VARIABLE_START) == -1)
			{
				//legacy message bundle support
				if (Settings.SupportLegacyMessageVariables && source != null && source.Contains(LEGACY_MSGVARIABLE_START))
				{
					return ResolveLegacyMessageBundleVariables(source);
				}
				else
				{
					return source;
				}
			}
			int lastWrittenPos = 0,
				currentVarPos = 0;

			StringBuilder result = new StringBuilder();
			while (-1 < (currentVarPos = source.IndexOf(DataContext.VARIABLE_START, currentVarPos)))
			{
				//static at beginning or malformed
				if (lastWrittenPos < currentVarPos)
				{
					result.Append(source.Substring(lastWrittenPos, currentVarPos - lastWrittenPos));
				}

				int endPos = source.IndexOf(DataContext.VARIABLE_END, currentVarPos);
				if (endPos == -1)
				{
					//move ahead one, then continue
					currentVarPos = Math.Min(currentVarPos + 1, source.Length - 1);
					continue;
				}
				else
				{
					//write resolved variable
					string currentKey = source.Substring(currentVarPos, (endPos - currentVarPos) + 1);
					string varExpression = DataContext.GetVariableExpression(currentKey);
					bool resolveVariable = false;
					if (null == variableKeys || variableKeys.Length == 0 || variableKeys[0] == "*")
					{
						resolveVariable = true;
					}
					else
					{
						string expRoot;
						if (varExpression.Contains("."))
						{
							expRoot = varExpression.Substring(0, varExpression.IndexOf("."));
						}
						else
						{
							expRoot = varExpression;
						}
						for (int i = 0; i < variableKeys.Length; i++)
						{
							if (expRoot.Equals(variableKeys[i], StringComparison.InvariantCultureIgnoreCase)
								|| variableKeys[i] == "*")
							{
								resolveVariable = true;
								break;
							}
						}
					}

					if (resolveVariable)
					{
						if (currentResolveReferenceDepth++ > MAX_REFERENCE_DEPTH)
						{
							currentResolveReferenceDepth = 0;
							LogCircularKeyReferenceError(varExpression);
							return null;
						}

						string curVal = this.CalculateVariableValue(varExpression);
						if (!string.IsNullOrEmpty(curVal))
						{
							if (curVal.Contains(DataContext.VARIABLE_START))
							{
								//handle recursive processing
								curVal = ResolveVariables(curVal, variableKeys);
							}
							result.Append(curVal);
						}
					}
					else
					{
						result.Append(currentKey);
					}
					lastWrittenPos = endPos + 1;
					currentVarPos = endPos;
				}
			}
			// Final static block
			if (lastWrittenPos < source.Length - 1)
			{
				result.Append(source.Substring(lastWrittenPos, source.Length - lastWrittenPos));
			}

			string finalString = result.ToString();

			if (!finalString.Contains(VARIABLE_START))
			{
				currentResolveReferenceDepth = 0;
			}

			//legacy message bundle support
			if (Settings.SupportLegacyMessageVariables && finalString != null && finalString.Contains(LEGACY_MSGVARIABLE_START))
			{
				finalString = ResolveLegacyMessageBundleVariables(finalString);
			}

			return finalString;
		}


		/// <summary>
		/// Resolve message bundle variables in legacy __MSG_somthing__ format
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		protected string ResolveLegacyMessageBundleVariables(string source)
		{
			if (!Settings.SupportLegacyMessageVariables || string.IsNullOrEmpty(source))
			{
				return source;
			}

			int startVarLen = DataContext.LEGACY_MSGVARIABLE_START.Length;
			int endVarLen = DataContext.LEGACY_MSGVARIABLE_END.Length;

			if (!source.Contains(LEGACY_MSGVARIABLE_START))
			{
				return source;
			}

			int lastWrittenPos = 0,
				currentVarPos = 0;

			StringBuilder result = new StringBuilder();
			while (-1 < (currentVarPos = source.IndexOf(DataContext.LEGACY_MSGVARIABLE_START, currentVarPos)))
			{
				//static at beginning or malformed
				if (lastWrittenPos < currentVarPos)
				{
					result.Append(source.Substring(lastWrittenPos, currentVarPos - lastWrittenPos));
				}

				int endPos = source.IndexOf(DataContext.LEGACY_MSGVARIABLE_END, currentVarPos + startVarLen);
				if (endPos == -1)
				{
					//move ahead, then continue
					currentVarPos = Math.Min(currentVarPos + startVarLen, source.Length - 1);
					continue;
				}
				else
				{
					//write resolved variable
					string currentKey = source.Substring(currentVarPos, (endPos - currentVarPos) + endVarLen);

					string varExpression = currentKey.Replace(LEGACY_MSGVARIABLE_START, VARIABLE_START + RESERVED_KEY_MESSAGE + ".");
					if (varExpression.EndsWith(LEGACY_MSGVARIABLE_END))
					{
						varExpression = varExpression.Substring(0, varExpression.LastIndexOf(LEGACY_MSGVARIABLE_END)) + VARIABLE_END;
					}
					bool resolveVariable = true;

					if (resolveVariable)
					{
						if (currentResolveReferenceDepth++ > MAX_REFERENCE_DEPTH)
						{
							currentResolveReferenceDepth = 0;
							LogCircularKeyReferenceError(varExpression);
							return null;
						}

						string curVal = this.CalculateVariableValue(varExpression);
						if (!string.IsNullOrEmpty(curVal))
						{
							if (curVal.Contains(DataContext.LEGACY_MSGVARIABLE_START))
							{
								//handle recursive processing
								curVal = ResolveLegacyMessageBundleVariables(curVal);
							}
							result.Append(curVal);
						}
					}
					else
					{
						result.Append(currentKey);
					}
					lastWrittenPos = endPos + endVarLen;
					currentVarPos = endPos;
				}
			}
			// Final static block
			if (lastWrittenPos < source.Length - 1)
			{
				result.Append(source.Substring(lastWrittenPos, source.Length - lastWrittenPos));
			}

			string finalString = result.ToString();

			if (!finalString.Contains(LEGACY_MSGVARIABLE_START))
			{
				currentResolveReferenceDepth = 0;
			}
			return finalString;
		}



		/// <summary>
		/// Resolve all variables in the source string with the given dataContext
		/// </summary>
		/// <param name="source"></param>
		/// <param name="dataContext"></param>
		/// <returns></returns>
		public string ResolveMessageBundleVariables(string source, string culture)
		{
			string currentCulture = Culture;
			if (!string.IsNullOrEmpty(culture))
			{
				Culture = culture;
			}

			string result = ResolveVariables(source, new string[] { DataContext.RESERVED_KEY_MESSAGE });

			if (!string.IsNullOrEmpty(culture) && !string.IsNullOrEmpty(currentCulture))
			{
				Culture = currentCulture;
			}
			return result;
		}


		/// <summary>
		/// Application and viewer-specific parameters
		/// </summary>
		[Obsolete("App parameters must be registered as a reserved data key")]
		public NameValueCollection ApplicationParameters { get; set; }


		private Dictionary<string, DataItem> _masterData;

		/// <summary>
		/// Master data hash holding all data items loaded for the entire DataContext
		/// </summary>
		public Dictionary<string, DataItem> MasterData
		{
			get
			{
				return _masterData;
			}
		}

		private static readonly Regex variableParserRegex = new Regex(
				"(\\${.*?})",
			 RegexOptions.CultureInvariant
			 | RegexOptions.Compiled
			 );


		/// <summary>
		/// Internal HTML encoding of a string for security policy.
		/// Done here to reduce dependencies.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private static string HtmlEncodeInteral(string source){
			if (string.IsNullOrEmpty(source))
			{
				return source;
			}
			if (!source.Contains("<") && !source.Contains(">")
				&& !source.Contains("\""))
			{
				return source;
			}
			else
			{
				return source.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
			}
		}


		/// <summary>
		/// Calculates a variable with an expression inside.
		/// This is the main entry point for resolving expressions
		/// </summary>
		/// <param name="var"></param>
		/// <returns></returns>
		public string CalculateVariableValue(string var){
			object val = CalculateVariableValue(var, new List<string>());
			if (val == null)
			{
				return null;
			}
			else
			{
				if (Settings.SecurityPolicy.EL_Escaping == EL_Escaping.None)
				{
					return val.ToString();
				}
				else //EL_Escaping.Html
				{
					return HtmlEncodeInteral(val.ToString());
				}
			}
		}

		/// <summary>
		/// Calculates a variable with an expression inside.
		/// This is the main entry point for resolving expressions
		/// </summary>
		/// <param name="var"></param>
		/// <returns></returns>
		public object CalculateVariableObjectValue(string var)
		{
			return CalculateVariableValue(var, new List<string>());
		}
		
		
		private object CalculateVariableValue(string var, IList<string> parentVariables)
		{
			//need to re-wrap variables
			if (string.IsNullOrEmpty(var))
			{
				return null;
			}
			//if (!var.StartsWith(VARIABLE_START) && !var.EndsWith(VARIABLE_END))
			//{
			//    var = VARIABLE_START + var + VARIABLE_END;
			//}

			ResolvedExpression resolvedExpression = Engine.ResolveExpression(var, this);
			if (resolvedExpression.HasException()) {
				return null;
			}
			else if (null == resolvedExpression.ResolvedValue)
			{
				return null;
			}
			else
			{
				if (resolvedExpression.ResolvedType == typeof(string))
				{
					string expressionValue = resolvedExpression.ResolvedValue.ToString();
					string[] variables = variableParserRegex.Split(expressionValue);
					StringBuilder expressionResult = new StringBuilder();
					foreach (string variable in variables)
					{
						if (variable.StartsWith("${"))
						{
							if (parentVariables.Count > MAX_REFERENCE_DEPTH || parentVariables.Contains(variable))
							{
								LogCircularKeyReferenceError(variable);
								expressionResult.Append(variable);
								continue;
							}
							parentVariables.Add(var);
							string subVariable = CalculateVariableValue(variable, parentVariables) as string;
							expressionResult.Append(subVariable ?? "");
						}
						else if (variable != null)
						{
							expressionResult.Append(variable);
						}
					}
					return expressionResult.ToString();
				}
				else
				{
					return resolvedExpression.ResolvedValue;
				}
			}
			
		}

		#region ResolveDataValues methods
		//todo - remove this method
		[Obsolete]
		public void ResolveDataValues()
		{
		}

		#endregion


		#region RegisterDataItem methods

		/// <summary>
		/// Registers a data item in the data context
		/// </summary>
		public void RegisterDataItem(IDataContextInvokable dataControl)
		{
			RegisterDataItem(dataControl, true);
		}

		/// <summary>
		/// Registers a literal data item in the data context
		/// </summary>
		public void RegisterDataItem(string key, string value)
		{
			RegisterDataItem(new LiteralDataItem(key, value));
		}

		/// <summary>
		/// Registers a literal data item in the data context
		/// </summary>
		public void RegisterDataItem(string key, object value)
		{
			RegisterDataItem(new LiteralDataItem(key, value));
		}

		/// <summary>
		/// Registers a data item in the data context
		/// </summary>
		/// <param name="dataControl"></param>
		public void RegisterDataItem(IDataContextInvokable dataControl, bool throwOnDuplicate)
		{
			RegisterDataItem(dataControl.Key, dataControl, null, throwOnDuplicate);
		}

		/// <summary>
		/// Registers a data item in the data context
		/// </summary>
		/// <param name="dataControl"></param>
		/// <param name="suppressClientKeyRegistration">When true pre-registration of this key on the client is suppressed.
		/// Set to true if the data item must defer resolution to the client.</param>
		/// <param name="throwOnDuplicate">When true an error is thrown if the key already exists</param>
		public void RegisterDataItem(IDataContextInvokable dataControl, bool suppressClientKeyRegistration, bool throwOnDuplicate)
		{
			RegisterDataItem(dataControl, null, suppressClientKeyRegistration, true, true);
		}

		/// <summary>
		/// Registers a data item in the data context
		/// </summary>
		/// <param name="dataControl"></param>
		/// <param name="viewContext">The name(s) of the view where this data item will be available.
		/// Null or * indicates all views</param>
		/// <param name="suppressClientKeyRegistration">When true pre-registration of this key on the client is suppressed.
		/// Set to true if the data item must defer resolution to the client.</param>
		/// <param name="requestServerResolution">Requests that the data value be resolved server-side</param>
		/// <param name="throwOnDuplicate">When true an error is thrown if the key already exists</param>
		public void RegisterDataItem(IDataContextInvokable dataControl, string viewContext, bool suppressClientKeyRegistration, bool requestServerResolution, bool throwOnDuplicate)
		{
			RegisterDataItem(dataControl.Key, dataControl, viewContext, throwOnDuplicate);
			if (suppressClientKeyRegistration)
			{
				MasterData[dataControl.Key].ExcludeFromClientContext = suppressClientKeyRegistration;
			}
			if (!requestServerResolution)
			{
				MasterData[dataControl.Key].RequestServerResolution = requestServerResolution;
			}
		}

		/// <summary>
		/// Registers a data item in the data context
		/// </summary>
		/// <param name="key"></param>
		/// <param name="dataControl"></param>
		public void RegisterDataItem(string key, IDataContextInvokable dataControl)
		{
			RegisterDataItem(key, dataControl, null, true);
		}

		public void RegisterDataItem(IDataContextInvokable dataControl, string viewContext)
		{
			RegisterDataItem(dataControl.Key, dataControl, viewContext, true);
		}

		/// <summary>
		/// Registers a data item in the data context
		/// </summary>
		/// <param name="key"></param>
		/// <param name="dataControl"></param>
		/// <param name="viewContext">The name(s) of the view where this data item will be available.
		/// Null or * indicates all views</param>
		public void RegisterDataItem(string key, IDataContextInvokable dataControl, string viewContext, bool throwOnDuplicate)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Data item must have a valid key");
			}
			DataItem addedItem = null;

			if (MasterData.ContainsKey(key) && dataControl != null)
			{
				//make sure view is not defined
				if (MasterData[key].IsValidForView(viewContext))
				{
					if (throwOnDuplicate)
					{
						throw new Exception("Duplicate key defined for view: " + key + " view: " + viewContext);
					}
				}
				else
				{
					string definedControlTag = null;
					if(MasterData[key].DataControl != null){
						definedControlTag = MasterData[key].DataControl.RawTag;
					}
					//same control defined - append the viewContext
					if (!string.IsNullOrEmpty(definedControlTag) && !string.IsNullOrEmpty(dataControl.RawTag)
						&& definedControlTag.Equals(dataControl.RawTag, StringComparison.InvariantCultureIgnoreCase))
					{
						if (MasterData[key].ViewContext == null)
						{
							MasterData[key].ViewContext = viewContext;
						}
						else
						{
							MasterData[key].ViewContext += "," + viewContext;
						}
						addedItem = MasterData[key];
					}
					else
					{
						addedItem = MasterData[key].AppendChainedDataItem(new DataItem(key, dataControl, viewContext));
					}
				}
			}
			else
			{
				addedItem = new DataItem(key, dataControl, viewContext);
				MasterData.Add(key, addedItem);
			}

			//insure any embedded values are pre-resolved
			if (dataControl != null && dataControl.Value != null && addedItem != null
				&& addedItem.Data != dataControl.Value)
			{
				addedItem.Data = dataControl.Value;
			}

		}
		#endregion


		#region Test support methods to force get and remove variable values


		/// <summary>
		/// Support method FOR TESTING ONLY.
		/// Sets the given key with the given value.
		/// Creates the key if not already defined
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void ForceSetValue(string key, object value)
		{
			if (MasterData.ContainsKey(key))
			{
				MasterData[key].Data = value;
			}
			else
			{
				DataItem item = new DataItem(key, null, null);
				item.Data = value;
				MasterData.Add(key,item);
			}
		}
		/// <summary>
		/// Support method FOR TESTING ONLY.
		/// Clears data from the given key, if found.
		/// </summary>
		/// <param name="key"></param>
		public void ForceClearValue(string key)
		{
			if (MasterData.ContainsKey(key))
			{
				MasterData[key].Data = null;
			}
		}
		/// <summary>
		/// Support method FOR TESTING ONLY.
		/// Completely purges the given key from the DataContext dictionary.
		/// </summary>
		/// <param name="key"></param>
		public void ForceRemoveValue(string key)
		{
			if (MasterData.ContainsKey(key))
			{
				MasterData.Remove(key);
			}
		}


		#endregion


		private string _globalRootKey = "Top";
		/// <summary>
		/// Root key for accessing the global context.
		/// This is used when inside local contexts.
		/// </summary>
		protected string GlobalRootKey
		{
			get
			{
				return _globalRootKey;
			}
			set
			{
				_globalRootKey = value;
			}
		}


		/// <summary>
		/// Registers a key to represent the global root.
		/// Any childen of this key are recognized as being
		/// root level values
		/// </summary>
		/// <param name="key"></param>
		public void RegisterGlobalRootKey(string key)
		{
			GlobalRootKey = key;
		}
		/// <summary>
		/// Clears the global root key
		/// </summary>
		public void RemoveGlobalRootKey()
		{
			GlobalRootKey = null;
		}


		/// <summary>
 		/// Sets the given key with the given value. Creates the key if not already defined.
		/// This is used for managing local variables within template processing.
		/// Adding variables with this method also adds them to the inferred variable stack
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void RegisterLocalValue(string key, object value)
		{
			RegisterLocalValue(key, value, false);
		}


		/// <summary>
		/// Sets the given key with the given value.
		/// Creates the key if not already defined.
		/// This is used for managing local variables within template processing.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="moveExistingToParent">Set to true when an existing value under the same key should be nested under the reserved "Parent" key.
		/// This only works if the value is a Dictionary-style object.</param>
		public void RegisterLocalValue(string key, object value, bool moveExistingToParent)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Key must be specified");
			}

			object realData = value;

			if (MasterData.ContainsKey(key))
			{
				object prevData = MasterData[key].Data;

				MasterData[key].Data = realData;
				//look for "Parent" value
				if (moveExistingToParent && realData is IDictionary<string, string>
					|| realData is IDictionary<string, object>)
				{
					IDictionary<string, object> tmp = realData as IDictionary<string, object>;
					if (tmp != null)
					{
						tmp[RESERVED_KEY_PARENT] = prevData;
					}
					else if(prevData is String)
					{

						IDictionary<string, string> tmp2 = realData as IDictionary<string, string>;
						tmp2[RESERVED_KEY_PARENT] = (String)prevData;
					}
				}

				if (InferredLocalVariableKeyStack[InferredLocalVariableKeyStack.Count - 1] != key)
				{
					InferredLocalVariableKeyStack.Add(key);
				}
			}
			else
			{
				DataItem item = new DataItem(key, null, null);
				item.Data = realData;
				MasterData.Add(key, item);

				InferredLocalVariableKeyStack.Add(key);
			}

			

		}





		/// <summary>
		/// Completely purges the given key from the DataContext dictionary.
		/// This is used for managing local variables within template processing.
		/// </summary>
		/// <remarks>
		/// When a Parent value is found in the existing item, it is promoted to the root of the key
		/// instead of deleting the key.
		/// </remarks>
		/// <param name="key"></param>
		public void RemoveLocalValue(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Key must be specified");
			}

			bool popKeyFromStack = true;
			if (MasterData.ContainsKey(key))
			{
				object realData = MasterData[key];
				DataItem itemVal = realData as DataItem;

				if(itemVal != null){
					realData = itemVal.Data;
				}

				if (realData is IDictionary<string, string>
					|| realData is IDictionary<string, object>)
				{
					IDictionary<string, object> tmp = realData as IDictionary<string, object>;
					if (tmp != null)
					{
						if (tmp.ContainsKey(RESERVED_KEY_PARENT))
						{
							itemVal.Data = tmp[RESERVED_KEY_PARENT];
							popKeyFromStack = false;
						}
					}
					else
					{
						IDictionary<string, string> tmp2 = realData as IDictionary<string, string>;
						if (tmp2.ContainsKey(RESERVED_KEY_PARENT))
						{
							itemVal.Data = tmp2[RESERVED_KEY_PARENT];
							popKeyFromStack = false;
						}
					}
				}
			}
			if (popKeyFromStack)
			{
				for (int i = InferredLocalVariableKeyStack.Count - 1; i >= 0; i--)
				{
					if (InferredLocalVariableKeyStack[i].Equals(key))
					{
						InferredLocalVariableKeyStack.RemoveAt(i);
						break;
					}
				}
				if (MasterData.ContainsKey(key))
				{
					MasterData.Remove(key);
				}

			}			
		}



		/// <summary>
		/// Tests to see if a variable has been registered under the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool HasVariable(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Key must be specified");
			}

			return MasterData.ContainsKey(key);
		}

		public DataItem GetActiveViewDataItem(string key)
		{
			if (!HasVariable(key))
			{
				return null;
			}
			else
			{
				return MasterData[key].GetViewSpecificDataItem(ActiveViewScope);
			}
		}


		#region Getters for variables


		/// <summary>
		/// Gets the data value associated with the given key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		internal object GetVariable(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Key must be specified to retrieve a value");
			}

			return GetVariable(MasterData, key);
		}

		/// <summary>
		/// Gets the variable
		/// </summary>
		/// <param name="dataDictionary">IDictionary<string, object></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private object GetVariable(object dataDictionary, string key)
		{
			if (dataDictionary == null || key == null)
			{
				throw new ArgumentNullException();
			}
			if(!(dataDictionary is Dictionary<string, object> || dataDictionary is Dictionary<string, DataItem>
				|| dataDictionary is IDictionary<string, object>)){
				throw new ArgumentException("Invalid type passed to dataDictionary");
			}


			string cleanedKey = key;
			bool isIndexGet = false;
			int indexedPos = 0;
			string selectorString = null;
			string subVar = null;
			//check for indexed item root key
			if (key.Contains("[") && key.EndsWith("]"))
			{
				isIndexGet = true;
				int endBracket = key.IndexOf("]");
				int startBraket = key.IndexOf("[");
				cleanedKey = key.Substring(0, startBraket);
				if (endBracket > startBraket)
				{
					subVar = key.Substring(startBraket + 1, (endBracket - startBraket - 1));
					if (!Int32.TryParse(subVar, out indexedPos))
					{
						if (subVar.StartsWith("@"))
						{
							selectorString = subVar;
						}
						else
						{
							subVar = CalculateVariableValue(subVar);
							Int32.TryParse(subVar, out indexedPos);
						}
					}
				}
			}


			object val = null;

			if (dataDictionary is Dictionary<string, DataItem>)
			{
				Dictionary<string, DataItem> diDict = dataDictionary as Dictionary<string, DataItem>;

				if (diDict.ContainsKey(cleanedKey))
				{
					val = diDict[cleanedKey].GetData(ActiveViewScope);
				}
				else if(dataDictionary == MasterData)
				{
					val = GetInferredScopeValue(cleanedKey);
				}
			}
			else if (dataDictionary is IDictionary<string, object>)
			{
				IDictionary<string, object> doDict = dataDictionary as IDictionary<string, object>;
				if (doDict.ContainsKey(cleanedKey))
				{
					val = doDict[cleanedKey];
				}
			}
			//see if this is an expression referencing another variable
			if (val is string && IsVariable((string)val))
			{
				val = CalculateVariableObjectValue((string)val);
			}

			if (isIndexGet)
			{
				if (selectorString == null)
				{
					val = GetNestedObjectWithIndex(val, indexedPos);
				}
				else
				{
					val = GetNestedObjectWithSelector(val, selectorString);

				}
			}
			return val;
		}

		/// <summary>
		/// Gets a nested object where a selctor (using an attribute '@') is specified
		/// </summary>
		/// <param name="val"></param>
		/// <param name="selectorString"></param>
		/// <returns></returns>
		private object GetNestedObjectWithSelector(object val, string selectorString)
		{
			if (val == null || string.IsNullOrEmpty(selectorString))
			{
				return null;
			}

			IEnumerable simpleCollection = null;
			if (val is IEnumerable)
			{
				simpleCollection = val as IEnumerable;
			}
			else if (val is IEnumerableDataWrapper)
			{
				simpleCollection = ((IEnumerableDataWrapper)val).EnumerableData;
			}

			string dataMember, targetValue;
			if (!JsonData.ParseSelectorString(selectorString, out dataMember, out targetValue))
			{
				return null;
			}

			if (val is JsonData)
			{
				val = ((JsonData)val).ResolveExpressionValue(selectorString);
			}
			else if (simpleCollection != null)
			{
				foreach (object item in simpleCollection)
				{
					IExpressionEvaluator itemEv = null;
					if (item is IExpressionEvaluator)
					{
						itemEv = (IExpressionEvaluator)item;
					}
					else if (item is KeyValuePair<string, object>)
					{
						itemEv = ((KeyValuePair<string, object>)item).Value as IExpressionEvaluator;
					}
					if (itemEv != null)
					{
						string curVal = itemEv.ResolveExpressionValue(dataMember) as string;
						if (targetValue == curVal)
						{
							val = itemEv;
							break;
						}
					}
				}
			}
			return val;
		}

		/// <summary>
		/// Returns a nested object from val.
		/// </summary>
		/// <param name="val">An enumerable object</param>
		/// <param name="indexedPos"></param>
		/// <returns></returns>
		private static object GetNestedObjectWithIndex(object val, int indexedPos)
		{
			if (val == null)
			{
				return null;
			}

			IEnumerable simpleCollection = null;
			if (val is IEnumerable)
			{
				simpleCollection = val as IEnumerable;
			}
			else if (val is IEnumerableDataWrapper)
			{
				simpleCollection = ((IEnumerableDataWrapper)val).EnumerableData;
			}


			if (val is JsonData)
			{
				val = ((JsonData)val).ResolveExpressionValue(indexedPos.ToString());
			}
			else if (simpleCollection != null)
			{
				if (simpleCollection is IList)
				{
					IList colVar = simpleCollection as IList;
					if (indexedPos < colVar.Count)
					{
						val = colVar[indexedPos];
					}
				}
				else
				{
					int currentItem = 0;
					foreach (object item in simpleCollection)
					{
						if (currentItem == indexedPos)
						{
							val = item;
							break;
						}
						currentItem++;
					}
				}
			}
			return val;
		}

		/// <summary>
		/// Gets a variable's value, scope validated to the given context
		/// </summary>
		/// <param name="key"></param>
		/// <param name="context">Change to View enum</param>
		/// <returns></returns>
		public object GetVariable(string key, string viewContext)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Key must be specified to retrieve a value");
			}

			object val = null;
			if (MasterData.ContainsKey(key))
			{
				DataItem item = MasterData[key];
				if (item.ViewContext.IndexOf(viewContext) > -1)
				{
					val = item.GetData(ActiveViewScope);
				}
			}

			return val;

		}

		#endregion

		/// <summary>
		/// Returns the string representation of an object.
		/// </summary>
		/// <param name="variableKey"></param>
		/// <returns></returns>
		public string GetVariableValue(string variableKey)
		{
			object val = GetVariableObject(variableKey);
			if (val == null)
			{
				currentCalculateReferenceDepth = 0;
				return null;
			}
			else
			{
				if (val is string)
				{
					string str = (string)val;
					if (IsVariable(str))
					{
						return CalculateVariableValue(str);
					}
					else
					{
						currentCalculateReferenceDepth = 0;
						return str;
					}
				}
				else
				{
					//try to check for numerics
					string stringval = val.ToString();
					Double numeric;
					DateTime dateVal;
					if (Double.TryParse(stringval, out numeric))
					{
						currentCalculateReferenceDepth = 0;
						return stringval;
					}
					else if (DateTime.TryParse(stringval, out dateVal))
					{
						currentCalculateReferenceDepth = 0;
						return stringval;
					}
					else
					{
						currentCalculateReferenceDepth = 0;
						return string.Empty;
					}
				}
			}
		}


		/// <summary>
		/// Gets a variable object and confirms it is an IEnumerable object.
		/// If result is not enumerable this returns null.
		/// Return value could be IList, IDictionary, or any other type of enumerable
		/// </summary>
		/// <param name="variableKey"></param>
		/// <returns></returns>
		public IEnumerable GetEnumerableVariableObject(string variableKey)
		{
			if (string.IsNullOrEmpty(variableKey))
			{
				return null;
			}

			object repeatObjectList = this.GetVariableObject(variableKey);

			if (null == repeatObjectList)
			{
				return null;
			}

			IEnumerable simpleList = null;
			if (repeatObjectList is IEnumerable)
			{
				simpleList = repeatObjectList as IEnumerable;
			}
			else if (repeatObjectList is IEnumerableDataWrapper)
			{
				simpleList = ((IEnumerableDataWrapper)repeatObjectList).EnumerableData;
			}

			return simpleList;
		}

		private IList<ELException> _elErrors = new List<ELException>();
		public IList<ELException> ELErrors { 
			get{
				return _elErrors;
			}
		}

		/// <summary>
		/// Get a deep resolved variable value of form key.MemberName
		/// Returns empty string if requested value is invalid
		/// </summary>
		/// <param name="variableKey"></param>
		/// <returns></returns>
		public object GetVariableObject(string variableKey)
		{
			if (String.IsNullOrEmpty(variableKey)) return String.Empty;

			currentCalculateReferenceDepth++;
			if (currentCalculateReferenceDepth > MAX_REFERENCE_DEPTH)
			{
				currentCalculateReferenceDepth = 0;
				LogCircularKeyReferenceError(variableKey);
				return null;
			}

			if (variableKey.StartsWith(DataContext.VARIABLE_START))
			{
				variableKey = GetVariableExpression(variableKey);
			}

			object retval = null;

			int dotPos = variableKey.IndexOf('.');
			if (dotPos == -1)
			{
				retval = GetVariable(variableKey);
				if (null == retval)
				{
					retval = GetInferredScopeValue(variableKey);
					if (null == retval)
					{
						LogVariableCalculateError(variableKey, "Inferred value not found for key: " + variableKey);
					}
				}
				return retval;
			}
			else
			{
				string[] parts = variableKey.Split(new char[] { '.' });
				string rootKey = parts[0];
				string theRestOfIt = variableKey.Substring(dotPos + 1);


				if (!string.IsNullOrEmpty(GlobalRootKey) && rootKey.Equals(GlobalRootKey, StringComparison.InvariantCultureIgnoreCase))
				{
					rootKey = null;
					if (parts.Length > 1)
					{
						rootKey = parts[1];
						parts = SliceArray(parts);
						theRestOfIt = string.Empty;
						for (int i = 1; i < parts.Length; i++)
						{
							if (i > 1)
							{
								theRestOfIt += ".";
							}
							theRestOfIt += parts[i];
						}

					}
				}

				retval = GetVariable(rootKey);
				if (null == retval)
				{
					retval = GetInferredScopeValue(variableKey);
					if (null == retval)
					{
						LogMissingKeyRequestError(variableKey);
						//TODO log bad variable request error
						return null; //variableKey + " Root object not found";
					}
					return retval;
				}
				else
				{
					if (retval is IExpressionEvaluator)
					{
						return ((IExpressionEvaluator)retval).ResolveExpressionValue(theRestOfIt);
					}
					else
					{
						if (parts.Length == 1)
						{
							return retval;
						}
						else
						{
							Stack<string> memberKeyStack = new Stack<string>();
							for (int i = parts.Length - 1; i > 0; i--)
							{
								memberKeyStack.Push(parts[i]);
							}
							object result = GetNestedObjectValue(retval, memberKeyStack);
							return result;
						}
					}
				}
			}
		}

		/// <summary>
		/// Attempt to resolve a variable based on InferredScope stack
		/// </summary>
		/// <param name="variableKey"></param>
		/// <returns></returns>
		private object GetInferredScopeValue(string variableKey)
		{
			object obj = null;
			//try the scope stack loop on the expression
			for (int i = this.InferredLocalVariableKeyStack.Count - 1; i >= 0; i--)
			{
				if (MasterData.ContainsKey(InferredLocalVariableKeyStack[i]))
				{

					obj = MasterData[InferredLocalVariableKeyStack[i]].GetData(ActiveViewScope);  //GetVariable(InferredLocalVariableKeyStack[i]);
					if (obj != null)
					{
						object possibleValue = GetRecursiveCurrentOrParentValue(obj, variableKey);
						if (possibleValue != null)
						{
							return possibleValue;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Recursively look for the variableKey, walking up the Parent nodes, if applicable
		/// </summary>
		/// <param name="rootObject"></param>
		/// <param name="variableKey"></param>
		/// <returns></returns>
		private static object GetRecursiveCurrentOrParentValue(object rootObject, string variableKey)
		{
			if (rootObject == null || string.IsNullOrEmpty(variableKey))
			{
				return null;
			}
			object obj = null;

			if (rootObject is IExpressionEvaluator)
			{
				obj = ((IExpressionEvaluator)rootObject).ResolveExpressionValue(variableKey);
				if (obj != null)
				{
					return obj;
				}
				//look for Parent
				obj = ((IExpressionEvaluator)rootObject).ResolveExpressionValue(RESERVED_KEY_PARENT);
				if (obj != null)
				{
					return GetRecursiveCurrentOrParentValue(obj, variableKey);
				}
			}
			else if (rootObject is IDictionary<string, string> || rootObject is IDictionary<string, object>)
			{
				IDictionary<string, object> dict = rootObject as IDictionary<string, object>;
				if (dict.ContainsKey(variableKey))
				{
					return dict[variableKey];
				}
				if (dict.ContainsKey(RESERVED_KEY_PARENT))
				{
					return GetRecursiveCurrentOrParentValue(dict[RESERVED_KEY_PARENT], variableKey);
				}
			}
			return obj;
		}


		/// <summary>
		/// Recursively walk down the dot notation chain the resolve the values
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="memberKeys"></param>
		/// <returns></returns>
		private object GetNestedObjectValue(object obj, Stack<string> memberKeys)
		{
			if (null == memberKeys || memberKeys.Count == 0)
			{
				return obj;
			}

			if (null == obj)
			{
				return null;
			}

			if (obj is IExpressionEvaluator)
			{
				string expression = memberKeys.Pop();
				while (memberKeys.Count > 0)
				{
					expression += "." + memberKeys.Pop();
				}
				return ((IExpressionEvaluator)obj).ResolveExpressionValue(expression);
			}

			object next = GetNestedObjectValue(obj, memberKeys.Pop());
			if (memberKeys.Count == 0)
			{
				return next;
			}
			else
			{
				return GetNestedObjectValue(next, memberKeys);
			}
		}

		/// <summary>
		/// Returns a new array with the first element removed and
		/// all subsequent elements shifted down.
		/// The use of this method is deprecated.  
		/// Instead, use a stack in reverse order.
		/// </summary>
		/// <param name="originalArray"></param>
		/// <returns></returns>
		string[] SliceArray(string[] originalArray)
		{
			if (originalArray.Length == 1)
			{
				return (string[])originalArray.Clone();
			}
			else
			{
				int len = originalArray.Length - 1;
				List<string> tmp = new List<string>();
				for (int i = 1; i < originalArray.Length; i++)
				{
					tmp.Add(originalArray[i]);
				}
				return tmp.ToArray();
			}
		}


		/// <summary>
		/// Gets the nested object invoked via dot notation.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		protected object GetNestedObjectValue(object obj, string memberKey)
		{
			if (obj == null)
			{
				return null;
			}

			//a string is enumerable, so we must check it first
			if (obj is String && IsVariable((string)obj))
			{
				IExpressionEvaluator objResult = 
					GetVariableObject((string)obj) as IExpressionEvaluator;
				if (objResult != null && !string.IsNullOrEmpty(memberKey))
				{
					return objResult.ResolveExpressionValue(memberKey);
				}
				return null;
			}
			
			if (obj is IEnumerable)
			{
				if (obj is IDictionary<string, string>)
				{
					IDictionary<string, string> tmp = (IDictionary<string, string>)obj;
					if (tmp.ContainsKey(memberKey))
					{
						return tmp[memberKey];
					}
					else
					{
						if (tmp.ContainsKey(memberKey))
						{
							return tmp[memberKey];
						}
						else
						{
							return null;
						}
					}
				}
				else if (obj is IDictionary<string, object>)
				{
					IDictionary<string, object> tmp = (IDictionary<string, object>)obj;

					return GetVariable(tmp, memberKey);
				}
				else if (obj is Hashtable)
				{
					Hashtable tmp = (Hashtable)obj;
					if (tmp.ContainsKey(memberKey))
					{
						return tmp[memberKey];
					}
					else
					{
						if (tmp.ContainsKey(memberKey))
						{
							return tmp[memberKey];
						}
						else
						{
							return null;
						}
					}
				}
				else if (obj is IList)
				{
					//try to pop member key
					IList tmp = (IList)obj;
					return null;
				}
				else
				{
					return null;
				}
			}
			//else if (obj is IAccountLite)
			//{
			//    return GetAccountLiteMemberValue((IAccountLite)obj, memberKey);
			//}
			else
			{
				return GetObjectMemberValue(obj, memberKey);
			}
		}

		/// <summary>
		/// Tests to see if this is a variable by checking
		/// for the Variable expression start and end tags
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static bool IsVariable(string member)
		{
			if (string.IsNullOrEmpty(member))
			{
				return false;
			}
			if (member.StartsWith(DataContext.VARIABLE_START)
				&& member.EndsWith(DataContext.VARIABLE_END))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Tests to see if a messagebundle variable is in the source string.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static bool ContainsMessageBundleVariable(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return false;
			}
			if ((source.Contains(LEGACY_MSGVARIABLE_START) && source.Contains(LEGACY_MSGVARIABLE_END))
				|| source.Contains(VARIABLE_START + RESERVED_KEY_MESSAGE))
			{
				return true;
			}
			return false;
		}



		///// <summary>
		///// Gets a late-bound value member from the given object.
		///// This is primarily used for development against AccountLite directly.
		///// Production version will use ROA opensocial objects
		///// </summary>
		///// <param name="Viewer"></param>
		///// <param name="p"></param>
		///// <returns></returns>
		//[Obsolete("Use IExpressionEvaluator objects now")]
		//private string GetAccountLiteMemberValue(IAccountLite person, string member)
		//{
		//    if (null == person)
		//    {
		//        return null;
		//    }

		//    Type t = person.GetType();

		//    //normalize to our AccountLite fields
		//    if ("Name" == member)
		//    {
		//        member = "DisplayName";
		//    }
		//    else if ("Id" == member || "id" == member)
		//    {
		//        member = "UserId";
		//    }

		//    object value;
		//    if ("Image" == member)
		//    {
		//        value = person.GetImageUrl(ImageBucketType.s);
		//    }
		//    else
		//    {
		//        value = GetObjectMemberValue(person, member); // t.InvokeMember(member, BindingFlags.GetProperty | BindingFlags.GetField, null, person, null);
		//    }
			

		//    if (value == null)
		//    {
		//        return null;
		//    }
		//    else
		//    {
		//        return value.ToString();
		//    }
		//}

		/// <summary>
		/// Gets a late-bound value member from the given object
		/// </summary>
		/// <param name="Viewer"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		private object GetObjectMemberValue(object obj, string member)
		{
			if (null == obj)
			{
				return null;
			}
			if(string.IsNullOrEmpty(member)){
				return obj;
			}

			GenericExpressionEvalWrapper wrapper = new GenericExpressionEvalWrapper(obj);
			return wrapper.ResolveExpressionValue(member);
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_masterData != null)
				{
					_masterData.Clear();
					_masterData = null;
				}


			}
		}

		#endregion

		/// <summary>
		/// Strips off start and end brackets and returns variable text.
		/// variableString passed in MUST be a variable and pre-stripped.
		/// TODO - Make more robust
		/// </summary>
		/// <param name="variableString">Pre-(whitespace)stripped variable block</param>
		/// <returns></returns>
		public static string GetVariableExpression(string variableString)
		{
			if (String.IsNullOrEmpty(variableString) || variableString.Length < 3)
			{
				return variableString;
			}
			if (!variableString.StartsWith(DataContext.VARIABLE_START))
			{
				return variableString;
			}
			return variableString.Substring(2, variableString.Length - 3);
		}

		/// <summary>
		/// Extracts the root key from a variable expression
		/// </summary>
		/// <param name="variableString"></param>
		/// <returns></returns>
		public static string GetVariableExpressionRootKey(string variableString)
		{
			if (string.IsNullOrEmpty(variableString))
			{
				return null;
			}
			string cleaned = GetVariableExpression(variableString);
			int dotPos = cleaned.IndexOf(".");
			if (dotPos > -1)
			{
				cleaned = cleaned.Substring(0, dotPos);
			}
			int arrayPos = cleaned.IndexOf("[");
			if (arrayPos > -1)
			{
				cleaned = cleaned.Substring(0, arrayPos);
			}
			return cleaned;
		}



		/// <summary>
		/// Writes context out as a JSON object under format:
		/// { key: {value_object}}.
		/// This results in a simple JSON object with all values recorded
		/// as key:value.
		/// </summary>
		/// <param name="stream"></param>
		public void WriteClientContext(System.IO.TextWriter writer)
		{
			if (MasterData.Count == 0)
			{
				writer.Write("{}"); //empty object
				return;
			}
			CheckSetAllowGlobalReflectiveSerialization();

			writer.Write("{");
			bool isFirstItem = true;
			foreach (KeyValuePair<string, DataItem> keyset in MasterData)
			{
				DataItem item = keyset.Value;
				if (item.ExcludeFromClientContext) continue;


				if (!isFirstItem)
				{
					writer.Write(", ");
					isFirstItem = false;
				}
				//stream.Write(item.Key)
				writer.Write(JsonData.JSSafeQuote(item.Key));
				writer.Write(":");
				item.WriteAsJSON(writer);				
			}

			writer.Write("}");

		}

		/// <summary>
		/// Writes context out as JSON object using itemTemplate for each item.
		/// The item template makes use of two tokens: ##KEY## and ##DATA## for replacment.
		/// Quotation handling is internal, so don't add extra quotes to your template.
		/// </summary>
		/// <remarks>
		/// Sample template for OpenSocial:
		/// opensocial.data.getDataContext().putDataSet(##KEY##, ##DATA##);
		/// </remarks>
		/// <param name="stream"></param>
		/// <param name="itemTemplate">Single line statement to use as template.  
		/// This is expected to be a fully contained line of javascript</param>
		public void WriteClientContext(System.IO.TextWriter writer, string itemTemplate)
		{
			if (String.IsNullOrEmpty(itemTemplate))
			{
				throw new ArgumentNullException("Must specify a template");
			}

			if (MasterData.Count == 0)
			{
				return;
			}
			string token_key = "##KEY##";
			string token_data = "##DATA##";

			int keyPos = itemTemplate.IndexOf(token_key);
			int dataPos = itemTemplate.IndexOf(token_data);

			if(keyPos == -1 || dataPos == -1){
				throw new ArgumentException("Both " + token_key + " AND " + token_data + " must be specified in itemTemplate");
			}

			CheckSetAllowGlobalReflectiveSerialization();

			string templatePrior = itemTemplate.Substring(0, dataPos);
			string templateAfter = itemTemplate.Substring(dataPos + token_data.Length);

			bool keyInFirstPart = (keyPos < dataPos);

			foreach (KeyValuePair<string, DataItem> keyset in MasterData)
			{
				DataItem item = keyset.Value.GetViewSpecificDataItem(ActiveViewScope);
				if(item == null || item.ExcludeFromClientContext) continue;


				if (keyInFirstPart)
				{
					writer.Write(templatePrior.Replace(token_key, JsonData.JSSafeQuote(item.Key)));
					item.WriteAsJSON(writer);
					writer.WriteLine(templateAfter);
				}
				else
				{
					writer.Write(templatePrior);
					item.WriteAsJSON(writer);
					writer.Write(templateAfter.Replace(token_key, JsonData.JSSafeQuote(item.Key)));
				}

			}
		}

		/// <summary>
		/// DO NOT USE THIS CALL - unless you don't like performance.
		/// Gets the JSON representation of the data item.
		/// This is normally only used for testing purposes.
		/// Call DataContext.WriteClientContext instead to dump
		/// the client context.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetJsonData(string key)
		{
			if (!MasterData.ContainsKey(key))
			{
				//JsonData.JSSafeQuote
				return JSONERR_KEY_NOT_FOUND;
			}
			DataItem item = MasterData[key];
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			item.WriteAsJSON(writer);
			return GetStreamContent(stream);
		}

		/// <summary>
		/// Reads out the contents of a stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		internal static string GetStreamContent(Stream stream)
		{
			string val = null;
			stream.Seek(0, SeekOrigin.Begin);
			using (StreamReader reader = new StreamReader(stream))
			{
				val = reader.ReadToEnd();
			}
			return val;
		}

		/// <summary>
		/// Count of the number of registered data items
		/// </summary>
		public int DataItemCount
		{
			get
			{
				return MasterData.Count;
			}
		}



		#region Support for reflection-based data resolution

		/// <summary>
		/// Flag to allow for global use of reflective serialization of data.
		/// USE ONLY FOR TESTING/INITIAL DEVELOPMENT
		/// </summary>
		private bool _globalAllowReflectiveClientSerialization = false;

		/// <summary>
		/// Flag to allow for global use of reflective serialization of data.
		/// USE ONLY FOR TESTING/INITIAL DEVELOPMENT
		/// </summary>
		protected bool GlobalAllowReflectiveClientSerialization
		{
			get
			{
				return _globalAllowReflectiveClientSerialization;
			}
			private set
			{
				_globalAllowReflectiveClientSerialization = value;
			}
		}

		/// <summary>
		/// Checks to see if global reflective serialization is enabled.
		/// If so, sets the flag on all data items.
		/// NEVER, EVER USE THIS EXCEPT IN TESTING
		/// </summary>
		private void CheckSetAllowGlobalReflectiveSerialization()
		{
			if (!GlobalAllowReflectiveClientSerialization) return;
			foreach (KeyValuePair<string, DataItem> keyset in MasterData)
			{
				DataItem item = keyset.Value;
				item.AllowReflectiveJsonSerialization = true;
			}

		}


		/// <summary>
		/// USE ONLY FOR TESTING/INITIAL DEVELOPMENT
		/// 
		/// </summary>
		public void SetAllowGlobalReflectiveSerialization(bool allow)
		{
			GlobalAllowReflectiveClientSerialization = allow;
		}

		/// <summary>
		/// USE AT YOUR OWN RISK
		/// Reflective serialization has significant performance issues.
		/// Use only for testing.  Implement an IJsonSerializable object wrapper instead.
		/// </summary>
		/// <param name="key"></param>
		public void SetAllowReflectiveSerialization(string key, bool allow)
		{
			if (MasterData.ContainsKey(key))
			{
				MasterData[key].AllowReflectiveJsonSerialization = allow;
			}
		}

		#endregion

		#region Error Logging support

		private List<string> _variableCalculateErrors = null;

		/// <summary>
		/// Tests to see if any errors occurred when calculating variable values
		/// </summary>
		/// <returns></returns>
		public bool HasVariableCalculateErrors()
		{
			return (_variableCalculateErrors != null && _variableCalculateErrors.Count > 0);
		}

		/// <summary>
		/// Errors occurring from calls to <c>CalculateVariableValue</c>
		/// </summary>
		public List<string> VariableCalculateErrors
		{
			get
			{
				if (_variableCalculateErrors == null)
				{
					_variableCalculateErrors = new List<string>();
				}
				return _variableCalculateErrors;
			}
		}

		/// <summary>
		/// Logs an error where a key is found to be part of a circular reference
		/// </summary>
		/// <param name="key"></param>
		protected void LogCircularKeyReferenceError(string key)
		{
			LogVariableCalculateError(key, "Circular Reference Detected: " + key);
		}


		/// <summary>
		/// Logs an error where a key that doesn't exist was requested
		/// </summary>
		/// <param name="key"></param>
		protected void LogMissingKeyRequestError(string key)
		{
			LogVariableCalculateError(key, "Mising Key: " + key);
		}
		/// <summary>
		/// Logs an error where a call to CalculateVariableValue goes awray.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		protected void LogVariableCalculateError(string key, string message)
		{
			string err = "Key: {0} \nMessage:{1}";
			VariableCalculateErrors.Add(String.Format(err, key, message));
		}
		#endregion

	}
}
