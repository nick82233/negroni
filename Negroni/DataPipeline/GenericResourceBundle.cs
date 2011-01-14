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

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Generic implementation of a ResourceBundle.
	/// </summary>
    public class GenericResourceBundle : IResourceBundle
    {
        /// <summary>
        /// Internal catalog of strings
        /// </summary>
		private Dictionary<string, string> bundle = new Dictionary<string, string>();


		public GenericResourceBundle() { }

		public GenericResourceBundle(string cultureCode)
		{
			CultureCode = cultureCode;
		}

		public void AddString(string key, string message)
        {
			//Fail to add string if missing
			if (string.IsNullOrEmpty(key))
			{
				System.Diagnostics.Debug.WriteLine("Bad key attempted to write for message: " + message);
				return;
			}

            bundle.Add(key.ToLowerInvariant(), message);
        }

		/// <summary>
		/// Attempts to get the string value out of this bundle.
		/// On success returns true and places result in <paramref name="message"/>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		/// <returns>True if successful, false if key is not found</returns>
        public bool TryGetString(string key, out string message)
        {
            return bundle.TryGetValue(key.ToLowerInvariant(), out message);
        }

		private string _cultureCode = null;

		/// <summary>
		/// Culture key used for this bundle
		/// </summary>
        public string CultureCode
        {
			get {
				if (string.IsNullOrEmpty(_cultureCode))
				{
					if (!string.IsNullOrEmpty(_languageCode))
					{
						_cultureCode = _languageCode.ToLowerInvariant();
						if (!string.IsNullOrEmpty(_countryCode))
						{
							_cultureCode += "-" + _countryCode.ToUpperInvariant();
						}
					}
					else
					{
						_cultureCode = "ALL";
					}

				}
				return _cultureCode;
			}
			set {
				_cultureCode = value;
				if (string.IsNullOrEmpty(_cultureCode))
				{
					_languageCode = null;
					_countryCode = null;
				}
				int m = _cultureCode.IndexOf("-");
				if (m == -1)
				{
                    _languageCode = _cultureCode.ToLowerInvariant();
				}
				else
				{
                    _languageCode = _cultureCode.Substring(0, m).ToLowerInvariant();
                    _countryCode = _cultureCode.Substring(m + 1).ToUpperInvariant();
				}
			}
        }

		/// <summary>
		/// Tests to see if this resource bundle has values added
		/// </summary>
		/// <returns></returns>
		public bool HasValues()
		{
			return (bundle.Count > 0);
		}

		/// <summary>
		/// Gets a copy of the defined messages.  Each call to this method
		/// creates a new copy of the message bundle.
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetDefinedMessages()
		{
			return new Dictionary<string,string>(bundle);
		}


		public bool HasString(string key)
		{
			return bundle.ContainsKey(key);
		}

		public string GetString(string key)
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Null key");
            key = key.ToLowerInvariant();
			if (bundle.ContainsKey(key))
			{
				return bundle[key];
			}
			return null;
		}

		public string[] GetDefinedKeys()
		{
			List<string> keys = new List<string>();
			foreach (string key in bundle.Keys)
			{
				keys.Add(key);
			}
			return keys.ToArray();
		}


		private string _languageCode = null;
		/// <summary>
		/// Language code component of the culture
		/// </summary>
		public string LanguageCode
		{
			get { return _languageCode; }
		}

		private string _countryCode = null;
		/// <summary>
		/// Country code component of the culture
		/// </summary>
		public string CountryCode
		{
			get { return _countryCode; }
		}

		public bool IsInvariantCulture
		{
			get
			{
				if (forceInvariantCulture.HasValue)
				{
					return forceInvariantCulture.Value;
				}
				else
				{
					return (string.IsNullOrEmpty(CultureCode) 
						|| CultureCode == ResourceBundleCatalog.INVARIANT_CULTURE_KEY);
				}
			}
		}

		public bool IsInvariantLanguage
		{
			get { return string.IsNullOrEmpty(CountryCode); }
		}

		private bool? forceInvariantCulture;
		/// <summary>
		/// Forces the current resource bundle to behave as the invariant culture
		/// </summary>
		/// <param name="isInvariant"></param>
		/// <returns></returns>
		public void SetAsInvariantCulture(bool isInvariant)
		{
			if (isInvariant)
			{
				forceInvariantCulture = true;
			}
			else
			{
				forceInvariantCulture = null;
			}
		}
	}
}
