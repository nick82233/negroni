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
using Negroni.OpenSocial.EL.Operators;

namespace Negroni.OpenSocial.EL.Elements
{
	public abstract class ElementOperator: Element
	{

		protected static readonly IDictionary<OperatorType, Operator> operators = new Dictionary<OperatorType, Operator>();

		static ElementOperator()
		{
			operators.Add(OperatorType.Addition, AdditionOperator.Instance);
			operators.Add(OperatorType.Substraction, SubtractionOperator.Instance);
			
			operators.Add(OperatorType.Multiplication, MultiplicationOperator.Instance);
			operators.Add(OperatorType.Division, DivisionOperator.Instance);
			operators.Add(OperatorType.Modulo, ModuloOperator.Instance);
			
			operators.Add(OperatorType.Equals, EqualsOperator.Instance);
			operators.Add(OperatorType.NotEquals, NotEqualsOperator.Instance);

			operators.Add(OperatorType.LessThan, LessThanOperator.Instance);
			operators.Add(OperatorType.LessThanEquals, LessThanEqualsOperator.Instance);
			
			operators.Add(OperatorType.GreaterThan, GreaterThanOperator.Instance);
			operators.Add(OperatorType.GreaterThanEquals, GreaterThanEqualsOperator.Instance);
			
			operators.Add(OperatorType.Negative, NegativeOperator.Instance);
			operators.Add(OperatorType.Not, NotOperator.Instance);
			operators.Add(OperatorType.Empty, EmptyOperator.Instance);
			
			operators.Add(OperatorType.ToBoolean, ToBooleanOperator.Instance);
			operators.Add(OperatorType.ToString, ToStringOperator.Instance);
			operators.Add(OperatorType.ToNumber, ToNumberOperator.Instance);

			operators.Add(OperatorType.MathCeil, MathCeilOperator.Instance);
			operators.Add(OperatorType.MathFloor, MathFloorOperator.Instance);
			operators.Add(OperatorType.MathRound, MathRoundOperator.Instance);
			
			operators.Add(OperatorType.And, AndOperator.Instance);
			operators.Add(OperatorType.Or, OrOperator.Instance);
			
			operators.Add(OperatorType.HtmlDecode, HtmlDecodeOperator.Instance);
			operators.Add(OperatorType.HtmlEncode, HtmlEncodeOperator.Instance);
			operators.Add(OperatorType.UrlDecode, UrlDecodeOperator.Instance);
			operators.Add(OperatorType.UrlEncode, UrlEncodeOperator.Instance);
			operators.Add(OperatorType.JsStringEscape, JsStringEscapeOperator.Instance);
		}
		
		public Operator Operator{get; protected set;}
		public OperatorType OperatorType{ get; protected set; }

		public ElementOperator(OperatorType operatorType, TokenType tokenType)
			: base(tokenType)
		{
			OperatorType = operatorType;
			Operator = operators[operatorType];
		}

		internal static void ProcessOperator(Parser.ParseContext context, ElementOperator element)
		{
			Operator.AssignmentEnum assigment = element.Operator.Assigment;
			int precedence = element.Operator.Precedence;

			while (context.Stack.Count > 0)
			{
				Element lastElement = context.Stack.Pop();

				if (lastElement.Type == TokenType.BinaryOperator || lastElement.Type == TokenType.UnitaryOperator || lastElement.Type == TokenType.Function)
				{
					Operator prevOperator = ((ElementOperator)lastElement).Operator;
					if ((assigment == Operator.AssignmentEnum.Left && precedence >= prevOperator.Precedence) ||
						(assigment == Operator.AssignmentEnum.Right && precedence > prevOperator.Precedence))
					{
						context.Ouput.Push(lastElement);
					}
					else
					{
						context.Stack.Push(lastElement);
						break;
					}
				}
				else
				{
					context.Stack.Push(lastElement);
					break;
				}
			}
			context.Stack.Push(element);
		}
		
		internal static bool Parse(Parser.ParseContext context){
			if (ElementBinaryOperator.ParseBinary(context)){
					return true;
			}
			else if (ElementUnitaryOperator.ParseUnitary(context)){
				return true;
			}
			else if (ElementDot.ParseDot(context)){
				return true;
			}
			else if (ElementFunction.ParseFunction(context)){
				return true;
			}
			return false;
		}
	}
}
