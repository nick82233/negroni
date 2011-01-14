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
using System.Text.RegularExpressions;

namespace Negroni.OpenSocial.EL.Elements
{
	public class ElementVariable : Element
	{
		public string Expression {get; private set;}
		public ElementVariable(string expression):base(TokenType.Variable){
			Expression = expression;
		}

		private static readonly Regex VariableStartRegExp = new Regex("[a-zA-Z#]",
			RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled
		);

		private static readonly Regex VariableValidCharsRegExp = new Regex("[\\w#]",
			RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled
		);
		
		internal static bool Parse(Parser.ParseContext context){
			string c = context.Expression.Substring(context.TokenPosition, 1);
			if (VariableStartRegExp.IsMatch(c)){
				ValidateTokenOrder(context, TokenType.Variable);
				StringBuilder variableStr = new StringBuilder();
				while(context.TokenPosition < context.Expression.Length){
					c = context.Expression.Substring(context.TokenPosition, 1);
					//if (c == ".")
					//{
					//   variableStr.Append(c);
					//   context.TokenPosition++;
					//   c = context.Expression.Substring(context.TokenPosition, 1);
					//   if (VariableStartRegExp.IsMatch(c))
					//   {
					//      variableStr.Append(c);
					//      context.TokenPosition++;
					//      continue;
					//   }
					//   else
					//   {
					//      throw new ELException("Invalid token at position " + context.TokenPosition);
					//   }
					//}
					if (!VariableValidCharsRegExp.IsMatch(c)){
						break;
					}
					variableStr.Append(c);
					context.TokenPosition ++;
				}
				ElementVariable element = new ElementVariable(variableStr.ToString());
				context.Ouput.Push(element);
				return true;
			}
			return false;
			
		}

		public override string ToString()
		{
			return Expression;
		}

		public override int GetHashCode()
		{
			return Expression.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			ElementVariable ev = obj as ElementVariable;
			if (ev == null) return false;
			return this.Expression.Equals(ev.Expression);
		}

		public override void Eval(Stack<Element> output)
		{
			throw new NotImplementedException();
		}
	}
}
