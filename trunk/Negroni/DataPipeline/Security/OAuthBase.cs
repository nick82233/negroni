using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Negroni.DataPipeline.Security
{
	/// <summary>
	///		<para>Encapsulates cryptographic algorithms supported by the OAuth protocol.</para>
	///		http://code.google.com/p/oauth/
	///		
	/// </summary>
    public class OAuthBase
    {
        /// <summary>
        /// Provides a predefined set of hashing algorithms 
        ///		that are supported officially by the OAuth protocol.
        /// </summary>
        public enum SignatureTypes
        {
			/// <summary>
			///		<para>The HMAC-SHA1 signature method uses the HMAC-SHA1 signature algorithm 
			///		as defined in [RFC2104] where the Signature Base String is the text and the 
			///		key is the concatenated values (each first encoded per Parameter Encoding 
			///		(Parameter Encoding)) of the Consumer Secret and Token Secret, separated by an '&' 
			///		character (ASCII code 38) even if empty. </para>
			/// </summary>
            HMACSHA1,
			/// <summary>
			///		<para> The  PLAINTEXT  method does not provide any security protection and 
			///		SHOULD only be used over a secure channel such as HTTPS.</para>
			/// </summary>
            PLAINTEXT,
			/// <summary>
			///		<para>The RSA-SHA1 signature method uses the RSASSA-PKCS1-v1_5 signature 
			///		algorithm as defined in [RFC3447] section 8.2 (more simply known as PKCS#1), 
			///		using SHA-1 as the hash function for EMSA-PKCS1-v1_5. It is assumed that the 
			///		Consumer has provided its RSA public key in a verified way to the Service 
			///		Provider, in a manner which is beyond the scope of this specification. </para>
			/// </summary>
            RSASHA1
        }

        /// <summary>
        /// Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter
        {
            private string name = null;
            private string value = null;

            public QueryParameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get { return name; }
            }

            public string Value
            {
                get { return value; }
            }
        }

        /// <summary>
        /// Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter>
        {
            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }
        }

        public const string OAuthVersion = "1.0";
        public const string OAuthParameterPrefix = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        public const string OAuthBodyHashKey = "oauth_body_hash";
        public const string OAuthConsumerKeyKey = "oauth_consumer_key";
        public const string OAuthCallbackKey = "oauth_callback";
        public const string OAuthVersionKey = "oauth_version";
        public const string OAuthSignatureMethodKey = "oauth_signature_method";
        public const string OAuthSignatureKey = "oauth_signature";
        public const string OAuthTimestampKey = "oauth_timestamp";
        public const string OAuthNonceKey = "oauth_nonce";
        public const string OAuthTokenKey = "oauth_token";
        public const string OAuthTokenSecretKey = "oauth_token_secret";

        public const string HMACSHA1SignatureType = "HMAC-SHA1";
        public const string PlainTextSignatureType = "PLAINTEXT";
        public const string RSASHA1SignatureType = "RSA-SHA1";

        //protected Random random = new Random();

        protected const string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

		/// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <param name="decodeUrlParameters">MYSPACE ADDITION - UrlDecodes the query-string parameters before returning them or not. Added to work with various clients with various encoding i.e. Madgex OAuth.net</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
		private static List<QueryParameter> GetQueryParameters(string parameters, bool decodeUrlParameters)
		{
			return GetQueryParameters(parameters, decodeUrlParameters, false);
		}

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <param name="decodeUrlParameters">MYSPACE ADDITION - UrlDecodes the query-string parameters before returning them or not. Added to work with various clients with various encoding i.e. Madgex OAuth.net</param>
		/// <param name="allowOauthCallbackParameter">MYSPACE ADDITION - Some versions of the SDK sign and include the oauth_callback value in the generated signature </param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private static List<QueryParameter> GetQueryParameters(string parameters, bool decodeUrlParameters, bool allowOauthCallbackParameter)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) 
						&&
						(!s.StartsWith(OAuthParameterPrefix) || (allowOauthCallbackParameter && s.StartsWith("oauth_callback", StringComparison.InvariantCultureIgnoreCase))))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(
                                new QueryParameter(
                                    decodeUrlParameters ? HttpUtility.UrlDecode(temp[0]) : temp[0],
                                    decodeUrlParameters ? HttpUtility.UrlDecode(temp[1]) : temp[1]));
                        }
                        else
                        {
                            result.Add(
                                new QueryParameter(
                                    decodeUrlParameters ? HttpUtility.UrlDecode(s) : s,
                                    string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public static string UrlEncode(string value)
        {
            if (value == null)
                return null;

            if (value.Length == 0)
                return "";

            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Normalizes the request parameters accoriding to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <param name="encodeRequestParameters">MYSPACE ADDITION - Added to work with various clients with various encoding i.e. Madgex OAuth.net</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected static string NormalizeRequestParameters(IList<QueryParameter> parameters, bool encodeRequestParameters)
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                p = parameters[i];
                sb.AppendFormat(
                    "{0}={1}",
                    encodeRequestParameters ? UrlEncode(p.Name) : p.Name,
                    encodeRequestParameters ? UrlEncode(p.Value) : p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignatureBase(
            Uri url,
            string consumerKey,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            string signatureType,
            bool standard,
            bool encodeRequestParameters,
            bool decodeUrlParameters)
        {
            return GenerateSignatureBase(
                url,
                consumerKey,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                OAuthVersion,
                standard,
                encodeRequestParameters,
                decodeUrlParameters);
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignatureBase(
            Uri url,
            string consumerKey,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            string signatureType,
            string version,
            bool standard,
            bool encodeRequestParameters,
            bool decodeUrlParameters)
        {
            return GenerateSignatureBase(
                url,
                consumerKey,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                version,
                null,
                standard,
                encodeRequestParameters,
                decodeUrlParameters);
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignatureBase(
            Uri url,
            string consumerKey,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            string signatureType,
            string version,
            Dictionary<string, string> bodyParams,
            bool standard,
            bool encodeRequestParameters,
            bool decodeUrlParameters)
        {
            return GenerateSignatureBase(
                url,
                consumerKey,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                version,
                null,
                null,
                standard,
                encodeRequestParameters,
                decodeUrlParameters);
        }
		public static string GenerateSignatureBase(
			Uri url,
			string consumerKey,
			string token,
			string tokenSecret,
			string httpMethod,
			string timeStamp,
			string nonce,
			string signatureType,
			string version,
			string bodyHash,
			Dictionary<string, string> bodyParams,
			bool standard,
			bool encodeRequestParameters,
			bool decodeUrlParameters)
		{
			return GenerateSignatureBase( url,
				consumerKey,
				token,
				tokenSecret,
				httpMethod,
				timeStamp,
				nonce,
				signatureType,
				version,
				bodyHash,
				bodyParams,
				standard,
				encodeRequestParameters,
				decodeUrlParameters,
				false);

		}
        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <returns>The signature base</returns>
        public static string GenerateSignatureBase(
            Uri url,
            string consumerKey,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            string signatureType,
            string version,
            string bodyHash,
            Dictionary<string, string> bodyParams,
            bool standard,
            bool encodeRequestParameters,
            bool decodeUrlParameters,
			bool allowOauthCallbackParameter)
        {

            if (token == null)
                token = string.Empty;

            if (tokenSecret == null)
                tokenSecret = string.Empty;

			List<QueryParameter> parameters = GetQueryParameters(url.Query, decodeUrlParameters, allowOauthCallbackParameter);

            if (bodyParams != null)
            {
                foreach (KeyValuePair<string, string> pair in bodyParams)
                {
                    if (pair.Key.StartsWith(OAuthParameterPrefix))
                        continue;

                    parameters.Add(new QueryParameter(pair.Key, pair.Value));
                }
            }

            if (string.IsNullOrEmpty(version))
            {
                if (false == standard)
                {
                    parameters.Add(new QueryParameter(OAuthVersionKey, string.Empty));
                }
            }
            else
            {
                parameters.Add(new QueryParameter(OAuthVersionKey, version));
            }

            if (!string.IsNullOrEmpty(bodyHash))
                parameters.Add(new QueryParameter(OAuthBodyHashKey, bodyHash));
            
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));

            if (string.IsNullOrEmpty(token))
            {
                if (false == standard)
                {
                    parameters.Add(new QueryParameter(OAuthTokenKey, string.Empty));
                }
            }
            else
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, token));
            }

            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));
            
            parameters.Sort(new QueryParameterComparer());

            StringBuilder normalizedUrl = new StringBuilder();

            normalizedUrl.Append(url.Scheme);
            normalizedUrl.Append("://");
            normalizedUrl.Append(url.Host);

            if ((url.Port != 80) && (url.Port != 443))
            {
                normalizedUrl.Append(":");
                normalizedUrl.Append(url.Port);
            }

            normalizedUrl.Append(url.AbsolutePath);

            string normalizedRequestParameters = NormalizeRequestParameters(parameters, encodeRequestParameters);

            StringBuilder signatureBase = new StringBuilder();

            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl.ToString()));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

		/// <summary>
		/// 	<para>Generate the signature value based on the given signature base and hash algorithm.</para>
		/// </summary>
		/// <param name="signatureBase">
		/// 	<para>The string to produce the hash from, as produced by <see cref="GenerateSignatureBase"/> 
		///		or by any other means.  Could be an empty string.</para>
		/// </param>
		/// <param name="hashAlgorithm">
		/// 	<para>The hash algorithm used to perform the hashing. 	If the hashing algorithm requires initialization or a key it should be set 	prior to calling this method.</para>
		/// </param>
		/// <returns>
		/// 	<para>A Base64 encoded string of the hash value.  Never <see langword="null"/>.</para>
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// 	<para>The argument <paramref name="signatureBase"/> is <see langword="null"/>.</para>
		/// 	<para>-or-</para>
		/// 	<para>The argument <paramref name="hashAlgorithm"/> is <see langword="null"/>.</para>
		/// </exception>
        public static string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hashAlgorithm)
        {

			byte[] dataBuffer = Encoding.UTF8.GetBytes(signatureBase);
			byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

			return Convert.ToBase64String(hashBytes);
		}

        public static string GenerateBodyHash(byte[] bytes)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();

            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignature(
            Uri url, 
            string consumerKey, 
            string consumerSecret, 
            string token, 
            string tokenSecret, 
            string httpMethod, 
            string timeStamp, 
            string nonce)
        {
            return GenerateSignature(
                url,
                consumerKey,
                consumerSecret,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                SignatureTypes.HMACSHA1);
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignature(
            Uri url,
            string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,            
            SignatureTypes signatureType)
        {
            return GenerateSignature(url,
                consumerKey,
                consumerSecret,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                OAuthVersion,
                true,
                false,
                false);
        }
        
        [Obsolete("Use the other overload")]
        public static string GenerateSignature(
            Uri url,
            string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            SignatureTypes signatureType,
            string version)
        {
            return GenerateSignature(
                url,
                consumerKey,
                consumerSecret,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                version,
                null);
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignature(
            Uri url,
            string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            SignatureTypes signatureType,
            string version,
            Dictionary<string, string> bodyParams)
        {
            return GenerateSignature(url,
                consumerKey,
                consumerSecret,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                version,
                bodyParams,
                true,
                false,
                false);
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignature(
                Uri url,
                string consumerKey,
                string consumerSecret,
                string token,
                string tokenSecret,
                string httpMethod,
                string timeStamp,
                string nonce,
                SignatureTypes signatureType,
                bool standard,
                bool encodeRequestParameters,
                bool decodeUrlParameters)
        {
            return GenerateSignature(
                url,
                consumerKey,
                consumerSecret,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                OAuthVersion,
                standard,
                encodeRequestParameters,
                decodeUrlParameters);
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignature(
            Uri url,
            string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            SignatureTypes signatureType,
            string version,
            bool standard,
            bool encodeRequestParameters,
            bool decodeUrlParameters)
        {
            return GenerateSignature(
                url,
                consumerKey,
                consumerSecret,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                version,
                null,
                standard,
                encodeRequestParameters,
                decodeUrlParameters);
        }

        [Obsolete("Use the other overload")]
        public static string GenerateSignature(
            Uri url,
            string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            SignatureTypes signatureType,
            string version,
            Dictionary<string, string> bodyParams,
            bool standard,
            bool encodeRequestParameters,
            bool decodeUrlParameters)
        {
            return GenerateSignature(
                url,
                consumerKey,
                consumerSecret,
                token,
                tokenSecret,
                httpMethod,
                timeStamp,
                nonce,
                signatureType,
                version,
                null,
                bodyParams,
                standard,
                encodeRequestParameters,
                decodeUrlParameters);
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <returns>A base64 string of the hash value</returns>
		public static string GenerateSignature(
			Uri url,
			string consumerKey,
			string consumerSecret,
			string token,
			string tokenSecret,
			string httpMethod,
			string timeStamp,
			string nonce,
			SignatureTypes signatureType,
			string version,
			string bodyHash,
			Dictionary<string, string> bodyParams,
			bool standard,
			bool encodeRequestParameters,
			bool decodeUrlParameters)
		{
			return GenerateSignature(
				url,
				consumerKey,
				consumerSecret,
				token,
				tokenSecret,
				httpMethod,
				timeStamp,
				nonce,
				signatureType,
				version,
				bodyHash,
				bodyParams,
				standard,
				encodeRequestParameters,
				decodeUrlParameters, 
				false);
		}
		//TODO CCOLE
        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
		/// <param name="allowOauthCallbackParameter">Legacy support of MyspaceID SDK where oauth_callback value is signed</param>
        /// <returns>A base64 string of the hash value</returns>
        public static string GenerateSignature(
            Uri url,
            string consumerKey,
            string consumerSecret,
            string token,
            string tokenSecret,
            string httpMethod,
            string timeStamp,
            string nonce,
            SignatureTypes signatureType,
            string version,
            string bodyHash,
            Dictionary<string, string> bodyParams,
            bool standard,
            bool encodeRequestParameters,
            bool decodeUrlParameters,
			bool allowOauthCallbackParameter)
        {
            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));

                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(
                        url,
                        consumerKey,
                        token,
                        tokenSecret,
                        httpMethod,
                        timeStamp,
                        nonce,
                        HMACSHA1SignatureType,
                        version,
                        bodyHash,
                        bodyParams,
                        standard,
                        encodeRequestParameters,
                        decodeUrlParameters,
						allowOauthCallbackParameter);

                    HMACSHA1 hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), UrlEncode(tokenSecret)));

                    return GenerateSignatureUsingHash(signatureBase, hmacsha1);

                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long totSecs = (long)ts.TotalSeconds;
            return totSecs.ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public static string GenerateNonce()
        {
            //return Crypto random number
            return GetCryptographicRandomNumber(100000000, 999999999).ToString();
        }

        public static int GetCryptographicRandomNumber(int lBound, int uBound)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            
            // Assumes lBound >= 0 && lBound < uBound  // returns an int >= lBound and < uBound  
            uint urndnum;
            byte[] rndnum = new Byte[4];
            
            if (lBound == uBound - 1)
            {
                // test for degenerate case where only lBound can be returned    
                return lBound;
            }
            
            uint xcludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(uBound - lBound)));
            
            do
            {
                rng.GetBytes(rndnum);
                urndnum = System.BitConverter.ToUInt32(rndnum, 0);
            } while (urndnum >= xcludeRndBase);

            return (int)(urndnum % (uBound - lBound)) + lBound;
        }
    }
}