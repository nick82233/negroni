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

	}
}
