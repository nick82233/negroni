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
using System.Text.RegularExpressions;

namespace Negroni.OpenSocial.EL.Operators
{
	public abstract class Operator
	{
		public enum AssignmentEnum
		{
			None = 0,
			Left = 1,
			Right = 2,
		}


		protected Operator(OperatorType operatorType, AssignmentEnum assigment, int precedence)
		{
			OperatorType = operatorType;
			Assigment = assigment;
			Precedence = precedence;
		}

		public OperatorType OperatorType { get; private set; }
		public AssignmentEnum Assigment { get; private set; }
		public int Precedence { get; private set; }

		//------------------------------------------------------
		// Utility Methods
		//------------------------------------------------------

		/// <summary>
		/// Tests to see if both are numbers.
		/// If so, the parsed results are placed in output parameters
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="leftNumber"></param>
		/// <param name="rightNumber"></param>
		/// <returns></returns>
		static internal bool BothAreNumbers(object left, object right, out double leftNumber, out double rightNumber)
		{
			bool leftResult = Double.TryParse(left.ToString(), out leftNumber);
			bool rightResult = Double.TryParse(right.ToString(), out rightNumber);
			if (leftResult && rightResult)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private static readonly Regex numberRegexp = new Regex(
		"^\\d*\\.?\\d+(?:e[+-]?\\d)?",
		 RegexOptions.Singleline
		| RegexOptions.CultureInvariant
		| RegexOptions.Compiled
		);
		
		static internal IElementValue ToBoolean(IElementValue element){
			if (element.Value == null)
			{
				return ElementLiteral.falseInstance;
			}
			switch (element.Type)
			{
				case TokenType.BooleanLiteral:
					return element;
				case TokenType.DecimalLiteral:
				case TokenType.IntegerLiteral:
					double doubleVal = Convert.ToDouble(element.Value);
					if (doubleVal == 0)
					{
						return ElementLiteral.falseInstance;
					}
					else
					{
						return ElementLiteral.trueInstance;
					}
				case TokenType.StringLiteral:
					if (element.Value.Equals("true"))
					{
						return ElementLiteral.trueInstance;
					}
					else if (element.Value.Equals("false"))
					{
						return ElementLiteral.falseInstance;
					}
					else
					{
						throw new ELOperationException(string.Format("Cannot convert string [] to boolean", element.Value));
					}
				default:
					throw new ELOperationException("Cannot convert objects or arrays to boolean");
			}
		}
		
		static internal IElementValue ToNumber(IElementValue element){
			if (element.Value == null)
			{
				return new ElementLiteral(TokenType.IntegerLiteral, 0);
			}
			switch (element.Type)
			{
				case TokenType.DecimalLiteral:
				case TokenType.IntegerLiteral:
					return element;
				case TokenType.BooleanLiteral:
					if (((bool)element.Value) == true)
					{
						return new ElementLiteral(TokenType.IntegerLiteral, 1);
					}
					else
					{
						return new ElementLiteral(TokenType.IntegerLiteral, 0);
					}
				case TokenType.StringLiteral:
					if ("Infinity".Equals(element.Value))
					{
						return ElementLiteral.InfinityInstance;
					}
					else if ("-Infinity".Equals(element.Value))
					{
						return ElementLiteral.NegativeInfinityInstance;
					}


					Match match = numberRegexp.Match((string)element.Value);

					if (!match.Success)
					{
						throw new ELOperationException(string.Format("Can not convert [{0}] to number", element.Value));
					}

					double doubleValue = Double.Parse(match.Value, System.Globalization.NumberStyles.Float);
					if (doubleValue == Math.Floor(doubleValue))
					{
						return new ElementLiteral(TokenType.IntegerLiteral, (int)doubleValue);
					}
					else
					{
						return new ElementLiteral(TokenType.DecimalLiteral, doubleValue);
					}
				default:
					throw new ELOperationException("Cannot convert to number objects or arrays.");
			}
		}
		
		static internal IElementValue ToNumeric(double val){
			if (Math.Floor(val) == val && !double.IsInfinity(val))
			{
				return new ElementLiteral(TokenType.IntegerLiteral, Convert.ToInt32(val));
			}
			else
			{
				return new ElementLiteral(TokenType.DecimalLiteral, val);
			}
		}
		
	}
}
