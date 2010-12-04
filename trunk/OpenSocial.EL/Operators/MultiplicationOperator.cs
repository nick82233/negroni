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
	/// Singleton implementation of the multiplication (*) operator
	/// </summary>
	public class MultiplicationOperator : BinaryOperator
	{

		private static readonly MultiplicationOperator instance = new MultiplicationOperator();


		private MultiplicationOperator()
			: base(OperatorType.Multiplication, AssignmentEnum.Left, 15)
		{
		}

		public static MultiplicationOperator Instance
		{
			get { return instance; }
		}

		public override IElementValue Apply(IElementValue left, IElementValue right)
		{
		
			double leftValue = Convert.ToDouble(Operator.ToNumber(left).Value);
			double rightValue = Convert.ToDouble(Operator.ToNumber(right).Value);
			
			if ((double.IsInfinity(leftValue) && rightValue == 0d) || 
				(double.IsInfinity(rightValue) && leftValue == 0d))
			{
				throw new ELOperationException("Cannot multipy (+/-)Infinity by 0");
			}
			
			double result = leftValue * rightValue;
			
			return Operator.ToNumeric(result);
			
			

		}

	}
}
