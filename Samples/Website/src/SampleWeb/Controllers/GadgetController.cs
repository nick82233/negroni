using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SampleWeb.Models;


using Negroni.OpenSocial;
using Negroni.OpenSocial.Gadget;
using Negroni.TemplateFramework;

using SampleWeb.GadgetCode;

namespace SampleWeb.Controllers
{
    public class GadgetController : Controller
    {

        private GadgetsEntities gadgetDB;

        public GadgetController()
        {
            gadgetDB = new GadgetsEntities();
        }
        //
        // GET: /Gadget/

        public ActionResult Index()
        {
            return View();
        }

		public void ReloadConfig()
		{
			try
			{
				Negroni.TemplateFramework.Configuration.NegroniFrameworkConfig.ReloadConfiguration();
				Response.Write("Config Reloaded");
			}
			catch (Exception ex)
			{
				Response.Write("Config reload failed: " + ex.Message);
			}
		}

        public void Render(int id)
        {

            try
            {
                var gContents = (from gc in gadgetDB.GadgetContents
                             where (gc.GadgetID == id && gc.RecordTypeID == RecordTypeValues.GadgetXML)
                             select gc).First();

				if (gContents == null)
				{
					Response.Write("Not found");
					return;
				}


				GadgetMaster gm = GadgetMaster.CreateGadget(GadgetManager.GADGET_FACTORY_KEY, gContents.Content);

				TextWriter writer = new StreamWriter(Response.OutputStream);
				gm.RenderContent(writer);
			}
            catch(Exception ex)
            {
                Response.Write("Error getting gadget: " + ex.Message);
                return;
            }

        }

    }
}
