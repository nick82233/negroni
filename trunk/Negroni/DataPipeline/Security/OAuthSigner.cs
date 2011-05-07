using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Negroni.DataPipeline.Security
{
    public class OAuthParameters
    {
        public string BodyHash { get; set; }
        public string Realm { get; set; }
        public string ConsumerKey { get; set; }
        public string Nonce { get; set; }
        public string Signature { get; set; }
        public string SignatureMethod { get; set; }
        public string TimeStamp { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string Version { get; set; }
        public string RequestType { get; set; }
        public Uri RequestUrl { get; set; }
        public Dictionary<string, string> BodyParams { get; set; }
    }

    public class OAuthSigner
    {

        private string oauthBodyHash;
        private string oauthRealm;
        private string oauthConsumerKey;
        private string oauthNonce;
        private string oauthSignature;
        private string oauthSignatureMethod;
        private string oauthTimeStamp;
        private string oauthToken;
        private string oauthTokenSecret;
        private string oauthVersion;
        private string requestType;
        private Uri requestUrl;
        private Dictionary<string, string> bodyParams;
        private string[] schemes;

        public OAuthSigner(OAuthParameters parameters)
        {

            this.oauthBodyHash = parameters.BodyHash;
            this.oauthRealm = parameters.Realm;
            this.oauthConsumerKey = parameters.ConsumerKey;
            this.oauthNonce = parameters.Nonce;
            this.oauthSignature = parameters.Signature;
            this.oauthSignatureMethod = parameters.SignatureMethod;
            this.oauthTimeStamp = parameters.TimeStamp;
            this.oauthToken = parameters.Token;
            this.oauthTokenSecret = parameters.TokenSecret;
            this.oauthVersion = parameters.Version;
            this.requestType = parameters.RequestType;
            this.requestUrl = parameters.RequestUrl;
            this.bodyParams = parameters.BodyParams;
            this.schemes = new string[] { requestUrl.Scheme };
        }

        public string[] Schemes
        {
            get { return schemes; }
            set { this.schemes = value; }
        }

        public bool IsValidSignature(string consumerSecret)
        {
            int signatureStage = 0;
            string[] secrets;

            if (string.IsNullOrEmpty(oauthToken))
            {
                secrets = new string[] { consumerSecret };
            }
            else
            {
                secrets = new string[] { consumerSecret, string.Empty };
            }

            foreach (string secret in secrets)
            {
                foreach (string scheme in schemes)
                {
                    Uri origUri = requestUrl;
                    StringBuilder newUrl = new StringBuilder();

                    newUrl.Append(scheme);
                    newUrl.Append("://");
                    newUrl.Append(requestUrl.Host);

                    if ((requestUrl.Port != 80) && (requestUrl.Port != 443))
                    {
                        newUrl.Append(":");
                        newUrl.Append(requestUrl.Port.ToString());
                    }

                    newUrl.Append(requestUrl.AbsolutePath);
                    newUrl.Append(requestUrl.Query);

                    Uri newUri = new Uri(newUrl.ToString());
                    string sig;

                    if (null == oauthToken) oauthToken = string.Empty; // token may or may not be present 
                    if (null == oauthTokenSecret) oauthTokenSecret = string.Empty; // tokenSecret may or may not be present

                    // Try all unencoded first
                    //
                    sig = OAuthBase.GenerateSignature(
                        newUri,
                        oauthConsumerKey,
                        secret,
                        oauthToken,
                        oauthTokenSecret,
                        requestType,
                        oauthTimeStamp,
                        oauthNonce,
                        OAuthBase.SignatureTypes.HMACSHA1,
                        oauthVersion,
                        oauthBodyHash,
                        bodyParams,
                        true,
                        false,
                        false);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}

                    // Try all encoded consumer key, everything else unencoded
                    //
                    sig = OAuthBase.GenerateSignature(
                        newUri,
                        HttpUtility.UrlEncode(oauthConsumerKey),
                        secret,
                        oauthToken,
                        oauthTokenSecret,
                        requestType,
                        oauthTimeStamp,
                        oauthNonce,
                        OAuthBase.SignatureTypes.HMACSHA1,
                        oauthVersion,
                        oauthBodyHash,
                        bodyParams,
                        true,
                        false,
                        false);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}

                    // Try with unencoded consumer key since the spec is ambiguous on this point
                    //
                    sig = OAuthBase.GenerateSignature(
                        newUri,
                        oauthConsumerKey,
                        secret,
                        OAuthBase.UrlEncode(oauthToken),
                        oauthTokenSecret,
                        requestType,
                        OAuthBase.UrlEncode(oauthTimeStamp),
                        OAuthBase.UrlEncode(oauthNonce),
                        OAuthBase.SignatureTypes.HMACSHA1,
                        OAuthBase.UrlEncode(oauthVersion),
                        oauthBodyHash,
                        bodyParams,
                        true,
                        false,
                        false);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}

                    // Try with encoded consumer key since the spec is ambiguous on this point
                    //
                    sig = OAuthBase.GenerateSignature(
                        newUri,
                        OAuthBase.UrlEncode(oauthConsumerKey),
                        secret,
                        OAuthBase.UrlEncode(oauthToken),
                        oauthTokenSecret,
                        requestType,
                        OAuthBase.UrlEncode(oauthTimeStamp),
                        OAuthBase.UrlEncode(oauthNonce),
                        OAuthBase.SignatureTypes.HMACSHA1,
                        OAuthBase.UrlEncode(oauthVersion),
                        OAuthBase.UrlEncode(oauthBodyHash),
                        bodyParams,
                        true,
                        false,
                        false);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}

                    // If standard did not work, try legacy
                    //
                    sig = OAuthBase.GenerateSignature(
                        newUri,
                        OAuthBase.UrlEncode(oauthConsumerKey),
                        secret,
                        OAuthBase.UrlEncode(oauthToken),
                        oauthTokenSecret,
                        requestType,
                        OAuthBase.UrlEncode(oauthTimeStamp),
                        OAuthBase.UrlEncode(oauthNonce),
                        OAuthBase.SignatureTypes.HMACSHA1,
                        OAuthBase.UrlEncode(oauthVersion),
                        OAuthBase.UrlEncode(oauthBodyHash),
                        bodyParams,
                        false,
                        false,
                        false);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}

                    // Try legacy with unencoded consumer key
                    //
                    sig = OAuthBase.GenerateSignature(
                        newUri,
                        oauthConsumerKey,
                        secret,
                        OAuthBase.UrlEncode(oauthToken),
                        oauthTokenSecret,
                        requestType,
                        OAuthBase.UrlEncode(oauthTimeStamp),
                        OAuthBase.UrlEncode(oauthNonce),
                        OAuthBase.SignatureTypes.HMACSHA1,
                        OAuthBase.UrlEncode(oauthVersion),
                        OAuthBase.UrlEncode(oauthBodyHash),
                        bodyParams,
                        false,
                        false,
                        false);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}

                    // Try with request parameters encoded method i.e. OAuth.NET (Magdex)
                    //
                    sig = OAuthBase.GenerateSignature(
                        newUri,
                        oauthConsumerKey,
                        secret,
                        oauthToken,
                        oauthTokenSecret,
                        requestType,
                        oauthTimeStamp,
                        oauthNonce,
                        OAuthBase.SignatureTypes.HMACSHA1,
                        oauthVersion,
                        oauthBodyHash,
                        bodyParams,
                        true,
                        true,
                        true);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}

					// Try legacy request_token with callback, allowing oauth_callback parameter
					// This comes from the MyspaceID SDK signer implementations
					sig = OAuthBase.GenerateSignature(
						newUri,
						oauthConsumerKey,
						secret,
						oauthToken,
						oauthTokenSecret,
						requestType,
						oauthTimeStamp,
						oauthNonce,
						OAuthBase.SignatureTypes.HMACSHA1,
						oauthVersion,
						oauthBodyHash,
						bodyParams,
						true,
						false,
						false,
						true);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}
					// Try legacy request_token with callback, allowing oauth_callback parameter and encoding consumer key
					// This comes from the MyspaceID SDK signer implementations
					sig = OAuthBase.GenerateSignature(
						newUri,
						OAuthBase.UrlEncode(oauthConsumerKey),
						secret,
						oauthToken,
						oauthTokenSecret,
						requestType,
						oauthTimeStamp,
						oauthNonce,
						OAuthBase.SignatureTypes.HMACSHA1,
						oauthVersion,
						oauthBodyHash,
						bodyParams,
						true,
						false,
						false,
						true);

					if (OauthSignaturesMatch(oauthSignature, sig, scheme, signatureStage++))
					{
						return true;
					}
				}
            }
            return false;
        }

		/// <summary>
		/// Tests two signatures to see if they match.  
		/// Performs various cross encodings to make sure signature matching is very fuzzy.
		/// </summary>
		/// <param name="suppliedSignature"></param>
		/// <param name="calculatedSignature"></param>
		/// <returns></returns>
		private bool OauthSignaturesMatch(string suppliedSignature, string calculatedSignature, string scheme, int signatureStage)
		{
			if (false == string.IsNullOrEmpty(calculatedSignature))
			{
				if (suppliedSignature.Equals(calculatedSignature)) return true;
				if (suppliedSignature.Equals(HttpUtility.UrlEncode(calculatedSignature))) return true;
				if (suppliedSignature.Equals(OAuthBase.UrlEncode(calculatedSignature))) return true;
				if (suppliedSignature.Equals(HttpUtility.UrlDecode(calculatedSignature))) return true;
			}
			return false;
		}





        public string GetSignatureBaseString()
        {
            return OAuthBase.GenerateSignatureBase(
                requestUrl,
                oauthConsumerKey,
                oauthToken,
                oauthTokenSecret,
                requestType,
                oauthTimeStamp,
                oauthNonce,
                oauthSignatureMethod,
                oauthVersion,
                oauthBodyHash,
                bodyParams,
                true,
                false,
                false);
        }
    }
}