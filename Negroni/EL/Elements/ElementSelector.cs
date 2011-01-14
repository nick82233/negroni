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
	/// <summary>
	/// Special variable that performs a select operation similar to an XPath selector
	/// </summary>
	public class ElementSelector : Element, IElementValue
	{
		public string Expression {get; private set;}
		public ElementSelector(string expression)
			: base(TokenType.Selector)
		{
			Expression = expression;
		}

		//private static readonly Regex VariableStartRegExp = new Regex("[a-zA-Z]",
		//    RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled
		//);

		private static readonly Regex VariableValidCharsRegExp = new Regex("[\\w]",
			RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled
		);
		
		internal static bool Parse(Parser.ParseContext context){
			char c = context.Expression[context.TokenPosition];
			if (c == '@'){
				ValidateTokenOrder(context, TokenType.Selector);
				
				StringBuilder selectorStr = new StringBuilder();
				while(context.TokenPosition < context.Expression.Length){
					c = context.Expression[context.TokenPosition];
					//if (!VariableValidCharsRegExp.IsMatch(c)){
					if(c == ']'){
						break;
					}
					selectorStr.Append(c);
					context.TokenPosition ++;
				}
				ElementSelector element = new ElementSelector(selectorStr.ToString());
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

		#region IElementValue Members

		public object Value
		{
			get { return Expression; }
		}

		public T GetValue<T>()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
