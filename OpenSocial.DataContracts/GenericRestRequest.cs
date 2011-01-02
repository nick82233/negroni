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
	/// <summary>
	/// Generic REST request wrapper for public endpoints.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DataContract(Name = "request", Namespace = "http://ns.opensocial.org/2008/opensocial")]
	public class GenericRestRequest<T> : BaseRestRequest
	{
		[DataMemberAttribute(Name = "entry", EmitDefaultValue = false)]
		public T Entry { get; set; }

	}
}
