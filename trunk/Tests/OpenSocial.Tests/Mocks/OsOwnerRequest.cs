using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;

namespace Negroni.OpenSocial.Test
{
	/// <summary>
	/// A person request for the specific owner
	/// </summary>
	[MarkupTag("os:OwnerRequest")]
	[ContextGroup(typeof(BaseContainerControl))]
	[ContextGroup(typeof(DataScript))]
	public class OsOwnerRequest : BaseDataControl
	{

		public OsOwnerRequest()
		{}


		public OsOwnerRequest(string markup)
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

        public override object InvokeTarget(object[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                return parameters[0];
            }
            return null;
        }
    }
}
