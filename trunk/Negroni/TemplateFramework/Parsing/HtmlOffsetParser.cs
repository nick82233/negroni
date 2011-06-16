<<<<<<< .mine
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Negroni.TemplateFramework.Parsing
{
	internal class HtmlOffsetParser : IOffsetParser
	{


		public HtmlOffsetParser() { }

		public HtmlOffsetParser(ControlFactory controlFactory) 
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
					throw new ControlFactoryNotDefinedException("No factory identified for HtmlParser");
				}
				return _myControlFactory;
			}
			set
			{
				_myControlFactory = value;
			}
		}



		public OffsetItem ParseOffsets(string markup)
		{
			throw new NotImplementedException();
		}

		public OffsetItem ParseOffsets(string markup, ParseContext context)
		{
			throw new NotImplementedException();
		}

		public void AddNamespace(string prefix, string uri)
		{
			throw new NotImplementedException();
		}




	}
}
=======
﻿/* *********************************************************************
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
	internal class HtmlOffsetParser : IOffsetParser
	{

		public HtmlOffsetParser() { }

		public HtmlOffsetParser(ControlFactory controlFactory) 
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


		public OffsetItem ParseOffsets(string markup)
		{
			throw new NotImplementedException();
		}

		public OffsetItem ParseOffsets(string markup, ParseContext context)
		{
			throw new NotImplementedException();
		}

		public void AddNamespace(string prefix, string uri)
		{
			throw new NotImplementedException();
		}
	}
}
>>>>>>> .r104
