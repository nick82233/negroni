using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.OpenSocial.DataContracts
{
	[DataContract(Name = "application", Namespace = "http://ns.myspace.com/roa/09/application")]
	public class ApplicationItem
	{
		[DataMember(Name = "appId")]
		public int AppId { get; set; }

		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "iconUrl", EmitDefaultValue = false)]
		public string IconUrl { get; set; }

		[DataMember(Name = "thumbnailUrl", EmitDefaultValue = false)]
		public string ThumbnailUrl { get; set; }

		[DataMember(Name = "title", EmitDefaultValue = false)]
		public string Title { get; set; }

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(Name = "categories", EmitDefaultValue = false)]
		public List<string> Categories { get; set; }

		[DataMember(Name = "companyUrl", EmitDefaultValue = false)]
		public string CompanyUrl { get; set; }

		[DataMember(Name = "appType", EmitDefaultValue = false)]
		public string AppType { get; set; }

		[DataMember(Name = "status", EmitDefaultValue = false)]
		public string Status { get; set; }

		[DataMember(Name = "installs", EmitDefaultValue = false)]
		public string Installs { get; set; }

//		[DataMember(Name = "author", EmitDefaultValue = false)]
//		public string Author { get; set; }

		[DataMember(Name = "developer", EmitDefaultValue = false)]
		public Person Developer { get; set; }

		[DataMember(Name = "canvasUrl", EmitDefaultValue = false)]
		public string CanvasUrl { get; set; }

		[DataMember(Name = "mobilecanvasUrl", EmitDefaultValue = false)]
		public string MobileCanvasUrl { get; set; }

		/// <summary>
		/// Held as a list of pipe-delimited view|url pairs
		/// </summary>
		[DataMember(Name = "renderUrls", EmitDefaultValue = false)]
		public List<string> RenderUrls { get; set; }

		[DataMember(Name = "views", EmitDefaultValue = false)]
		public List<string> ViewList { get; set; }

		/// <summary>
		/// Member to get or set IsCopyrightInfringement
		/// </summary>
		[DataMember(Name = "isCopyrightInfringement", EmitDefaultValue = false)]
		public bool IsCopyrightInfringement { get; set; }

	}
}
