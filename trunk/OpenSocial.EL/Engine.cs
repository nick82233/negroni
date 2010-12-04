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
using System.Collections;
using Negroni.OpenSocial.EL.Operators;
using Negroni.OpenSocial.EL.Elements;

namespace Negroni.OpenSocial.EL
{
	
	
	
	/// <summary>
	/// Main processing engine for EL
	/// </summary>
	public static class Engine
	{

		#region character definitions

		/// <summary>
		/// Start of Variable expression
		/// </summary>
		public const string VARIABLE_START = "${";
		/// <summary>
		/// End of Variable expression
		/// </summary>
		public const string VARIABLE_END = "}";

		public const string QUOTE_SINGLE = "'";

		public const string QUOTE_DOUBLE = "\"";

		#endregion


		/// <summary>
		/// Strips off start and end brackets and returns variable text.
		/// Initial validation of expression
		/// </summary>
		/// <param name="variableString">variable containing start and end brackets</param>
		/// <returns>variable with out brackets</returns>
		/// <exception cref="ELException">Exception if the variable do not contains start '${' and end '}' brackets or the content is emtpy</exception>
		public static string GetVariableExpression(string variableString)
		{
			string variable = string.IsNullOrEmpty(variableString) ? variableString : variableString.Trim();

			if (String.IsNullOrEmpty(variable))
			{
				throw new ELException("Invalid expression. Null or empty expression passed.");
			}
			else if (!variable.StartsWith(VARIABLE_START) && !variable.EndsWith(VARIABLE_END))
			{
				return variable;
			}

			variable = variable.Substring(VARIABLE_START.Length, variable.Length - (VARIABLE_START.Length + VARIABLE_END.Length)).Trim();
			if (string.IsNullOrEmpty(variable))
			{
				throw new ELException("Invalid Expression. Empty expression");
			}
			return variable;
		}

		public static ResolvedExpression ResolveExpression(string expression, IObjectResolver dc)
		{
			if (string.IsNullOrEmpty(expression)){
				return new ResolvedExpression();
			}
			try{
				string variableExpression = GetVariableExpression(expression);
				IList<Element> parseData = Parser.Parse(variableExpression);
				IElementValue response = EvaluateExpression(parseData, dc);
				return new ResolvedExpression(response);
				
			}
			catch(ELException exception){
				dc.ELErrors.Add(exception);
				return new ResolvedExpression(exception);
			}
			catch(Exception e){
				dc.ELErrors.Add(new ELException("Unexpected error in expression language"));
				return new ResolvedExpression();
			}
			
		}

		private static IElementValue EvaluateExpression(IList<Element> data, IObjectResolver dc)
		{
			if (data.Count == 0) return null;
			Stack<IElementValue> output = new Stack<IElementValue>();
			for(int j = 0; j < data.Count; j++){
				Element element = data[j];
				switch(element.Type){
					case TokenType.BinaryOperator:
						ElementBinaryOperator binaryElement = (ElementBinaryOperator)element;
						BinaryOperator binaryOperator = (BinaryOperator)binaryElement.Operator;
						IElementValue b2 = output.Pop();
						IElementValue b1 = output.Pop();
						IElementValue binaryOutput = binaryOperator.Apply(b1, b2);
						output.Push(binaryOutput);
						break;
					case TokenType.UnitaryOperator:
						ElementUnitaryOperator unitaryElement = (ElementUnitaryOperator)element;
						UnitaryOperator unitaryOperator = (UnitaryOperator)unitaryElement.Operator;
						IElementValue u1 = output.Pop();
						IElementValue unitaryOutput = unitaryOperator.Apply(u1);
						output.Push(unitaryOutput);
						break;
					case TokenType.Function:
						ElementFunction functionElement = (ElementFunction)element;
						FunctionOperator functionOperator = (FunctionOperator)functionElement.Operator;
						IElementValue[] paramVals = new IElementValue[functionElement.Parameters.Count];
						for(int paramIndex = 0; paramIndex < functionElement.Parameters.Count; paramIndex++){
							paramVals[paramIndex] = EvaluateExpression(functionElement.Parameters[paramIndex], dc);
						}
						IElementValue functionOutput = functionOperator.Apply(paramVals);
						output.Push(functionOutput);
						break;
					case TokenType.DecimalLiteral:
					case TokenType.IntegerLiteral:
					case TokenType.StringLiteral:
					case TokenType.BooleanLiteral:
					case TokenType.Selector:
						output.Push((IElementValue)element);
						break;
					case TokenType.OpenParenthesis:
						int parenthesis = 1;
						IList<Element> parenthesisElements = new List<Element>();
						for (j = j + 1; j < data.Count; j++){
							Element parenthesisElement = data[j];
							if (parenthesisElement.Type == TokenType.OpenParenthesis){
								parenthesis++;
							}
							else if (parenthesisElement.Type == TokenType.CloseParenthesis){
								parenthesis--;
								if (parenthesis == 0){
									IElementValue parenthesisOutput = EvaluateExpression(parenthesisElements, dc);
									output.Push(parenthesisOutput);
									break;
								}
							}
							else{
								parenthesisElements.Add(parenthesisElement);
							}
						}
						break;
					case TokenType.TernaryIf:
						IElementValue ternaryIfValue = output.Pop();
						if (ternaryIfValue.Type != TokenType.BooleanLiteral)
						{
							throw new ELException("Condition is not boolean.");
						}
						bool condition = (bool)ternaryIfValue.Value;
						IList<Element> ternaryElements = new List<Element>();
						bool elseReached = false;
						for (j = j + 1; j < data.Count; j++){
							Element conditionValue = data[j];
							if (conditionValue.Type == TokenType.TernaryElse)
							{
								elseReached = true;
							}
							if (condition == true){
								if (elseReached == true){ 
									j = data.Count;
									break;
								}
								ternaryElements.Add(conditionValue);
							}
							else{
								if (elseReached == true)
								{
									ternaryElements.Add(conditionValue);
								}
							}
						}
						IElementValue ternaryOutput = EvaluateExpression(ternaryElements, dc);
						output.Push(ternaryOutput);
						break;
					case TokenType.Variable:
						StringBuilder variableStr = new StringBuilder();
						string expression;
						expression = ((ElementVariable)element).Expression;
						variableStr.Append(expression);
						bool variableBoundryHit = false;
						bool end = false;
						while (j + 1 < data.Count && end == false){
							switch(data[j + 1].Type){
								case TokenType.Variable:
									if (!variableBoundryHit)
									{
										end = true;
										break;
									}
									j++;
									variableStr.Append(((ElementVariable) data[j]).Expression);
									variableBoundryHit = false;
									break;
								case TokenType.OpenBracket:
									j++;
									int brackets = 1;
									variableBoundryHit = true;
									IList<Element> bracketElements = new List<Element>();
									for (j = j + 1; j < data.Count; j++)
									{
										Element bracketElement = data[j];
										if (bracketElement.Type == TokenType.OpenBracket)
										{
											brackets++;
										}
										else if (bracketElement.Type == TokenType.CloseBracket)
										{
											brackets--;
											if (brackets == 0)
											{
												IElementValue bracketOutput = EvaluateExpression(bracketElements, dc);
												if (bracketOutput.Type != TokenType.IntegerLiteral
													&& bracketOutput.Type != TokenType.Selector){
													throw new ELException("Integers and Selectors can only be used as brackets indexes.");
												}
												variableStr.Append("[").Append(bracketOutput.Value).Append("]");
												break;
											}
										}
										else
										{
											bracketElements.Add(bracketElement);
										}
									}
									break;
								case TokenType.Dot:
									j++;
									variableBoundryHit = true;
									variableStr.Append(".");
									break;
								default:
									end = true;
									break;
							}
						}

						object value = dc.GetVariableObject(variableStr.ToString());
						IElementValue elementValue;
						if (value is string){
							elementValue = new ElementLiteral(TokenType.StringLiteral, value);
						}
						else if (value is bool){
							elementValue = new ElementLiteral(TokenType.BooleanLiteral, value);
						}
						else if (value is int){
							elementValue = new ElementLiteral(TokenType.IntegerLiteral, value);
						}
						else if(value is double || value is float || value is decimal){
							elementValue = new ElementLiteral(TokenType.DecimalLiteral, Convert.ToDouble(value));
						}
						else{
							elementValue = new ElementLiteral(TokenType.Object, value);
						}
						output.Push(elementValue);
						break;
				}
			}
			if (output.Count != 1){
				throw new ELException("Error evaluating expression");
			}
			return (IElementValue)output.Pop();
		}

	}
}
