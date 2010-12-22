using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.DataContracts;
using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Test.TestData
{
	static class AccountTestData
	{

		public static void ResolveDataControlValues(DataContext dataContext, Person viewer, Person owner, List<Person> viewerFriends)
		{
			GenericExpressionEvalWrapper v = new GenericExpressionEvalWrapper(viewer);
			GenericExpressionEvalWrapper o = new GenericExpressionEvalWrapper(owner);

			foreach (KeyValuePair<string, DataItem> item in dataContext.MasterData)
			{
				if (item.Value.DataControl is OsViewerRequest)
				{
					item.Value.Data = v;
					item.Value.DataControl.Value = v;
				}
				if (item.Value.DataControl is OsOwnerRequest)
				{
					item.Value.Data = o;
					item.Value.DataControl.Value = o;
				}
				if (item.Value.DataControl is OsPeopleRequest)
				{

					item.Value.Data = viewerFriends;
					item.Value.DataControl.Value = new GenericExpressionEvalWrapper(viewerFriends);
				}

			}
		}


		public const string viewerName = "Steve";
		public const int viewerId = 99;

		public static Person Viewer = null;

		static AccountTestData()
		{
			Viewer = new Person
			{
				Id = viewerId.ToString(),
				DisplayName = viewerName
			};
		}



	}
}
