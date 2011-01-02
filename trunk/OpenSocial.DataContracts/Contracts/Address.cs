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
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Address
	{
		[DataMemberAttribute(Name = "country", EmitDefaultValue = false)]
		public string Country;

		[DataMemberAttribute(Name = "extendedAddress", EmitDefaultValue = false)]
		public string ExtendedAddress;

		[DataMemberAttribute(Name = "latitude", EmitDefaultValue = false)]
		public string Latitude;

		[DataMemberAttribute(Name = "locality", EmitDefaultValue = false)]
		public string Locality;

		[DataMemberAttribute(Name = "longitude", EmitDefaultValue = false)]
		public string Longitude;

		[DataMemberAttribute(Name = "poBox", EmitDefaultValue = false)]
		public string PoBox;

		[DataMemberAttribute(Name = "postalCode", EmitDefaultValue = false)]
		public string PostalCode;

		[DataMemberAttribute(Name = "primary", EmitDefaultValue = false)]
		public string Primary;

		[DataMemberAttribute(Name = "region", EmitDefaultValue = false)]
		public string Region;

		[DataMemberAttribute(Name = "streetAddress", EmitDefaultValue = false)]
		public string StreetAddress;

		[DataMemberAttribute(Name = "type", EmitDefaultValue = false)]
		public string Type;

		[DataMemberAttribute(Name = "formatted", EmitDefaultValue = false)]
		public string Formatted;
	}
}
