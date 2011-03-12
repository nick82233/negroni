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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// DataControl base class for dealing with paged data
	/// </summary>
	public class BasePagedDataControl : BaseDataControl, IPagedDataControl
	{



		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);

			int i;

			if (Int32.TryParse(GetAttribute("startIndex"), out i))
			{
				StartIndex = i;
			}
			if (Int32.TryParse(GetAttribute("count"), out i))
			{
				Count = i;
			}

		}


		private int _startIndex = 1;

		/// <summary>
		/// Start Index for paging.  By default this is 1.
		/// Value is specified with the "startIndex" attribute.
		/// </summary>
		public int StartIndex
		{
			get { return _startIndex; }
			set { _startIndex = value; }
		}


		private int _count;

		/// <summary>
		/// Number of items to return on each page of data.
		/// Value is specified with the "count" attribute
		/// </summary>
		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}

		/// <summary>
		/// Default count size for paging.  Implementing classes should
		/// set this value in constructor to have a different default count.
		/// </summary>
		protected int defaultPageSize = 20;

	}
}
