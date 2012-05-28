using System;
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
