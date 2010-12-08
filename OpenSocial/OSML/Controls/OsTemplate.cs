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
using System.Xml;
using System.Xml.Schema;
using Negroni.OpenSocial.Gadget;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget.Controls; using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// Template holder
	/// </summary>
	[MarkupTag("script")]
	[AttributeTagDependent("type", "text/os-template")]
	[ContextGroup(typeof(ContentBlock))]
	[ContextGroup(typeof(BaseContainerControl))]
	[ContextGroupContainer(true)]
	[OffsetKey("TemplateScript")]
	public class OsTemplate : BaseContainerControl
	{
		public OsTemplate() 
		{
			MyTemplate = this;
			MyParseContext = new ParseContext(typeof(ContentBlock));
		}

		public OsTemplate(string name) : this()
		{
			this.Name = name;
		}

		public OsTemplate(string name, string markup)
			: this(name)
		{
			LoadTag(markup);
		}

		public OsTemplate(string name, string markup, OffsetList offsets)
			: this(name)
		{
			LoadNewChildOffsets(offsets);
			LoadTag(markup);
		}

		public OsTemplate(string name, string markup, OffsetItem thisOffset)
			: this(name)
		{
			MyOffset = thisOffset;
			LoadTag(markup);
		}

		/// <summary>
		/// Confirms that a root level offset has been set for the MyOffset value.
		/// Adds it if it is not found
		/// </summary>
		protected override void ConfirmDefaultOffset()
		{
			if (null == MyOffset)
			{
				MyOffset = new OffsetItem(0, "OsTemplate");
			}
		}



		/// <summary>
		/// Anonymous name part to use when constructing a name
		/// </summary>
		public const string ANON_NAMEPART = "_unnamed_";

		private string _name = null;
		/// <summary>
		/// Unique name for this template for the purposes of targetting
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				IsAnonymous = String.IsNullOrEmpty(value);
				_name = value;
			}
		}



		/// <summary>
		/// True if this is an anonymous (unnamed) template.
		/// </summary>
		public bool IsAnonymous { get; set; }


	}
}
