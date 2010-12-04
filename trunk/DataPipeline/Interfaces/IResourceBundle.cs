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

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Interface for localized string container.  In Gadgets this corresponds to 
	/// a MessageBundle
	/// </summary>
	public interface IResourceBundle
	{

		/// <summary>
		/// Registers a new message under the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		void AddString(string key, string message);

		/// <summary>
		/// Culture code defined for this resource bundle
		/// </summary>
		string CultureCode { get; set;}

		/// <summary>
		/// Language code.  Language component of culture
		/// </summary>
		string LanguageCode { get; }

		/// <summary>
		/// Country code.  Country component of culture
		/// </summary>
		string CountryCode { get; }

		/// <summary>
		/// Identifies if this is the invariant culture.
		/// </summary>
		bool IsInvariantCulture { get; }

		/// <summary>
		/// Identifies if this is the invariant language
		/// </summary>
		bool IsInvariantLanguage { get; }

		/// <summary>
		/// Attempts to get a message based on the given key.
		/// If the key is defined, results are placed in message.
		/// If key not defined, false is returned.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		bool TryGetString(string key, out string message);

		/// <summary>
		/// Tests to see if then given key has been registered
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool HasString(string key);

		/// <summary>
		/// Gets a message for the given key.
		/// Returns null if not found
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string GetString(string key);

		/// <summary>
		/// Tests to see if any values have been defined
		/// </summary>
		/// <returns></returns>
		bool HasValues();

		/// <summary>
		/// Gets a string array of all keys defined in this bundle.
		/// </summary>
		/// <returns></returns>
		string[] GetDefinedKeys();

	}
}
