using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public class PluralValueSet
	{
		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;

		[DataMemberAttribute(Name = "type", EmitDefaultValue = false)]
		public string Type;

		[DataMemberAttribute(Name = "primary", EmitDefaultValue = false)]
		public bool Primary;
	}
}
