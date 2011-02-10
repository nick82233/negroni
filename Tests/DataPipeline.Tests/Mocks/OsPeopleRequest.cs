using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.DataContracts;

namespace Negroni.DataPipeline.Tests
{
	/// <summary>
	/// Base class for data controls
	/// </summary>
	[MarkupTag("os:PeopleRequest")]
	[ContextGroup(typeof(BaseContainerControl))]
	public class OsPeopleRequest : BaseDataControl
	{
		public OsPeopleRequest() : base() { }

		public OsPeopleRequest(string markup)
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

		//public override object InvokeTarget(object[] parameters)
		//{
		//    if (parameters != null && parameters.Length > 1)
		//    {
		//        List<Person> tmp = parameters[1] as List<Person>;
		//        if (tmp != null)
		//        {
		//            return tmp;
		//        }
		//        else
		//        {
		//            return null;
		//        }
		//    }
		//    return null;
		//}
    }
}
