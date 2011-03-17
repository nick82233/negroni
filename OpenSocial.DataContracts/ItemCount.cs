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
using System.Runtime.Serialization;
using System.Text;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContract(Name="itemCount")]
	public class ItemCount
	{
		[DataMember(Name="id")]
		public string Id { get; set; }

		[DataMember(Name="name")]
		public string Name { get; set; }

		/// <summary>
		/// Location URI for this resource
		/// </summary>
		[DataMember(Name = "uri", EmitDefaultValue = false)]
		public string Uri { get; set; }

		/// <summary>
		/// Total value represented by this count.
		/// Often this is the sum of individual values
		/// </summary>
		[DataMember(Name = "total")]
		public int Total { get; set; }


		/// <summary>
		/// Count of the individual data points to make up total
		/// </summary>
		[DataMember(Name = "count", EmitDefaultValue = false)]
		public int Count { get; set; }

	}
}
