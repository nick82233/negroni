using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class Url
	{
		[DataMemberAttribute(Name = "value", EmitDefaultValue = false)]
		public string Value;

		[DataMemberAttribute(Name = "linkText", EmitDefaultValue = false)]
		public string LinkText;

		[DataMemberAttribute(Name = "type", EmitDefaultValue = false)]
		public string Type;
	}

}
