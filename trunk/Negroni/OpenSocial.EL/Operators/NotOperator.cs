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
	public class NotOperator: UnitaryOperator
	{
		private static readonly NotOperator instance = new NotOperator();

		private NotOperator() : base(OperatorType.Not, AssignmentEnum.Right, 10) { }
		
		public static NotOperator Instance {
			get{return instance;}
		}

		public override IElementValue Apply(IElementValue operatorElement)
		{
			switch (operatorElement.Type)
			{
				case TokenType.BooleanLiteral:
					if ((bool)operatorElement.Value == true)
					{
						return ElementLiteral.FalseInstance;
					}
					return ElementLiteral.TrueInstance;
				default:
					throw new ELOperationException("Invalid Element only Booleans are allowed");
			}
		}
	}
}
