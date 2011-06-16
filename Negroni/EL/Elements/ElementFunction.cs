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
	public class ElementFunction : ElementOperator
	{

		public IList<IList<Element>> Parameters{get; private set;}

		public ElementFunction(OperatorType operatorType, IList<IList<Element>> parameters) : base(operatorType, TokenType.Function){
			Parameters = parameters;
		}

		private static IList<IList<Element>> GetParameters(Parser.ParseContext context){
			char c;
			while(true){
				context.TokenPosition++;
				c = context.Expression[context.TokenPosition];
				if (c == ' ' || c == '\t' || c == '\n' || c == '\r'){
					continue;
				}
				else if (c != '('){
					throw new ELException("Invalid token");
				}
				else{
					break;
				}
			}
			if (c != '('){
				throw new ELException("Invalid token");
			}
			context.TokenPosition ++;
			int parentesisCount = 1;
			StringBuilder param = new StringBuilder();
			IList<IList<Element>> parms = new List<IList<Element>>();
			string paramVal;
			while(parentesisCount > 0 && context.TokenPosition < context.Expression.Length){
				c = context.Expression[context.TokenPosition];
				switch (c){
					case ')':
						parentesisCount --;
						context.TokenPosition++;
						if (parentesisCount == 0){
							paramVal = param.ToString().Trim();
							if (string.IsNullOrEmpty(paramVal)){
								throw new ELException("Invalid token parameter");
							}
							else{
								parms.Add(Parser.Parse(paramVal));
							}
							continue;
						}
						break;
					case '(':
						parentesisCount++;
						break;
					case ',':
						paramVal = param.ToString().Trim();
						if (string.IsNullOrEmpty(paramVal)){
							throw new EL.ELException("Invalid parameter");
						}
						else{
							parms.Add(Parser.Parse(paramVal));
						}
						context.TokenPosition ++;
						param.Remove(0, param.Length);
						continue;
				}
				param.Append(c);
				context.TokenPosition++;
			}
			return parms;
		}

		internal static bool ParseFunction(Parser.ParseContext context){
			char c = context.Expression[context.TokenPosition];
			int offset = 0;
			string subStr;
			OperatorType operationType = OperatorType.None;
			TokenType lastTokenType = context.LastTokenType;
			switch (c)
			{
				case 'e':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(6, context.Expression.Length - context.TokenPosition));
					if (subStr.Equals("empty ") || subStr.Equals("empty("))
					{
						offset = 4;
						operationType = OperatorType.Empty;
						break;
					}
					return false;
				case 't':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(9, context.Expression.Length - context.TokenPosition));
					if (subStr == "toNumber " || subStr == "toNumber(")
					{
						offset = 7;
						operationType = OperatorType.ToNumber;
						break;
					}
					else if (subStr == "toString " || subStr == "toString(")
					{
						offset = 7;
						operationType = OperatorType.ToString;
						break;
					}
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(10, context.Expression.Length - context.TokenPosition));
					if (subStr == "toBoolean " || subStr == "toBoolean(")
					{
						offset = 8;
						operationType = OperatorType.ToBoolean;
						break;
					}
					return false;
				case 'm':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(10, context.Expression.Length - context.TokenPosition));
					if (subStr == "mathRound " || subStr == "mathRound(")
					{
						offset = 8;
						operationType = OperatorType.MathRound;
						break;
					}
					else if (subStr == "mathFloor " || subStr == "mathFloor(")
					{
						offset = 8;
						operationType = OperatorType.MathFloor;
						break;
					}
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(9, context.Expression.Length - context.TokenPosition));
					if (subStr == "mathCeil " || subStr == "mathCeil(")
					{
						offset = 7;
						operationType = OperatorType.MathCeil;
						break;
					}
					return false;
				case 'o':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(14, context.Expression.Length - context.TokenPosition));
					if (subStr == "os:htmlEncode " || subStr == "os:htmlEncode("){
						offset = 12;
						operationType = OperatorType.HtmlEncode;
						break;
					}
					if (subStr == "os:htmlDecode " || subStr == "os:htmlDecode(")
					{
						offset = 12;
						operationType = OperatorType.HtmlDecode;
						break;
					}
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(13, context.Expression.Length - context.TokenPosition));
					if (subStr == "os:urlEncode " || subStr == "os:urlEncode(")
					{
						offset = 11;
						operationType = OperatorType.UrlEncode;
						break;
					}
					if (subStr == "os:urlDecode " || subStr == "os:urlDecode(")
					{
						offset = 11;
						operationType = OperatorType.UrlDecode;
						break;
					}
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(18, context.Expression.Length - context.TokenPosition));
					if (subStr == "os:jsStringEscape " || subStr == "os:jsStringEscape(")
					{
						offset = 16;
						operationType = OperatorType.JsStringEscape;
						break;
					}
					return false;
				case 'h':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(11, context.Expression.Length - context.TokenPosition));
					if (subStr == "htmlEncode " || subStr == "htmlEncode(")
					{
						offset = 9;
						operationType = OperatorType.HtmlEncode;
						break;
					}
					if (subStr == "htmlDecode " || subStr == "htmlDecode(")
					{
						offset = 9;
						operationType = OperatorType.HtmlDecode;
						break;
					}
					return false;
				case 'u':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(10, context.Expression.Length - context.TokenPosition));
					if (subStr == "urlEncode " || subStr == "urlEncode(")
					{
						offset = 8;
						operationType = OperatorType.UrlEncode;
						break;
					}
					if (subStr == "urlDecode " || subStr == "urlDecode(")
					{
						offset = 8;
						operationType = OperatorType.UrlDecode;
						break;
					}
					return false;
				case 'j':
					subStr = context.Expression.Substring(context.TokenPosition, Math.Min(15, context.Expression.Length - context.TokenPosition));
					if (subStr == "jsStringEscape " || subStr == "jsStringEscape(")
					{
						offset = 13;
						operationType = OperatorType.JsStringEscape;
						break;
					}
					return false;
				default:
					return false;
			}

			ValidateTokenOrder(context, TokenType.Function);
			context.TokenPosition += offset;

			IList<IList<Element>> parameters = GetParameters(context);
			ElementOperator element = new ElementFunction(operationType, parameters);
			ProcessOperator(context, element);
			return true;
		}

		public override string ToString()
		{
			switch (OperatorType)
			{
				case OperatorType.Empty:
					return "empty";
				case OperatorType.ToBoolean:
					return "toBoolean";
				case OperatorType.ToString:
					return "toString";
				case OperatorType.ToNumber:
					return "toNumber";
				case OperatorType.MathFloor:
					return "mathFloor";
				case OperatorType.MathCeil:
					return "mathCeil";
				case OperatorType.MathRound:
					return "mathRound";
				case OperatorType.UrlDecode:
					return "urlDecode";
				case OperatorType.UrlEncode:
					return "urlEncode";
				case OperatorType.HtmlDecode:
					return "htmlDecode";
				case OperatorType.HtmlEncode:
					return "htmlEncode";
				case OperatorType.JsStringEscape:
					return "jsStringEscape";
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
