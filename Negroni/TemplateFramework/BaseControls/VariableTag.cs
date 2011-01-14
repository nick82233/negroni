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
using Negroni.DataPipeline;
using Negroni.DataPipeline.Serialization;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Base class for a tag-based variable declaration.
	/// </summary>
	public class VariableTag : BaseGadgetControl
	{
		#region Public props to control parsing
		/// <summary>
		/// Attribute name for the attribute that identifies the variable name.
		/// Default value is "key"
		/// </summary>
		public string VariableKeyAttributeName = "key";

		/// <summary>
		/// Attribute name for the attribute that identifies the variable value.
		/// Default value is "value"
		/// </summary>
		public string VariableValueAttributeName = "value";

		/// <summary>
		/// Flag to determine if variable value can be defined within the tag
		/// instead of just in the @value attribute.
		/// </summary>
		public bool AllowValueInTagContent = true;
		#endregion

		public override void LoadTag(string markup)
		{
			VariableKey = null;
			VariableRawValue = null;

			base.LoadTag(markup);
			
			if (HasAttributes())
			{
				VariableKey = GetAttribute(VariableKeyAttributeName);
				if (HasAttribute(VariableValueAttributeName))
				{
					VariableRawValue = GetAttribute(VariableValueAttributeName);
				}
			}
			if (string.IsNullOrEmpty(VariableRawValue))
			{
				VariableRawValue = InnerMarkup;
			}
		}

		public override void Render(TextWriter writer)
		{
			object value = GetVariableValue();
			if (MyDataContext.HasVariable(VariableKey))
			{
				MyDataContext.RemoveLocalValue(VariableKey);
			}
			if (string.IsNullOrEmpty(ScopeVariableKey))
			{
				this.MyDataContext.RegisterDataItem(VariableKey, value);
			}
			else
			{
				Dictionary<string, object> scopeDictionary = null;
				if (!MyDataContext.MasterData.ContainsKey(ScopeVariableKey))
				{
					scopeDictionary = new Dictionary<string, object>();
					MyDataContext.RegisterLocalValue(ScopeVariableKey, scopeDictionary);
				}
				else
				{
					scopeDictionary = MyDataContext.GetVariableObject(ScopeVariableKey) as Dictionary<string, object>;
				}
				if (scopeDictionary != null)
				{
					scopeDictionary.Add(VariableKey, value);
				}
				else
				{
					//TODO: REGISTER ERROR
				}
			}
		}


		virtual public object GetVariableValue()
		{
			object retval = null;

			if (VariableRawValue.Contains(DataContext.VARIABLE_START))
			{
				if (VariableRawValue.StartsWith(DataContext.VARIABLE_START)
					&& VariableRawValue.EndsWith(DataContext.VARIABLE_END))
				{
					return MyDataContext.CalculateVariableObjectValue(VariableRawValue);
				}
				else
				{
					//complex string
					retval = ResolveDataContextVariables(VariableRawValue, MyDataContext);

					if(JsonData.ValueIsObject((string)retval)
						|| JsonData.ValueIsArray((string)retval))
					{
						retval = new JsonData((string)retval);
					}
					return retval;
				}

			}
			else
			{
				double dVal;
				bool bVal;

				if (Double.TryParse(VariableRawValue, out dVal))
				{
					return dVal;
				}
				else if (Boolean.TryParse(VariableRawValue, out bVal))
				{
					return bVal;
				}
				else
				{
					if (JsonData.ValueIsObject(VariableRawValue)
						|| JsonData.ValueIsArray(VariableRawValue))
					{
						return new JsonData(VariableRawValue);
					}
					else
					{
						return VariableRawValue;
					}
				}
			}
		}

		/// <summary>
		/// Containing scope key to register the variable under.
		/// If null this variable is registered in the global scope
		/// during Render.  If this is a key, it registers under
		/// that root key
		/// </summary>
		/// <remarks>
		/// ScopeVariableKey = "My"
		/// VariableKey = "otherVar"
		/// Registered in dictionary under My with key "otherVar"
		/// </remarks>
		public string ScopeVariableKey { get; set; }

		/// <summary>
		/// Key used to refer to this variable
		/// </summary>
		public string VariableKey { get; set; }

		private string _variableRawValue = null;
		/// <summary>
		/// Raw string value defined for this variable.
		/// </summary>
		public string VariableRawValue
		{
			get
			{
				return _variableRawValue;
			}
			set
			{
				if (value != null)
				{
					_variableRawValue = value.Trim();
				}
				else
				{
					_variableRawValue = value;
				}
			}
		}
	}
}
