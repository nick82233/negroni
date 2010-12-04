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

namespace Negroni.OpenSocial.EL.Elements
{
	public class ElementGrouping: Element
	{

		private ElementGrouping(TokenType tokenType): base (tokenType){}


		internal static bool Parse(Parser.ParseContext context)
		{
			char c = context.Expression[context.TokenPosition];
			switch (c){
				case ')':
					return ParseCloseParenthesis(context);
				case '(':
					return ParseOpenParenthesis(context);
				case ']':
					return ParseCloseBracket(context);
				case '[':
					return ParseOpenBracket(context);
				default:
					return false;
			}
		}

		private static bool ParseOpenBracket(Parser.ParseContext context)
		{
			context.BpOrder.Push(TokenType.OpenBracket);
			ValidateTokenOrder(context, TokenType.OpenBracket);
			ElementGrouping element = new ElementGrouping(TokenType.OpenBracket);
			context.Ouput.Push(element);
			context.Stack.Push(element);
			context.TokenPosition++;
			return true;
		}

		private static bool ParseCloseBracket(Parser.ParseContext context)
		{
			if (context.BpOrder.Count == 0 || context.BpOrder.Pop() != TokenType.OpenBracket)
			{
				throw new ELException("Invalid token at " + context.TokenPosition);
			}
			ValidateTokenOrder(context, TokenType.CloseBracket);
			ElementGrouping element = new ElementGrouping(TokenType.CloseBracket);
			while (context.Stack.Count > 0){
				Element lastElement = context.Stack.Pop();
				if (lastElement.Type == TokenType.OpenBracket){
					break;
				}
				context.Ouput.Push(lastElement);
			}
			context.Ouput.Push(element);
			context.TokenPosition++;
			return true;
		}

		private static bool ParseOpenParenthesis(Parser.ParseContext context)
		{
			context.BpOrder.Push(TokenType.OpenParenthesis);
			ValidateTokenOrder(context, TokenType.OpenParenthesis);
			Element element = new ElementGrouping(TokenType.OpenParenthesis);
			context.Ouput.Push(element);
			context.Stack.Push(element);
			context.TokenPosition++;
			return true;
		}

		private static bool ParseCloseParenthesis(Parser.ParseContext context)
		{
			if (context.BpOrder.Count == 0 || context.BpOrder.Pop() != TokenType.OpenParenthesis)
			{
				throw new ELException("Invalid token at " + context.TokenPosition);
			}
			ValidateTokenOrder(context, TokenType.CloseParenthesis);
			Element element = new ElementGrouping(TokenType.CloseParenthesis);
			while (context.Stack.Count > 0)
			{
				Element lastElement = context.Stack.Pop();
				if (lastElement.Type.Equals(TokenType.OpenParenthesis))
				{
					break;
				}
				context.Ouput.Push(lastElement);
			}
			context.Ouput.Push(element);
			context.TokenPosition++;
			return true;
		}

		public override string ToString()
		{
			switch(Type){
				case TokenType.OpenBracket:
					return "[";
				case TokenType.OpenParenthesis:
					return "(";
				case TokenType.CloseBracket:
					return "]";
				case TokenType.CloseParenthesis:
					return ")";
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
			//TODO Override equals correctly
			return base.Equals(obj);
		}

		public override void Eval(Stack<Element> output)
		{
			throw new NotImplementedException();
		}
	}
}
