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
using Negroni.OpenSocial.EL;

namespace Negroni.OpenSocial.EL.Elements
{
	public class ElementTernaryOperator : Element
	{
		public ElementTernaryOperator(TokenType tokenType)
			: base(tokenType)
		{
			
		}
		
		internal static bool Parse(Parser.ParseContext context){
			char c = context.Expression[context.TokenPosition];
			if (c.Equals('?')){
				return ParseTenaryIf(context);
			}
			else if (c.Equals(':')){
				return ParseTernaryElse(context);
			}
			return false;
		}

		private static bool ParseTernaryElse(Parser.ParseContext context)
		{
			if (context.BpOrder.Count == 0 || context.BpOrder.Pop() != TokenType.TernaryIf)
			{
				throw new ELException("Invalid token at position " + context.TokenPosition);
			}
			ValidateTokenOrder(context, TokenType.TernaryElse);
			Element element = new ElementTernaryOperator(TokenType.TernaryElse);
			while (context.Stack.Count > 0)
			{
				Element lastElement = context.Stack.Pop();
				if (lastElement.Type == TokenType.TernaryIf)
				{
					break;
				}
				else
				{
					context.Ouput.Push(lastElement);
				}

			}
			context.Ouput.Push(element);
			context.TokenPosition++;
			return true;
		}

		private static bool ParseTenaryIf(Parser.ParseContext context)
		{
			context.BpOrder.Push(TokenType.TernaryIf);
			ValidateTokenOrder(context, TokenType.TernaryIf);
			Element element = new ElementTernaryOperator(TokenType.TernaryIf);
			context.LastTokenType = TokenType.TernaryIf;
			while (context.Stack.Count > 0)
			{
				Element lastElement = context.Stack.Pop();
				if (lastElement.Type == TokenType.OpenParenthesis)
				{
					context.Stack.Push(lastElement);
					break;
				}
				else
				{
					context.Ouput.Push(lastElement);
				}
			}
			context.Stack.Push(element);
			context.Ouput.Push(element);
			context.TokenPosition++;
			return true;
		}

		public override string ToString()
		{
			switch(Type){
				case TokenType.TernaryIf: 
					return "?";
				case TokenType.TernaryElse:
					return ":";
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
			//TODO override Equals
			return base.Equals(obj);
		}

		public override void Eval(Stack<Element> output)
		{
			throw new NotImplementedException();
		}
	}
}
