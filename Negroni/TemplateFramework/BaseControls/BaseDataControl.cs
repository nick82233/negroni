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
using System.Text;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Base class for data controls
	/// </summary>
	abstract public class BaseDataControl : BaseGadgetControl, IDataContextInvokable
	{
		/// <summary>
		/// Initializes common values from the markup tag.
		/// LoadTag must have been called prior to calling this method
		/// </summary>
		protected void InitializeCommonValues()
		{
			if (String.IsNullOrEmpty(RawTag))
			{
				return;
			}
			if (HasAttributes())
			{
				Key = GetAttribute("key");
			}
		}

		/// <summary>
		/// Loads the markup tag and initializes common attribute values
		/// </summary>
		/// <param name="markup"></param>
		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			InitializeCommonValues();
		}

		/// <summary>
		/// Key used for this DataControl
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Comma delimited view names for which this data item is registered
		/// </summary>
		public string ViewName { get; set; }

		/// <summary>
		/// Underlying value.  It is up to the implementer
		/// to determine how to place value in here and
		/// if it should be used.
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Fetches (or resolves) the control data. Sets the <code>Value</code> property on success.
		/// When paired with an IDataPipelineResolver to resolve the data values, 
		/// this method should be implemented to perform the resolution of data.
		/// </summary>
		/// <remarks>
		/// User-specific information can usually be found in MyDataContext.RequestContext
		/// </remarks>
		public virtual void InvokeTarget() { }

		/// <summary>
		/// Flag to indicate data resolution is delayed to client.
		/// If true the control should have the Render method overridden
		/// to emit proper code for data fetching client-side.
		/// </summary>
		public bool UseClientDataResolver
		{
			get; set;
		}

		/// <summary>
		/// Data Controls do not render.
		/// This may change
		/// </summary>
		/// <param name="writer"></param>
		public override void Render(System.IO.TextWriter writer)
		{
			return;
		}


		/// <summary>
		/// Relative path to REST endpoint for current call
		/// </summary>
		virtual public string GetRestEndpoint() {
			return null;
		}
		/// <summary>
		/// Absolute path to REST endpoint for current call
		/// </summary>
		/// <param name="basePath">Root of the URL.  Specific parameters appended after</param>
		virtual public string GetRestEndpoint(string basePath)
		{
			if (!string.IsNullOrEmpty(basePath))
			{
				if (!basePath.EndsWith("/"))
				{
					basePath += "/";
				}
			}
			return basePath + GetRestEndpoint();
		}

		/// <summary>
		/// Type of data expected to be returned as a result of this
		/// tag being processed.  This may be used by a deserializer
		/// </summary>
		virtual public Type ExpectedDataType
		{
			get
			{
				return typeof(object);
			}
		}


		/// <summary>
		/// Returns an array of key dependencies for this item.
		/// Keys are the sanitized root keys, not the full variable expressions.
		/// </summary>
		/// <returns></returns>
		public string[] GetDynamicKeyDependencies()
		{
			if (!HasDynamicParameters())
			{
				return new string[] { };
			}
			else
			{
				List<string> keys = new List<string>();
				foreach (DictionaryEntry keyset in this.AttributesCollection)
				{
					string val = keyset.Value as string;
					if (string.IsNullOrEmpty(val))
					{
						continue;
					}
					string expr = ExtractVariable(val);
					if (!string.IsNullOrEmpty(expr))
					{
						keys.Add(DataContext.GetVariableExpressionRootKey(expr));
					}
				}
				return keys.ToArray();
			}
		}

		/// <summary>
		/// Extracts the variable component from an arbitrary string
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		string ExtractVariable(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return null;
			}
			if (!source.Contains(DataContext.VARIABLE_START)
				&& !source.Contains(DataContext.VARIABLE_END))
			{
				return null;
			}
			int startPos = source.IndexOf(DataContext.VARIABLE_START);
			int endPos = source.IndexOf(DataContext.VARIABLE_END, startPos);
			if (endPos == -1)
			{
				return null;
			}
			return source.Substring(startPos, endPos - startPos + DataContext.VARIABLE_END.Length);
		}



		/// <summary>
		/// Determines if the control is making use of dynamic parameters
		/// </summary>
		/// <returns></returns>
		public bool HasDynamicParameters()
		{
			foreach (DictionaryEntry keyset in this.AttributesCollection)
			{
				string val = keyset.Value as string;
				if (string.IsNullOrEmpty(val))
				{
					continue;
				}
				if (val.Contains(DataContext.VARIABLE_START))
				{
					return true;
				}
			}

			return false;
		}


		/// <summary>
		/// Resolves dynamic parameters from the current result set
		/// </summary>
		/// <param name="results"></param>
		public virtual void ResolveDynamicParameters()
		{
			//need to use this to reset values from outside the foreach loop
			Dictionary<string, string> newAttributes = new Dictionary<string, string>();

			foreach (DictionaryEntry keyset in this.AttributesCollection)
			{
				//TODO - check excluded list

				string val = keyset.Value as string;
				if (string.IsNullOrEmpty(val))
				{
					continue;
				}
				if (val.Contains(DataContext.VARIABLE_START))
				{
					string dcVal = this.MyDataContext.CalculateVariableValue(val);
					newAttributes.Add(keyset.Key as string, dcVal);
				}
			}

			if (newAttributes.Count > 0)
			{
				foreach (KeyValuePair<string, string> keyset in newAttributes)
				{
					this.SetAttribute(keyset.Key, keyset.Value);
				}
			}
		}

	}
}
