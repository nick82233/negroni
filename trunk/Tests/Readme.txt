
*******************************************************************************

  Negroni Unit Tests

  *******************

  Negroni is an open source XML parser/renderer and implementation of
  the OpenSocial gadget spec.
  For more information visit project negroni on google code
  http://code.google.com/p/negroni
*******************************************************************************


Negroni has several unit test suites to exercise different parts of the framework
and the packaged OpenSocial implementation.  All the unit tests are currently
configured to compile against MbUnit v2 (compiled with .Net 3.5 Framework).

A compiled version of MbUnit that can be used to run unit tests 
is included with this distribution in the following directory location:
"Tests\lib\mbunit\v3_5_Framework\MbUnit.3_5.GUI.exe"

NOTE: MbUnit 3.0 (aka Gaillo) fails to properly load the test suite and is 
      not recommened for use.

Some of the unit test frameworks are configured to run under multiple test runners 
(currently Negroni.DataPipeline.Tests and Negroni.TemplateFramework.Tests).
Target unit runner is controlled by usage of a compiler directive.
Use the following conditional compiliation symbols for the associated runner:

* MBUNIT   -> MbUnit 2.x (supplied as binary in Tests/lib/mbunit)
* XUNIT    -> XUnit
* NUNIT    -> NUnit


There is also an additional project named "UnitTestCompatibility".  
This project provides stub objects and adapters to normalize some minor
syntax differences and missing features across the test frameworks.

