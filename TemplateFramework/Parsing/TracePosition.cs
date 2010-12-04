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

namespace Negroni.TemplateFramework.Parsing
{
	/// <summary>
	/// Object to encapsulate all current positions within a trace parse.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("Pos: {CurrentPosition}  Contx: {Context.ContextName} Root: {RootOffset} Curr: {CurrentOffset}")]
	class TracePosition
	{
		/// <summary>
		/// Current character position within the trace
		/// </summary>
		public int CurrentPosition = 0;

		/// <summary>
		/// Position the most recent unwritten static item starts at
		/// </summary>
		public int StaticTagStartPosition = 0;

		/// <summary>
		/// Flag to notify if trace is within a static tag
		/// </summary>
		public bool InStaticTag = false;

		/// <summary>
		/// Tag element that was just passed. This helps us track position
		/// on nested elements of the same type
		/// </summary>
		public string PreviousStartTag = null;

		/// <summary>
		/// End tag element that was just passed. This helps us track position
		/// on nested elements of the same type
		/// </summary>
		public string PreviousEndTag = null;

		/// <summary>
		/// Flag to identify the prior loop as being a start element
		/// </summary>
		public bool PriorLoopWasStartElement = false;

		/// <summary>
		/// Info on current non-container tag
		/// </summary>
		public NonContainerTagInfo CurrentNonContainerTagInfo;

		private bool _inNonContainerControl = false;
		/// <summary>
		/// Flag to indicate trace is within a non-container tag.
		/// If true, everything is to be treated as static internal contents
		/// until closing element is found
		/// </summary>
		public bool InNonContainerControl
		{
			get
			{
				return _inNonContainerControl;
			}
			private set
			{
				_inNonContainerControl = value;
			}
		}

		/// <summary>
		/// Marks the trace as being inside a non-container tag.
		/// </summary>
		/// <param name="nodeName"></param>
		/// <param name="nodeDepth"></param>
		/// <param name="offsetKey"></param>
		public void SetCurrentNodeAsNonContainerTag(string nodeName, int nodeDepth, string offsetKey)
		{
			if (InStaticTag)
			{
				FlushLiteralControlOffset();
			}

			InNonContainerControl = true;
			CurrentNonContainerTagInfo.Tag = nodeName;
			CurrentNonContainerTagInfo.NodeDepth = nodeDepth;
			CurrentNonContainerTagInfo.StartPosition = CurrentPosition;
			CurrentNonContainerTagInfo.OffsetKey = offsetKey;
		}

		/// <summary>
		/// Writes the current non-container tag offset information
		/// </summary>
		public void FlushNonContainerTagOffset()
		{
			if (!InNonContainerControl) return;

			//throw if the static tag info is also set
			if (InStaticTag)
			{
				throw new Exception(
					String.Format("Invalid static set discovered when flushing non-container {0}.  Position: {1}"
					, CurrentNonContainerTagInfo.Tag, CurrentPosition));
			}

			//write remainder of static
			OffsetItem tmp = this.AddChildOffset(GetNonContainerTagStartLocalPosition(), CurrentNonContainerTagInfo.OffsetKey);
			tmp.EndPosition = GetLocalPosition();
			CurrentNonContainerTagInfo.StartPosition = 0;
			CurrentNonContainerTagInfo.Tag = null;
			CurrentNonContainerTagInfo.OffsetKey = null;
			CurrentNonContainerTagInfo.NodeDepth = 0;
			this.InNonContainerControl = false;


		}


		/// <summary>
		/// Stack holding information about nested osml container-type elements encountered
		/// in the parse.
		/// </summary>
		/// <remarks>Each item in the list represents a container element and stores the
		/// tag and associated XML node depth.  The bottom of the stack (first item) will
		/// be an OsTemplate instace.</remarks>
		public Stack<TraceContainerTagInfo> ContainerTagStack = new Stack<TraceContainerTagInfo>();


		public Stack<ParseContext> ParseContextStack = new Stack<ParseContext>();

		public void AddCurrentNodeAsContainerTag(string nodeName, int nodeDepth)
		{
			TraceContainerTagInfo containerTagInfo;
			containerTagInfo.NodeDepth = nodeDepth;
			containerTagInfo.Tag = nodeName;
			this.ContainerTagStack.Push(containerTagInfo);
			if (this.CurrentOffset.ChildOffsets.Count > 0)
			{
				this.CurrentOffset = CurrentOffset.ChildOffsets[CurrentOffset.ChildOffsets.Count - 1];
			}
		}


		/// <summary>
		/// Root level offset in the Offsets tree
		/// </summary>
		public OffsetItem _rootOffset = null;


		/// <summary>
		/// Root offset of the trace.  
		/// If this is not manually set it is initialized with a generic BaseContainerControl
		/// </summary>
		public OffsetItem RootOffset
		{
			get
			{
				//if (null == _rootOffset)
				//{
				//    _rootOffset = new OffsetItem(0, ControlFactory.RESERVEDKEY_GENERIC_CONTAINER);
				//}
				return _rootOffset;
			}
			set
			{
				_rootOffset = value;
			}
		}

		/// <summary>
		/// Adds a new ChildOffset to the CurrentOffset.
		/// If the root has not been initialized this new offset will become
		/// the root offset.
		/// </summary>
		/// <param name="localPosition"></param>
		/// <param name="offsetKey"></param>
		/// <returns></returns>
		public OffsetItem AddChildOffset(int localPosition, string offsetKey)
		{
			if (!IsInitialized())
			{
				RootOffset = new OffsetItem(localPosition, offsetKey);
				CurrentOffset = RootOffset; //this would happen naturally
				RootOffset.ParentOffset = RootOffset;
				return CurrentOffset;
			}
			else
			{
				return CurrentOffset.ChildOffsets.AddOffset(localPosition, offsetKey);
			}
			//    //{
			//    //    int firstIndex = markup.IndexOf("<" + reader.Name);
			//    //    string firstTag = markup.Substring(firstIndex, GetEndOfTagPosition(markup, firstIndex) - firstIndex);
			//    //    trace.RootOffset = MyControlFactory.CreateRootOffset(markup, context);
			//    //}
			//if(_currentOffset == null)
			//CurrentOffset.ChildOffsets.AddOffset(trace.GetLocalPosition(), MyControlFactory.GetOffsetKey(t, trace.Context));

		}


		private OffsetItem _currentOffset = null;

		/// <summary>
		/// Currently handled offset
		/// </summary>
		public OffsetItem CurrentOffset
		{
			get
			{
				if (_currentOffset == null)
				{
					_currentOffset = RootOffset;
				}
				return _currentOffset;
			}
			set
			{
				_currentOffset = value;
			}
		}


		/// <summary>
		/// Current level of parsing being handled
		/// </summary>
		public ParseContext Context
		{
			get
			{
				if (this.ParseContextStack.Count == 0)
				{
					return ParseContext.DefaultContext;
				}
				else
				{
					return ParseContextStack.Peek();
				}
			}
		}


		/// <summary>
		/// Flag to indicate the trace is at the beginning of a section.
		/// This aids the code in discarding leading whitespace and
		/// empty lines.
		/// </summary>
		public bool AtStartOfContextSection { get; set; }


		/// <summary>
		/// Gets the <c>TraceContainerTagInfo</c> item which is current 
		/// (on the top of the stack)
		/// </summary>
		/// <returns></returns>
		public TraceContainerTagInfo GetCurrentContainerTag()
		{
			if (ContainerTagStack.Count == 0)
			{
				return new TraceContainerTagInfo();
			}
			else
			{
				return ContainerTagStack.Peek();
			}
		}
		/// <summary>
		/// Calculates the local position of the StaticTagStartPosition (static item) by accounting
		/// for the containing element's offset
		/// </summary>
		/// <returns></returns>
		public int GetStaticTagStartLocalPosition()
		{
			return GetLocalPosition(StaticTagStartPosition);
		}

		/// <summary>
		/// Calculates the local position of the NonContainerTagStartPosition by accounting
		/// for the containing element's offset
		/// </summary>
		/// <returns></returns>
		public int GetNonContainerTagStartLocalPosition()
		{
			return GetLocalPosition(CurrentNonContainerTagInfo.StartPosition);
		}

		/// <summary>
		/// Calculates the local position of the CurrentPosition (current item) by accounting
		/// for the containing element's offset
		/// </summary>
		/// <returns></returns>
		public int GetLocalPosition()
		{
			return GetLocalPosition(CurrentPosition);
		}


		/// <summary>
		/// Calculates the local position of the absolute reference point.
		/// </summary>
		/// <returns></returns>
		public int GetLocalPosition(int absolutePosition)
		{
			if (0 == absolutePosition)
			{
				return 0;
			}
			if (!IsInitialized())
			{
				return absolutePosition;
			}
			int pos = absolutePosition;
			if (0 == CurrentOffset.Position
				&& null != CurrentOffset.ParentOffset
				&& 0 == CurrentOffset.ParentOffset.Position)
			{
				return pos;
			}

			//recursively look up to find local position
			int breakoutMax = 1000;
			int loopCount = 0;
			int diff = 0;
			OffsetItem oi = CurrentOffset;
			while (oi != oi.ParentOffset && ++loopCount < breakoutMax)
			{
				diff += oi.Position;
				oi = oi.ParentOffset;
			}
			//catch root element
			diff += oi.Position;

			return pos - diff;
		}

		/// <summary>
		/// Tests to see if this TracePosition has been fully initialized with a RootOffset.
		/// </summary>
		/// <returns></returns>
		public bool IsInitialized()
		{
			return (RootOffset != null);
		}


		/// <summary>
		/// Flushes a pending Literal (static) control's offsets.
		/// </summary>
		internal void FlushLiteralControlOffset()
		{
			if (!InStaticTag) return;

			//write remainder of static
			OffsetItem tmp = this.AddChildOffset(GetStaticTagStartLocalPosition(), ControlFactory.RESERVEDKEY_LITERAL);
			tmp.EndPosition = GetLocalPosition();
			this.StaticTagStartPosition = 0;
			this.InStaticTag = false;
		}
	}
}
