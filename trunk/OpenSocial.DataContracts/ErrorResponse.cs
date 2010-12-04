using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;


namespace Negroni.OpenSocial.DataContracts
{
	/// <summary>
	/// Error object included in the response when an error has occurred.
	/// </summary>
	[DataContract(Name="error")]
	public class ErrorResponse
	{
		/// <summary>
		/// Integer code as outlined in the JSON-RPC spec for OpenSocial Core API Server
		/// </summary>
		/// <remarks>
		/// See codes at http://opensocial-resources.googlecode.com/svn/spec/1.1/Core-API-Server.xml#RPC-Error
		/// </remarks>
		[DataMember(Name="code", IsRequired=true)]
		public int Code { get; set; }

		/// <summary>
		/// Human readable error message
		/// </summary>
		[DataMember(Name = "message")]
		public string Message { get; set; }

		/// <summary>
		/// JSON object string that give more error information
		/// </summary>
		[DataMember(Name = "data")]
		public string Data { get; set; }

	}
}
