using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Negroni.TemplateFramework;

namespace WebNoSql
{
	public partial class Editor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string template = "<option value='{0}'>{0}</option>";

			var keys = Negroni.TemplateFramework.Configuration.NegroniFrameworkConfig.ControlFactories.Keys;
			StringBuilder sb = new StringBuilder();
			foreach (var keyset in keys)
			{
				if (keyset == Negroni.TemplateFramework.Configuration.NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY) continue;
				sb.AppendFormat(template, keyset);
			}
			literalParsers.Text = sb.ToString();
		}
	}
}
