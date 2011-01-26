
/********************************************

Basic Records

*********************************************/


SET IDENTITY_INSERT RecordType ON

INSERT INTO RecordType(RecordTypeID, Name, Description)
VALUES (1, 'Gadget', 'Main gadget XML file')

INSERT INTO RecordType(RecordTypeID, Name, Description)
VALUES (2, 'Message Bundle Aggregate', 'Aggregated message bundle containing all languages')

INSERT INTO RecordType(RecordTypeID, Name, Description)
VALUES (3, 'Template Definition', 'External template definition file')

INSERT INTO RecordType(RecordTypeID, Name, Description)
VALUES (4, 'Manifest', 'Manifest XML record for gadgets with multiple source gadget XML files')

SET IDENTITY_INSERT RecordType OFF
GO


/*********************  Sample App Records from iGoogle Directory ***********************


http://hosting.gmodules.com/ig/gadgets/file/112581010116074801021/hamster.xml


http://www.google.com/ig/modules/calendar3.xml

http://inspirationalgadget.googlecode.com/svn/trunk/wisdom.xml

http://widgets.tvguide.com/widgets/igoogle.aspx

http://www.ljmsite.com/google/gadgets/worldclocks.xml

http://gadgets.presse-papiers.fr/clock/clock.xml

************************************************/


/*********** **************/


DECLARE @appID_osVar INT,
		@appID_helloWorld INT

DECLARE @gadgetID_osVar INT,
		@gadgetID_helloWorld INT

INSERT INTO App(Name) VALUES ('Hello World Gadget')
SELECT @appID_helloWorld = SCOPE_IDENTITY()

INSERT INTO Gadget(AppID, Title, SourceURL) VALUES (@appID_helloWorld, 'Hello World Gadget', 'ComplianceTests\OpenSocial_1.0\helloViewerAndFriends.xml')
SELECT @gadgetID_helloWorld = SCOPE_IDENTITY()

INSERT INTO App(Name) VALUES ('osVar Test Gadget')
SELECT @appID_osVar = SCOPE_IDENTITY()

INSERT INTO Gadget(AppID, Title, SourceURL) VALUES (@appID_osVar, 'osVar Test Gadget', 'ComplianceTests\OpenSocial_1.0\osVarTestGadget.xml')
SELECT @gadgetID_osVar = SCOPE_IDENTITY()


INSERT INTO GadgetContent(GadgetID, RecordTypeID, Content)
VALUES(@gadgetID_helloWorld, 1, 
'<?xml version="1.0" encoding="utf-8"?>
<Module xmlns:os="http://ns.opensocial.org/2008/markup" >
  <ModulePrefs title="Friends need Hello also" description="This is the desc">
    <Require feature="opensocial-1.0"/>
    <Require feature="opensocial-templates"/>
  </ModulePrefs>
  <Content type="html" view="canvas">
    <script type="text/os-data">
     <!--
	 <os:ViewerRequest key="vwr" />
     <os:PeopleRequest key="friends" userId="@viewer" groupid="@friends" />
	 -->
	 <os:Var key="vwr">{"displayName": "John Doe"}</os:Var>
	 <os:Var key="friends">[
	 {"displayName": "Goosemanjack", "thumbnailUrl": "http://c3.ac-images.myspacecdn.com/images01/127/s_cc580d00a49ac8e2be1a13a4735072ce.jpg"}
	 ,{"displayName": "Tom", "thumbnailUrl": "http://x.myspacecdn.com/modules/common/static/img/placeholders/userplaceholder.png"}
	 ,{"displayName": "Ticker", "thumbnailUrl": "http://c1.ac-images.myspacecdn.com/images02/65/s_0f0243eda1b94c1794d78be329c3ac7c.png"}
	 ]
	 
	 </os:Var>
 
    </script>
    <script type="text/os-template">
     <h1>Hello world, ${vwr.displayName}</h1>
	 Your friends are:
 
	 <div>
	 <os:Repeat expression="${friends}">
	 <p>
	 Friend number ${Context.index} is: ${Cur.displayName}
	 <img src="${Cur.thumbnailUrl}" />
	 </p>	 
	 </os:Repeat>	 
	 </div>
 
  </script>
 </Content>
</Module>
')
















