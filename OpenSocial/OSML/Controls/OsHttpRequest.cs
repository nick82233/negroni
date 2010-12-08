/* *********************************************************************
   Copyright 2009-2010 MySpace

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
********************************************************************* */

using System;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls; 
using Negroni.TemplateFramework;
using System.Security.Cryptography;

namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// Data request tag to an arbitrary third-party server.
	/// This uses makeRequest on the client to satisfy the request
	/// </summary>
	[MarkupTag("os:HttpRequest")]
	[ContextGroup(typeof(DataScript))]
	[ContextGroup(typeof(BaseContainerControl))]
	public class OsHttpRequest : BaseDataControl , IExternalDataSource
	{

		private ExternalRequestControl clientControlHelper = null;


		public OsHttpRequest()
		{
			clientControlHelper = new ExternalRequestControl();
		}

        public override void LoadTag(string markup)
        {
            
            base.LoadTag(markup);
			this.UseClientDataResolver = true;

			clientControlHelper.LoadTag(markup);
			clientControlHelper.ResponseType = ExternalRequestControl.ClientResponseType.Data;
			clientControlHelper.DataKey = GetAttribute("key");
			
        }

		public override void Render(System.IO.TextWriter writer)
		{
			clientControlHelper.Render(writer);
		}


		#region IExternalDataSource Members


		/// <summary>
		/// Alias to the DataContext key
		/// </summary>
		public string ResultKey
		{
			get
			{
				return base.Key;
			}
			set
			{
				Key = value;
			}
		}

		public ExpectedResponseType ExpectedResponse
		{
			get { return clientControlHelper.ExpectedResponse; }
		}

		public string Method
		{
			get { return clientControlHelper.Method; }
		}

		public ResolveAt ResolveLocation
		{
			get
			{
				return clientControlHelper.ResolveLocation;
			}
			set
			{
				clientControlHelper.ResolveLocation = value;
			}
		}

		public string SourceUri
		{
			get { return clientControlHelper.SourceUri; }
		}

		#endregion
	}
}
