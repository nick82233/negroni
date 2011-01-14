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
using Negroni.DataPipeline;
using Negroni.TemplateFramework.Parsing;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Base class to inherit from for controls that may contain other controls.
	/// Used alone this will render children in a div.
	/// </summary>
	[OffsetKey(ControlFactory.RESERVEDKEY_GENERIC_CONTAINER)]
	public class BaseContainerControl : BaseGadgetControl
	{

		/// <summary>
		/// Collection of controls held within this control
		/// </summary>
		public List<BaseGadgetControl> Controls = new List<BaseGadgetControl>();

		/// <summary>
		/// Flag to write a wrapping div tag around all elements.
		/// By default this is false so that only control markup is written
		/// </summary>
		public bool WriteDivWrapper { get; set; }


		/// <summary>
		/// CustomTagFactory used by thistemplate for rendering custom tags
		/// </summary>
		protected CustomTagFactory MyCustomTagFactory
		{
			get
			{
				return MyRootMaster.MasterCustomTagFactory;
			}
		}


		private ControlFactory _myControlFactory = null;

		/// <summary>
		/// Hook to the ControlFactory which should be used for this control.
		/// Defaults to the Gadget ControlFactory instance.
		/// </summary>
		public ControlFactory MyControlFactory
		{
			get
			{
				if (_myControlFactory == null)
				{
					if (MyRootMaster != null
						&& MyRootMaster != this)
					{
						_myControlFactory = MyRootMaster.MyControlFactory;
					}
					else
					{
						throw new MissingControlFactoryException("Missing control factory");
					}
				}
				return _myControlFactory;
			}
			set
			{
				_myControlFactory = value;
			}
		}


		/// <summary>
		/// Tests to see if the MyControlFactory value has been explicitly set
		/// </summary>
		/// <returns></returns>
		public bool HasControlFactorySet()
		{
			return (_myControlFactory != null);
		}


		/// <summary>
		/// Parse the contents (raw markup) of the template and build out the control tree.
		/// This is the general entry point for building control trees.  Make sure you always
		/// trigger base.Parse() if this is overridden.
		/// <para>
		/// Most cases can be handled by overriding AddControl instead.
		/// </para>
		/// </summary>
		override public void Parse()
		{
			if (!OffsetsAreParsed())
			{
				// MyControlFactory.GetChildControlContextGroup(this.GetType(), MyParseContext)
				LoadOffset(MyRootMaster.OffsetParser.ParseOffsets(this.RawTag, MyParseContext));
			}
			if (this.Controls.Count > 0)
			{
				Controls.Clear();
			}
			if (null != MyOffset)
			{
				BuildControlTreeFromOffsets();
			}
			IsParsed = true;
		}

		/// <summary>
		/// Given a populated <c>OffsetList</c>, build the controls from the RawTag.
		/// Make sure you invoke this method, if overridden.
		/// <para>
		/// Most cases can be handled by overriding AddControl instead.
		/// </para>
		/// </summary>
		protected virtual void BuildControlTreeFromOffsets()
		{
			if (null == MyOffset)
			{
				throw new ArgumentException("Offsets must be built prior to loading control tree");
			}

			int i = 0;
			int markupEnd, markupStart, initialScriptAdjustment = 0;
			int startAdjustment = 0, endAdjustment = 0;
			OffsetItem item;
			string blob;

			// TODO: Abstract this out for non-hard coding
			if (("GadgetRoot" == MyOffset.OffsetKey || "Templates" == MyOffset.OffsetKey
				|| MyOffset.OffsetKey == this.MyControlFactory.RootElement.OffsetKey) 				
				&& MyOffset.Position > 0)
			{
				startAdjustment = MyOffset.Position;
				endAdjustment = startAdjustment;
			}
			if (MyOffset.HasChildList())
			{
				for (i = 0; i < MyOffset.ChildOffsets.Count; i++)
				{
					item = MyOffset.ChildOffsets[i];
					int nextItemStart = 0;
					if (i < MyOffset.ChildOffsets.Count - 1)
					{
						nextItemStart = MyOffset.ChildOffsets[i + 1].Position;
					}
					//markupEnd = item.GetAbsoluteEndPosition(nextItemStart);
					markupEnd = GetMarkupEndPosition(MyOffset.ChildOffsets, i);
					if (markupEnd <= 0)
					{
						markupEnd = RawTag.Length;
					}
					else if (endAdjustment > 0)
					{
						markupEnd += endAdjustment;
					}
					//					GetMarkupEndPosition(MyOffset.ChildOffsets, i);
					if (0 == i && 0 == item.Position)
					{
						markupStart = initialScriptAdjustment;
						markupEnd = markupEnd - initialScriptAdjustment;
					}
					else
					{
						markupStart = item.Position + startAdjustment;
					}

					blob = RawTag.Substring(markupStart, markupEnd - markupStart);

					this.AddResolveControlTag(item, blob);

				}
			}
		}


		/// <summary>
		/// Extracts the tag name from the markup blob
		/// </summary>
		/// <param name="markup"></param>
		/// <returns></returns>
		protected string GetBlobTagName(string markup)
		{
			if (string.IsNullOrEmpty(markup)) return string.Empty;
			if (markup[0] != '<') return string.Empty;

			int tagEnd = markup.IndexOf(">", 1);
			int firstSpace = markup.IndexOf(" ", 1);
			int endPart;
			if (-1 == tagEnd)
			{
				throw new Exception("Malformed custom tag - parse failure");
			}
			if (-1 == firstSpace)
			{
				endPart = tagEnd;
			}
			else
			{
				endPart = Math.Min(tagEnd, firstSpace);
			}

			if (-1 == endPart)
			{
				return string.Empty;
			}

			string tag = markup.Substring(1, endPart - 1);
			return tag;
		}

		/// <summary>
		/// Creates and resolves an OSML control and adds it to the control tree.
		/// Override this method to allow for special logic - ex: custom tags
		/// </summary>
		/// <param name="offsetItem"></param>
		/// <param name="markup"></param>
		/// <returns></returns>
		protected virtual BaseGadgetControl AddResolveControlTag(OffsetItem offsetItem, string markup)
		{
			if (ControlFactory.RESERVEDKEY_CUSTOM_TAG == offsetItem.OffsetKey)
			{
				string customTagName = GetBlobTagName(markup);
				return this.AddControl(MyCustomTagFactory.CreateTagInstance(customTagName, markup));
			}
			else
			{
				return this.AddControl(MyControlFactory.CreateControl(offsetItem, markup, MyControlFactory.GetChildControlContextGroup(this.GetType(), this.MyParseContext), MyRootMaster));
			}
		}



		/// <summary>
		/// Adds a control to the controls collection and sets references.
		/// Initializes any container pointers as well.
		/// Override for specialized handling.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		virtual public BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			if (null == control)
			{
				return null;
				//also log here
			}
			control.MyRootMaster = this.MyRootMaster;
			control.MyTemplate = this.MyTemplate;
			control.Parent = this;
			if (control.ViewMask <= 0)
			{
				control.ViewMask = this.ViewMask;
			}
			Controls.Add(control);

			if (MyRootMaster != null && 
				control is DataPipeline.IExternalDataSource && 
				((DataPipeline.IExternalDataSource)control).ResolveLocation == DataPipeline.ResolveAt.Server)
			{
				MyRootMaster.RegisterExternalServerRenderControl((DataPipeline.IExternalDataSource)control, control.ViewMask);
			}

			return control;
		}



		/// <summary>
		/// Recursively counts all controls in the control tree.
		/// This performs a DFS traversal to obtain the count.
		/// Only call this if you really, really need to know.
		/// </summary>
		/// <returns></returns>
		public int CountInternalControls()
		{
			return RecurseCountControls(this) - 1;
		}

		/// <summary>
		/// Return count control plus all child controls and their children
		/// by recursively traversing the tree.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		private int RecurseCountControls(BaseGadgetControl control)
		{
			if (control is BaseContainerControl)
			{
				BaseContainerControl container = (BaseContainerControl)control;
				if (container.Controls.Count == 0)
				{
					return 1;
				}
				else
				{
					int total = 1; //count self

					foreach (BaseGadgetControl child in container.Controls)
					{
						total += RecurseCountControls(child);
					}
					return total;
				}
			}
			else
			{
				return 1;
			}
		}

		/// <summary>
		/// Render all child controls wrapped in a div.
		/// Override this for specialized behavior
		/// </summary>
		/// <param name="writer"></param>
		public override void Render(System.IO.TextWriter writer)
		{

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
		}


		/// <summary>
		/// Flag to indicate that this control is in it's disposal method
		/// so that a stack overflow doesn't occur when the child controls dispose.
		/// </summary>
		public bool InDisposer { get; set; }



		/// <summary>
		/// Clean up this control and all child controls
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			InDisposer = true;
			if (disposing)
			{
				if (this.Controls.Count > 0)
				{
					foreach (BaseGadgetControl item in Controls)
					{
						item.Dispose();
					}
				}

				base.Dispose(disposing);
			}
			InDisposer = false;
		}

	}
}
