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
using Negroni.OpenSocial.OSML.Controls;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget.Controls; using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// Represents a &lt; UserPref &gt; item in GadgetXML markup
	/// </summary>
	/// <summary>
	/// Gadget module preferences section
	/// </summary>
	[MarkupTag("Icon")]
	[ContextGroup(typeof(ModulePrefs))]
	public class ModuleIcon : BaseGadgetControl
	{
		const string ALLOWED_MODE_BASE64 = "base64";

		public ModuleIcon() 
		{
			this.MyParseContext = new ParseContext(typeof(ModulePrefs));
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="content">Raw content characters</param>
		public ModuleIcon(string markup)
			: this()
		{
			LoadTag(markup);
		}


		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			if (this.HasAttributes())
			{
				this.Mode = GetAttribute("mode");
				this.MimeType = GetAttribute("type");
			}
			if (this.Mode != ALLOWED_MODE_BASE64)
			{
				this.Src = InnerMarkup;
			}
		}

		/// <summary>
		/// Source URI of the icon file
		/// </summary>
		public string Src { get; set; }

		private string _iconData = null;
		/// <summary>
		/// Base64 encoded icon data
		/// </summary>
		public string IconData
		{
			get
			{
				if (!string.IsNullOrEmpty(Mode) && Mode == ALLOWED_MODE_BASE64)
				{
					return InnerMarkup;
				}
				return null;
			}
			set
			{
				_iconData = value;
				if (!string.IsNullOrEmpty(_iconData))
				{
					Mode = ALLOWED_MODE_BASE64;
				}
			}
		}

		/// <summary>
		/// Mime type of this image specified by the "type" attribute.
		/// </summary>
		public string MimeType { get; set; }

		private string _mode = null;
		/// <summary>
		/// Encoding mode allowed.  Only base64 is currently allowed
		/// </summary>
		public string Mode { get; set; }

		/// <summary>
		/// Render will invoke outputting this as client side javascript
		/// </summary>
		/// <param name="writer"></param>
		override public void Render(TextWriter writer)
		{
			return;
		}

	}
}
