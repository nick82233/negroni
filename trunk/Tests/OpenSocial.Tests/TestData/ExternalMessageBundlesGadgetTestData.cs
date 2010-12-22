using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;


namespace Negroni.OpenSocial.Test.TestData
{
	public class ExternalMessageBundlesGadgetTestData : TestableMarkupDef
	{


		public int ExpectedLocaleCount = 8;

		public string ExpectedEsCanvas { get; set; }

		public ExternalMessageBundlesGadgetTestData()
		{

			this.Source =
@"<?xml version='1.0' encoding='UTF-8' ?>
<Module>
  <ModulePrefs title='External Messages'>
    <Locale messages='http://doc.examples.googlepages.com/ALL_ALL.xml'/>
    <Locale lang='ru' messages='http://doc.examples.googlepages.com/ru_ALL.xml'/>
    <Locale lang='fr' messages='http://doc.examples.googlepages.com/fr_ALL.xml'/>
    <Locale lang='ja' messages='http://doc.examples.googlepages.com/ja_ALL.xml'/>
    <Locale lang='es' messages='http://doc.examples.googlepages.com/es_ALL.xml'/>
    <Locale lang='it' messages='http://doc.examples.googlepages.com/it_ALL.xml'/>
    <Locale lang='iw' messages='http://doc.examples.googlepages.com/iw_ALL.xml' language_direction='rtl'/>
    <Locale lang='ar' messages='http://doc.examples.googlepages.com/ar_ALL.xml' language_direction='rtl'/>
  </ModulePrefs>
  <Content type='html' view='canvas, home,profile'>
    <h2>${Msg.hello_world}</h2>
    Helloo world is above
  </Content>
</Module>
";
			this.ExpectedCanvas = @"    <h2>Hello World.</h2>
    Hello world is above";
			ExpectedEsCanvas = ExpectedCanvas.Replace("Hello World.", "Hola Mundo.");
		}
	}
}
