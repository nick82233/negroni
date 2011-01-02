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
	/// <summary>
	/// Contract for a generic linkage of a user with some resource
	/// </summary>
	[DataContract(Name="idSet")]
	public class GenericIdSet
	{
		[DataMember(Name="userId", EmitDefaultValue=false)]
		public string UserId { get; set; }

		/// <summary>
		/// Relationship between UserId and ResourceId
		/// </summary>
		[DataMember(Name = "relationship", EmitDefaultValue = false)]
		public string Relationship { get; set; }

		/// <summary>
		/// Action to perform on the two IDs
		/// </summary>
		[DataMember(Name = "action", EmitDefaultValue = false)]
		public string Action { get; set; }

		/// <summary>
		/// Result of the action performed
		/// </summary>
		[DataMember(Name = "result", EmitDefaultValue = false)]
		public string Result { get; set; }

		/// <summary>
		/// Unique ID of the resource
		/// </summary>
		[DataMember(Name="resourceId", EmitDefaultValue=false)]
		public string ResourceId { get; set; }

		/// <summary>
		/// URI to the location of the resource
		/// </summary>
		[DataMember(Name = "resourceUri", EmitDefaultValue = false)]
		public string ResourceUri { get; set; }

		/// <summary>
		/// Full Resource object being referenced
		/// </summary>
		[DataMember(Name = "resource", EmitDefaultValue = false)]
		public GenericResource Resource { get; set; }
	}
}
