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
using System.Collections.Generic;
using System.IO;
using System.Text;

using Negroni.DataPipeline;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Encapsulation of errors that might occur in a Gadget
	/// </summary>
	public class GadgetErrors
	{

		#region Helper methods for converting exceptions to useful messages

		/// <summary>
		/// Interrogates the exception to return the content two lines above and one line below
		/// exception
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public string GetParseExceptionContent(System.Xml.XmlException exception)
		{
			if (null == Parent)
			{
				throw new Exception("Parent GadgetMaster has not been initialized");
			}
			if (null == exception)
			{
				return null;
			}
			string fullGadgetSrc = Parent.RawTag;
			if (string.IsNullOrEmpty(fullGadgetSrc))
			{
				return null;
			}

			// ... Line NN, position MM.
			string lineSearch = "Line ";
			// string posSearch = "position ";

			string msg = exception.Message;
			int pos = msg.IndexOf(lineSearch);
			int endOfLinePos = pos + lineSearch.Length;
			int commaPos = msg.IndexOf(",", endOfLinePos);
			if (pos == -1 || commaPos == -1)
			{
				return null;
			}

			string tmp = msg.Substring(endOfLinePos, commaPos - endOfLinePos);
			int line;
			if (!Int32.TryParse(tmp, out line))
			{
				return null;
			}

			string[] lines = new string[4];
			using (StringReader reader = new StringReader(fullGadgetSrc))
			{
				int curLine = 0;
				while (curLine++ < (line - 2) && null != reader.ReadLine()) { }

				for (int i = 0; i < 4; i++)
				{
					lines[i] = reader.ReadLine();
					if (lines[i] == null)
					{
						break;
					}

				}
			}
			return String.Join("\n", lines);
		}
		#endregion


		public GadgetErrors()
		{
		}

		public GadgetErrors(RootElementMaster parent)
		{
			Parent = parent;
		}


		/// <summary>
		/// GadgetMaster to which this object belongs
		/// </summary>
		public RootElementMaster Parent { get; set; }

		#region Warnings

		private List<string> _warnings = null;

		/// <summary>
		/// Issues that generate a warning, but are not a full blown error.
		/// </summary>
		public List<string> Warnings
		{
			get
			{
				if (_warnings == null)
				{
					_warnings = new List<string>();
				}
				return _warnings;
			}
		}

		/// <summary>
		/// Checks to see if there are any warning messages
		/// </summary>
		/// <returns></returns>
		public bool HasWarnings()
		{
			return (_warnings != null && Warnings.Count > 0);
		}

		#endregion


		#region ParseErrors

		private List<Exception> _parseErrors = null;

		/// <summary>
		/// Errors occuring while parsing
		/// </summary>
		public List<Exception> ParseErrors
		{
			get
			{
				if (_parseErrors == null)
				{
					_parseErrors = new List<Exception>();
				}
				return _parseErrors;
			}
		}


		private List<Exception> _metaDataErrors = null;

		/// <summary>
		/// Validation errors within the meta-data - icons, etc.
		/// </summary>
		public List<Exception> MetaDataErrors
		{
			get
			{
				if (_metaDataErrors == null)
				{
					_metaDataErrors = new List<Exception>();
				}
				return _metaDataErrors;
			}
		}

        public bool HasMetaDataErrors()
        {
            return (this._metaDataErrors != null && _metaDataErrors.Count > 0);
        }

		/// <summary>
		/// Tests to see if any errors were encountered during parse.
		/// Returns false if gadget has not been parsed.
		/// </summary>
		/// <returns></returns>
		public bool HasParseErrors()
		{
			return (_parseErrors != null && _parseErrors.Count > 0);
		}
		#endregion


		#region Circular Data references

		/// <summary>
		/// Object assisting in looking for circular references.
		/// </summary>
		private class KeyReferenceCounter
		{
			public KeyReferenceCounter() { }

			public KeyReferenceCounter(string key)
			{
				Key = key;
			}

			public KeyReferenceCounter(string key, string[] dependentKeys)
			{
				Key = key;
				if (dependentKeys != null)
				{
					for (int i = 0; i < dependentKeys.Length; i++)
					{
						PointsTo.Add(dependentKeys[i], dependentKeys[i]);
					}
				}
			}

			/// <summary>
			/// Data control key
			/// </summary>
			public string Key = null;
			/// <summary>
			/// Number of times this is referenced
			/// </summary>
			public int RefCount = 0;
			/// <summary>
			/// Keys this control points to
			/// </summary>
			public Dictionary<string, string> PointsTo = new Dictionary<string, string>();
			/// <summary>
			/// Keys pointing at this control
			/// </summary>
			public Dictionary<string, string> PointedAtFrom = new Dictionary<string, string>();
		}

		/// <summary>
		/// Tests all registered data controls to see if there are circular references present
		/// between controls.
		/// </summary>
		/// <remarks>
		/// This should be called initially in the verification process prior to save.
		/// During resolution we assume controls are valid.
		/// </remarks>
		/// <param name="circularControlKeys">Output array of control keys that are part of the circular reference</param>
		/// <returns></returns>
		public bool HasCircularControlParameterReferences(out string[] circularControlKeys)
		{
			if (null == Parent)
			{
				throw new Exception("Parent GadgetMaster has not been initialized");
			}
			circularControlKeys = new string[] { };

			//_cyclicDataReferenceKeys = new List<string>();
			CyclicDataReferenceKeys.Clear();

			foreach (KeyValuePair<string, DataItem> keyset in Parent.MasterDataContext.MasterData)
			{
				Dictionary<string, string> keysAlreadyUsed = new Dictionary<string, string>();
				string badKey;
				if (!Parent.IsResolvableKey(keyset.Key, keysAlreadyUsed, out badKey))
				{
					if (!string.IsNullOrEmpty(badKey))
					{
						CyclicDataReferenceKeys.Add(badKey);
					}
				}
			}
			if (CyclicDataReferenceKeys.Count == 0)
			{
				return false;
			}
			else
			{
				circularControlKeys = CyclicDataReferenceKeys.ToArray();
				return true;
			}
		}


		private List<string> _cyclicDataReferenceKeys = null;

		/// <summary>
		/// List of data keys that have Cyclic references.  
		/// This is populated as a side effect of the call to <c>HasCircularControlParameterReferences</c>
		/// </summary>
		public List<string> CyclicDataReferenceKeys
		{
			get
			{
				if (_cyclicDataReferenceKeys == null)
				{
					_cyclicDataReferenceKeys = new List<string>();
				}
				return _cyclicDataReferenceKeys;
			}
			private set
			{
				_cyclicDataReferenceKeys = value;
			}
		}


		#endregion


		#region MessageBundle errors
		private List<MessageBundleItemError> _messageBundleErrors = null;

		/// <summary>
		/// List of message bundle items that have errors
		/// This is populated as a side effect of the call to <c>HasMessageBundleValidationErrors</c>
		/// </summary>
		public List<MessageBundleItemError> MessageBundleErrors
		{
			get
			{
				if (_messageBundleErrors == null)
				{
					_messageBundleErrors = new List<MessageBundleItemError>();
				}
				return _messageBundleErrors;
			}
			private set
			{
				_messageBundleErrors = value;
			}
		}

	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gadget"></param>
		/// <param name="errorMessages"></param>
		/// <returns></returns>
		public bool HasMessageBundleValidationErrors()
		{
			if (null == Parent)
			{
				throw new Exception("Parent GadgetMaster has not been initialized");
			}
			MessageBundleErrors.Clear();

			string[] cultures = Parent.MasterDataContext.ResourceStringCatalog.GetDefinedCultures();
			if (null == cultures || cultures.Length == 0)
			{
				return false;
			}
			//test invariant culture first
			bool errFound = false;
			//errFound = HasMessageBundleValidationErrors(Parent.MasterDataContext,
			//    ResourceBundleCatalog.INVARIANT_CULTURE_KEY, errorMessages);

			//if (errFound)
			//{
			//    return errFound;
			//}
			for (int i = 0; i < cultures.Length; i++)
			{
				if(HasMessageBundleValidationErrors(cultures[i]))
				{
					errFound = true;
				}
			}

			return errFound;
		}

		/// <summary>
		/// Looks for errors in the MessageBundle for a given culture.
		/// </summary>
		/// <param name="dataContext"></param>
		/// <param name="culture"></param>
		/// <param name="errorMessages"></param>
		/// <returns></returns>
		internal bool HasMessageBundleValidationErrors(string culture)
		{
			DataContext dataContext = Parent.MasterDataContext;
			string[] keys;
			keys = dataContext.ResourceStringCatalog.GetKeysDefinedForCulture(culture);

			bool hasError = false;
			if (keys.Length == 0)
			{
				return false;
			}
			else
			{
				dataContext.VariableCalculateErrors.Clear();
				int errStartCount = dataContext.VariableCalculateErrors.Count;
				for (int i = 0; i < keys.Length; i++)
				{
					string testString = DataContext.VARIABLE_START + DataContext.RESERVED_KEY_MESSAGE + "." + keys[i] + DataContext.VARIABLE_END;
					string resolved = dataContext.ResolveMessageBundleVariables(testString, culture);
					if (!string.IsNullOrEmpty(resolved)
						&& (resolved.Contains("<") || resolved.Contains(">")))
					{
						hasError = true;
						this.MessageBundleErrors.Add(new MessageBundleItemError(DataContext.RESERVED_KEY_MESSAGE + "." + keys[i], culture, 
							"Key: " + keys[i] + " Message: Illegal markup found under key: " + keys[i] + "  culture: " + culture,
							MessageBundleErrorType.IllegalContent));
					}
				}
				if (errStartCount < dataContext.VariableCalculateErrors.Count)
				{
					hasError = true;
					foreach (string errMsg in dataContext.VariableCalculateErrors)
					{
						string key = null;
						if (errMsg != null && errMsg.Contains("Key: "))
						{
							string keyStr = "Key: ";
							int index = errMsg.IndexOf(keyStr) + keyStr.Length;
							int nextIndex = errMsg.IndexOf(" ", index);
							int crIndex = errMsg.IndexOf("/n", index);
							if (crIndex > index)
							{
								nextIndex = Math.Min(nextIndex, crIndex);
							}
							if (index < nextIndex)
							{
								key = errMsg.Substring(index, nextIndex - index);
							}
						}
						if (string.IsNullOrEmpty(key))
						{
							key = errMsg;
						}
						MessageBundleErrors.Add(new MessageBundleItemError(key, culture,
							errMsg + " for culture " + culture,
							MessageBundleErrorType.CircularReference));
					}
				}
				
				return hasError;
			}
		}

		#endregion


		#region External Template Library errors

		private List<Exception> _templateLibraryErrors = null;

		/// <summary>
		/// Validation errors within the meta-data - icons, etc.
		/// </summary>
		public List<Exception> TemplateLibraryErrors
		{
			get
			{
				if (_templateLibraryErrors == null)
				{
					_templateLibraryErrors = new List<Exception>();
				}
				return _templateLibraryErrors;
			}
		}

		public bool HasTemplateLibraryErrors()
		{
			return (this._templateLibraryErrors != null && _templateLibraryErrors.Count > 0);
		}

		#endregion

	}
}
