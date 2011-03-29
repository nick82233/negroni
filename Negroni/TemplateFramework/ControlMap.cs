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
using Negroni.TemplateFramework.Parsing;

namespace Negroni.TemplateFramework
{

	/// <summary>
	/// Mapping object to represent a control, it's offset type,
	/// and the markup tag used.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("Tag: {MarkupTag} Control: {ControlType.Name}  OffsetKey: {OffsetKey}")]
	public class ControlMap
	{
#if DEBUG
		public string myId = Guid.NewGuid().ToString();
#endif

		public string MarkupTag = null;
		public string AttributeTagAlternative = null;
		public int AttributeTagPrecedenceWeight = 0;

		private List<string> _additionalMarkupTags = null;

		/// <summary>
		/// Markup tags in addition to the main MarkupTag.
		/// This allows for the same control to operate under different tags
		/// as the spec changes.
		/// </summary>
		public List<string> AdditionalMarkupTags
		{
			get
			{
				if (_additionalMarkupTags == null)
				{
					_additionalMarkupTags = new List<string>();
				}
				return _additionalMarkupTags;
			}
		}

		/// <summary>
		/// For elements that must have an additional attribute to be valid
		/// </summary>
		public string AttributeTagDependentKey = null;
		/// <summary>
		/// Required value of dependent element attribute
		/// </summary>
		public string AttributeTagDependentValue = null;

		//public OffsetItemType OffsetType;
		public string OffsetKey = null;
		public Type ControlType = null;
		public bool IsContextGroupContainer = false;
		/// <summary>
		/// True when the control being described is a Root level element
		/// </summary>
		public bool IsRootElement = false;

		/// <summary>
		/// True when the item may perform both as a Root or as a nested item.
		/// </summary>
		public bool IsOptionalRootElement = false;

		private List<ParseContext> _originalContextGroups = null;

		/// <summary>
		/// Originally requested context groups for this control.
		/// Note: it may appear in other context groups
		/// </summary>
		public List<ParseContext> OriginalContextGroups
		{
			get
			{
				if (_originalContextGroups == null)
				{
					_originalContextGroups = new List<ParseContext>();
				}
				return _originalContextGroups;
			}
		}

		public ControlMap() { }

		public ControlMap(string markupTag, Type controlType)
			: this(markupTag, markupTag.Replace(":", "_"), controlType)
		{}


		public ControlMap(string markupTag, string offsetKey, Type controlType)
		{
			this.MarkupTag = markupTag;
			this.OffsetKey = offsetKey;
			this.ControlType = controlType;
		}

		/// <summary>
		/// True if there is a dependent attribute which must be tested as well as the MarkupTag
		/// </summary>
		/// <returns></returns>
		public bool HasAttributeDependency()
		{
			return !(String.IsNullOrEmpty(this.AttributeTagDependentKey));
		}

		/// <summary>
		/// Set the MarkupTag value and initialize the OffsetKey, if not already specified.
		/// </summary>
		/// <param name="tag"></param>
		public void SetMarkupTag(string tag)
		{
			if (string.IsNullOrEmpty(tag)) return;

			if (string.IsNullOrEmpty(MarkupTag))
			{
				MarkupTag = tag;
			}
			else
			{
				AdditionalMarkupTags.Add(tag);
			}

			if (String.IsNullOrEmpty(OffsetKey))
			{
				OffsetKey = tag.Replace(":", "_");
			}
		}

		/// <summary>
		/// Generates an appropriate markup tag, wrapping contents
		/// </summary>
		/// <param name="contents"></param>
		/// <returns></returns>
		public string ToTag(string contents)
		{
			return ToBeginTag() + contents + ToEndTag();
		}
		/// <summary>
		/// Generates an appropriate markup tag
		/// </summary>
		/// <returns></returns>
		public string ToBeginTag()
		{
			if (HasAttributeDependency())
			{
				return string.Format("<{0} {1}=\"{2}\" >", MarkupTag, this.AttributeTagDependentKey, this.AttributeTagDependentValue);
			}
			else
			{
				return "<" + MarkupTag + ">";
			}

		}
		/// <summary>
		/// Generates an appropriate closing markup tag.
		/// </summary>
		/// <returns></returns>
		public string ToEndTag()
		{
			return "</" + MarkupTag + ">";
		}
	}
}
