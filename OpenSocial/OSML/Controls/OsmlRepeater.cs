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
using System.Text;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls; 
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// Any element which as had a repeat attribute applied.
	/// </summary>
	[MarkupTag("os:Repeat")]
	[AttributeTagAlternative("repeat", 10)]
	public class OsmlRepeater : BaseRepeaterControl
	{
		public OsmlRepeater() { }

		string _repeatOnKey = null;

		/// <summary>
		/// Contextual key used for repeater
		/// </summary>
		public string RepeatOnKey
		{
			get
			{
				return _repeatOnKey;
			}
			set
			{
				_repeatOnKey = CleanVariableKey(value);
			}
		}

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);

			bool includeWrappingTag = false;

			RepeatOnKey = this.GetAttribute("expression");
			if (string.IsNullOrEmpty(RepeatOnKey))
			{
				//used when attr based
				RepeatOnKey = this.GetAttribute("repeat");
				includeWrappingTag = true;
				//check for conditional
				base.LoopConditionalExpression = this.GetAttribute("if");
			}
			//set scopeDataContext
			DataContext scopeData = new DataContext();

			string prequel = null;
			string sequel = null;

			if (includeWrappingTag)
			{
				prequel = GetFilteredOpenTag();

				if (!this.IsEmptyTag)
				{
					int lastIndex = RawTag.LastIndexOf("<");
					if (lastIndex > -1)
					{
						sequel = RawTag.Substring(lastIndex);
					}
				}
			}

			base.LoopPrequel = prequel;
			base.LoopSequel = sequel;
			base.LoopItemKey = GetCurrentLoopItemVariable();
			base.LoopContextVariableKey = GetContextVariable();
			base.RepeatedDataKey = GetRealRepeatKey();
		}


		public string GetRealRepeatKey()
		{
			if(String.IsNullOrEmpty(RepeatOnKey)){
				return string.Empty;
			}
			if (RepeatOnKey.StartsWith("Top."))
			{
				if (RepeatOnKey.Length > 4)
				{
					return RepeatOnKey.Substring(4);
				}
			}
			else
			{
				return RepeatOnKey;
			}
			return RepeatOnKey;
		}

		/// <summary>
		/// Constructs an opening tag for the repeat line which has server-processed
		/// attributes stripped out.  This is only used when the AttributeTagAlternative
		/// form of a repeated element is used.
		/// </summary>
		/// <remarks>
		/// TODO: safely HTML encode things
		/// </remarks>
		/// <returns></returns>
		string GetFilteredOpenTag()
		{
			string tag = base.GetBlobTagName(RawTag);
			if (HasAttributes())
			{
				foreach (DictionaryEntry entry in AttributesCollection)
				{
					if (!IsServerReservedAttribute((string)entry.Key))
					{
						string val = EscapeString((string)entry.Value);
						tag += " " + entry.Key + "=\"" + val + "\"";
					}
				}
			}
			if (this.IsEmptyTag)
			{
				return "<" + tag + " />";
			}
			else
			{
				if (tag.Contains("="))
				{
					return "<" + tag + " >";
				}
				else
				{
					return "<" + tag + ">";
				}
			}
		}

		private string EscapeString(string str)
		{
			if (String.IsNullOrEmpty(str)) return string.Empty;
			if (str.IndexOf("\"") == -1) return str;
			return str.Replace("\"", "&quot;");
		}

		/// <summary>
		/// Returns true for any attribute which is to be server processed.
		/// TODO: Implement
		/// </summary>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		bool IsServerReservedAttribute(string attributeName)
		{
			string[] resevedAttributes =
			{
				"repeat",
				"context",
				"var",
				"if"
			};

			for (int i = 0; i < resevedAttributes.Length; i++)
			{
				if (attributeName.Equals(resevedAttributes[i], StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
								
			}
			return false;
		}



		public override void Render(System.IO.TextWriter writer)
		{
			if (!this.HasAttributes())
			{
				return;
			}
			if (string.IsNullOrEmpty(RepeatOnKey))
			{
				return;
			}

			base.Render(writer);

		}

		/// <summary>
		/// Gets the variable name to use for the current item.
		/// Defaults to "Cur"
		/// </summary>
		/// <returns></returns>
		private string GetCurrentLoopItemVariable()
		{
			string var = null;
			if (this.HasAttribute("var"))
			{
				var = this.GetAttribute("var");
			}
			if (string.IsNullOrEmpty(var))
			{
				return "Cur";
			}
			else
			{
				return var;
			}
		}

		/// <summary>
		/// Gets the variable name to use for the Context special variable
		/// Defaults to "Context"
		/// </summary>
		/// <returns></returns>
		private string GetContextVariable()
		{
			string var = null;
			if (this.HasAttribute("context"))
			{
				var = this.GetAttribute("context");
			}
			if (string.IsNullOrEmpty(var))
			{
				return "Context";
			}
			else
			{
				return var;
			}
		}
	}
}
