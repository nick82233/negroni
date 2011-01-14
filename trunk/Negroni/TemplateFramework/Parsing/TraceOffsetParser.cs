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
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Negroni.TemplateFramework;

namespace Negroni.TemplateFramework.Parsing
{

	/// <summary>
	/// Main parsing object for Gadget XML controls.  This reads in the XML and utilizes a 
	/// <c>ControlFactory</c> instance to identify all controls in the XML in order to construct
	/// an OffsetItem representing all controls found in the XML.
	/// </summary>
	/// <remarks>
	/// This is called a "TraceOffsetParse" because there is a trace object which trails the
	/// XmlReader to identify the true position of all items within the XML.
	/// </remarks>
	internal class TraceOffsetParser : IOffsetParser
	{

		public TraceOffsetParser() { }

		public TraceOffsetParser(ControlFactory controlFactory) 
		{
			MyControlFactory = controlFactory;
		}


		private ControlFactory _myControlFactory = null;

		/// <summary>
		/// Accessor for myControlFactory.
		/// If this is not explicitly initialized it will use the Gadget ControlFactory instance
		/// </summary>
		public ControlFactory MyControlFactory
		{
			get
			{
				if (_myControlFactory == null)
				{
					///_myControlFactory = ControlFactory.Instance;
					throw new ControlFactoryNotDefinedException("No factory identified for TraceParser");
				}
				return _myControlFactory;
			}
			set
			{
				_myControlFactory = value;
			}
		}


		#region TracePosition class encapsulation


		#endregion

		public OffsetItem ParseOffsets(string markup)
		{
			return ParseOffsets(markup, ParseContext.DefaultContext);
		}

		List<string> customNamespaces = null;


		/// <summary>
		/// Hash to hold all custom tags defined in trace
		/// </summary>
		private Dictionary<string, string> _customTagTemplates = null;

		/// <summary>
		/// Registered custom tags.
		/// </summary>
		public Dictionary<string, string> CustomTagTemplates
		{
			get
			{
				if (null == _customTagTemplates)
				{
					_customTagTemplates = new Dictionary<string, string>();
				}
				return _customTagTemplates;
			}
		}

		public bool IsCustomTagDefined(string tag)
		{
			return IsCustomTagDefined(tag, ParseContext.DefaultContext);
		}

		/// <summary>
		/// Tests to see if the tag has be defined as a custom template.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public bool IsCustomTagDefined(string tag, ParseContext context)
		{
			//custom tags only in template, for now
			if (ParseContext.DefaultContext != context)
			{
				return false;
			}

			if (null == _customTagTemplates)
			{
				return false;
			}
			return _customTagTemplates.ContainsKey(tag);
		}

		/// <summary>
		/// Registers a new custom tag with the parser
		/// </summary>
		/// <param name="tag"></param>
		public void RegisterCustomTag(string tag)
		{
			if (!CustomTagTemplates.ContainsKey(tag))
			{
				CustomTagTemplates.Add(tag, tag);
			}
		}

		/// <summary>
		/// Add a new namespace to the declarations for the parse
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="uri"></param>
		public void AddNamespace(string prefix, string uri)
		{
			if (string.IsNullOrEmpty(prefix)) return;
			if (null == customNamespaces)
			{
				customNamespaces = new List<string>();
			}
			customNamespaces.Add(prefix);
		}

		private string[] GetCustomNamespaces()
		{
			if(null == customNamespaces){
				return new string[]{};
			}
			else{
				return customNamespaces.ToArray();
			}
		}



		/// <summary>
		/// Main method to parse for offsets.
		/// </summary>
		/// <remarks>
		/// This may be called recursively, but is designed to do a single loop parse (O)N
		/// </remarks>
		/// <param name="markup"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public OffsetItem ParseOffsets(string markup, ParseContext context)
		{
			if (String.IsNullOrEmpty(markup)) { return null; }

			XmlReader reader = ParserFactory.InitializeTagReader(markup, GetCustomNamespaces());
			
			return ParseOffsets(markup, context, reader);
		}

		/// <summary>
		/// Main method to parse for offsets.
		/// </summary>
		/// <remarks>
		/// This may be called recursively, but is designed to do a single loop parse (O)N
		/// </remarks>
		/// <param name="markup"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		private OffsetItem ParseOffsets(string markup, ParseContext context, XmlReader reader)
		{
			if (String.IsNullOrEmpty(markup)) { return null; }

			TracePosition trace = CreateTracePosition(context);


			while (reader.Read())
			{
				//skip declaration
				if (XmlNodeType.XmlDeclaration == reader.NodeType 
					|| XmlNodeType.Whitespace == reader.NodeType)
				{
					continue;
				}

				this.HandleContextParse(reader, markup, trace);
				continue;

			}

			return trace.RootOffset;
		}

		private TracePosition CreateTracePosition(ParseContext context)
		{
			TracePosition trace = new TracePosition();
			trace.ParseContextStack.Push(context);
//			trace.RootOffset = MyControlFactory.CreateRootOffset(context);
			return trace;
		}

		/// <summary>
		/// Finds the closing position of the current tag
		/// </summary>
		/// <param name="markup"></param>
		/// <param name="currentPosition"></param>
		/// <returns></returns>
		private int GetEndOfTagPosition(string markup, int currentPosition)
		{
			return markup.IndexOf(">", currentPosition) + 1;
		}

	

		/// <summary>
		/// Handle processing within markup contexts consisting only of defined-tag context.
		/// For processing markup which may contain custom tag templates or recognizes Literal sections,
		/// use the method <c>HandleOsContainerTagParse</c>.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="markup"></param>
		/// <param name="trace"></param>
		private void HandleContextParse(XmlReader reader, string markup, TracePosition trace)
		{
			bool isStartElement = (XmlNodeType.Element == reader.NodeType);
			bool isEndElement = (XmlNodeType.EndElement == reader.NodeType);

			if (XmlNodeType.Whitespace == reader.NodeType)
			{
				return;
			}

			TraceContainerTagInfo containerInfo = trace.GetCurrentContainerTag();
			Type t = typeof(GadgetLiteral); //default control type for current tag

			if (isStartElement || isEndElement)
			{
				//if null tag, this is in a ContentBlock ??
				//if (String.IsNullOrEmpty(containerInfo.Tag))
				//{
				//    containerInfo.Tag = "Content";
				//}
				if (isEndElement)
				{

					int prevCurrentPosition = trace.CurrentPosition;
					trace.CurrentPosition = markup.IndexOf("</" + reader.Name, trace.CurrentPosition);
					if (prevCurrentPosition == trace.CurrentPosition
						&& !trace.PriorLoopWasStartElement
						&& reader.Name == trace.PreviousEndTag)
					{
						trace.CurrentPosition = markup.IndexOf("</" + reader.Name, trace.CurrentPosition + 1);
					}

					trace.PreviousStartTag = null;
					trace.PriorLoopWasStartElement = false;
					trace.PreviousEndTag = reader.Name;

					//check for non-container tag
					if (trace.InNonContainerControl)
					{
						if (reader.Name == trace.CurrentNonContainerTagInfo.Tag
							&& reader.Depth == trace.CurrentNonContainerTagInfo.NodeDepth)
						{
							trace.CurrentPosition = GetEndOfTagPosition(markup, trace.CurrentPosition);
							trace.FlushNonContainerTagOffset();
							return;
						}
						else
						{
							return;
						}
					}


					ControlMap curMap;
					if (trace.IsInitialized())
					{
						curMap = MyControlFactory.GetControlMap(trace.CurrentOffset.OffsetKey);
					}
					else
					{
						//special case for CustomTag parsing
						//Add in a missing context and container
						if (trace.InStaticTag && trace.StaticTagStartPosition == 0)
						{
							trace.StaticTagStartPosition = GetEndOfTagPosition(markup, 0);
						}
						trace.AddChildOffset(0, MyControlFactory.GetContextGroupOffsetKey(trace.Context));
						//push first element as container tag, if empty
						if (null == containerInfo.Tag)
						{
							trace.AddCurrentNodeAsContainerTag(ControlFactory.GetTagName(markup), 0);
							containerInfo = trace.GetCurrentContainerTag();
						}
						//this is questionable
						//curMap = MyControlFactory.GetControlMap(GadgetLiteral.DefaultOffsetKey);
						curMap = MyControlFactory.GetControlMap(t);
					}

					if (reader.Name == containerInfo.Tag && reader.Depth == containerInfo.NodeDepth)
					{

						//if (AllowLiteralTags(trace.Context))
						//{
						//check for final static tag
						if (trace.InStaticTag)
						{
							trace.FlushLiteralControlOffset();
						}
						//						}
						//set closing position
						SetAscendTraceTree(markup, trace, curMap.IsContextGroupContainer);
						return;
					}
					else if (MyControlFactory.AllowCustomTags(trace.Context) && IsCustomTagDefined(reader.Name))
					{
						//handle closing custom tag so it isn't interpreted as a static
						t = typeof(CustomTag);
					}
					else
					{
						//advance to end of current close tag to handle final static script tag
						//we're in a nested tag that is a static, but the same as the container tag
						//watch to make sure this is not in error
						if (reader.Name == containerInfo.Tag)
						{
							trace.CurrentPosition = GetEndOfTagPosition(markup, trace.CurrentPosition);
						}
					}
				}
				else if (isStartElement)
				{
					int prevCurrentPosition = trace.CurrentPosition;
					trace.CurrentPosition = markup.IndexOf("<" + reader.Name, trace.CurrentPosition);
					if (prevCurrentPosition == trace.CurrentPosition
						&& trace.PriorLoopWasStartElement
						&& reader.Name == trace.PreviousStartTag)
					{
						trace.CurrentPosition = markup.IndexOf("<" + reader.Name, trace.CurrentPosition + 1);
					}

					trace.PreviousStartTag = reader.Name;
					trace.PriorLoopWasStartElement = true;
					trace.PreviousEndTag = null;

					if (trace.InNonContainerControl)
					{
						return;
					}


					string fullTag = markup.Substring(trace.CurrentPosition, GetEndOfTagPosition(markup, trace.CurrentPosition) - trace.CurrentPosition);

					t = MyControlFactory.GetControlType(fullTag, trace.Context);
					//if custom tag, register it
					if (ControlFactory.InheritsFromType(t, typeof(CustomTagTemplate)))
					{
						string custTag = ControlFactory.GetTagAttributeValue(fullTag, CustomTagTemplate.ATTRIBUTE_TAGDEF);
						RegisterCustomTag(custTag);
					}

					//Look for a custom tag if it is a literal.  Then continue or exit
					if (t == typeof(GadgetLiteral))
					{
						if (MyControlFactory.AllowCustomTags(trace.Context))
						{
							if (IsCustomTagDefined(reader.Name))
							{
								t = typeof(CustomTag);
							}
						}
						if (!MyControlFactory.AllowLiteralTags(trace.Context) && t != typeof(CustomTag))
						{
							return;
						}
					}

					//Look for a non-container control and set value, if required
					if (t != typeof(GadgetLiteral))
					{
						if (!ControlFactory.InheritsFromType(t, typeof(BaseContainerControl))
							&& !trace.InNonContainerControl && !reader.IsEmptyElement)
						{
							trace.SetCurrentNodeAsNonContainerTag(reader.Name, reader.Depth, MyControlFactory.GetOffsetKey(t, trace.Context));
							return;
						}
						else if (trace.InStaticTag) //Flush a pending static tag
						{
							trace.FlushLiteralControlOffset();
						}

						OffsetItem tempOffset = trace.AddChildOffset(trace.GetLocalPosition(), MyControlFactory.GetOffsetKey(t, trace.Context));

						trace.CurrentPosition = GetEndOfTagPosition(markup, trace.CurrentPosition);
						if (reader.IsEmptyElement)
						{
							tempOffset.EndPosition = (trace.GetLocalPosition());
							trace.PreviousStartTag = null;
							trace.PriorLoopWasStartElement = false;
						}
						else
						{
							//check to see if we're moving into a new context
							ControlMap curMap = MyControlFactory.GetControlMap(tempOffset.OffsetKey);
							if (curMap.IsContextGroupContainer)
							{
								if (trace.Context.ContainerControlType != curMap.ControlType)
								{
									trace.ParseContextStack.Push(new ParseContext(curMap.ControlType));
								}
								//push current as container tag, if empty
								trace.AddCurrentNodeAsContainerTag(reader.Name, reader.Depth);
							}
							else if (ControlFactory.InheritsFromType(curMap.ControlType, typeof(BaseContainerControl)))
							{
								trace.AddCurrentNodeAsContainerTag(reader.Name, reader.Depth);
							}
						}
					}
				}
			}
			else
			{
				trace.PreviousEndTag = null;
				trace.PreviousStartTag = null;
				trace.PriorLoopWasStartElement = false;
			}


			//Look for non-container controls
			if (trace.InNonContainerControl)
			{
				return;
			}


			//Look for static elements
			if (!MyControlFactory.AllowLiteralTags(trace.Context))
			{
				return;
			}
			else if (t == typeof(GadgetLiteral))
			{
				if (!trace.InStaticTag)
				{
					trace.StaticTagStartPosition = trace.CurrentPosition;
					trace.InStaticTag = true;
				}
				return;
			}
		}

		/// <summary>
		/// Adjusts the trace object to ascend back up the parsing tree, 
		/// since this is the closing element for the current section.
		/// </summary>
		/// <param name="markup"></param>
		/// <param name="trace"></param>
		private void SetAscendTraceTree(string markup, TracePosition trace, bool isContextClose)
		{
			OffsetItem tmpOffset;
			int endPos = GetEndOfTagPosition(markup, trace.CurrentPosition);
			tmpOffset = trace.CurrentOffset;
			//move to content block
			trace.CurrentOffset = trace.CurrentOffset.ParentOffset;
			trace.ContainerTagStack.Pop();
			if (isContextClose)
			{
				trace.ParseContextStack.Pop();
			}

			tmpOffset.EndPosition = trace.GetLocalPosition(endPos);
			//advance CurrentPosition
			trace.CurrentPosition = endPos;
		}



		/// <summary>
		/// Extracts the attribute value from the node attribute dictionary 
		/// Returns value or empty string
		/// </summary>
		/// <param name="attributes">result of call to GetAttributeValues()</param>
		/// <param name="key"></param>
		/// <returns></returns>
		private string GetAttributeValue(HybridDictionary attributes, string key){
			if(attributes.Contains(key)){
				return attributes[key] as string;
			}
			else{
				return string.Empty;
			}
		}

		/// <summary>
		/// Extracts the attribute value from the current tag
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="attribute">Attribute name to look for</param>
		/// <returns></returns>
		string GetAttributeValue(XmlReader reader, string attribute)
		{
			if (!reader.IsStartElement())
			{
				return string.Empty;
			}
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					if (reader.Name == attribute)
					{
						reader.ReadAttributeValue();
						return reader.Value;
					}
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// Returns a dictionary of all attributes or null if no attributes
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		HybridDictionary GetAttributeValues(XmlReader reader)
		{
			if (!reader.IsStartElement())
			{
				return null;
			}
			if (reader.HasAttributes)
			{
				HybridDictionary dict = new HybridDictionary();
				
				while (reader.MoveToNextAttribute())
				{
					string name = reader.Name;
					reader.ReadAttributeValue();
					string value = reader.Value;
					dict.Add(name, value);
				}
				return dict;
			}
			return null;
		}


	}
}
