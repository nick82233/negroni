using System;
using System.Collections.Generic;
using System.IO;

using Negroni.TemplateFramework;
using Negroni.DataPipeline;


namespace WebNoSql.SiteOSML
{
	[Negroni.TemplateFramework.MarkupTag("sample:GadgetList")]
	public class GadgetFileList : BaseDataControl
	{
		public const string GADGET_DIRECTORY = "Gadgets";

		private List<string> _gadgetList = null;

		public List<string> GadgetFiles
		{
			get {
				if (_gadgetList == null)
				{
					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GADGET_DIRECTORY);
					
					string[] foo = Directory.GetFiles(path, "*.xml");

					_gadgetList = new List<string>();
					int len = path.Length;
					for (int i = 0; i < foo.Length; i++)
					{
						if (!string.IsNullOrEmpty(foo[i])
							&& foo[i].Length + 1 >= len)
						{
							_gadgetList.Add(foo[i].Substring(len + 1));
						}
					}

				}
				return _gadgetList; 
			}
		}

		bool wasInvoked = false;

		public override void InvokeTarget()
		{
			if (!wasInvoked)
			{
				this.MyDataContext.RegisterDataItem(this.Key, this.GadgetFiles);
				wasInvoked = true;
			}
		}

		public override void Render(System.IO.TextWriter writer)
		{
			InvokeTarget();
			//base.Render(writer);
		}
	}
}