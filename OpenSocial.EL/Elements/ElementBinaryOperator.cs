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
	public class ElementBinaryOperator : ElementOperator
	{


		public ElementBinaryOperator(OperatorType operatorType)
			: base(operatorType, TokenType.BinaryOperator)
		{}

		private static bool IsNumeric(Parser.ParseContext context, out OperatorType operatorType, out int offset)
		{
			operatorType = OperatorType.None;
			offset = 0;
			string subStr;
			
			char c = context.Expression[context.TokenPosition];
			switch(c){
				case '+' :
					operatorType = OperatorType.Addition;
					offset = 1;
					return true;
				case '/' :
					operatorType = OperatorType.Division;
					offset = 1;
					return true;
				case '%' :
					operatorType = OperatorType.Modulo;
					offset = 1;
					return true;
				case '*' :
					operatorType = OperatorType.Multiplication;
					offset = 1;
					return true;
				case '-':
					TokenType lastTokenType = context.LastTokenType;
					if (lastTokenType == TokenType.CloseParenthesis || lastTokenType == TokenType.StringLiteral ||
						lastTokenType == TokenType.DecimalLiteral || lastTokenType == TokenType.IntegerLiteral || 
						lastTokenType == TokenType.BooleanLiteral || lastTokenType == TokenType.Variable || 
						lastTokenType == TokenType.CloseBracket)
					{
						operatorType = OperatorType.Substraction;
						offset = 1;
						return true;
					}
					return false;
				case 'm':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(4, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("mod ")){
						operatorType = OperatorType.Modulo;
						offset = 4;
						return true;
					}
					return false;
				case 'd':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(4, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("div ")){
						operatorType = OperatorType.Division;
						offset = 4;
						return true;
					}
					return false;
			}
			return false;
		}
		
		//private static bool IsObjectElement(Parser.ParseContext context, out OperatorType operatorType, out int offset){
		//   operatorType = OperatorType.None;
		//   offset = 0;
		//   string c = context.Expression.Substring(context.TokenPosition, 1);
		//   if (c.Equals(".")){
		//      operatorType = OperatorType.Dot;
		//      offset = 1;
		//      return true;
		//   }
		//   return false;
		//}

		private static bool IsEquality(Parser.ParseContext context, out OperatorType operatorType, out int offset)
		{
			operatorType = OperatorType.None;
			offset = 0;
			string c = context.Expression.Substring(context.TokenPosition, Math.Min(2, context.Expression.Length - context.TokenPosition));
			
			switch (c)
			{
				case "==":
					operatorType = OperatorType.Equals;
					offset = 2;
					return true;
				case "!=":
					operatorType = OperatorType.NotEquals;
					offset = 2;
					return true;
			}
			c = context.Expression.Substring(context.TokenPosition, Math.Min(3, context.Expression.Length - context.TokenPosition));
			switch (c){
				case "eq ":
					operatorType = OperatorType.Equals;
					offset = 3;
					return true;
				case "eq(":
					operatorType = OperatorType.Equals;
					offset = 2;
					return true;
				case "ne ":
					operatorType = OperatorType.NotEquals;
					offset = 3;
					return true;
				case "ne(":
					operatorType = OperatorType.NotEquals;
					offset = 2;
					return true;
			}
			return false;
		}
		
		private static bool IsLogical(Parser.ParseContext context, out OperatorType operatorType, out int offset){
			offset = 0;
			operatorType = OperatorType.None;
			char c = context.Expression[context.TokenPosition];
			string subStr;
			switch (c)
			{
				case '&':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(2, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("&&"))
					{
						operatorType = OperatorType.And;
						offset = 2;
						return true;
					}
					return false;
				case '|':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(2, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("||"))
					{
						operatorType = OperatorType.Or;
						offset = 2;
						return true;
					}
					return false;
				case 'a':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(4, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("and ")){
						operatorType = OperatorType.And;
						offset = 4;
						return true;
					}
					else if(subStr.Equals("and(")){
						operatorType = OperatorType.And;
						offset = 3;
						return true;
					}
					return false;
				case 'o':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(3, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("or "))
					{
						operatorType = OperatorType.Or;
						offset = 3;
						return true;
					}
					else if (subStr.Equals("or("))
					{
						operatorType = OperatorType.Or;
						offset = 2;
						return true;
					}
					return false;
			}
			return false;
		}

		private static bool IsComparator(Parser.ParseContext context, out OperatorType operatorType, out int offset)
		{
			offset = 0;
			operatorType = OperatorType.None;
			char c = context.Expression[context.TokenPosition];
			string subStr;
			switch (c)
			{
				case 'l':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(3, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("lt "))
					{
						operatorType = OperatorType.LessThan;
						offset = 3;
						return true;
					}
					else if (subStr.Equals("lt(")){
						operatorType = OperatorType.LessThan;
						offset = 2;
						return true;
					}
					else if (subStr.Equals("le "))
					{
						operatorType = OperatorType.LessThanEquals;
						offset = 3;
						return true;
					}
					else if (subStr.Equals("le(")){
						operatorType = OperatorType.LessThanEquals;
						offset = 2;
						return true;
					}
					return false;
				case 'g':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(3, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("gt "))
					{
						operatorType = OperatorType.GreaterThan;
						offset = 3;
						return true;
					}
					else if (subStr.Equals("gt("))
					{
						operatorType = OperatorType.GreaterThan;
						offset = 2;
						return true;
					}
					else if (subStr.Equals("ge "))
					{
						operatorType = OperatorType.GreaterThanEquals;
						offset = 3;
						return true;
					}
					else if (subStr.Equals("ge("))
					{
						operatorType = OperatorType.GreaterThanEquals;
						offset = 2;
						return true;
					}
					return false;
				case '>':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(2, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals(">="))
					{
						operatorType = OperatorType.GreaterThanEquals;
						offset = 2;
						return true;
					}
					else
					{
						operatorType = OperatorType.GreaterThan;
						offset = 1;
						return true;
					}
				case '<':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(2, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("<="))
					{
						operatorType = OperatorType.LessThanEquals;
						offset = 2;
						return true;
					}
					else
					{
						operatorType = OperatorType.LessThan;
						offset = 1;
						return true;
					}
			}
			return false;
		}
		
		//private static bool IsDot(Parser.ParseContext context, out OperatorType operatorType, out int offset)
		//{
		//   offset = 0;
		//   operatorType = OperatorType.None;
		//   string c = context.Expression.Substring(context.TokenPosition, 1);
		//   if (c == "."){
		//      operatorType = OperatorType.Dot;
		//      offset = 1;
		//      return true;
		//   }
		//   return false;
		//}
		
		internal static bool ParseBinary(Parser.ParseContext context)
		{
			int offset = 0;
			OperatorType operatorType = OperatorType.None;
			bool isBinary = IsNumeric(context, out operatorType, out offset);
			if (!isBinary){
				isBinary = IsEquality(context, out operatorType, out offset);
			}
			if (!isBinary)
			{
				isBinary = IsComparator(context, out operatorType, out offset);
			}
			if (!isBinary){
				isBinary = IsLogical(context, out operatorType, out offset);
			}
			if (!isBinary){
				return false;
			}
			ValidateTokenOrder(context, TokenType.BinaryOperator);
			ElementOperator element = new ElementBinaryOperator(operatorType);
			ProcessOperator(context, element);
			context.TokenPosition += offset;
			return true;
		}

		public override string ToString()
		{
			switch (OperatorType){
				case OperatorType.Addition:
					return "+";
				case OperatorType.And:
					return "&&";
				case OperatorType.Division:
					return "/";
				case OperatorType.Equals:
					return "==";
				case OperatorType.GreaterThan:
					return ">";
				case OperatorType.GreaterThanEquals:
					return ">=";
				case OperatorType.LessThan:
					return "<";
				case OperatorType.LessThanEquals:
					return "<=";
				case OperatorType.Modulo:
					return "%";
				case OperatorType.Multiplication:
					return "*";
				case OperatorType.NotEquals:
					return "!=";
				case OperatorType.Or:
					return "||";
				case OperatorType.Substraction:
					return "-";
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
			//TODO Jorge Reyes override
			return base.Equals(obj);
		}

		public override void Eval(Stack<Element> output)
		{
			throw new NotImplementedException();
			//BinaryOperator binaryOperator = (BinaryOperator)Operator;
			//Element op2 = output.Pop();
			//Element op1 = output.Pop();
			//Element result = binaryOperator.Apply(op1, op2);
			//output.Push(result);
		}
		
	}
}
