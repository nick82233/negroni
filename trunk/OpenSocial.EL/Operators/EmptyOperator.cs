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
using System.Collections;

namespace Negroni.OpenSocial.EL.Operators
{
	public class EmptyOperator: FunctionOperator
	{
		private static readonly EmptyOperator instance = new EmptyOperator();

		private EmptyOperator()
			: base(OperatorType.Empty, AssignmentEnum.Right, 5)
		{
		}
		
		public static EmptyOperator Instance{
			get{return instance;}
		}

		public override IElementValue Apply(IElementValue[] parms)
		{
			if (parms == null || parms.Length == 0){
				throw new ELOperationException("Invalid number of parameter for function Empty");
			}

			IElementValue operatorElement = parms[0];
			switch (operatorElement.Type)
			{
				case TokenType.DecimalLiteral:
				case TokenType.IntegerLiteral:
				case TokenType.BooleanLiteral:
					return ElementLiteral.FalseInstance;
				case TokenType.StringLiteral:
					if (string.IsNullOrEmpty(operatorElement.ToString())){
						return ElementLiteral.TrueInstance;
					}
					return ElementLiteral.FalseInstance;
				case TokenType.Object:
					if (operatorElement.Value == null){
						return ElementLiteral.TrueInstance;
					}
					if (operatorElement.Value is ICollection)
					{
						if (((ICollection)operatorElement.Value).Count == 0){
							return ElementLiteral.TrueInstance;
						}
					}
					return ElementLiteral.FalseInstance;
				
				default:
					throw new ELOperationException("Invalid Element");
			}
		}
	}
}
