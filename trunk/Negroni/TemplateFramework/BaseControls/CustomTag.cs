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
//using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Instance of a custom-defined tag.  The tag used must correspond to
	/// a tag template previously registered with the <c>CustomTagFactory</c>
	/// instance in this object's <c>GadgetMaster</c>
	/// </summary>
	/// <remarks>
	/// This object will look up a reference to its corresponding <c>OsTagTemplate</c>
	/// that is registered with the <c>CustomTagFactory</c>.  The RawMarkup will
	/// contain all markup for this tag instance.
	/// </remarks>
	[OffsetKey("CustomTag")]
	public class CustomTag : BaseGadgetControl
	{

		readonly string VARIABLE_ELEMENT = "os:Var";

		#region Constructors

		public CustomTag() { }


		public CustomTag(string tag)
		{
			if (string.IsNullOrEmpty(tag))
			{
				throw new ArgumentNullException("Tag must be specified");
			}
			this.MarkupTag = tag;
		}

		public CustomTag(string tag, string markup)
			:this(tag)
		{
			LoadTag(markup);
			this.TagTemplate = MyRootMaster.MasterCustomTagFactory.GetTagTemplate(tag);
			if (null == TagTemplate)
			{
				throw new Exception("Specified tag: " + tag + " is not defined");
			}
		}

		public CustomTag(string tag, CustomTagTemplate template)
		{
			MyRootMaster = template.MyRootMaster;
			this.MarkupTag = tag;
			this.TagTemplate = template;
		}

		public CustomTag(string tag, CustomTagTemplate template, string markup)
			:this(tag, template)
		{
			LoadTag(markup);
		}

		#endregion

		private CustomTagTemplate _tagTemplate = null;
		/// <summary>
		/// Template definition used when rendering this custom tag
		/// </summary>
		public CustomTagTemplate TagTemplate
		{
			get
			{
				return _tagTemplate;
			}
			private set
			{
				_tagTemplate = value;
			}
		}


		/// <summary>
		/// Loads and parses the contents of this tag instance
		/// </summary>
		/// <param name="markup"></param>
		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);

			//parse elements
			LoadElementParameters();


			//parse attributes
			//these overwrite matching elements
			if (this.HasAttributes())
			{
				foreach (DictionaryEntry entry in AttributesCollectionCased)
				{
					string value = entry.Value as string;
					//todo - make sure not a string block
					if (!string.IsNullOrEmpty(value) && DataContext.IsVariable(value) && value.Contains(LocalParameterContextKey))
					{
						Parameters[(string)entry.Key] = MyRootMaster.MasterDataContext.GetVariableObject(value);
					}
					else
					{
						Parameters[(string)entry.Key] = value;
					}
				}
			}

		}

		/// <summary>
		/// Parses internal elements and places them in parameter values.
		/// </summary>
		/// <remarks>
		/// The parse only goes down one level.
		/// Nested markup will be placed as a chunk in a parameter key based
		/// on the top level tag.
		/// </remarks>
		private void LoadElementParameters()
		{
			if (IsEmptyElement(this.RawTag, this.MarkupTag))
			{
				return;
			}

			if (!IsTagBalancedElement(this.RawTag))
			{
				throw new System.Xml.XmlException("Custom tag contents is not valid XML");
			}

			int startPos, endPos;
			string guts;
			startPos = RawTag.IndexOf(">");
			endPos = RawTag.LastIndexOf("<");

			if (startPos == -1 || endPos == -1 || startPos + 1 >= endPos-1)
			{
				return;
			}
			startPos++;
			guts = RawTag.Substring(startPos, endPos - startPos);
			startPos = guts.IndexOf("<");
			while (startPos > -1)
			{
				int nextTagEnd = guts.IndexOf(">", startPos);
				if (nextTagEnd == -1)
				{
					break;
				}
				string tag = ControlFactory.GetTagName(guts.Substring(startPos, nextTagEnd - startPos));
				endPos = guts.IndexOf("</" + tag, startPos);

				bool isEmptyElement = false;
				//look for empty tag parameter
				if (endPos == -1)
				{
					endPos = guts.IndexOf("/>", startPos);
					if (endPos == -1)
					{
						break;
					}
					else
					{
						endPos += 2;
					}
					isEmptyElement = true;
				}
				else{
					endPos = guts.IndexOf(">", endPos);
					if(endPos == -1){
						break;
					}
					else{
						endPos++;
					}
				}
				string fullTag = guts.Substring(startPos, endPos - startPos);
//				string tag = ControlFactory.GetTagName(fullTag);
				GadgetLiteral tmpElem = new GadgetLiteral(fullTag);
				string innerMarkup;
				if (isEmptyElement)
				{
					innerMarkup = null;
				}
				else
				{
					int innerStart = fullTag.IndexOf(">");
					int innerEnd = fullTag.LastIndexOf("<");
					if (innerStart > -1 && innerEnd > -1
						&& innerEnd > innerStart)
					{
						innerMarkup = fullTag.Substring(innerStart + 1, innerEnd - innerStart - 1);
					}
					else
					{
						innerMarkup = null;
					}
				}
				if (tag.Equals(VARIABLE_ELEMENT))
				{
					if (tmpElem.HasAttribute("key"))
					{
						if (tmpElem.HasAttribute("value"))
						{
							this.Parameters[tmpElem.GetAttribute("key")] = tmpElem.GetAttribute("value");
						}
						else
						{
							this.Parameters[tmpElem.GetAttribute("key")] = innerMarkup;
						}
					}
				}
				else
				{
					if (tmpElem.HasAttributes())
					{
						Dictionary<string, string> paramObj = new Dictionary<string, string>();
						string[] attrKeys = tmpElem.GetAttributeKeys();
						for (int i = 0; i < attrKeys.Length; i++)
						{
							paramObj.Add(attrKeys[i], tmpElem.GetAttribute(attrKeys[i]));
						}
						if (!string.IsNullOrEmpty(innerMarkup))
						{
							paramObj["innerXML"] = innerMarkup;
						}
						Parameters[tag] = paramObj;
					}
					else
					{
						// multiple matching elements - flip to an array
						if (Parameters.ContainsKey(tag))
						{
							object existingValue = Parameters[tag];
							List<string> valueList;
							if (existingValue is string)
							{
								valueList = new List<string>();
								valueList.Add(existingValue as string);
								valueList.Add(innerMarkup);
								Parameters[tag] = valueList;
							}
							else if (existingValue is List<string>)
							{
								valueList = (List<string>)existingValue;
								valueList.Add(innerMarkup);
							}
						}
						else
						{
							this.Parameters[tag] = innerMarkup;
						}
					}
				}

				startPos = guts.IndexOf("<", endPos);
			}

		}

		/// <summary>
		/// Parses to see if this is an empty tag element.
		/// Do a manual parse to avoid overhead of XmlDocument object
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static bool IsEmptyElement(string markup, string tag)
		{
			if (string.IsNullOrEmpty(markup))
			{
				return true;
			}

			int firstEnd = markup.IndexOf(">");
			if (firstEnd == -1)
			{
				return true; //malformed.  perhaps throw
			}
			if (firstEnd == markup.Length - 1)
			{
				return true;
			}
			firstEnd++; //move beyond end tag
			int nextStart = markup.IndexOf("<", firstEnd);
			int tagEnd = markup.IndexOf(tag, firstEnd);
			if(tagEnd == -1){
				return true; //malformed, perhaps throw
			}
			string guts = markup.Substring(firstEnd, tagEnd - firstEnd);
			int endStartBraket = guts.LastIndexOf("<");
			if (nextStart == endStartBraket)
			{
				return true;
			}
			else
			{
				guts = guts.Substring(0, endStartBraket);
			}
			return (String.IsNullOrEmpty(guts.Trim()));
		}


		/// <summary>
		/// Creates a copy of the current custom tag without instance markup
		/// </summary>
		/// <returns></returns>
		public CustomTag NewInstance()
		{
			return NewInstance(null);
		}
		/// <summary>
		/// Creates a copy of current custom tag with new instance markup
		/// </summary>
		/// <param name="markup"></param>
		/// <returns></returns>
		public CustomTag NewInstance(string markup)
		{
			CustomTag newTag = new CustomTag();
			newTag.MarkupTag = this.MarkupTag;
			newTag.TagTemplate = this.TagTemplate;
			if (!string.IsNullOrEmpty(markup))
			{
				newTag.LoadTag(markup);
			}
			newTag.MyRootMaster = this.MyRootMaster;
			return newTag;
		}

		


		public override void Render(System.IO.TextWriter writer)
		{
			if (null == this.TagTemplate) return;

			//set data context
			RegisterPrivateContext();
			this.TagTemplate.SetLocalVariableContextKey(LocalParameterContextKey);
			this.TagTemplate.Render(writer);
			UnRegisterPrivateContext();
		}

		
		/// <summary>
		/// Adds the private context to the processing data context
		/// </summary>
		protected virtual void RegisterPrivateContext()
		{
			MyDataContext.RegisterLocalValue(LocalParameterContextKey, Parameters, true);
		}

		/// <summary>
		/// Removes the private context to the processing data context
		/// </summary>
		protected virtual void UnRegisterPrivateContext()
		{
//			object current = MyDataContext.GetVariableObject(LocalParameterContextKey);
			MyDataContext.RemoveLocalValue(LocalParameterContextKey);
		}

		private string _localParameterContextKey = "My";

		/// <summary>
		/// Special key to register private parameters under in the DataContext.
		/// By default this value is "My"
		/// </summary>
		public string LocalParameterContextKey
		{
			get
			{
				return _localParameterContextKey;
			}
			set
			{
				_localParameterContextKey = value;
			}
		}


		#region ICloneable Members

		public object Clone()
		{
			return NewInstance();
		}

		#endregion

		#region Parameter support

		private Dictionary<string, object> _parameters = null;

		/// <summary>
		/// Accessor for parameters.
		/// Any dynamic parameters are resolved at render time
		/// </summary>
		public Dictionary<string, object> Parameters
		{
			get
			{
				if (_parameters == null)
				{
					_parameters = new Dictionary<string, object>();
				}
				return _parameters;
			}
		}

		/// <summary>
		/// Tests to see if this instance has any custom parameters defined.
		/// </summary>
		/// <returns></returns>
		public bool HasParameters()
		{
			return (_parameters.Count > 0);
		}

		#endregion

	}
}
