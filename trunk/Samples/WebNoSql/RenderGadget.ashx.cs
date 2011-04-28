using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.DataPipeline;

using WebNoSql.SiteOSML;

namespace WebNoSql
{
	/// <summary>
	/// Summary description for RenderGadget
	/// </summary>
	public class RenderGadget : IHttpHandler
	{
		public static readonly string GADGET_FILES = GadgetFileList.GADGET_DIRECTORY + @"\Gfiles";
		public static readonly string TEMPLATE_FILES = GadgetFileList.GADGET_DIRECTORY + @"\Templates";


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

					if (gm.HasExternalMessageBundles())
					{
						LoadMessageBundles(gm, gadgetFile);
					}

					if (gm.HasExternalTemplateLibraries())
					{
						LoadTemplates(gm, gadgetFile);
					}

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
					rm.MyDataResolver = new SimpleDataPipelineResolver();
					context.Response.StatusCode = 200;
					context.Response.ContentType = "text/html";

					if (rm.Errors.HasParseErrors())
					{
						context.Response.Write("<div style='border:2px solid red;'><ul>");
						foreach (var err in rm.Errors.ParseErrors)
						{
							context.Response.Write("<li>");
							context.Response.Write(err.ToString());
							context.Response.Write("</li>");
						}
						context.Response.Write("</ul></div>");
					}

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

		private void LoadTemplates(GadgetMaster gm, string gadgetFile)
		{
			throw new NotImplementedException();
		}

		private void LoadMessageBundles(GadgetMaster gm, string gadgetFile)
		{
				string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GadgetFileList.GADGET_DIRECTORY + "\\" + gadgetFile);
				if (File.Exists(filePath))
				{

					using (StreamReader sr = new StreamReader(filePath))
					{
						gadgetString = sr.ReadToEnd();
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