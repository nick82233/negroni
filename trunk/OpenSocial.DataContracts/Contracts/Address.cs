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
