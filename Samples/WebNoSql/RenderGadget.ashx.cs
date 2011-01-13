using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;

using WebNoSql.SiteOSML;

namespace WebNoSql
{
	/// <summary>
	/// Summary description for RenderGadget
	/// </summary>
	public class RenderGadget : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			string gadgetFile = context.Request["gadget"];

			if (!string.IsNullOrEmpty(gadgetFile))
			{
				string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GadgetFileList.GADGET_DIRECTORY + "\\" + gadgetFile);
				if (File.Exists(filePath))
				{
					string gadgetString = null;
					using (StreamReader sr = new StreamReader(filePath))
					{
						gadgetString = sr.ReadToEnd();
					}
					GadgetMaster gm = GadgetMaster.CreateGadget("gadget_v1.0", gadgetString);
					context.Response.StatusCode = 200;
					context.Response.ContentType = "text/html";
					StreamWriter writer = new StreamWriter(context.Response.OutputStream);
					gm.RenderContent(writer); //or  context.Response.Write(gm.RenderToString());
				}
				else
				{
					context.Response.ContentType = "text/plain";
					context.Response.StatusCode = 400;
					context.Response.Write("File not found: " + gadgetFile);
				}
			}

		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}