using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{ 
	public class GadgetWithSimpleTemplate : DataGadgetTestData
	{
		public GadgetWithSimpleTemplate()
		{
			//owner.id
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module  xmlns:os='http://ns.opensocial.org/2008/markup' >  <ModulePrefs title='Hello World'>
  <Require feature='opensocial-0.9'/>
  <Require feature='opensocial-templates'/>
  <Require feature='opensocial-datapipelining'/>
  <Require feature='views'/>
 </ModulePrefs>
 <Content type='html' view='canvas'>
 <script type='text/os-data'>
  <os:ViewerRequest key='viewer' />
 </script>
 <script type='text/os-template' tag='my:name'>
  <os:If condition='${My.id == viewer.id}'>
   you
  </os:If>
  <os:If condition='${My.id != viewer.id}'>
   someone else
  </os:If>
 </script>
 </Content>

 <Content type='html' view='canvas'>
 <script type='text/os-template'>
  User 6221:
  <my:name id='6221'></my:name>
  User 26000001:
  <my:name id='26000001'></my:name>
 </script>
 </Content>
</Module>
";


			this.ExpectedCanvas = "User 6221: you User 26000001: someone else";
			this.ExpectedProfile = "";

		}

	}
}
