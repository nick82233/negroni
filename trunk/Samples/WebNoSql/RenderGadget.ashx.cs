using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.DataPipeline;

using WebNoSql.SiteOSML;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.DataPipeline.RequestProcessing;

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

		/// <summary>
		/// Loads any local templates
		/// </summary>
		/// <param name="gm"></param>
		/// <param name="gadgetFile"></param>
		private void LoadTemplates(GadgetMaster gm, string gadgetFile)
		{
			if(!gm.HasExternalTemplateLibraries()){
				return;
			}
			
			string gadgetPrivatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RenderGadget.TEMPLATE_FILES + "\\" + gadgetFile + "\\");
			string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RenderGadget.TEMPLATE_FILES);

			LinkedList<TemplateLibraryDef> siteTemplates = new LinkedList<TemplateLibraryDef>();
			LinkedList<TemplateLibraryDef> urlTemplates = new LinkedList<TemplateLibraryDef>();

			foreach (var def in gm.TemplateLibraries.Libraries)
			{
				if (!string.IsNullOrEmpty(def.Uri))
				{
					if (def.Uri.StartsWith("site:"))
					{
						siteTemplates.AddFirst(def);
					}
					else if (def.Uri.StartsWith("http"))
					{
						urlTemplates.AddFirst(def);
					}
				}
			}

			if (urlTemplates.Count > 0)
			{
				List<KeyValuePair<TemplateLibraryDef, IAsyncResult>> fetchResults = new List<KeyValuePair<TemplateLibraryDef, IAsyncResult>>();
				foreach (var def in urlTemplates)
				{
					fetchResults.Add(new KeyValuePair<TemplateLibraryDef, IAsyncResult>(def, AsyncRequestProcessor.EnqueueRequest(def.Uri)));
				}
				foreach (var keyset in fetchResults)
				{
					IAsyncResult resultHandle = keyset.Value;
					resultHandle.AsyncWaitHandle.WaitOne(800); //wait 800 ms
					if (!resultHandle.IsCompleted)
					{
						//failedFetch.Add(keyset);
					}
					else
					{
						try
						{
							RequestResult thisResult = AsyncRequestProcessor.EndRequest(resultHandle);
							if (thisResult.ResponseCode == 200)
							{
								gm.LoadTemplateLibrary(keyset.Key.Uri, thisResult.ResponseString);
							}
						}
						catch { }
					}
				}
			}

			//site templates
			if (siteTemplates.Count > 0)
			{
				foreach (var def in siteTemplates)
				{
					int pos = def.Uri.IndexOf("site:");
					if(pos == -1){
						continue;
					}
					string part = def.Uri.Substring(pos + 5);
					string filePath = Path.Combine(templatePath, part);

					if (File.Exists(filePath))
					{
						string templateString = null;
						using (StreamReader sr = new StreamReader(filePath))
						{
							templateString = sr.ReadToEnd();
						}
						if (!string.IsNullOrEmpty(templateString))
						{
							gm.LoadTemplateLibrary(def.Uri, templateString);
						}
					}
				}
			}
		}

		private void LoadMessageBundles(GadgetMaster gm, string gadgetFile)
		{
				string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GadgetFileList.GADGET_DIRECTORY + "\\" + gadgetFile);
				if (File.Exists(filePath))
				{
					string gadgetString = null;
					using (StreamReader sr = new StreamReader(filePath))
					{
						gadgetString = sr.ReadToEnd();
					}
					if (!string.IsNullOrEmpty(gadgetString))
					{
						gm.LoadConsolidatedMessageBundles(gadgetString);
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