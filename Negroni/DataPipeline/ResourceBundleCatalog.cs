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
    /// Catalog of localized resource resourceStringCatalog
    /// </summary>
	public class ResourceBundleCatalog : IExpressionEvaluator
	{
		/// <summary>
		/// Key to use for invariant cultures
		/// </summary>
		public const string INVARIANT_CULTURE_KEY = "ALL";

		#region Properties

		/// <summary>
		/// Dictionary of resourceStringCatalog based on culture code
		/// </summary>
		private Dictionary<string, IResourceBundle> bundles = new Dictionary<string, IResourceBundle>();

		#endregion


		private IResourceBundle _invariantCultureBundle = null;

		/// <summary>
		/// Final fallback Bundle for the invariant culture
		/// </summary>
		public IResourceBundle InvariantCultureBundle
		{
			get
			{
				if (_invariantCultureBundle == null)
				{
					IResourceBundle fallbackInvariantBundle = null;
					foreach (KeyValuePair<string, IResourceBundle> keyset in bundles)
					{
						if (keyset.Value.IsInvariantCulture)
						{
							_invariantCultureBundle = keyset.Value;
							break;
						}
						if (keyset.Key.Equals("en-us", StringComparison.InvariantCultureIgnoreCase)
							|| keyset.Key.Equals("en", StringComparison.InvariantCultureIgnoreCase)
							|| keyset.Key.Equals(INVARIANT_CULTURE_KEY, StringComparison.InvariantCultureIgnoreCase))
						{
							fallbackInvariantBundle = keyset.Value;
						}
					}
					if (_invariantCultureBundle == null)
					{
						if (fallbackInvariantBundle != null)
						{
							_invariantCultureBundle = fallbackInvariantBundle;
						}
						else
						{
							_invariantCultureBundle = new GenericResourceBundle();
						}
					}
				}
				return _invariantCultureBundle;
			}
		}

		/// <summary>
		/// Registers a new string bundle for the culture
		/// </summary>
		/// <param name="bundle"></param>
		/// <returns></returns>
		public void AddBundle(IResourceBundle bundle)
		{
			if (bundle == null)
				throw new ArgumentNullException("bundle");

			if (bundle.CultureCode == null)
			{
				bundle.CultureCode = INVARIANT_CULTURE_KEY;
			}

			if (bundles.ContainsKey(bundle.CultureCode))
			{
				if (bundle.HasValues())
				{
					if (bundles[bundle.CultureCode].HasValues())
					{
						//throw new Exception("Culture already defined: " + bundle.CultureCode);
						string[] newkeys = bundle.GetDefinedKeys();
						for (int i = 0; i < newkeys.Length; i++)
						{
							if (!bundles[bundle.CultureCode].HasString(newkeys[i]))
							{
								bundles[bundle.CultureCode].AddString(newkeys[i], bundle.GetString(newkeys[i]));
							}
						}

					}
					else
					{
						bundles[bundle.CultureCode] = bundle;
					}
				}
			}

			if (bundle.IsInvariantCulture || INVARIANT_CULTURE_KEY == bundle.CultureCode)
			{
				bundles[INVARIANT_CULTURE_KEY] = bundle;
				_invariantCultureBundle = bundle;
			}
			else
			{
				if(!bundles.ContainsKey(bundle.CultureCode)){
					bundles.Add(bundle.CultureCode, bundle);
				}
			}

		}

		/// <summary>
		/// Preferred culture code for current message processing
		/// </summary>
		public string PreferredCulture { get; set; }

		/// <summary>
		/// Gets an array of cultures defined in this ResourceBundleCatalog.
		/// </summary>
		/// <returns></returns>
		public string[] GetDefinedCultures()
		{
			List<string> cultureKeys = new List<string>();
			foreach (string key in bundles.Keys)
			{
				cultureKeys.Add(key);
			}
			return cultureKeys.ToArray();
		}

		/// <summary>
		/// Obtains a string array of the keys specifically defined
		/// for the given culture.
		/// </summary>
		/// <param name="culture"></param>
		/// <returns></returns>
		public string[] GetKeysDefinedForCulture(string culture)
		{
			IResourceBundle bundle = null;
			if (culture == INVARIANT_CULTURE_KEY)
			{
				bundle = InvariantCultureBundle;
			}
			else
			{
				if (bundles.ContainsKey(culture))
				{
					bundle = bundles[culture];
				}
				else
				{
					if (bundles.ContainsKey(culture.ToLower()))
					{
						bundle = bundles[culture.ToLower()];
					}
					else if (bundles.ContainsKey(culture.ToUpper()))
					{
						bundle = bundles[culture.ToUpper()];
					}
					else
					{
						return new string[] { };
					}
				}
			}
			if (bundle != null)
			{
				return bundle.GetDefinedKeys();
			}
			return null;
		}


		/// <summary>
		/// Resolves the message using the currently set
		/// LanguageCode and CountryCode.
		/// If message is not found logic falls back to less specific culture,
		/// then invarient culture.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetMessage(string key, string preferredCulture)
		{
			if(string.IsNullOrEmpty(key)){
				throw new ArgumentNullException("Resource string Key must be specified");
			}

            key = key.ToLowerInvariant();

            string message = null;

			if (string.IsNullOrEmpty(preferredCulture))
			{
				preferredCulture = INVARIANT_CULTURE_KEY;
			}
			else
			{
                preferredCulture = preferredCulture.ToLowerInvariant();
			}

			if (bundles.ContainsKey(preferredCulture))
			{
				if (bundles[preferredCulture].TryGetString(key, out message))
				{
					return message;
				}
			}
			int idx = preferredCulture.IndexOf("-");
			if (idx > -1)
			{
				string lang = preferredCulture.Substring(0, idx);
				if (bundles.ContainsKey(lang))
				{
					if (bundles[lang].TryGetString(key, out message))
					{
						return message;
					}
				}
			}

			if (InvariantCultureBundle.TryGetString(key, out message))
			{
				return message;
			}
			return null;
        }

		#region IExpressionEvaluator Members

		public object ResolveExpressionValue(string expression)
		{
			return GetMessage(expression, PreferredCulture);
		}

		#endregion
	}
}