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
	public class ElementUnitaryOperator : ElementOperator
	{
		public ElementUnitaryOperator(OperatorType operatorType): base(operatorType, TokenType.UnitaryOperator){
		}
		
		internal static bool ParseUnitary(Parser.ParseContext context){
			char c = context.Expression[context.TokenPosition];
			int offset = 0;
			string subStr;
			OperatorType operationType = OperatorType.None;
			TokenType lastTokenType = context.LastTokenType;
			switch(c){
				case '-':
					if (lastTokenType == TokenType.OpenBracket || lastTokenType == TokenType.OpenParenthesis 
						|| lastTokenType == TokenType.UnitaryOperator || lastTokenType == TokenType.Start || 
						lastTokenType == TokenType.BinaryOperator){
							offset = 1;
							operationType = OperatorType.Negative;
							break;
						}
						return false;
				case '!':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(2,context.Expression.Length - context.TokenPosition));
					if (!subStr.Equals("!=")){
						offset = 1;
						operationType = OperatorType.Not;
						break;
					}
					return false;
				case 'n':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(4, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("not ") || subStr.Equals("not(")){
						operationType = OperatorType.Not;
						offset = 3;
						break;
					}
					return false;
				default:
					return false;
			}
			
			ValidateTokenOrder(context, TokenType.UnitaryOperator);
			ElementOperator element = new ElementUnitaryOperator(operationType);
			ProcessOperator(context, element);
			context.TokenPosition += offset;
			return true;
		}

		public override string ToString()
		{
			switch (OperatorType){
				case OperatorType.Negative:
					return "-";
				case OperatorType.Not:
					return "!";
				default:
					return "";
			}
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			//TODO override equals
			return base.Equals(obj);
		}

		public override void Eval(Stack<Element> output)
		{
			throw new NotImplementedException();
		}
	}
}
