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
using System.IO;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline;
using Negroni.TemplateFramework.Parsing;


namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Base control object for all other control types.
	/// Anything that is to be parsed by the control framework must inherit from
	/// this class or one of its decendents.
	/// </summary>
	/// <remarks>
	/// This base class contains all the common tag-level parsing and pointers expected
	/// in all controls
	/// </remarks>
	public abstract class BaseGadgetControl: IDisposable
	{

		/// <summary>
		/// Tests to see if the open/close brackets match up
		/// </summary>
		/// <param name="markup"></param>
		/// <returns></returns>
		public static bool IsTagBalancedElement(string markup)
		{
			return ControlFactory.IsTagBalancedElement(markup);
		}

		/// Tests markup to determine if it is a single element
		/// fully encapsulated by markup, or if there are multiple root-level
		/// markup elements.
		/// </summary>
		/// <param name="markup"></param>
		/// <returns>True if encapsulated or a literal text string, False if multiple root tags</returns>
		public static bool IsEncapsulatedElement(string markup)
		{
			return ControlFactory.IsEncapsulatedElement(markup);
		}

		private RootElementMaster _myRootMaster = null;

		/// <summary>
		/// Tests to see if the underlying GadgetMaster is set or not
		/// </summary>
		/// <returns></returns>
		protected bool IsRootMasterSet()
		{
			return (null != _myRootMaster);
		}

		/// <summary>
		/// Reference to GadgetMaster for this control
		/// </summary>
		virtual public RootElementMaster MyRootMaster
		{
			get
			{
				if (_myRootMaster == null)
				{
					_myRootMaster = new RootElementMaster();
				}

				return _myRootMaster;
			}
			set
			{
				_myRootMaster = value;
			}
		}


		/// <summary>
		/// Reference to DataContext for this control
		/// </summary>
		public DataContext MyDataContext
		{
			get
			{
				if (_scopeData != null)
				{
					return _scopeData;
				}
				else
				{
					return MyRootMaster.MasterDataContext;
				}
			}
            set
            {
                _scopeData = value;
            }
		}


		private DataContext _scopeData = null;

		/// <summary>
		/// Sets a data context privately scoped to this control.
		/// This is used for repeaters, content blocks, and any other items
		/// which may deal with a subset of global data
		/// </summary>
		/// <param name="scopeData"></param>
		public void SetScopedData(DataContext scopeData)
		{
			_scopeData = scopeData;
		}

		/// <summary>
		/// Clears privately scoped data
		/// </summary>
		internal void ClearScopedData()
		{
			_scopeData = null;
		}



		private OffsetItem _myOffset = null;

		/// <summary>
		/// Holder of the offsets of this item relative to the containing item markup,
		/// plus all offsets of the internal markup
		/// </summary>
		public OffsetItem MyOffset
		{
			get
			{
				//if (null == _myOffset)
				//{
				//    _myOffset = new OffsetItem();
				//}
				return _myOffset;
			}
			set
			{
				_myOffset = value;
			}
		}

		public bool OffsetsAreParsed()
		{
			bool basicIsParsed = (null != MyOffset && null != MyOffset.ChildOffsets);
			if(basicIsParsed){
				//test advanced parsing needs
				//if empty control, don't check count
				if(string.IsNullOrEmpty(RawTag)){
					return true;
				}
				int startPos = RawTag.IndexOf("<");
				if(-1 == startPos){
					return true;
				}
				int endPos = RawTag.IndexOf(">", startPos);
				int emptyEnd = RawTag.IndexOf("/>", startPos);
				if (emptyEnd < endPos)
				{
					return true;
				}
				else
				{
					return (MyOffset.ChildOffsets.Count > 0);
				}
			}
			return  false;
		}

		/// <summary>
		/// Assigns a default OffsetItem to MyOffset if one is not set.
		/// Does not overwrite an existing offset.
		/// </summary>
		protected virtual void ConfirmDefaultOffset()
		{
			if (null == MyOffset)
			{
				MyOffset = new OffsetItem(0, ControlFactory.RESERVEDKEY_LITERAL);
			}
		}

		/// <summary>
		/// Loads a new root level offset into the <c>MyOffset</c>property
		/// </summary>
		/// <param name="offset"></param>
		protected virtual void LoadOffset(OffsetItem offset)
		{
			if (MyOffset != offset)
			{
				MyOffset = offset;
			}
		}

		protected virtual void LoadNewChildOffsets(OffsetList offsets)
		{
			ConfirmDefaultOffset();

			MyOffset.ChildOffsets.Clear();
			foreach (OffsetItem item in offsets)
			{
				MyOffset.ChildOffsets.Add(item);
			}
		}

		/// <summary>
		/// Cleans off the variable definition brackets so that the raw key
		/// is left over.  Use this to scrub and normalize keys
		/// </summary>
		/// <param name="variableKey"></param>
		/// <returns></returns>
		protected string CleanVariableKey(string variableKey)
		{
			if (String.IsNullOrEmpty(variableKey))
			{
				return String.Empty;
			}

			if (!variableKey.StartsWith(DataContext.VARIABLE_START))
			{
				return variableKey;
			}
			else
			{
				int endPos = variableKey.IndexOf(DataContext.VARIABLE_END);
				if (-1 == endPos)
				{
					endPos = variableKey.Length - 1;
				}
				int len = endPos - DataContext.VARIABLE_START.Length;
				return variableKey.Substring(DataContext.VARIABLE_START.Length, len);
			}
		}



		/// <summary>
		/// Resolves any OSML variables in the hostingControl
		/// </summary>
		/// <remarks>
		/// If dataContext is null the source is returned without any attempt at variable resolution.
		/// </remarks>
		/// <param name="source">Source markup containing variables</param>
		/// <param name="dataContext">Context to utilize when resolving variables</param>
		/// <returns></returns>
		public static string ResolveDataContextVariables(string source, DataContext dataContext)
		{
			if (null == dataContext) return source;
			//delegate to dataContext
			return dataContext.ResolveVariables(source);
		}




		private string _id = null;

		/// <summary>
		/// Unique identifier for control.
		/// Carries through as base for any constituent controls which
		/// must have an ID'ed control.
		/// </summary>
		public string ID {
			get
			{
				if (_id == null)
				{
					_id = this.GetType().Name + "_01";
				}
				return _id;
			}
			set{
				if (!String.IsNullOrEmpty(value))
				{
					if (value.IndexOf('"') > -1)
					{
						throw new ArgumentException("Quotation marks are not allowed in the ID field");
					}
				}
				_id = value;
			} 
		}


		private string _markupTag = null;
		/// <summary>
		/// Hook to the attribute-defined tag for current class.
		/// Empty string if not defined.
		/// </summary>
		public string MarkupTag
		{
			get
			{
				if (null == _markupTag)
				{
					Type t = GetType();
					object[] attrs = t.GetCustomAttributes(typeof(MarkupTagAttribute), false);
					if (attrs.Length > 0)
					{
						_markupTag = ((MarkupTagAttribute)attrs[0]).MarkupTag;
					}
					else
					{
						_markupTag = String.Empty;
					}
				}
				return _markupTag;
			}
			// Set is only used for custom controls which may wish to override this value
			internal set
			{
				_markupTag = value;
				_isEmptyTag = null;
			}
		}


		private string _offsetKey = null;
		/// <summary>
		/// Hook to the attribute-defined tag for current class.
		/// Empty string if not defined.
		/// </summary>
		public string OffsetKey
		{
			get
			{
				if (null == _offsetKey)
				{
					Type t = GetType();
					object[] attrs = t.GetCustomAttributes(typeof(OffsetKeyAttribute), false);
					if (attrs.Length > 0)
					{
						_offsetKey = ((OffsetKeyAttribute)attrs[0]).Key;
					}
					else
					{
						_offsetKey = this.MarkupTag.Replace(":", "_");
					}
				}
				return _offsetKey;
			}
			// Set is only used for custom controls which may wish to override this value
			internal set
			{
				_offsetKey = value;
			}
		}


		private bool? _isEmptyTag = null;

		/// <summary>
		/// Indicates if this tag's contents represents an empty tag.
		/// </summary>
		public bool IsEmptyTag
		{
			get
			{
				if (!_isEmptyTag.HasValue)
				{
					if (string.IsNullOrEmpty(RawTag))
					{
						_isEmptyTag = true;
					}
					else
					{
						int startTagPos = RawTag.IndexOf("<");
						int endTagPos = RawTag.LastIndexOf("<");
						int closeTagPos = RawTag.LastIndexOf("/>");

						if (startTagPos > -1
							&& endTagPos == startTagPos
							&& closeTagPos > endTagPos)
						{
							_isEmptyTag = true;
						}
						else
						{
							_isEmptyTag = false;
						}
					}
				}
				return _isEmptyTag.Value;
			}
		}

		/// <summary>
		/// Template containing this control.
		/// TODO: Evaluate if this is still needed
		/// </summary>
		public BaseContainerControl MyTemplate { get; set; }

		/// <summary>
		/// Direct parent of this control.
		/// </summary>
		public BaseContainerControl Parent { get; set; }

		/// <summary>
		/// Calculate the end position of the current tag's markup.
		/// This is used when the end position may not be specified.
		/// In that case, the next item's start position is used.
		/// </summary>
		/// <param name="offsets"></param>
		/// <param name="currentItem"></param>
		/// <returns></returns>
		protected int GetMarkupEndPosition(OffsetList offsets, int currentItem)
		{
			OffsetItem item = offsets[currentItem]; //assuming since this is internal we're not beyond list
			if (item.EndPosition > 0)
			{
				return item.EndPosition;
			}
			else
			{
				if (currentItem + 1 >= offsets.Count)
				{
					return 0;
				}
				else
				{
					return offsets[currentItem + 1].Position;
				}
			}
		}


		#region Attribute Management and storage

		/// <summary>
		/// Case-insensitive attributes
		/// </summary>
		private System.Collections.Specialized.HybridDictionary _attributes = null;

		/// <summary>
		/// Case-sensitive attributes
		/// </summary>
		private System.Collections.Specialized.HybridDictionary _attributesCaseSensitive = null;


		/// <summary>
		/// Load the attributes from the current RawTag into the control
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="control"></param>
		protected void ParseControlAttributes()
		{
			if (string.IsNullOrEmpty(RawTag)) return;

			//if not a tag, exit
			if (!RawTag.StartsWith("<")) return;
			//if a close tag, exit
			if (RawTag.StartsWith("</")) return;

			int adjustment = 0;
			//test for xml declaration
			if (RawTag.StartsWith("<?xml", StringComparison.InvariantCultureIgnoreCase))
			{
				adjustment = RawTag.IndexOf("?>", 4);
				if (adjustment == -1)
				{
					return; //mal-formed
				}
				else
				{
					adjustment = RawTag.IndexOf("<", adjustment + 2);
					if (adjustment == -1)
					{
						return;
					}
				}
			}

			int endPos = RawTag.IndexOf(">", adjustment);
			if(-1 == endPos) return;

			int curPos = 0, eqPos = 0;
			string name, value;
			string tagPart = RawTag.Substring(adjustment, (endPos + 1) - adjustment);

			//exit if no attributes
			eqPos = tagPart.IndexOf("=");
			if (-1 == endPos) return;

			//normalize spaces in attributes
			string[] eqVariants = 
			{
				" =",
				"= ",
				"\t=",
				"=\t",
				"\r\n=",
				"=\r\n",
				"\n=",
				"=\n"
			};

			for (int i = 0; i < eqVariants.Length; i++)
			{
				while (tagPart.Contains(eqVariants[i]))
				{
					tagPart = tagPart.Replace(eqVariants[i], "=");
				}
				
			}


			eqPos = tagPart.IndexOf("=");
			while (eqPos > -1)
			{
				//get preceeding split character, usually a space, to find attribute name
				int prevCurPos = curPos;
				curPos = tagPart.IndexOf(" ", curPos, eqPos - curPos);
				if (curPos == -1)
				{
					//look to see if this is a tab
					curPos = tagPart.IndexOf("\t", prevCurPos, eqPos - prevCurPos);
					if (curPos == -1)
					{
						//look to see if this is carriage return or tab
						curPos = tagPart.IndexOf("\r\n", prevCurPos, eqPos - prevCurPos);
						if (curPos > -1)
						{
							curPos++; //account for two characters
						}
						else if (curPos == -1)
						{
							//look to see if this is carriage return or tab
							curPos = tagPart.IndexOf("\n", prevCurPos, eqPos - prevCurPos);
						}
					}

					//final exit - malformed tag
					if (curPos == -1)
					{
						return;
					}
				}
				name = tagPart.Substring(curPos + 1, (eqPos - curPos) - 1).Trim();
				string quoteChar = "\""; //default to double quote
				//find next quote - single or double
				//int n = 1;
				string tmpQuote = tagPart.Substring(eqPos + 1, 1);
				if (tmpQuote == "\"")
				{
					quoteChar = "\"";
				}
				else if (tmpQuote == "'")
				{
					quoteChar = "'";
				}

				curPos = tagPart.IndexOf(quoteChar, eqPos + 2); //get quote char
				if (-1 == curPos)
				{
					if (tagPart.EndsWith("/>"))
					{
						curPos = tagPart.Length - 2; //not getting terminating />
					}
					else
					{
						curPos = tagPart.Length - 1; //not getting terminating >
					}
				}
				if (curPos - eqPos > 2)
				{
					value = tagPart.Substring(eqPos + 2, (curPos - eqPos) - 2);
				}
				else
				{
					value = string.Empty;
				}
				SetAttribute(name, value);

				eqPos = tagPart.IndexOf("=", curPos);
			}

		}



		/// <summary>
		/// Number of attributes stored for this control.
		/// </summary>
		public int AttributeCount
		{
			get
			{
				if (null == _attributes) return 0;
				return _attributes.Count;
			}
		}

		/// <summary>
		/// Gets the named attribute. Returns an empty string if not found.
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		/// <returns></returns>
		public string GetAttribute(string name)
		{
			return GetAttribute(name, false);
		}

		/// <summary>
		/// Generates a string array of attribute keys defined in this control.
		/// This is used for attribute enumeration.
		/// </summary>
		/// <returns></returns>
		public string[] GetAttributeKeys()
		{
			if (0 == AttributeCount) return new string[] { };
			List<string> keys = new List<string>();
			foreach (var key in _attributesCaseSensitive.Keys)
			{
				keys.Add(key as string);
			}
			return keys.ToArray();
		}

		/// <summary>
		/// Gets the named attribute. Returns an empty string if not found.
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		/// <returns></returns>
		public string GetAttribute(string name, bool caseSensitive)
		{
			if (0 == AttributeCount) return string.Empty;
			if (!caseSensitive)
			{
				name = name.ToLowerInvariant();
				if (_attributes.Contains(name))
				{
					return _attributes[name] as string;
				}
				else return string.Empty;
			}
			else
			{
				if (_attributesCaseSensitive.Contains(name))
				{
					return _attributesCaseSensitive[name] as string;
				}
				else return string.Empty;
			}
		}

		/// <summary>
		/// Tests to see if an attribute is present.
		/// This should be used as <c>GetAttribute</c> will return 
		/// an empty string for a missing or empty attribute.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool HasAttribute(string name)
		{
			if (0 == AttributeCount) return false;
            name = name.ToLowerInvariant();
			return (_attributes.Contains(name));
		}

		/// <summary>
		/// Tests to see if an attribute is present.
		/// This should be used as <c>GetAttribute</c> will return 
		/// an empty string for a missing or empty attribute.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool HasAttribute(string name, bool caseSensitive)
		{
			if (!caseSensitive)
			{
				return HasAttribute(name);
			}
			else
			{
				if (0 == AttributeCount) return false;
				return (_attributesCaseSensitive.Contains(name));
			}
		}

		/// <summary>
		/// Raw access to the Attribute store.  Do not use this except for enumerating
		/// all attributes.  This has all keys normalized to lower-case
		/// </summary>
		protected System.Collections.Specialized.HybridDictionary AttributesCollection
		{
			get
			{
				return _attributes;
			}
		}

		/// <summary>
		/// Raw access to the Attribute store.  Do not use this except for enumerating
		/// all attributes.  This has all keys left in a case-sensitive form
		/// </summary>
		protected System.Collections.Specialized.HybridDictionary AttributesCollectionCased
		{
			get
			{
				return _attributesCaseSensitive;
			}
		}


		/// <summary>
		/// Sets a new attribute value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetAttribute(string name, string value)
		{
			if (string.IsNullOrEmpty(name)) return;

			if (name.IndexOf(' ') > -1)
			{
				name = name.Trim();
			}

			if (null == _attributes)
			{
				_attributes = new System.Collections.Specialized.HybridDictionary();
				_attributesCaseSensitive = new System.Collections.Specialized.HybridDictionary();
			}

			string invariantName = name.ToLowerInvariant();

			if (_attributes.Contains(invariantName))
			{
				_attributes[invariantName] = value;
			}
			else
			{
				_attributes.Add(invariantName, value);
			}

			if (_attributesCaseSensitive.Contains(name))
			{
				_attributesCaseSensitive[name] = value;
			}
			else
			{
				_attributesCaseSensitive.Add(name, value);
			}

			
		}

		/// <summary>
		/// Tests to see if this control has any attributes defined
		/// </summary>
		/// <returns></returns>
		public bool HasAttributes()
		{
			return (AttributeCount > 0);
		}


		#endregion

		virtual protected void Clear() { }

		private string _rawTag = null;
		/// <summary>
		/// Raw markup representation of this tag
		/// </summary>
		public string RawTag
		{
			get
			{
				return _rawTag;
			}
			protected set
			{
				_rawTag = value;
				if (null != _innerMarkup)
				{
					_innerMarkup = null;
				}
			}
		}


		private string _innerMarkup = null;

		/// <summary>
		/// Inner raw markup representation of this tag.
		/// This is similar to innerHTML, but is in an unprocessed state.
		/// </summary>
		virtual public string InnerMarkup
		{
			get
			{
				if (_innerMarkup == null)
				{
					_innerMarkup = GetInnerMarkup();
				}
				return _innerMarkup;
			}
		}

		/// <summary>
		/// Extracts the innerMarkup for this RawTag
		/// </summary>
		/// <returns></returns>
		protected string GetInnerMarkup()
		{
			if (string.IsNullOrEmpty(RawTag))
			{
				return null;
			}
			string tag = MarkupTag;
			//if (string.IsNullOrEmpty(tag))
			//{
			//    return RawTag;
			//}
			int pos = RawTag.IndexOf(">");
			if (pos == -1)
			{
				return RawTag;
			}
			//now find close tag
			int endTagPos = RawTag.LastIndexOf("<");
			if (endTagPos == -1) //is malformed if this happens
			{
				return RawTag;
			}
			//int finalTagTextPos = RawTag.LastIndexOf(tag, StringComparison.InvariantCultureIgnoreCase);
			bool isEmptyTag = (endTagPos < pos);

			if (!isEmptyTag)
			{
				int startPos = pos + 1;
				return RawTag.Substring(startPos, endTagPos - startPos);
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Indicates if this the markup has been parsed or not
		/// </summary>
		public bool IsParsed { get; set; }

		private ParseContext _myParseContext = ParseContext.DefaultContext;
		/// <summary>
		/// Parsing context where this control is registered.
		/// This value should always correspond to the <c>ParseContext</c> representation
		/// of the value registered in the <c>ContextGroupAttribute</c> for the control
		/// </summary>
		protected ParseContext MyParseContext{
			get{
				return _myParseContext;
			}
			set{
				_myParseContext = value;
			}
		}

		/// <summary>
		/// Override in implementing classes.
		/// TODO: re-evaluate this design to push more logic into BaseControl
		/// </summary>
		virtual public void Parse()
		{}

		public virtual void LoadTag(string markup)
		{
			RawTag = markup;
			ParseControlAttributes();
			SetStandardAttributeProperties();
			if (this is BaseContainerControl)
			{
				if (ControlFactory.InheritsFromType(this.GetType(), typeof(RootElementMaster))
					&& markup.StartsWith("<?xml", StringComparison.InvariantCultureIgnoreCase))
				{
					((RootElementMaster)this).HasXmlDeclaration = true;
				}
				Parse();
			}
			IsParsed = true;
		}

		/// <summary>
		/// Setup of standard properties from common attributes
		/// </summary>
		private void SetStandardAttributeProperties()
		{
			string val = GetAttribute("id");
			if (!string.IsNullOrEmpty(val))
			{
				this.ID = val;
			}
			val = GetAttribute("view");
			if (!string.IsNullOrEmpty(val) && this.MyRootMaster != null)
			{
				ViewMask = MyRootMaster.RegisterViews(val);
			}
		}

		/// <summary>
		/// Views on which this control will be valid, stored as a bitmask.
		/// </summary>
		public int ViewMask { get; set; }

		abstract public void Render(TextWriter writer);


		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (MyTemplate != null && !MyTemplate.InDisposer)
				{
					MyTemplate.Dispose();
					MyTemplate = null;
				}
				if (MyOffset != null)
				{
					MyOffset = null;
				}
				if (IsRootMasterSet() && !MyRootMaster.InDisposer)
				{
					MyRootMaster.Dispose();
					MyRootMaster = null;
				}
				if (Parent != null) Parent = null;
			}

		}

		#endregion
	}
}
