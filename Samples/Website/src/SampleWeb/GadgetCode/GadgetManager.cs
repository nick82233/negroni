using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using Negroni.DataPipeline.RequestProcessing;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;

namespace SampleWeb.GadgetCode
{
	public class GadgetManager
	{

		/// <summary>
		/// Control factory parsing key
		/// </summary>
		public const string GADGET_FACTORY_KEY = "gadget_v1.0";


		static public GadgetMaster FetchGadget(string url)
		{
			return FetchGadget(url, GADGET_FACTORY_KEY);
		}

		static public GadgetMaster FetchGadget(string url, string controlFactoryKey)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentNullException("URL must be specified");
			}

			string gadgetString = null;

			HttpWebRequest gadgetRequest = HttpWebRequest.Create(url) as HttpWebRequest;

			HttpWebResponse gadgetResponse = gadgetRequest.GetResponse() as HttpWebResponse;


			using (Stream stream = gadgetResponse.GetResponseStream())
			{
				StreamReader sr = new StreamReader(stream);
				gadgetString = sr.ReadToEnd();
				sr.Close();
			}

			GadgetMaster gm = GadgetMaster.CreateGadget(controlFactoryKey, gadgetString);

			gm.FetchMessageBundles();

			return gm;

		}


	}
}