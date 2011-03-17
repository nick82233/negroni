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
			string gadgetString = null;
			string controlFactory = "gadget_v1.0";

			if (!string.IsNullOrEmpty(gadgetFile))
			{
				string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GadgetFileList.GADGET_DIRECTORY + "\\" + gadgetFile);
				if (File.Exists(filePath))
				{
					
					using (StreamReader sr = new StreamReader(filePath))
					{
						gadgetString = sr.ReadToEnd();
					}
					GadgetMaster gm = GadgetMaster.CreateGadget(controlFactory, gadgetString);
					context.Response.StatusCode = 200;
					context.Response.ContentType = "text/html";
					StreamWriter writer = new StreamWriter(context.Response.OutputStream);
					gm.Render(writer); //or  context.Response.Write(gm.RenderToString());
				}
				else
				{
					context.Response.ContentType = "text/plain";
					context.Response.StatusCode = 400;
					context.Response.Write("File not found: " + gadgetFile);
				}
			}
			else if (context.Request.HttpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
			{
				//check for form post
				gadgetString = context.Request.Form["source"];
				if (string.IsNullOrEmpty(gadgetString))
				{
					context.Response.ContentType = "text/plain";
					context.Response.StatusCode = 400;
					context.Response.Write("No content posted to render");
				}
				else
				{
					//gadgetString = gadgetString.Trim();
					string tmp = context.Request.Form["parser"];
					if (!string.IsNullOrEmpty(tmp))
					{
						controlFactory = tmp;
					}
					ControlFactory cf = ControlFactory.GetControlFactory(controlFactory);

					RootElementMaster rm = cf.BuildControlTree(gadgetString);
					context.Response.StatusCode = 200;
					context.Response.ContentType = "text/html";
					StreamWriter writer = new StreamWriter(context.Response.OutputStream);
					writer.AutoFlush = true;
					rm.Render(writer);
					
				}

			}
			else
			{
				context.Response.StatusCode = 200;
				context.Response.ContentType = "text/html";
				context.Response.Write("[No content specified]");
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