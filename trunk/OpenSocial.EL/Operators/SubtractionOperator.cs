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
	/// Singleton implementation of the subtraction (-) operator
	/// </summary>
	public class SubtractionOperator : BinaryOperator
	{

		private static readonly SubtractionOperator instance = new SubtractionOperator();


		private SubtractionOperator()
			: base(OperatorType.Substraction, AssignmentEnum.Left, 20)
		{
		}

		public static SubtractionOperator Instance
		{
			get { return instance; }
		}

		public override IElementValue Apply(IElementValue left, IElementValue right)
		{
			double leftValue = Convert.ToDouble(Operator.ToNumber(left).Value);
			double rightValue = Convert.ToDouble(Operator.ToNumber(right).Value);

			if (double.IsPositiveInfinity(leftValue) && double.IsNegativeInfinity(rightValue))
			{
				return left;
			}
			else if (double.IsNegativeInfinity(leftValue) && double.IsPositiveInfinity(rightValue))
			{
				return left;
			}
			else if (double.IsNegativeInfinity(leftValue) && double.IsNegativeInfinity(rightValue)){
				throw new ELOperationException("Cannot substract negative and negative infinity values");
			}
			else if (double.IsPositiveInfinity(leftValue) && double.IsPositiveInfinity(rightValue))
			{
				throw new ELOperationException("Cannot substract positive and positive infinity values");
			}
			else if (double.IsInfinity(leftValue))
			{
				return left;
			}
			else if (double.IsPositiveInfinity(rightValue))
			{
				return new ElementLiteral(TokenType.DecimalLiteral, double.NegativeInfinity);
			}
			else if (double.IsNegativeInfinity(rightValue)){
				return new ElementLiteral(TokenType.DecimalLiteral, double.PositiveInfinity);
			}

			double result = leftValue - rightValue;

			return Operator.ToNumeric(result);
			
			
		}
	}
}

