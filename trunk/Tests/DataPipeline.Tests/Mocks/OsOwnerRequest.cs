using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;

namespace Negroni.DataPipeline.Tests
{
	/// <summary>
	/// A person request for the specific owner
	/// </summary>
	[MarkupTag("os:OwnerRequest")]
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
