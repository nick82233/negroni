using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Test.TestData.Partials
{
	/// <summary>
	/// Gadget module preferences
	/// </summary>
	public class ModulePrefsFormattedLargeData
	{
		public const string ExpectedIcon = "";
		public const string ExpectedThumbnail = "__MSG_thumbnail__";
		public const string ExpectedTitle = "Inspector Gadget FIG";

		public static readonly string Source =
@"<ModulePrefs
            title=""" + ExpectedTitle + @"""
            nottitle=""__MSG_abbr__ - __MSG_title__ __MSG_current_version__""
            title_url=""http://shonzilla.com/gadgets/fig.xml""
            author=""__MSG_author__""
            author_email=""__MSG_author_email__""
            author_affiliation=""__MSG_author_affiliation__""
            author_link=""__MSG_author_link__""
            author_location=""__MSG_author_location__""
            description=""__MSG_description__ __MSG_last_version__: __MSG_current_version__, __MSG_date__: __MSG_current_version_date__""
            thumbnail=""__MSG_thumbnail__""
            screenshot=""__MSG_screenshot__"">
        <Locale messages=""http://shonzilla.com/gadgets/ALL_ALL.xml""/>
        <Locale lang=""de"" messages=""http://shonzilla.com/gadgets/de_ALL.xml""/>
        <Locale lang=""es"" messages=""http://shonzilla.com/gadgets/es_ALL.xml""/>
        <Locale lang=""sr"" messages=""http://shonzilla.com/gadgets/sr_ALL.xml""/>
        <!--
        Standard gadget features according to Apache Shindig OpenSocial container 
        -->
        <Optional feature=""analytics"" />

        <Optional feature=""content-rewrite"">
            <Param name=""expires"">86400</Param>
            <Param name=""include-url"">.jpg</Param>
            <Param name=""exclude-url"">.png</Param>
        </Optional>
        <Optional feature=""core"" />
        <Optional feature=""core.io"" />

        <Optional feature=""dynamic-height"" />
        <Optional feature=""dynamic-height.util"" />
        <Optional feature=""flash"" />
        <Optional feature=""i18n"" />
        <Optional feature=""minimessage"" />
        <Optional feature=""oauthpopup"" />
        <Optional feature=""opensocial-0.6"" />
        <Optional feature=""opensocial-0.7"" />
        <Optional feature=""opensocial-0.8"" />

        <Optional feature=""opensocial-0.9"" />
        <Optional feature=""opensocial-base"" />
        <Optional feature=""opensocial-data"" />
        <Optional feature=""opensocial-data-context"" />
        <Optional feature=""opensocial-jsonrpc"" />
        <Optional feature=""opensocial-reference"" />
        <Optional feature=""opensocial-templates"" />
        <Optional feature=""osapi"" />
        <Optional feature=""pubsub"" />

        <Optional feature=""rpc"" />
        <Optional feature=""setprefs"" />
        <Optional feature=""settitle"" />
        <Optional feature=""skins"" />
        <Optional feature=""swfobject"" />
        <Optional feature=""tabs"" />
        <Optional feature=""views"" />
        <Optional feature=""xmlutil"" />
        
        <!-- 
        Container-specific gadget features
        -->

        <Optional feature=""ads"" />
        <Optional feature=""drag"" />
        <Optional feature=""finance"" />
        <Optional feature=""google.blog"" />
        <Optional feature=""google.sharedstate"" />
        <Optional feature=""grid"" />
        <Optional feature=""multisize"" />
        <Optional feature=""sharedmap"" />
        <Optional feature=""wave"" />

        <Optional feature=""xing-ext"" />
        <!--
        TODO:
        check http://www.socialtext.net/open/index.cgi?socialtext_specific_container_features
        -->
        
    </ModulePrefs>
    ";

	}
}
