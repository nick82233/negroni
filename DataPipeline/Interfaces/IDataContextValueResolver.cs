/* *********************************************************************
   Copyright 2009-2010 MySpace

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
********************************************************************* */

using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Object that will resolve IDataContextInvokable item values.
	/// Typically an IDataContextValueResolver is paired with a set of
	/// data tags so that the two will work in sync.
	/// </summary>
	public interface IDataContextValueResolver
	{
		/// <summary>
		/// Resolve the DataItem values in the passed DataContext
		/// </summary>
		/// <param name="dataContext"></param>
		void ResolveValues(DataContext dataContext);
	}
}
