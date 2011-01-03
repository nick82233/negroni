using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using SampleWeb.GadgetCode;
using SampleWeb.Models;
using Negroni.OpenSocial.Gadget;

namespace SampleWeb.Controllers
{
    public class AppController : Controller
    {
        private GadgetsEntities gadgetDB;

        public AppController()
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

        //
        // GET: /App/Details/5

        public ActionResult Details(int id)
        {
            var app = gadgetDB.Apps.FirstOrDefault<App>(a => a.AppId == id);
            return View(app);
        }


		[HttpPost]
		public void Add(FormCollection form)
		{
			string url = form["url"];

			if (string.IsNullOrEmpty(url))
			{
				Response.Write("<h1>Empty URL not allowed</h1>");
				return;
			}

		}


        public ActionResult Create()
        {
            return View(new App());
        } 

        //
        // POST: /App/Create

        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            try
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

                // If valid, save movie to database
                if (ModelState.IsValid)
                {
                    gadgetDB.AddToApps(appToAdd);
                    gadgetDB.SaveChanges();
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


				GadgetMaster gMaster = GadgetManager.FetchGadget(appToAdd.SourceUrl);

                GadgetContent gContent = gadgetDB.CreateObject<GadgetContent>();
                gContent.GadgetID = gadget.GadgetID;
                gContent.CreateDate = appToAdd.CreateDate;
                gContent.UpdateDate = appToAdd.UpdateDate;
                gContent.RecordTypeID = RecordTypeValues.GadgetXML;
				gContent.Content = gMaster.RawTag;

                gadgetDB.AddToGadgetContents(gContent);
                gadgetDB.SaveChanges();

				if (gMaster.HasExternalMessageBundles())
				{
					gContent = gadgetDB.CreateObject<GadgetContent>();
					gContent.RecordTypeID = RecordTypeValues.MessageBundle;
					gContent.CreateDate = appToAdd.CreateDate;
					gContent.UpdateDate = appToAdd.UpdateDate;
					gContent.Content = gMaster.GetConsolidatedMessageBundles();
					gadgetDB.AddToGadgetContents(gContent);
					gadgetDB.SaveChanges();
				}

				appToAdd.LatestGadgetID = gadget.GadgetID;
				gadgetDB.SaveChanges();

				return RedirectToAction("Details/" + appToAdd.AppId.ToString());
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /App/Edit/5
 
        public ActionResult Edit(int id)
        {
            var app = gadgetDB.Apps.Single<App>(a => a.AppId == id);
            return View(app);
        }

        //
        // POST: /App/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /App/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /App/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
