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
	public abstract class Element
	{


		private static readonly IDictionary<TokenType, TokenType[]> tokenOrder = new Dictionary<TokenType, TokenType[]>();
		static Element(){
			InitializeTokenOrder();
		}

		/// <summary>
		/// Initializes the validation ordering for tokens
		/// </summary>
		private static void InitializeTokenOrder()
		{
			tokenOrder.Add(
					TokenType.OpenBracket,
					new TokenType[]{
						TokenType.Function,
						TokenType.UnitaryOperator,TokenType.StringLiteral,
						TokenType.DecimalLiteral, 
						TokenType.IntegerLiteral,
						TokenType.BooleanLiteral,
						TokenType.Variable,
						TokenType.OpenParenthesis,
						TokenType.Selector
					});
			tokenOrder.Add(
					TokenType.CloseBracket,
					new TokenType[]{
						TokenType.CloseParenthesis,
						TokenType.BinaryOperator,
						TokenType.End,
						TokenType.OpenBracket,
						TokenType.CloseBracket,
						TokenType.TernaryElse,
						TokenType.TernaryIf, 
						TokenType.Dot
					});
			tokenOrder.Add(
					TokenType.Start,
					new TokenType[]{
						TokenType.Function,
						TokenType.StringLiteral,
						TokenType.DecimalLiteral, 
						TokenType.IntegerLiteral,
						TokenType.BooleanLiteral,
						TokenType.Variable,
						TokenType.OpenParenthesis,
						TokenType.UnitaryOperator});
			tokenOrder.Add(
					TokenType.BinaryOperator,
					new TokenType[]{
						TokenType.Function,
						TokenType.StringLiteral,
						TokenType.DecimalLiteral, 
						TokenType.IntegerLiteral,
						TokenType.BooleanLiteral,
						TokenType.Variable,
						TokenType.OpenParenthesis,
						TokenType.UnitaryOperator});
			tokenOrder.Add(
					TokenType.UnitaryOperator,
					new TokenType[]{
						TokenType.Function,
						TokenType.OpenParenthesis,
						TokenType.StringLiteral,
						TokenType.DecimalLiteral, 
						TokenType.IntegerLiteral,
						TokenType.BooleanLiteral,
						TokenType.Variable,
						TokenType.UnitaryOperator});
			tokenOrder.Add(
					TokenType.StringLiteral,
					new TokenType[]{
						TokenType.BinaryOperator,
						TokenType.End,
						TokenType.CloseParenthesis,
						TokenType.CloseBracket,
						TokenType.TernaryIf,
						TokenType.TernaryElse, 
						TokenType.Dot});
			tokenOrder.Add(
					TokenType.DecimalLiteral,
					new TokenType[]{
						TokenType.BinaryOperator,
						TokenType.End,
						TokenType.CloseParenthesis,
						TokenType.CloseBracket,
						TokenType.TernaryIf,
						TokenType.TernaryElse});
			tokenOrder.Add(
				TokenType.IntegerLiteral,
					new TokenType[]{
						TokenType.BinaryOperator,
						TokenType.End,
						TokenType.CloseParenthesis,
						TokenType.CloseBracket,
						TokenType.TernaryIf,
						TokenType.TernaryElse});
			tokenOrder.Add(
					TokenType.BooleanLiteral,
					new TokenType[]{
						TokenType.BinaryOperator,
						TokenType.End,
						TokenType.CloseParenthesis,
						TokenType.CloseBracket,
						TokenType.TernaryIf,
						TokenType.TernaryElse});
			tokenOrder.Add(
					TokenType.Variable,
					new TokenType[]{
						TokenType.BinaryOperator,
						TokenType.End,
						TokenType.CloseParenthesis,
						TokenType.OpenBracket,
						TokenType.TernaryIf,
						TokenType.TernaryElse, 
						TokenType.Dot, 
						TokenType.CloseBracket,});
			tokenOrder.Add(
					TokenType.OpenParenthesis,
					new TokenType[]{
						TokenType.Function,
						TokenType.UnitaryOperator,
						TokenType.OpenParenthesis,
						TokenType.StringLiteral,
						TokenType.DecimalLiteral,
						TokenType.IntegerLiteral, 
						TokenType.BooleanLiteral,
						TokenType.Variable});
			tokenOrder.Add(
					TokenType.CloseParenthesis,
					new TokenType[]{
						TokenType.CloseParenthesis,
						TokenType.End,
						TokenType.BinaryOperator,
						TokenType.TernaryIf,
						TokenType.TernaryElse, 
						TokenType.CloseBracket});
			tokenOrder.Add(
					TokenType.Function,
					new TokenType[]{
						TokenType.CloseParenthesis,
						TokenType.End,
						TokenType.BinaryOperator,
						TokenType.TernaryIf,
						TokenType.TernaryElse, 
						TokenType.CloseBracket});
			tokenOrder.Add(
					TokenType.TernaryIf,
					new TokenType[]{
						TokenType.Function,
						TokenType.UnitaryOperator,
						TokenType.StringLiteral,
						TokenType.DecimalLiteral, 
						TokenType.IntegerLiteral,
						TokenType.BooleanLiteral,
						TokenType.Variable,
						TokenType.OpenParenthesis});
			tokenOrder.Add(
					TokenType.TernaryElse,
					new TokenType[]{
						TokenType.Function,
						TokenType.UnitaryOperator,
						TokenType.StringLiteral,
						TokenType.DecimalLiteral, 
						TokenType.IntegerLiteral,
						TokenType.BooleanLiteral,
						TokenType.Variable,
						TokenType.OpenParenthesis});
			tokenOrder.Add(
					TokenType.Dot,
					new TokenType[] { 
						TokenType.Variable });
			tokenOrder.Add(
					TokenType.Selector,
					new TokenType[]{
						TokenType.CloseBracket
					});
		}

		public Element(TokenType type)
		{
			Type = type;
		}

		public TokenType Type { get; private set; }

		internal static void ValidateTokenOrder(Parser.ParseContext context, TokenType tokenType)
		{
			bool hasToken = false;
			for (int i = 0; i < tokenOrder[context.LastTokenType].Length; i++)
			{
				if (tokenOrder[context.LastTokenType][i] == tokenType)
				{
					hasToken = true;
					break;
				}
			}

			if (!hasToken)
			{
				throw new ELException("Invalid token at position " + context.TokenPosition);
			}
			context.LastTokenType = tokenType;
		}
		
		public abstract void Eval(Stack<Element> output);
	}
}
