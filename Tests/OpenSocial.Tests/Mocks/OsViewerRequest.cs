using System;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;

namespace Negroni.OpenSocial.Tests
{
	/// <summary>
	/// Base class for data controls
	/// </summary>
	[MarkupTag("os:ViewerRequest")]
	[ContextGroup(typeof(BaseContainerControl))]
	[ContextGroup(typeof(DataScript))]
	public class OsViewerRequest : BaseDataControl
	{


		public OsViewerRequest()
		{}


		public OsViewerRequest(string markup)
		{
			LoadTag(markup);
		}


		/// <summary>
		/// Data Controls do not render.
		/// This may change
		/// </summary>
		/// <param name="writer"></param>
		public override void Render(System.IO.TextWriter writer)
		{
			return;
		}

    }
}
