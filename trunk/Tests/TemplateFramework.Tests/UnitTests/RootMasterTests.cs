using System;
using System.Text;
using System.Collections.Generic;

using Negroni.TemplateFramework.Tests.ExampleControls;

#if XUNIT
using Xunit;
#elif NUNIT
using NUnit;
using NUnit.Framework;
using NUnitExtension.RowTest;
#else
using MbUnit.Framework;
#endif


namespace Negroni.TemplateFramework.Tests
{
	/// <summary>
	/// Summary description for RootMasterTests
	/// </summary>
	[TestFixture]
	public class RootMasterTests
	{
		public RootMasterTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		string factoryKey = "testFactory";

		string RootGadgetA = @"<RootA someprop='I like Ike'>
<mytest:SpecialContainer>
<mytest:SampleHeading>Hello World</mytest:SampleHeading>
</mytest:SpecialContainer>

</RootA>";


		string SecondRootGadget = @"<SecondRoot>
<mytest:SpecialContainer>
<mytest:SampleHeading>Hello World</mytest:SampleHeading>
</mytest:SpecialContainer>

</SecondRoot>";


		[Test]
		public void CreateFromRoot()
		{
			ControlFactory cf = ControlFactory.GetControlFactory(factoryKey);

			Assert.IsNotNull(cf);

			RootElementMaster masterA = cf.BuildControlTree(RootGadgetA) as RootElementMaster;

			Assert.IsTrue(masterA is RootMasterA);

			RootElementMaster masterB = cf.BuildControlTree(SecondRootGadget) as RootElementMaster;

			Assert.IsTrue(masterB is RootMasterSecond);

		}

	}
}
