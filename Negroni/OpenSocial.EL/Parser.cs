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

namespace Negroni.OpenSocial.EL
{
	static class Parser
	{

		
		internal class ParseContext{
		
		public ParseContext(string expression){
				Ouput = new Stack<Element>();
				Stack = new Stack<Element>();
				BpOrder = new Stack<TokenType>();
				LastTokenType = TokenType.Start;
				TokenPosition = 0;
				Expression = expression;
			}
		
			public string Expression{get; private set;}
			public Stack<Element> Ouput {get; private set;}
			public Stack<Element> Stack { get; private set; }
			public Stack<TokenType> BpOrder { get; private set; }
			public TokenType LastTokenType { get; set; }
			public int TokenPosition { get; set; }
		}

		private static readonly Regex WhiteSpaceRegExp = new Regex("\\s",
			RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled
		);
		
		public static IList<Element> Parse(string expression){
			//TODO we may want to chache the expressions for better performance
			ParseContext context = new ParseContext(expression);

			while (context.TokenPosition < expression.Length)
			{
				string c = context.Expression.Substring(context.TokenPosition, 1);
				if (WhiteSpaceRegExp.IsMatch(c)){
					context.TokenPosition++;
					continue;
				}
				if (ElementGrouping.Parse(context)){
					continue;
				}
				else if (ElementTernaryOperator.Parse(context))
				{
					continue;
				}
				else if (ElementOperator.Parse(context)){
					continue;
				}
				else if (ElementLiteral.Parse(context)){
					continue;
				}
				else if (ElementVariable.Parse(context)){
					continue;
				}
				else if (ElementSelector.Parse(context))
				{
					continue;
				}
				else
				{
					throw new ELException("Invalid character at position" + context.TokenPosition);
				}
			}
			
			Element.ValidateTokenOrder(context, TokenType.End);
			if (context.BpOrder.Count != 0){
				throw new ELException("Not all parentesis or brakets were closed or ? with out :");
			}
			
			while (context.Stack.Count > 0){
				context.Ouput.Push(context.Stack.Pop());
			}
			//reverse order
			List<Element> result = new List<Element>(context.Ouput);
			result.Reverse();
			return result;
		}
		
	}
}
