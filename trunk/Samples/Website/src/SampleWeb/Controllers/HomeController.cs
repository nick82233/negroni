using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using System.Data.Objects;
using System.Data.Objects.DataClasses;
using SampleWeb.Models;

using Negroni.DataPipeline.RequestProcessing;

namespace SampleWeb.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private GadgetsEntities gadgetDB;

        public HomeController()
        {
            gadgetDB = new GadgetsEntities();
        }


        public ActionResult Index()
        {
            ViewData["StartTime"] = DateTime.Now.Ticks;
            ViewData["Message"] = "Negroni Sample Gadgets Website";

            ViewData.Model = gadgetDB.Apps.ToList<App>();

            ViewData["BindEnd"] = DateTime.Now.Ticks;
            return View();
        }


        public ActionResult Edit()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(FormCollection form)
        {
            var appToAdd = gadgetDB.CreateObject<App>(); //new SampleWeb.Models.App();

            // Deserialize (Include white list!)
            TryUpdateModel(appToAdd, new string[] { "Name", "SourceUrl" }, form.ToValueProvider());

            // Validate
            if (String.IsNullOrEmpty(appToAdd.Name))
                ModelState.AddModelError("Name", "Name required!");
            if (String.IsNullOrEmpty(appToAdd.SourceUrl))
                ModelState.AddModelError("SourceUrl", "SourceUrl is required!");

            if (!string.IsNullOrEmpty(appToAdd.SourceUrl))
            {
                HttpWebRequest gadgetRequest = HttpWebRequest.Create(appToAdd.SourceUrl) as HttpWebRequest;

                HttpWebResponse gadgetResponse = gadgetRequest.GetResponse() as HttpWebResponse;

                string gadgetString = null;
                using (Stream stream = gadgetResponse.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    gadgetString = sr.ReadToEnd();
                    sr.Close();
                }
            }


            bool ok = false;

            EntityKey key = null;
            // If valid, save movie to database
            if (ModelState.IsValid)
            {
                gadgetDB.AddToApps(appToAdd);
                gadgetDB.SaveChanges();
                key = gadgetDB.CreateEntityKey("SampleWeb.Models.App", appToAdd);
                ok = true;
            }



            if (ok)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Otherwise, reshow form
                return View(appToAdd);
            }
        }


        public ActionResult Add()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Add(FormCollection form)
        {
            var appToAdd = gadgetDB.CreateObject<App>(); //new SampleWeb.Models.App();

            // Deserialize (Include white list!)
            TryUpdateModel(appToAdd, new string[] { "Name", "SourceUrl" }, form.ToValueProvider());

            appToAdd.CreateDate = DateTime.UtcNow;
            appToAdd.UpdateDate = DateTime.UtcNow;
            // Validate
            if (String.IsNullOrEmpty(appToAdd.Name))
                ModelState.AddModelError("Name", "Name required!");
            if (String.IsNullOrEmpty(appToAdd.SourceUrl))
                ModelState.AddModelError("SourceUrl", "SourceUrl is required!");

            string gadgetString = null;

            if (!string.IsNullOrEmpty(appToAdd.SourceUrl))
            {
                HttpWebRequest gadgetRequest = HttpWebRequest.Create(appToAdd.SourceUrl) as HttpWebRequest;

                HttpWebResponse gadgetResponse = gadgetRequest.GetResponse() as HttpWebResponse;

                
                using (Stream stream = gadgetResponse.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    gadgetString = sr.ReadToEnd();
                    sr.Close();
                }
            }            

            bool ok = false;

            // If valid, save movie to database
            if (ModelState.IsValid)
            {
                gadgetDB.AddToApps(appToAdd);
                gadgetDB.SaveChanges();
                ok = true;
            }
            else
            {
                return View(appToAdd);
            }

            Gadget gadget = gadgetDB.CreateObject<Gadget>();
            gadget.AppID = appToAdd.AppId;
            gadget.SourceUrl = appToAdd.SourceUrl;
            gadget.CreateDate = appToAdd.CreateDate;
            gadget.UpdateDate = appToAdd.UpdateDate;

            gadgetDB.AddToGadgets(gadget);
            gadgetDB.SaveChanges();

            appToAdd.LatestGadgetID = gadget.GadgetID;

            GadgetContent gContent = gadgetDB.CreateObject<GadgetContent>();
            gContent.GadgetID = gadget.GadgetID;
            gContent.CreateDate = appToAdd.CreateDate;
            gContent.UpdateDate = appToAdd.UpdateDate;
            gContent.RecordTypeID = 1;
            gContent.Content = gadgetString;

            gadgetDB.AddToGadgetContents(gContent);
            gadgetDB.SaveChanges();

            if (ok)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Otherwise, reshow form
                return View(appToAdd);
            }
        }




        public ActionResult About()
        {
            return View();
        }
    }
}
