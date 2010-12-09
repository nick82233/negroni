using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.DataContracts;

namespace Negroni.DataPipeline.Tests.TestData
{
	class MockDataHelper
	{

		internal static Person CreatePerson(int userId, string name, string picture)
		{
			Person p = new Person();
			p.Id = userId.ToString();
			p.DisplayName = name;
			p.ThumbnailUrl = picture;

			return p;
		}


	}
}
