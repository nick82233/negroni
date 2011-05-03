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
using System.Text;

namespace Negroni.TemplateFramework
{

	/// <summary>
	/// A custom-defined tag template.  This is used as the basis for
	/// rendering all instances of the custom tag.
	/// </summary>
	/// <remarks>
	/// An instance of this object is registered with the CustomTagFactory
	/// for use within a given RootElementMaster (XML gadget's) context or
	/// within a global CustomTagFactory attached to a ControlFactory instance.
	/// </remarks>
	public class CustomTagTemplate : BaseTemplate
	{
		/// <summary>
		/// Attribute identifying the tag being defined
		/// </summary>
		public const string ATTRIBUTE_TAGDEF = "tag";
		/// <summary>
		/// Attribute indicating if this template is registered for client-side use
		/// after initial processing
		/// </summary>
		public const string ATTRIBUTE_CLIENT_REGISTERED = "clientregister";


		#region Constructors

		public CustomTagTemplate()
		{
			MyTemplate = this;
		}

		public CustomTagTemplate(string tag)
			: this()
		{
			this.Tag = tag;
		}

		public CustomTagTemplate(string tag, string markup, RootElementMaster master)
			: this(tag)
		{
			this.MyRootMaster = master;
			LoadTag(markup);
		}

		
		public CustomTagTemplate(string tag, string markup, OffsetItem thisOffset, RootElementMaster master)
			: this(tag)
		{
			this.MyRootMaster = master;
			MyOffset = thisOffset;
			LoadTag(markup);
		}

		#endregion


		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			this.Tag = GetAttribute(ATTRIBUTE_TAGDEF);
			MyRootMaster.OffsetParser.AddNamespace(Prefix, string.Empty);

			string crs = GetAttribute(ATTRIBUTE_CLIENT_REGISTERED);
			if (!string.IsNullOrEmpty(crs))
			{
				bool val;
				if (Boolean.TryParse(crs, out val))
				{
					ClientRegister = val;
				}
			}

			BuildVariableTagsList(this);
		}

		/// <summary>
		/// Recursively register all VariableTag controls for easy reference
		/// </summary>
		/// <param name="currentControl"></param>
		protected void BuildVariableTagsList(BaseGadgetControl currentControl)
		{
			if (null == currentControl) return;

			if (currentControl is VariableTag)
			{
				VariableTags.Add((VariableTag)currentControl);
			}
			else if (currentControl is BaseContainerControl)
			{
				foreach (BaseGadgetControl child in ((BaseContainerControl)currentControl).Controls)
				{
					BuildVariableTagsList(child);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetLocalVariableContextKey(string scopedVariableKey)
		{
			if (string.IsNullOrEmpty(scopedVariableKey) || _variableTags == null)
			{
				return;
			}
			if (VariableTags.Count > 0)
			{
				foreach (VariableTag varTag in VariableTags)
				{
					varTag.ScopeVariableKey = scopedVariableKey;
				}
			}		
		}

		private List<VariableTag> _variableTags = null;

		/// <summary>
		/// Any VariableTag definitions within this template
		/// </summary>
		public List<VariableTag> VariableTags
		{
			get
			{
				if (_variableTags == null)
				{
					_variableTags = new List<VariableTag>();
				}
				return _variableTags;
			}
		}


		/// <summary>
		/// Overrides the default registered markup tag for this control for this instance only.
		/// This is primarily used for testing purposes.
		/// </summary>
		/// <param name="markupTag"></param>
		public void OverrideInstanceMarkupTag(string markupTag)
		{
			this.MarkupTag = markupTag;
		}


		private bool _clientRegister = true;
		/// <summary>
		/// Indicates if this template should be registered for client-side use
		/// after initial processing.  Default value is true
		/// </summary>
		public bool ClientRegister
		{
			get
			{
				return _clientRegister;
			}
			set
			{
				_clientRegister = value;
			}
		}


		private string _tag = null;
		/// <summary>
		/// Custom tag this template is defining.  This is identified in the @tag attribute.
		/// </summary>
		public string Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
				_localTag = null;
				_prefix = null;
			}
		}


		private string _prefix = null;
		/// <summary>
		/// Namespace prefix on the markup element tag
		/// </summary>
		public string Prefix
		{
			get
			{
				if (null == _prefix)
				{
					if (null == Tag)
					{
						return null;
					}
					int pos = Tag.IndexOf(":");
					if (pos > -1)
					{
						_prefix = Tag.Substring(0, pos);
					}
					else
					{
						_prefix = string.Empty;
					}
				}
				return _prefix;

			}
		}

		private string _localTag = null;
		/// <summary>
		/// Local markup element tag without the namespace prefix
		/// </summary>
		public string LocalTag
		{
			get
			{
				if (null == _localTag)
				{
					if (null == Tag)
					{
						return null;
					}
					int pos = Tag.IndexOf(":");
					if (pos > -1 && pos < Tag.Length - 1)
					{
						_localTag = Tag.Substring(pos + 1);
					}
				}
				return _localTag;
			}
		}

		public override void Render(System.IO.TextWriter writer)
		{
			if (this.Controls.Count < 512 && this.Controls.Count < this.MyOffset.ChildOffsets.Count)
			{
				this.Controls.Clear();
				this.BuildControlTreeFromOffsets();
			}
			base.Render(writer);
			/*
			if (WriteDivWrapper)
			{
				writer.Write("<div");
				if (!String.IsNullOrEmpty(ID))
				{
					writer.Write(String.Format(" id=\"{0}\">{1}", ID, writer.NewLine));
				}
			}
			if (Controls.Count > 0)
			{
				foreach (BaseGadgetControl control in Controls)
				{
					control.Render(writer);
				}
			}
			if (WriteDivWrapper)
			{
				writer.Write(writer.NewLine + "</div >" + writer.NewLine);
			}
			 * */
		}
	}
}
