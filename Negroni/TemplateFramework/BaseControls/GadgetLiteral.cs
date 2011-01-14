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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Raw content literal control.  This contains no processed tags, 
	/// but may contain embedded script values.
	/// </summary>
	public class GadgetLiteral : BaseGadgetControl
	{

		public const string CDATA_START_TAG = "<![CDATA[";
		public const string CDATA_END_TAG = "]]>";

		/// <summary>
		/// Reserved offset key used for Literal controls
		/// </summary>
		static public readonly string DefaultOffsetKey = ControlFactory.RESERVEDKEY_LITERAL;


		public GadgetLiteral() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content">Raw content characters</param>
		public GadgetLiteral(string markup)
		{
			LoadTag(markup);
		}

		private string _innerMarkup = null;

		/// <summary>
		/// For literals, the InnerMarkup is equivilent to RawTag if this is not
		/// a tag balanced element
		/// </summary>
		public override string InnerMarkup
		{
			get
			{
				if (_innerMarkup == null)
				{
					if (string.IsNullOrEmpty(MarkupTag))
					{
						_innerMarkup = RawTag;
					}
					else
					{
						_innerMarkup = GetInnerMarkup();
					}
				}
				return _innerMarkup;
			}
		}

		public override void LoadTag(string markup)
		{
			_innerMarkup = null;
			base.LoadTag(markup);
			if (string.IsNullOrEmpty(markup))
			{
				return;
			}
			markup = markup.Trim();
			if (markup.StartsWith("<") && markup.EndsWith(">")
				&& IsEncapsulatedElement(markup)
				&& !markup.StartsWith(CDATA_START_TAG))
			{
				MarkupTag = ControlFactory.GetTagName(markup);
			}
			else
			{
				MarkupTag = string.Empty;
			}
		}

		/// <summary>
		/// Flag to suppress any wrapping CDATA tags when rendering.
		/// When set to true this will strip out CDATA tags if they are
		/// the first element and enclose the entire contents.
		/// </summary>
		public bool SuppressCDATATags { get; set; }


		override public void Render(TextWriter writer)
		{
			if (!String.IsNullOrEmpty(RawTag))
			{
				if (SuppressCDATATags && RawTag.Contains(CDATA_START_TAG)
					&& RawTag.Trim().StartsWith(CDATA_START_TAG))
				{
					string newRaw;
					int startPos = RawTag.IndexOf(CDATA_START_TAG) + CDATA_START_TAG.Length;
					int endPos = RawTag.LastIndexOf(CDATA_END_TAG);
					newRaw = RawTag.Substring(startPos, endPos - startPos);
					//look for extra at the end
					if (endPos + CDATA_END_TAG.Length < RawTag.Length)
					{
						newRaw += RawTag.Substring(endPos + CDATA_END_TAG.Length);
					}

					writer.Write(ResolveDataContextVariables(newRaw, MyDataContext));
				}
				else
				{
					writer.Write(ResolveDataContextVariables(RawTag, MyDataContext));
				}
			}
		}
		
	}
}
