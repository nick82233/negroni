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
	/// Item in an <c>OffsetList</c> containing position and type of item
	/// </summary>
	public class OffsetItem : IDisposable, ICloneable
	{
		/// <summary>
		/// Encapsulation of delimiters used in serialization/deserialization
		/// </summary>
		public static class Delimiters
		{
			/// <summary>
			/// inside item - between number and type enum
			/// </summary>
			public const char NUMBER_TO_TYPE = ':';

			/// <summary>
			/// splits a start/end position for an OffsetItem
			/// </summary>
			public const char NUMBER_RANGE = '-';
			/// <summary>
			/// between OffsetItems in an OffsetList
			/// </summary>
			public const char OFFSETITEM = '|';
			/// <summary>
			/// Beginning of a ChildList
			/// </summary>
			public const char CHILDLIST_START = '{';
			/// <summary>
			/// End of a ChildList
			/// </summary>
			public const char CHILDLIST_END = '}';

			/// <summary>
			/// Prefix indicator of a closing tag for an inline replaced OSML tag set
			/// ex: os:form
			/// </summary>
			public const char INLINEREPLACE_CLOSETAG = '~';
		}

		public OffsetItem() { }

		public OffsetItem(string serializedItem)
		{
			DeserializeString(serializedItem);
		}

		/// <summary>
		/// Deserializes and loads values from a string into current object
		/// </summary>
		/// <param name="serializedItem"></param>
		public void DeserializeString(string serializedItem)
		{
			if (String.IsNullOrEmpty(serializedItem))
			{
				throw new ArgumentException("No data to deserialize");
			}

			int pos = serializedItem.IndexOf(Delimiters.NUMBER_TO_TYPE);
			if (pos == -1)
			{
				ThrowBadSerializationFormat(serializedItem);
			}

			//try to split and parse number, then val
			LoadPositionSerializedValues(serializedItem.Substring(0, pos));

			//parse value
			string typePart = serializedItem.Substring(pos + 1, (serializedItem.Length - pos) - 1);
			LoadTypeSerializedValues(typePart);


		}

		/// <summary>
		/// Loads the type of this item and parses child offsets
		/// </summary>
		/// <param name="typePart"></param>
		private void LoadTypeSerializedValues(string typePart)
		{
			int childListPos = typePart.IndexOf(Delimiters.CHILDLIST_START);
			bool hasChildList = (-1 != childListPos);

			string offsetKey;
			if (hasChildList)
			{
				offsetKey = typePart.Substring(0, childListPos);
			}
			else
			{
				offsetKey = typePart;
			}

			//OffsetItemType parse
			int closeFlag = offsetKey.IndexOf(Delimiters.INLINEREPLACE_CLOSETAG);
			//no child items.
			if (closeFlag > -1 && closeFlag == 0) //hmm. only at front.  Maybe only check for 0
			{
				this.IsClosingReplaceTag = true;
			}
			this.OffsetKey = offsetKey.Substring(closeFlag + 1);

			if (!hasChildList)
			{
				return;
			}

			int closeListPos = typePart.LastIndexOf(Delimiters.CHILDLIST_END);
			if (-1 == closeListPos)
			{
				ThrowBadSerializationFormat(typePart);
			}

			this.ChildOffsets.DeserializeString(typePart.Substring(childListPos + 1, (closeListPos - childListPos) - 1));
            if (ChildOffsets.Count > 0)
            {
                foreach (OffsetItem item in ChildOffsets)
                {
                    item.ParentOffset = this;
                }
            }
		}

		/// <summary>
		/// Parses the number part of a serialized OffsetItem and loads the values
		/// </summary>
		/// <param name="numPart"></param>
		private void LoadPositionSerializedValues(string numPart)
		{
			int numValue;
			if (-1 == numPart.IndexOf(Delimiters.NUMBER_RANGE))
			{
				if (Int32.TryParse(numPart, out numValue))
				{
					Position = numValue;
				}
				else
				{
					ThrowBadSerializationFormat(numPart);
				}
			}
			else
			{
				int ptemp = numPart.IndexOf(Delimiters.NUMBER_RANGE);
				if (Int32.TryParse(numPart.Substring(0, ptemp), out numValue))
				{
					Position = numValue;
				}
				else
				{
					ThrowBadSerializationFormat(numPart);
				}
				//end number
				if (Int32.TryParse(numPart.Substring(ptemp + 1, numPart.Length - ptemp - 1), out numValue))
				{
					EndPosition = numValue;
				}
			}
		}

		/// <summary>
		/// Encapsulates throwing a formatted serialization message
		/// </summary>
		/// <param name="serializedItem"></param>
		private void ThrowBadSerializationFormat(string serializedItem)
		{
			throw new ArgumentException("Bad serialized string format: "
				+ serializedItem.Substring(0, Math.Min(45, serializedItem.Length)));
		}



        /// <summary>
        /// Calculates the absolute position of local <c>Position</c>
        /// value within entire markup.
        /// </summary>
        /// <remarks>
        /// Moves up the parent reference tree to calculate offset position from
        /// all local references.  Helps facilitate single-pass (non-recursive) rendering.
        /// </remarks>
        /// <returns></returns>
        public int GetAbsolutePosition()
        {
            return GetAbsolutePosition(Position);
        }

		/// <summary>
		/// Calculates the absolute end position a local <c>EndPosition</c>
		/// within the entire markup.  Returns 0 if EndPosition is not set
		/// </summary>
		/// <returns></returns>
		public int GetAbsoluteEndPosition()
		{
			return GetAbsoluteEndPosition(0);
		}

		/// <summary>
		/// Calculates the absolute end position a local <c>EndPosition</c> within the entire markup.  
		/// If endPosition is not found, nextSiblingStartPosition will be used to calculate
		/// an absolute endPosition
		/// </summary>
		/// <param name="nextSiblingStartPosition">Relative end position of the next sibling element</param>
		/// <returns></returns>
		public int GetAbsoluteEndPosition(int nextSiblingStartPosition)
		{
			if (0 == this.EndPosition && 0 == nextSiblingStartPosition) return 0;
			int end = this.EndPosition;
			if (0 == end)
			{
				end = nextSiblingStartPosition;
			}
			return GetAbsolutePosition(end);
		}

        /// <summary>
        /// Calculates absolute position within root parent markup of a given local index
        /// </summary>
        /// <param name="relativePosition"></param>
        /// <returns></returns>
        private int GetAbsolutePosition(int relativePosition)
        {
            int pos = relativePosition;

            //look upward to find local offset
            int breakoutMax = 1000;
            int loopCount = 0;
            int diff = 0;
            OffsetItem oi = this;
            if (null != oi.ParentOffset)
            {
                while (null != oi.ParentOffset 
                    && oi != oi.ParentOffset 
                    && ++loopCount < breakoutMax)
                {
                    oi = oi.ParentOffset;
                    diff += oi.Position;
                }
            }

            return pos + diff;
        }



		/// <summary>
		/// Constructs object values into a serialized string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Position);
			if (EndPosition > 0)
			{
				sb.Append(Delimiters.NUMBER_RANGE).Append(EndPosition);
			}
			sb.Append(Delimiters.NUMBER_TO_TYPE).Append(this.OffsetKey);
			if (this.HasChildList())
			{
				sb.Append(Delimiters.CHILDLIST_START);

				ChildOffsets.WriteToString(sb);

				sb.Append(Delimiters.CHILDLIST_END);
			}

			return sb.ToString();
		}



		public OffsetItem(int position, string offsetKey)
		{
			Position = position;
			OffsetKey = offsetKey;
		}
		public OffsetItem(long position, string offsetKey)
		{
			Position = Convert.ToInt32(Math.Min(Int32.MaxValue, position));
			OffsetKey = offsetKey;
		}

		public OffsetItem(int position, int endPosition, string offsetKey)
		{
			Position = position;
			EndPosition = endPosition;
			OffsetKey = offsetKey;
		}

		public OffsetItem(long position, long endPosition, string offsetKey)
			: this(Convert.ToInt32(Math.Max(Int32.MaxValue, position)), Convert.ToInt32(Math.Max(Int32.MaxValue, endPosition)), offsetKey)
		{ }


		/// <summary>
		/// OffsetKey value used to identify the associated control Type.
		/// Often this is the control's class name
		/// </summary>
		public string OffsetKey { get; set; }

		/// <summary>
		/// Character position offset from start of parent item.
		/// </summary>
		/// <remarks>
		/// Allows for O(1) lookups on renderings for all tags and static content.
		/// The end position may be obtained by taking the next OffsetItem.Position value -1
		/// if the <c>EndPosition</c> value is not specified.
		/// </remarks>
		public int Position { get; set; }


		/// <summary>
		/// Final position of current tag.  Allows for specifying end and scipping kruft.
		/// </summary>
		public int EndPosition { get; set; }

		private OffsetList _childOffsets = null;
		/// <summary>
		/// Child offset items for this parent item
		/// </summary>
		public OffsetList ChildOffsets
		{
			get
			{
				if (_childOffsets == null)
				{
					_childOffsets = new OffsetList();
					_childOffsets.ParentOffset = this;
				}
				return _childOffsets;
			}
		}


		public bool HasChildList()
		{
			return (_childOffsets != null && _childOffsets.Count > 0);
		}

		/// <summary>
		/// Flag to indicate that this is a closing tag from an IOsReplaceTagSet
		/// </summary>
		public bool IsClosingReplaceTag { get; set; }


		/// <summary>
		/// Used when building an OffsetTree in TraceOffsetParser
		/// </summary>
		internal OffsetItem ParentOffset { get; set; }


		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (null != ParentOffset)
				{
					ParentOffset = null;
				}
				if (HasChildList())
				{
					ChildOffsets.Dispose();
				}
			}


		}


		#endregion

		#region ICloneable Members

		/// <summary>
		/// Performs a deep clone of the item.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			OffsetItem newItem = new OffsetItem(this.ToString());
			return newItem;
		}
		#endregion

		/// <summary>
		/// Tests to see if two OffsetItems are equal.
		/// Disregards ParentOffset.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (!(obj is OffsetItem))
			{
				return false;
			}
			OffsetItem other = (OffsetItem)obj;

			return (this.Position == other.Position && this.EndPosition == other.EndPosition && this.OffsetKey == other.OffsetKey);
		}

		public override int GetHashCode()
		{
			int a = 0;
			if(!string.IsNullOrEmpty(this.OffsetKey)){
				a = OffsetKey.GetHashCode();
			}
			return a + Position + EndPosition;
		}

	}
}
