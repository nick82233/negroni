using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Negroni.TemplateFramework;

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Basic implementation of a DataPipelineResolver.  
	/// This cycles and triggers the InvokeTarget method of any applicable item in the data context.
	/// Resolution is synchronous in nature unless the InvokeTarget implementation in each control is Asynchronous.
	/// </summary>
	public class SimpleDataPipelineResolver : IDataPipelineResolver
	{
		public SimpleDataPipelineResolver() { }

		/// <summary>
		/// Flag to indicate if ResolveValues has been triggered.
		/// </summary>
		public bool ValuesResolved { get; set; }

		/// <summary>
		/// Reference to the DataContext to be used for resolving data.
		/// </summary>
		public DataContext MyDataContext { get; set; }


		/// <summary>
		/// Resolve values on the given data context
		/// </summary>
		/// <param name="dataContext"></param>
		public void ResolveValues(DataContext dataContext)
		{
			if (dataContext != null && dataContext != MyDataContext)
			{
				MyDataContext = dataContext;
			}
			
		}

		/// <summary>
		/// Resolve values on the current MyDataContext object
		/// </summary>
		public void ResolveValues()
		{
			if (MyDataContext == null)
			{
				throw new NullReferenceException("MyDataContext value must be specified prior to resolution");
			}

			if (MyDataContext.DataItemCount > 0)
			{
				foreach (var item in MyDataContext.MasterData.Values)
				{
					if (item.DataControl != null && item.DataControl is BaseDataControl)
					{
						((BaseDataControl)item.DataControl).InvokeTarget();
					}
				}
			}

			ValuesResolved = true;
		}
	}
}
