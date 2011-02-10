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
using System.Linq;
using System.Text;
using System.Globalization;

namespace Negroni.DataPipeline
{
	public class DataRequestContext
	{
		public DataRequestContext() { }

		private int? viewerId = null;
		public int? ViewerId
		{
			get { return viewerId; }
			set { viewerId = value; }
		}

		private int? ownerId = null;
		public int? OwnerId
		{
			get { return ownerId; }
			set { ownerId = value; }
		}

		private string clientIpAddress = String.Empty;
		public string ClientIpAddress
		{
			get { return string.Empty; }
			set { clientIpAddress = value; }
		}

		private bool isUtc;
		public bool IsUtc
		{
			get { return isUtc; }
			set { isUtc = value; }
		}

		private bool isWap;
		public bool IsWap
		{
			get { return isWap; }
			set { isWap = value; }
		}

		private Dictionary<string, object> propertyBag = new Dictionary<string,object>();
		public Dictionary<string, object> PropertyBag
		{
			get { return propertyBag; }
			set { propertyBag = value; }
		}

		private OpenSocialPermissions permissions = 0;
		public void GrantPermission(OpenSocialPermissions permission)
		{
			permissions |= permission;
		}
		public void RevokePermission(OpenSocialPermissions permission)
		{
			permissions &= ~permission;
		}
		public bool IsPermissionGranted(OpenSocialPermissions permission)
		{
			return (permissions & permission) == permission;
		}

		private int? applicationId;
		public int? ApplicationId
		{
			get { return applicationId; }
			set { applicationId = value; }
		}

		private CultureInfo culture;
		public CultureInfo Culture
		{
			get { return culture; }
			set { culture = value; }
		}
	}
}
