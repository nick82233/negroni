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

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Encapsulation of error information about a message bundle item
	/// </summary>
	public class MessageBundleItemError
	{
		public MessageBundleItemError() { }

		public MessageBundleItemError(string key)
			: this(key, null, null)
		{}
		public MessageBundleItemError(string key, string culture)
			: this(key, culture, null)
		{}

		public MessageBundleItemError(string key, string culture, string message)
		{
			Key = key;
			Culture = culture;
			Message = message;
		}
		public MessageBundleItemError(string key, string culture, MessageBundleErrorType errorType)
			: this(key, culture, null)
		{
			ErrorType = errorType;
		}
		public MessageBundleItemError(string key, string culture, string message, MessageBundleErrorType errorType)
			: this(key, culture, message)
		{
			ErrorType = errorType;
		}


		public string Key { get; set; }

		public string Culture { get; set; }


		private MessageBundleErrorType _errorType = MessageBundleErrorType.None;

		public MessageBundleErrorType ErrorType
		{
			get
			{
				return _errorType;
			}
			set
			{
				_errorType = value;
			}
		}

		public string Message { get; set; }

	}
}
