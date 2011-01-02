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
	/// Response wrapper contract to be used for all public REST endpoint responses.
	/// Use the generic type <c>GenericRestResponse</c> for actual usage
	/// </summary>
	[DataContract]
	public class BaseResponseEnvelope
	{
		/// <summary>
		/// Number of items on each page of data
		/// </summary>
		[DataMemberAttribute(Name = "itemsPerPage", EmitDefaultValue = false)]
		public int ItemsPerPage { get; set; }

		/// <summary>
		/// Start index of this page of data
		/// </summary>
		[DataMemberAttribute(Name = "startIndex", EmitDefaultValue = false)]
		public int StartIndex { get; set; }

		/// <summary>
		/// Total number of items in the entire result set across all data pages
		/// </summary>
		[DataMemberAttribute(Name = "totalResults", EmitDefaultValue = false)]
		public int TotalResults { get; set; }

		[DataMemberAttribute(Name = "isFiltered", EmitDefaultValue = false)]
		public string IsFiltered { get; set; }

		[DataMemberAttribute(Name = "isSorted", EmitDefaultValue = false)]
		public string IsSorted { get; set; }

		[DataMemberAttribute(Name = "isUpdatedSince", EmitDefaultValue = false)]
		public string IsUpdatedSince { get; set; }

		/// <summary>
		/// Error object if an error occurred.  This is only populated when an error occurrs.
		/// </summary>
		[DataMember(Name = "error", EmitDefaultValue = false)]
		public ErrorResponse Error { get; set; }

	}
}
