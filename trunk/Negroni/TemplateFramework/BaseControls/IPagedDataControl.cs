using System;

namespace Negroni.TemplateFramework
{
	interface IPagedDataControl
	{
		/// <summary>
		/// Start Index for paging.  By default this is 1.
		/// Value is specified with the "startIndex" attribute.
		/// </summary>
		int StartIndex { get; }

		/// <summary>
		/// Number of items to return on each page of data.
		/// Value is specified with the "count" attribute
		/// </summary>
		int Count { get; }
	}
}
