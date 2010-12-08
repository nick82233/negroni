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
using System.Text;

namespace Negroni.OpenSocial.Gadget
{
	/// <summary>
	/// Fixed width/height size
	/// </summary>
	/// <remarks>
	/// TODO: Make use of object handling fixed or percentage size
	/// </remarks>
	public class ViewSize
	{

		public const int DEFAULT_CANVAS_WIDTH = 0;
		public const int DEFAULT_HOME_WIDTH = 290;
		public const int DEFAULT_PROFILE_WIDTH = 300;


		public ViewSize() { }
		/// <summary>
		/// Initializes with source in form of width,height or just width
		/// </summary>
		/// <param name="source"></param>
		public ViewSize(string source)
			: this(source, false)
		{ }

		public ViewSize(string source, bool ignoreWidth)
		{
			if (string.IsNullOrEmpty(source)) { return; }

			int ht = 0;
			int wd = 0;
			if (source.IndexOf(",") > -1)
			{
				string[] parts = source.Split(new char[] { ',' });
				if (parts.Length >= 2)
				{
					if (!ignoreWidth)
					{
						if (Int32.TryParse(parts[0], out wd))
						{
							Width = wd;
						}
					}
					if (Int32.TryParse(parts[1], out ht))
					{
						Height = ht;
					}
				}
			}
			else
			{
				if (Int32.TryParse(source, out ht))
				{
					Height = ht;
				}
			}
		}

		public ViewSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public int Width;
		public int Height;
	}
}
