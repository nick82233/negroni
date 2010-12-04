using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public class KeyValue
	{
		[DataMemberAttribute(Name = "key", EmitDefaultValue = false)]
		public string Key;

		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;
	}
}
