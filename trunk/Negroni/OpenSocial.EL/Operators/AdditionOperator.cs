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
	/// Singleton implementation of the plus (+) operator
	/// </summary>
	public class AdditionOperator : BinaryOperator
	{
		
		private static readonly AdditionOperator instance = new AdditionOperator();


		private AdditionOperator()
			: base(OperatorType.Addition, AssignmentEnum.Left, 20)
		{
		}

		public static AdditionOperator Instance
		{
			get { return instance; }
		}

		public override IElementValue Apply(IElementValue left, IElementValue right)
		{
			TokenType leftType = left.Type;
			TokenType rightType = right.Type;
			//We validate that are accepted types
			
			if (left.Value == null && rightType == TokenType.StringLiteral){
				return new ElementLiteral(TokenType.StringLiteral, "null" + right.ToString());
			}
			else if (right.Value == null && leftType == TokenType.StringLiteral)
			{
				return new ElementLiteral(TokenType.StringLiteral, left.ToString() + "null");
			}
			
			if (
				(leftType == TokenType.BooleanLiteral && rightType == TokenType.StringLiteral) ||
				(leftType == TokenType.StringLiteral && rightType == TokenType.BooleanLiteral)
			){
				return new ElementLiteral(TokenType.StringLiteral,left.ToString() + right.ToString());
			}
			
			//If either is a string we concatenate the values
			if ((right.Type == TokenType.Object && right.Value != null) || 
			(left.Type == TokenType.Object && left.Value != null)){
				throw new ELOperationException("Cannot add not null objects or arrays.");
			}
			
			if (leftType == TokenType.StringLiteral || rightType == TokenType.StringLiteral)
			{
				return new ElementLiteral(TokenType.StringLiteral, string.Format("{0}{1}", left.ToString(), right.ToString()));
			}
			
			IElementValue leftVal = Operator.ToNumber(left);
			IElementValue rightVal = Operator.ToNumber(right);
			
			double leftValue = Convert.ToDouble(leftVal.Value);
			double rightValue = Convert.ToDouble(rightVal.Value);
			
			if (double.IsPositiveInfinity(leftValue) && double.IsPositiveInfinity(rightValue)){
				return leftVal;
			}
			else if (double.IsNegativeInfinity(leftValue) && double.IsNegativeInfinity(rightValue)){
				return leftVal;
			}
			else if ((double.IsNegativeInfinity(leftValue) && double.IsPositiveInfinity(rightValue)) || 
				(double.IsPositiveInfinity(leftValue) && double.IsNegativeInfinity(rightValue))){
					throw new ELOperationException("Cannot add positive and negative infinity values");
			}
			else if (double.IsInfinity(leftValue)){
				return leftVal;
			}
			else if (double.IsInfinity(rightValue)){
				return rightVal;
			}
			
			
			double result = leftValue + rightValue;
			
			return ToNumeric(result);
			
		}
	}
}
