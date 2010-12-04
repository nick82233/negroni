using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContractAttribute(Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public partial class UserAppData
	{
		[DataMemberAttribute(Name = "personId", EmitDefaultValue = false)]
		public string PersonId;

		[DataMemberAttribute(Name = "appData", EmitDefaultValue = false)]
		public List<KeyValue> AppDataArray;
	}
}
