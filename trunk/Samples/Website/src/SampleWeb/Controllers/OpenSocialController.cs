using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Mvc;

using Contracts = Negroni.OpenSocial.DataContracts;


namespace SampleWeb.Controllers
{
    public class OpenSocialController : Controller
    {
        //
        // GET: /OpenSocial/

        public ActionResult Index()
        {
            return View();
        }


        public void People()
        {

            Contracts.GenericRestResponse<List<Contracts.Person>> response = Contracts.ResponseFactory.CreatePeopleResponse();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Contracts.GenericRestResponse<List<Contracts.Person>>));

            response.StartIndex = 1;
            response.ItemsPerPage = 10;
            response.Entry.Add(new Contracts.Person { Id = "1", DisplayName = "John Doe" });
            response.Entry.Add(new Contracts.Person { Id = "2", DisplayName = "Jack Frost" });
            response.Entry.Add(new Contracts.Person { Id = "3", DisplayName = "Timmy" });
            response.Entry.Add(new Contracts.Person { Id = "4", DisplayName = "Mr Scrooge" });
            response.Entry.Add(new Contracts.Person { Id = "5", DisplayName = "Frosty" });
            response.Entry.Add(new Contracts.Person { Id = "6", DisplayName = "Old Man Winter" });
            Response.ContentType = "application/json";
            Response.Buffer = false;
            ser.WriteObject(Response.OutputStream, response);

            //string data;
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    ser.WriteObject(stream, response);
            //    stream.Flush();
            //    stream.Seek(0, SeekOrigin.Begin);
            //    StreamReader sr = new StreamReader(stream);
            //    data = sr.ReadToEnd();
            //}

            
            //return data;

        }


    }
}
