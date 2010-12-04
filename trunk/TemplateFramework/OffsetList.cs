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

namespace Negroni.TemplateFramework
{

	/// <summary>
	/// List of nested OffsetItems.  
	/// This is used to represent offsets for controls inside a markup block.
	/// </summary>
	/// <remarks>
	/// By using an offset list along with full markup we get (O)1 lookup for controls.
	/// </remarks>
	public class OffsetList : List<OffsetItem>, IDisposable
	{

		public OffsetList() : base()
		{ }

		public OffsetList(string serializedList)
		{
			DeserializeString(serializedList);
		}

		/// <summary>
		/// Used when building an OffsetTree in TraceOffsetParser
		/// </summary>
		internal OffsetItem ParentOffset { get; set; }


		public void DeserializeString(string serializedList)
		{
			this.Clear();
			if(String.IsNullOrEmpty(serializedList)){
				return;
			}

			int lastStartPos = 0;
			string curItemStr;
			int currentPos = GetCurrentListStartPos(serializedList, lastStartPos);
			while (-1 != currentPos)
			{
				curItemStr = serializedList.Substring(lastStartPos, currentPos - lastStartPos);
				this.Add(new OffsetItem(curItemStr));
				lastStartPos = currentPos + 1;
				if(lastStartPos >= serializedList.Length){
					break;
				}
				currentPos = GetCurrentListStartPos(serializedList, lastStartPos);
			}
			//get last item if it wasn't a list
			if (lastStartPos < (serializedList.Length - 1))
			{
				curItemStr = serializedList.Substring(lastStartPos, serializedList.Length - lastStartPos);
				this.Add(new OffsetItem(curItemStr));
			}
			return;

		}

		private int GetCurrentListStartPos(string serializedList, int lastStartPos)
		{
			int nestedListPos = serializedList.IndexOf(OffsetItem.Delimiters.CHILDLIST_START, lastStartPos);
			int currentPos = serializedList.IndexOf(OffsetItem.Delimiters.OFFSETITEM, lastStartPos);
			if (nestedListPos > -1 && currentPos > -1)
			{
				//there is a sub-list before next peer-level delim
				if (nestedListPos < currentPos)
				{
					currentPos = FindMatchingListCloseBracket(serializedList, nestedListPos + 1);
					currentPos = Math.Min(serializedList.Length, currentPos + 1);
				}

			}
			return currentPos;
		}

		/// <summary>
		/// Find the closing bracket for current list level
		/// </summary>
		/// <param name="fullSerializedList">Full string containing serialized items</param>
		/// <param name="startIndex">Starting position in full list to Count from</param>
		/// <returns></returns>
		private int FindMatchingListCloseBracket(string fullSerializedList, int startIndex)
		{
			int depth = 1;
			int endOfList = startIndex;
			//search until depth == 0
			for (; endOfList < fullSerializedList.Length; endOfList++)
			{
				char thisChar = fullSerializedList.ToCharArray(endOfList, 1)[0];
				if (thisChar == OffsetItem.Delimiters.CHILDLIST_START)
				{
					depth++;
				}
				else if (thisChar == OffsetItem.Delimiters.CHILDLIST_END)
				{
					depth--;
				}
				if (0 == depth)
				{
					break;
				}				
			}
			return endOfList;
		}


		///// <summary>
		///// Internal list of offsets
		///// </summary>
		//public List<OffsetItem> Offsets = new List<OffsetItem>();

		public OffsetItem AddOffset(long position, string offsetKey)
		{
			OffsetItem item = new OffsetItem(position, offsetKey);
			item.ParentOffset = this.ParentOffset;
			this.Add(item);
			return item;
		}
		public OffsetItem AddOffset(long position, int endPosition, string offsetKey)
		{
			OffsetItem item = new OffsetItem(position, offsetKey);
			item.EndPosition = endPosition;
			item.ParentOffset = this.ParentOffset;
			this.Add(item);
			return item;
		}

		public OffsetItem AddOffset(long position, long endPosition, string offsetKey)
		{
			return AddOffset(position, Convert.ToInt32(endPosition), offsetKey);
		}


		/// <summary>
		/// Returns a serialized verison of this list.
		/// You may also load results into an existing StringBuilder instance
		/// by calling <c>WriteToString</c>
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (Count == 0) return String.Empty;
			if (1 == Count) return this[0].ToString();

			StringBuilder sb = new StringBuilder();
			WriteToString(sb);
			return sb.ToString();

		}

		/// <summary>
		/// Loads a StringBuilder with the results of a ToString operation
		/// </summary>
		/// <param name="stringBuilder">StringBuilder to write results into</param>
		public void WriteToString(StringBuilder stringBuilder)
		{
			int lastIndex = this.Count - 1;
			for (int i = 0; i < this.Count; i++)
			{
				OffsetItem child = this[i];
				stringBuilder.Append(child.ToString());
				if (i < lastIndex)
				{
					stringBuilder.Append(OffsetItem.Delimiters.OFFSETITEM);
				}
			}
		}




		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}
		protected void Dispose(bool disposing)
		{
			this.ParentOffset = null;
			foreach (OffsetItem item in this)
			{
				item.Dispose();				
			}
		}

		#endregion
	}

}
