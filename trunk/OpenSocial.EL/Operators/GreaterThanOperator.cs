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
	/// Singleton implementation of the greater than (>) operator
	/// </summary>
	public class GreaterThanOperator : BinaryOperator
	{

		private static readonly GreaterThanOperator instance = new GreaterThanOperator();

		private GreaterThanOperator()
			: base(OperatorType.GreaterThan, AssignmentEnum.Left, 25)
		{
		}

		public static GreaterThanOperator Instance
		{
			get { return instance; }
		}

		public override IElementValue Apply(IElementValue left, IElementValue right)
		{
			TokenType leftType = left.Type;
			TokenType rightType = right.Type;

			switch (leftType)
			{
				case TokenType.DecimalLiteral:
				case TokenType.IntegerLiteral:
				case TokenType.StringLiteral:
				case TokenType.BooleanLiteral:
					break;
				default:
					throw new ELOperationException("Invalid operator type Only Decimal Integer or Strings are allowed");
			}
			switch (rightType)
			{
				case TokenType.DecimalLiteral:
				case TokenType.IntegerLiteral:
				case TokenType.StringLiteral:
				case TokenType.BooleanLiteral:
					break;
				default:
					throw new ELOperationException("Invalid operator type Only Decimal Integer or Strings are allowed");
			}

			if (leftType == TokenType.StringLiteral || rightType == TokenType.StringLiteral)
			{
				string leftString = left.ToString();
				string rightString = right.ToString();

				if (string.Compare(leftString, rightString, StringComparison.Ordinal) > 0)
				{
					return ElementLiteral.TrueInstance;
				}
				else
				{
					return ElementLiteral.FalseInstance;
				}

			}

			if (leftType == TokenType.BooleanLiteral || rightType == TokenType.BooleanLiteral)
			{
				throw new ELOperationException("Invalid operator can not compare booleans");
			}

			double leftValue = Convert.ToDouble(left.Value);
			double rightValue = Convert.ToDouble(right.Value);
			if (leftValue > rightValue)
			{
				return ElementLiteral.TrueInstance;
			}
			return ElementLiteral.FalseInstance;
		}
	}
}

