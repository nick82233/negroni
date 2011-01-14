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
	public class ElementLiteral : Element, IElementValue
	{
		
	
		internal static readonly ElementLiteral trueInstance = new ElementLiteral(TokenType.BooleanLiteral, true);
		internal static readonly ElementLiteral falseInstance = new ElementLiteral(TokenType.BooleanLiteral, false);

		internal static readonly ElementLiteral InfinityInstance = new ElementLiteral(TokenType.DecimalLiteral, double.PositiveInfinity);
		internal static readonly ElementLiteral NegativeInfinityInstance = new ElementLiteral(TokenType.DecimalLiteral, double.NegativeInfinity);
		
		public static ElementLiteral FalseInstance{get {return falseInstance;}}
		
		public static ElementLiteral TrueInstance{get{return trueInstance;}}
	
		public ElementLiteral(TokenType tokenType, object value):base(tokenType){
			Value = value;
		}
		
		public object Value{
			get; set;
		}
		
		public T GetValue<T>(){
			return (T) Value;
		}
		
		private static bool ParseString(Parser.ParseContext context, out TokenType tokenType, out object value){
			char c = context.Expression[context.TokenPosition];
			value = null;
			tokenType = TokenType.None;
			if (c == '"' || c == '\'')
			{
				//We validate first that way we don't need to process the rest of the string if not needed.
				ValidateTokenOrder(context, TokenType.StringLiteral);
				StringBuilder content = new StringBuilder();
				char encloseChar = c;
				context.TokenPosition++;
				while (context.TokenPosition < context.Expression.Length)
				{
					c = context.Expression[context.TokenPosition];
					if (c == encloseChar)
					{
						tokenType = TokenType.StringLiteral;
						value = content.ToString();
						context.TokenPosition ++;
						return true;
					}
					else if (c == '\\')
					{
						if (context.Expression.Length <= context.TokenPosition + 1)
						{
							throw new ELException("Invalid token at position " + context.TokenPosition);
						}
						else
						{
							context.TokenPosition++;
							c = context.Expression[context.TokenPosition];
							switch(c){
								case '\'':
								case '"':
									content.Append(c);
									break;
								//TODO add extra excape handling
								//http://ftp.icm.edu.pl/packages/tucows/html/programmer/jstut/jsTabChars.html
								default:
									throw new ELException("Invalid token at position " + context.TokenPosition);
							}
							context.TokenPosition++;
						}
					}
					else
					{
						content.Append(c);
						context.TokenPosition++;
					}
				}
				throw new ELException("Invalid expression string not closed");
			}
			return false;
		}
		
		private static bool IsNumber(char c){
			switch (c){
				case '.':
					return false;
				default:
					return IsNumeric(c);
			}
		}
		
		private static bool IsNumeric(char c){
			switch(c){
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '.':
					return true;
				default:
					return false;
			}
			
		}
		
		private static bool ParseNumeric(Parser.ParseContext context, out TokenType tokenType, out object value){
			tokenType = TokenType.None;
			value = null;
			char c = context.Expression[context.TokenPosition];
			if (IsNumeric(c)){
				//We validate first that way we don't need to process the rest of the string if not needed.
				ValidateTokenOrder(context, TokenType.DecimalLiteral);
				bool point = false;
				bool scientific = false;
				StringBuilder strValue = new StringBuilder();
				
				while (context.TokenPosition < context.Expression.Length){
					c = context.Expression[context.TokenPosition];
					if (c.Equals('.')){
						if (point || scientific){
							throw new ELException("Invalid Token at position " + context.TokenPosition);
						}
						point = true;
						strValue.Append(c);
						context.TokenPosition++;
					}
					else if (IsNumber(c)){
						strValue.Append(c);
						context.TokenPosition++;
					}
					else if (c == 'e' || c == 'E'){
						if (scientific)
						{
							throw new ELException("Invalid Token at position " + context.TokenPosition);
						}
						if (context.TokenPosition + 1 >= context.Expression.Length){
							throw new ELException("Invalid Token at position " + context.TokenPosition);
						}
						scientific = true;
						strValue.Append('e');
						context.TokenPosition++;
						c = context.Expression[context.TokenPosition];
						if (c == '-' || c == '+'){
							strValue.Append(c);
							if (context.TokenPosition + 1 >= context.Expression.Length)
							{
								throw new ELException("Invalid Token at position " + context.TokenPosition);
							}
							context.TokenPosition++;
							c = context.Expression[context.TokenPosition];
						}
						
						if (!IsNumber(c))
						{
							throw new ELException("Invalid Token at position " + context.TokenPosition);
						}
						strValue.Append(c);
						context.TokenPosition++;
					}
					else{
						break;
					}
					
				}
				
				if (point || scientific){
					
					double doubleValue = Double.Parse(strValue.ToString(), System.Globalization.NumberStyles.Float);
					if (doubleValue == Math.Floor(doubleValue)){
						tokenType = TokenType.IntegerLiteral;
						value = (int) doubleValue;
					}
					else{
						tokenType = TokenType.DecimalLiteral;
						value = doubleValue;
					}
					
				}
				else{
					tokenType = TokenType.IntegerLiteral;
					value = Int32.Parse(strValue.ToString());
				}
				
				return true;
			}
			return false;
		}
		private static readonly Regex testTrue = new Regex("true\\W*",
			RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled
		);
		private static readonly Regex testFalse = new Regex("false\\W*",
			RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled
		);
		
		private static bool ParseBool(Parser.ParseContext context, out TokenType tokenType, out object value){
			tokenType = TokenType.None;
			value = null;
			string subStr = context.Expression.Substring(context.TokenPosition, Math.Min(5, context.Expression.Length - context.TokenPosition));
			if (testTrue.IsMatch(subStr)){
				//We validate first that way we don't need to process the rest of the string if not needed.
				ValidateTokenOrder(context, TokenType.BooleanLiteral);
				tokenType = TokenType.BooleanLiteral;
				value = true;
				context.TokenPosition += 4;
				return true;
			}
			subStr = context.Expression.Substring(context.TokenPosition, Math.Min(6, context.Expression.Length - context.TokenPosition));
			if (testFalse.IsMatch(subStr)){
				//We validate first that way we don't need to process the rest of the string if not needed.
				ValidateTokenOrder(context, TokenType.BooleanLiteral);
				tokenType = TokenType.BooleanLiteral;
				value = false;
				context.TokenPosition += 5;
				return true;
			}
			return false;
		}
		
		internal static bool Parse(Parser.ParseContext context){
			TokenType tokenType =  TokenType.None;
			object value = null;
			bool isLiteral = ParseString(context, out tokenType, out value);
			if (!isLiteral){
				isLiteral = ParseNumeric(context, out tokenType, out value);
			}
			if (!isLiteral){
				isLiteral = ParseBool(context, out tokenType, out value);
			}
			if (!isLiteral){
				return false;
			}
			ElementLiteral element = new ElementLiteral(tokenType, value);
			context.Ouput.Push(element);
			return true;
		}

		public override string ToString()
		{
			switch (Type){
				case TokenType.StringLiteral:
					return Value.ToString();
				case TokenType.DecimalLiteral:
					if (double.IsNegativeInfinity((double)Value)){
						return "-Infinity";
					}
					else if (double.IsPositiveInfinity((double)Value)){
						return "Infinity";
					}
					return ((double)Value).ToString("0.0#########");
				case TokenType.IntegerLiteral:
					return ((int)Value).ToString();
				case TokenType.BooleanLiteral:
					return (bool)Value?"true":"false";
			}
			return string.Empty;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null) {
				return false;
			}
			if (!(obj is ElementLiteral)){
				return false;
			}
			ElementLiteral literal = (ElementLiteral)obj;
			
			if (literal.Type != this.Type){
				return false;
			}
			
			if (this.Type == TokenType.StringLiteral){
				return (string)this.Value == (string)literal.Value;
			}
			else if (this.Type == TokenType.DecimalLiteral || this.Type == TokenType.IntegerLiteral){
				double leftValue = Convert.ToDouble(this.Value);
				double rightValue = Convert.ToDouble(literal.Value);
				return leftValue == rightValue;
			}
			else if (this.Type == TokenType.BooleanLiteral){
				return (bool)this.Value == (bool)literal.Value;
			}
			return this.Value == literal.Value;
		}

		public override void Eval(Stack<Element> output)
		{
			output.Push(this);
		}
	}
}
