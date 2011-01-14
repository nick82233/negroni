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

namespace Negroni.TemplateFramework.Parsing
{
	/// <summary>
	/// Explicitly identifies the ContextGroups for the controls.
	/// If this is not specified all controls will go into the default context
	/// or ContextGroup values will be inferred.
	/// </summary>
	public interface IControlContextManifest
	{
		/// <summary>
		/// Returns an array of Type objects corresponding to the classes
		/// which have specific control context registrations.  All these types
		/// should inherit from <c>BaseContainerControl</c>.
		/// </summary>
		/// <returns></returns>
		Type[] GetContextContainerTypes();


		/// <summary>
		/// The class type (inheriting from <c>BaseContainerControl</c>) which
		/// should be used as the Root element in parse trees.  
		/// If there are multiple options or the root container is non-deterministic,
		/// this property should return null.
		/// </summary>
		Type RootContextType { get; }
	}
}
