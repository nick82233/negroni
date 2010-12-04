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
using Negroni.OpenSocial.EL.Elements;

namespace Negroni.OpenSocial.EL.Operators
{
	
	/// <summary>
	/// Singleton implementation of the equals (==) operator
	/// </summary>
	public class EqualsOperator : BinaryOperator
	{

		private static readonly EqualsOperator instance = new EqualsOperator();

		private EqualsOperator()
			: base(OperatorType.Equals, AssignmentEnum.Left, 30)
		{
		}

		public static EqualsOperator Instance
		{
			get { return instance; }
		}


		public override IElementValue Apply(IElementValue left, IElementValue right)
		{
			if (left is ElementLiteral && right is ElementLiteral){
				if ( ((ElementLiteral)left).Equals((ElementLiteral)right) ){
					return ElementLiteral.TrueInstance;
				}
			}
			else if(left is ElementObject && right is ElementObject){
				if ( ((ElementObject)left).Equals((ElementObject)right)){
					return ElementLiteral.TrueInstance;
				}
			}
			return ElementLiteral.FalseInstance;
		}

	}
}
