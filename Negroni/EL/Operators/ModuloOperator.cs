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
	/// Singleton implementation of the modulo (%) operator
	/// </summary>
	public class ModuloOperator : BinaryOperator
	{
		
		private static readonly ModuloOperator instance = new ModuloOperator();

		private ModuloOperator()
			: base(OperatorType.Modulo, AssignmentEnum.Left, 15)
		{
		}

		public static ModuloOperator Instance
		{
			get { return instance; }
		}

		public override IElementValue Apply(IElementValue left, IElementValue right)
		{
			double leftValue = Convert.ToDouble(Operator.ToNumber(left).Value);
			double rightValue = Convert.ToDouble(Operator.ToNumber(right).Value);
			
			if (rightValue == 0.0d){
				throw new ELOperationException("Modulo Right operator cannot be 0");
			}
			else if (double.IsInfinity(leftValue)){
				throw new ELOperationException("Modulo Left operator cannot be Infinity nor -Infinity");
			}
			else if(double.IsInfinity(rightValue)){
				if (Math.Floor(leftValue) == leftValue){
					return new ElementLiteral(TokenType.IntegerLiteral, Convert.ToInt32(leftValue));
				}
				else{
					return new ElementLiteral(TokenType.DecimalLiteral, leftValue);
				}
			}
			
			double returnValue = leftValue % rightValue;

			if (returnValue == Math.Floor(returnValue))
			{
				return new ElementLiteral(TokenType.IntegerLiteral, Convert.ToInt32(returnValue));
			}
			else
			{
				return new ElementLiteral(TokenType.DecimalLiteral, returnValue);
			}
		}

	}
}
