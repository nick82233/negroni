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
	/// Summary description for GadgetFetcher
	/// </summary>
	public class GadgetFetcher : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			if (!context.Request.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
			{
				context.Response.ContentType = "text/html";
				context.Response.StatusCode = 400;
				context.Response.Write("<h1>Error</h1> Invalid request");
				context.Response.End();
				return;
			}


			string src = context.Request.Form["source"];

			if (string.IsNullOrEmpty(src))
			{
				context.Response.ContentType = "text/plain";
				context.Response.Write("Fail - specify 'source' in form");
				context.Response.End();
				return;
			}

			ControlFactory cf = ControlFactory.GetControlFactory("gadget_v1.0");

			RootElementMaster root = cf.FetchGadget(src);

			if (root == null || string.IsNullOrEmpty(root.RawTag))
			{
				context.Response.ContentType = "text/plain";
				context.Response.Write("Fail - invalid gadget");
				context.Response.End();
				return;
			}
			
			//write to list.
			int pos = src.LastIndexOf("/");

			string file = src;
			if (pos > -1)
			{
				file = src.Substring(pos + 1);
			}

			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GadgetFileList.GADGET_DIRECTORY);

			path = Path.Combine(path, file);

			if (File.Exists(path))
			{
				/*
				context.Response.ContentType = "text/plain";
				context.Response.Write("File already loaded - delete manually: " + path);
				context.Response.End();
				return;
				 * */
				File.Delete(path);
			}

			FileStream fs = new FileStream(path, FileMode.CreateNew);
			StreamWriter writer = new StreamWriter(fs);
			writer.Write(root.RawTag);
			writer.Flush();
			writer.Close();

			context.Response.Redirect("Render.aspx?gadget=" + file);

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