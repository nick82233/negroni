using System;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class MessageBundleData
	{


		public static void LoadSampleMessageBundles(GadgetMaster master)
		{

			Locale locale;
			locale = new Locale(null, master);
			locale.Lang = "ru";
			locale.MessageSrc = "http://doc.examples.googlepages.com/ru_ALL.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_RU);
			master.ModulePrefs.AddControl(locale);

			locale = new Locale(null, master);
			locale.Lang = "it";
			locale.MessageSrc = "http://doc.examples.googlepages.com/it_ALL.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_IT);
			master.ModulePrefs.AddControl(locale);

			locale = new Locale(null, master);
			locale.Lang = "es";
			locale.MessageSrc = "http://doc.examples.googlepages.com/es_ALL.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_IT);
			master.ModulePrefs.AddControl(locale);

			locale = new Locale(null, master);
			locale.Lang = "fr";
			locale.MessageSrc = "http://doc.examples.googlepages.com/fr_ALL.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_FR);
			master.ModulePrefs.AddControl(locale);

			locale = new Locale(null, master);
			locale.Lang = "";
			locale.MessageSrc = "http://doc.examples.googlepages.com/ALL_ALL.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_EN);
			master.ModulePrefs.AddControl(locale);


		}

		public const string Source_FR = @"
<messagebundle>  
  <msg name='hello_world'>
    Bonjour Monde.
  </msg>
  <msg name='color'>Couleur</msg> 
  <msg name='red'>Rouge</msg> 
  <msg name='green'>Vert</msg> 
  <msg name='blue'>Bleu</msg> 
  <msg name='gray'>Gris</msg> 
  <msg name='purple'>Pourpre</msg> 
  <msg name='black'>Noir</msg>
</messagebundle>";

		public const string Source_JA = @"
<messagebundle>  
  <msg name=""hello_world"">
    こんにちは世界
  </msg>
  <msg name=""color"">色</msg> 
  <msg name=""red"">赤い</msg> 
  <msg name=""green"">緑</msg> 
  <msg name=""blue"">青い</msg> 
  <msg name=""gray"">灰色</msg> 
  <msg name=""purple"">紫色</msg> 
  <msg name=""black"">黒</msg>
</messagebundle>";

		public const string Source_IT = @"<messagebundle>  
  <msg name='hello_world'>
    Ciao Mondo.
  </msg>
  <msg name='color'>Colore</msg> 
  <msg name='red'>Rosso</msg> 
  <msg name='green'>Verde</msg> 
  <msg name='blue'>Blu</msg> 
  <msg name='gray'>Grigio</msg> 
  <msg name='purple'>Viola</msg> 
  <msg name='black'>Nero</msg>
</messagebundle>";

		public const string Source_ES = @"<messagebundle>  
  <msg name='hello_world'>
    Hola Mundo.
  </msg>
  <msg name='color'>Color</msg> 
  <msg name='red'>Rojo</msg> 
  <msg name='green'>Verde</msg> 
  <msg name='blue'>Azul</msg> 
  <msg name='gray'>Gris</msg> 
  <msg name='purple'>Púrpura</msg> 
  <msg name='black'>Negro</msg>
</messagebundle>";

		public const string Source_AR = @"
<messagebundle>
<msg name=""hello_world"">أهلاً بالعالم</msg> 
</messagebundle>";

		public const string Source_RU = @"
<messagebundle>  
  <msg name='hello_world'>
    Здравствулте! Мир
  </msg>
  <msg name='color'>Цвет</msg> 
  <msg name='red'>Красно</msg> 
  <msg name='green'>Зеленый цвет</msg> 
  <msg name='blue'>Голубо</msg> 
  <msg name='gray'>Серо</msg> 
  <msg name='purple'>Пурпурово</msg> 
  <msg name='black'>Чернота</msg>
</messagebundle>";

		public const string Source_DE = @"
<messagebundle>
  <msg name=""hello_world"">
    Hallo Welt.
  </msg>
  <msg name=""color"">Farbe</msg> 
  <msg name=""red"">Rot</msg> 
  <msg name=""green"">Grün</msg> 
  <msg name=""blue"">Blau</msg> 
  <msg name=""gray"">Grau</msg> 
  <msg name=""purple"">Purpurrot</msg> 
  <msg name=""black"">Schwarz</msg>
</messagebundle>";

		public const string Source_EN = @"
<messagebundle>
  <msg name=""hello_world"">
    Hello world
  </msg>
  <msg name=""color"">Color</msg> 
  <msg name=""red"">red</msg> 
  <msg name=""green"">green</msg> 
  <msg name=""blue"">blue</msg> 
  <msg name=""gray"">gray</msg> 
  <msg name=""purple"">Purple</msg> 
  <msg name=""black"">Black</msg>
</messagebundle>";

		public const string Source_EN_wXML = @"
<?xml version='1.0' encoding='utf-8'?>
<messagebundle>
  <msg name=""hello_world"">
    Hello world
  </msg>
  <msg name=""color"">Color</msg> 
  <msg name=""red"">red</msg> 
  <msg name=""green"">green</msg> 
  <msg name=""blue"">blue</msg> 
  <msg name=""gray"">gray</msg> 
  <msg name=""purple"">Purple</msg> 
  <msg name=""black"">Black</msg>
</messagebundle>";

		public const string Source_EN_wComment = @"
<!-- this wonderful message is bundled
into something swell -->
<messagebundle>
  <msg name=""hello_world"">
    Hello world
  </msg>
  <msg name=""color"">Color</msg> 
  <msg name=""red"">red</msg> 
  <msg name=""green"">green</msg> 
  <msg name=""blue"">blue</msg> 
  <msg name=""gray"">gray</msg> 
  <msg name=""purple"">Purple</msg> 
  <msg name=""black"">Black</msg>
</messagebundle>";

	}
}
