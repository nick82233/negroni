using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	public class InternationalGadgetTestData : TestableMarkupDef
	{
		public InternationalGadgetTestData()
		{
			this.Source =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Module xmlns:os=""http://opwnsocial.org/dtd/osml"" xmlns:myspace=""http://opwnsocial.org/dtd/osml"">
  <ModulePrefs title=""Message Bundle Test"" thumbnail=""http://c1.ac-images.myspacecdn.com/images02/111/l_f39574fb5a234a689e538155e9371afc.png"">
    <Require feature=""opensocial-0.9""/>
    <Require feature=""opensocial-templates""/>
    <Require feature=""opensocial-data""/>
	<Icon>http://c4.ac-images.myspacecdn.com/images02/78/l_7da98739a935462cae6fcb8773ad2c63.png</Icon>
	
	<Locale>
		<messagebundle>
			<msg name=""apptitle"">Tic-Tac-Toe</msg>
			<msg name=""challengetaunt"">So... you think you can beat me!? Check out my stats below...</msg>
			<msg name=""embedded"">Embedded game is called ${Msg.apptitle}</msg>
		</messagebundle>
	</Locale>
	<Locale lang=""ja"">
		<messagebundle>
			<msg name=""apptitle"">-チックタックつま先</msg>
			<msg name=""challengetaunt"">だから...あなたが私に勝つことができると思います！ 
			？以下に私の統計情報を確認する...</msg>
			<msg name=""embedded"">Embedded game is called ${Msg.apptitle}</msg>
		</messagebundle>
	</Locale>
	<Locale lang=""de"">
		<messagebundle>
			<msg name=""apptitle"">Tic-Tac-Toe</msg>
			<msg name=""challengetaunt"">So ... Sie denken, Sie können mich!? 
			Check out my stats unter ...</msg>
			<msg name=""embedded"">Embedded game is called ${Msg.apptitle}</msg>
		</messagebundle>
	</Locale>
	
	
  </ModulePrefs>
  <Content type=""html"" view=""profile, home,canvas"">
    <script type=""text/os-template"">
    <h1>${Msg.apptitle}</h1>
${Msg.challengetaunt}

<div style=""border:1px solid blue;margin:10px;padding:4px;"">
${Msg.embedded}
</div>

    </script>
  </Content>
</Module>";

			//this.ExpectedCanvas = "Canvas view";

		}

		//public string ExpectedOffsets = "40-467:GadgetRoot{11-116:ContentBlock{39-92:TemplateScript{32-44:Literal}}|119-222:ContentBlock{38-90:TemplateScript{32-43:Literal}}|225-340:ContentBlock{44-102:TemplateScript{32-49:Literal}}|343-456:ContentBlock{43-100:TemplateScript{32-48:Literal}}}";

	}
}
