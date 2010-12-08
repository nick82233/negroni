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
using System.Text;

using Negroni.DataPipeline;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;

namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// Helper class that builds calls to makeRequest for execution on the client.
	/// </summary>
	public class ExternalRequestControl : BaseGadgetControl, IExternalDataSource
	{

		#region contant strings

		const string AUTHVALUE_NONE = "NONE";
		const string AUTHVALUE_SIGNED = "SIGNED";
		const string AUTHVALUE_OAUTH = "OAUTH";

		/// <summary>
		/// gadgets.io.RequestParameters.METHOD
		/// </summary>
		const string GADGET_RQP_METHOD = "METHOD";
		/// <summary>
		/// gadgets.io.RequestParameters.AUTHORIZATION
		/// </summary>
		const string GADGET_RQP_AUTHORIZATION = "AUTHORIZATION";
		/// <summary>
		/// gadgets.io.RequestParameters.OAUTH_SERVICE_NAME
		/// </summary>
		const string GADGET_RQP_OAUTH_SERVICE_NAME = "OAUTH_SERVICE_NAME";
		/// <summary>
		/// gadgets.io.RequestParameters.OAUTH_TOKEN_NAME
		/// </summary>
		const string GADGET_RQP_OAUTH_TOKEN_NAME = "OAUTH_TOKEN_NAME";
		/// <summary>
		/// gadgets.io.RequestParameters.OAUTH_REQUEST_TOKEN
		/// </summary>
		const string GADGET_RQP_OAUTH_REQUEST_TOKEN = "OAUTH_REQUEST_TOKEN";
		/// <summary>
		/// gadgets.io.RequestParameters.OAUTH_REQUEST_SECRET
		/// </summary>
		const string GADGET_RQP_OAUTH_REQUEST_TOKEN_SECRET = "OAUTH_REQUEST_TOKEN_SECRET";
		//const string GADGET_RQP_OAUTH_USE_TOKEN = "OAUTH_USE_TOKEN";

		#endregion



		/// <summary>
		/// Type of request being handled.  
		/// This dictates how the client will manage the response.
		/// </summary>
		public enum ClientResponseType
		{
			/// <summary>
			/// Response will be registered with the client DataContext.
			/// This is most often JSON data
			/// </summary>
			Data,
			/// <summary>
			/// Response will be rendered directly into a DOM object.
			/// Most typically a Text (HTML) response
			/// </summary>
			Render
		}

		/// <summary>
		/// Flag to cause language and culture to be appended to URL
		/// </summary>
		public bool AppendCultureInfo { get; set; }

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			Src = GetAttribute("src");
			if (string.IsNullOrEmpty(Src))
			{
				Src = GetAttribute("href");
			}

			AuthorizationType = GetAttribute("authz");
			Format = GetAttribute("format");
			Method = GetAttribute("method");

			string res = GetAttribute("resolver");
			if (!string.IsNullOrEmpty(res) && res.Equals("server", StringComparison.InvariantCultureIgnoreCase))
			{
				ResolveLocation = ResolveAt.Server;
			}

		}

		public string ResultKey { get; set; }


		private string _method = "GET";
		/// <summary>
		/// Http method to use for this request
		/// </summary>
		public string Method
		{
			get
			{
				return _method;
			}
			set
			{
				if(!string.IsNullOrEmpty(value)){
					_method = value.ToUpper();
				}
				else{
					_method = "GET";
				}
			}
		}

		private string _format = null;
		/// <summary>
		/// Format request is expected to arrive in. Values are "text" and "json".
		/// </summary>
		public string Format
		{
			get
			{
				if (string.IsNullOrEmpty(_format))
				{
					if (ResponseType == ClientResponseType.Data)
					{
						_format = "json";
					}
					else if (ResponseType == ClientResponseType.Render)
					{
						_format = "text";
					}
				}
				return _format;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				string validValues = "text,json";
				if(validValues.Contains(value.ToLower())){
					_format = value.ToLower();
				}
			}
		}


		private string _authorizationType = null;
		/// <summary>
		/// Authorization type to use for request.
		/// Valid values are "NONE", "SIGNED", "OAUTH".
		/// The default value is NONE
		/// </summary>
		public string AuthorizationType
		{
			get
			{
				return _authorizationType;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_authorizationType = AUTHVALUE_NONE;
					return;
				}
				if (AUTHVALUE_NONE.Equals(value, StringComparison.InvariantCultureIgnoreCase))
				{
					_authorizationType = AUTHVALUE_NONE;
				}
				else if (AUTHVALUE_SIGNED.Equals(value, StringComparison.InvariantCultureIgnoreCase))
				{
					_authorizationType = AUTHVALUE_SIGNED;
				}
				else if (AUTHVALUE_OAUTH.Equals(value, StringComparison.InvariantCultureIgnoreCase))
				{
					_authorizationType = AUTHVALUE_OAUTH;
				}
				else
				{
					_authorizationType = AUTHVALUE_NONE;
				}
			}
		}

		/// <summary>
		/// Identifies the service element in the gadget spec to use for this request. Default is "".
		/// </summary>
		public string OAuthServiceName { get; set; }

		/// <summary>
		/// Identifies the OAuth token used to make a request to the service provider. Default is "".
		/// </summary>
		public string OAuthTokenName { get; set; }

		/// <summary>
		/// Identify a token that is pre-approved by the provider for access to the resource.
		/// </summary>
		public string OAuthRequestToken { get; set; }

		/// <summary>
		/// Secret associated with the pre-approved request token.
		/// </summary>
		public string OAuthRequestTokenSecret { get; set; }

		/// <summary>
		/// Control whether an OAuth token should be used with the request. Allowed values are: "always" | "if_available" | "never"
		/// </summary>
		public string OAuthUseToken { get; set; }

		private string _src = null;

		/// <summary>
		/// URI to request.  This decodes ampersands for the URL
		/// </summary>
		public string Src
		{
			get
			{
				return _src;
			}
			set{
				if (!string.IsNullOrEmpty(value))
				{
					if (value.Contains("&amp;"))
					{
						_src = value.Replace("&amp;", "&");
					}
					else
					{
						_src = value;
					}
				}
				else
				{
					_src = value;
				}
			}
		}


		/// <summary>
		/// URI to request
		/// </summary>
		[Obsolete("Retiring in favor of src")]
		public string HRef
		{
			get
			{
				return Src;
			}
			set
			{
				Src = value;
			}
		}


		/// <summary>
		/// Flag to indicate a new DOM element should be created.
		/// </summary>
		protected bool GenerateDomElement
		{
			get;
			set;
		}

		private ClientResponseType _responseType = ClientResponseType.Render;

		/// <summary>
		/// Indicator of how client code should handle response.
		/// </summary>
		public ClientResponseType ResponseType
		{
			get
			{
				return _responseType;
			}
			set
			{
				_responseType = value;
			}

		}

		/// <summary>
		/// URL parameters
		/// </summary>
		public string Params { get; set; }


		/// <summary>
		/// ID of element or key if different than current element
		/// </summary>
		public string TargetElement { get; set; }

		/// <summary>
		/// In cases where <c>ResponseTarget</c> is Data, this is the
		/// DataContext key to place the response under.
		/// </summary>
		public string DataKey { get; set; }


		/// <summary>
		/// Generates a unique ID for use in the DOM.
		/// TODO: Make this more efficient
		/// </summary>
		/// <returns></returns>
		protected string GetUniqueID()
		{
			string id = Guid.NewGuid().ToString();
			return id.Replace("-", "");
		}


		protected virtual string BuildJsRequestParamsObject(string objectName)
		{
			if(string.IsNullOrEmpty(objectName)){
				objectName = "crrParams";
			}
			StringBuilder sb = new StringBuilder();
			sb.Append("var ").Append(objectName).AppendLine(" = {};");
			AddRequestParamIfNotEmpty(sb, Method, GADGET_RQP_METHOD, objectName);
			AddRequestParamIfNotEmpty(sb, AuthorizationType, GADGET_RQP_AUTHORIZATION, objectName);

			if (AuthorizationType == "OAUTH")
			{
				AddRequestParamIfNotEmpty(sb, OAuthServiceName, GADGET_RQP_OAUTH_SERVICE_NAME, objectName);
				AddRequestParamIfNotEmpty(sb, OAuthTokenName, GADGET_RQP_OAUTH_TOKEN_NAME, objectName);
				AddRequestParamIfNotEmpty(sb, OAuthRequestToken, GADGET_RQP_OAUTH_REQUEST_TOKEN, objectName);
				AddRequestParamIfNotEmpty(sb, OAuthRequestTokenSecret, GADGET_RQP_OAUTH_REQUEST_TOKEN_SECRET, objectName);
				//AddRequestParamIfNotEmpty(sb, OAuthUseToken, GADGET_RQP_OAUTH_USE_TOKEN, objectName);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Adds parameter line to the makeRequest params object if the value is not empty
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="paramValue"></param>
		/// <param name="requestParameter">Value corresponding to gadgets.io.RequestParameters enum value</param>
		/// <param name="objectName">Name of the client-side object being generated</param>
		private void AddRequestParamIfNotEmpty(StringBuilder sb, string paramValue, string requestParameter, string objectName)
		{
			if (!string.IsNullOrEmpty(paramValue))
			{
				sb.AppendFormat("{0}[gadgets.io.RequestParameters.{1}] = '{2}';\n", objectName, requestParameter, paramValue);
			}
		}

		/// <summary>
		/// Provides for an override value to normal render behavior if the control
		/// has been server-side resolved
		/// </summary>
		public string ServerResolvedValue { get; set; }


		public override void Render(System.IO.TextWriter writer)
		{
			if(string.IsNullOrEmpty(Src)){
				return;
			}
			string localSrc = Src;
			if (AppendCultureInfo && MyDataContext != null && !string.IsNullOrEmpty(MyDataContext.Culture))
			{
				string[] cparts = MyDataContext.Culture.Split(new char[]{'-'});
				char splitter;
				if (localSrc.Contains("?"))
				{
					splitter = '&';
				}
				else
				{
					splitter = '?';
				}
				if (cparts.Length >= 2)
				{
					localSrc += String.Concat(splitter, "lang=", cparts[0], "&country=", cparts[1]);
				}
				else
				{
					localSrc += String.Concat(splitter, "lang=", cparts[0]);
				}
			}

			localSrc = MyDataContext.ResolveVariables(localSrc);

			//look for server-side override
			if (ServerResolvedValue != null || !string.IsNullOrEmpty(ResultKey))
			{
				if (string.IsNullOrEmpty(ServerResolvedValue))
				{
					ServerResolvedValue = DataContext.VARIABLE_START + DataContext.RESERVED_KEY_FETCHED_MARKUP + "." + ResultKey + DataContext.VARIABLE_END;
				}
				if (ServerResolvedValue.Contains(DataContext.VARIABLE_START)
					&& ServerResolvedValue.Contains(DataContext.VARIABLE_END))
				{
					writer.Write(MyDataContext.ResolveVariables(ServerResolvedValue));
				}
				else
				{
					writer.Write(ServerResolvedValue);
				}
				return;
			}

			if (string.IsNullOrEmpty(Method))
			{
				Method = "GET";
			}

			string targetId = TargetElement;
			if (string.IsNullOrEmpty(targetId))
			{
				if (string.IsNullOrEmpty(ID))
				{
					ID = GetUniqueID();
				}
				targetId = ID;
			}

			if (GenerateDomElement)
			{
				writer.Write("<div id=\"");
				writer.Write(ID);
				writer.WriteLine("\"></div>");
			}


			string regLine = "MyOpenSpace.ClientRequestProcessor.addRequest('##URL##', '##RESPONSE_TARGET##', '##ID##', crrParams);";
			if (ClientResponseType.Render == ResponseType)
			{
				regLine = regLine.Replace("##URL##", localSrc)
					.Replace("##RESPONSE_TARGET##", ResponseType.ToString().ToLower())
					.Replace("##ID##", targetId);
			}
			else
			{
				regLine = regLine.Replace("##URL##", localSrc)
					.Replace("##RESPONSE_TARGET##", ResponseType.ToString().ToLower())
					.Replace("##ID##", DataKey);
			}

			writer.Write(GadgetMaster.JS_START_BLOCK_TAGS);
			writer.Write(BuildJsRequestParamsObject("crrParams"));
			writer.WriteLine(regLine);
			writer.WriteLine("delete crrParams;");
			writer.Write(GadgetMaster.JS_END_BLOCK_TAGS);
			//"MyOpenSpace.TemplateProcessor.ClientRenderRequest.addRequest"
			
		}

		#region IExternalDataSource Members

		public ExpectedResponseType ExpectedResponse
		{
			get { 
				if(ResponseType == ClientResponseType.Data){
					return ExpectedResponseType.JsonData;
				}
				else if (ResponseType == ClientResponseType.Render)
				{
					return ExpectedResponseType.Markup;
				}
				else
				{
					return ExpectedResponseType.JsonData;
				}			
			}
		}

		public ResolveAt ResolveLocation
		{
			get ; set;
		}

		public string SourceUri
		{
			get { return Src; }
		}

		#endregion
	}
}
