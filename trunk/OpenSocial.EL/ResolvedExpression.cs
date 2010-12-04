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

namespace Negroni.OpenSocial.EL
{
	/// <summary>
	/// Expression that has been resolved by the EL processor
	/// </summary>
	public class ResolvedExpression
	{
		public static readonly Type booleanType = bool.TrueString.GetType();
		public static readonly Type intType = int.MinValue.GetType();
		public static readonly Type doubleType = double.MinValue.GetType();
		public static readonly Type objectType = (new object()).GetType();
		public static readonly Type stringType = string.Empty.GetType();

		internal ResolvedExpression(){
			ResolvedValue = null;
			EvalException = null;
			ResolvedType = objectType;
		}

		internal ResolvedExpression(IElementValue elementValue) : this()
		{
			switch(elementValue.Type){
				case TokenType.BooleanLiteral:
					ResolvedType = booleanType;
					break;
				case TokenType.DecimalLiteral:
					ResolvedType = doubleType;
					break;
				case TokenType.IntegerLiteral:
					ResolvedType = intType;
					break;
				case TokenType.Object:
					ResolvedType = objectType;
					break;
				case TokenType.StringLiteral:
					ResolvedType = stringType;
					break;
				default:
					ResolvedType = null;
					break;
			}
			ResolvedValue = elementValue.Value;
			EvalException = null;
		}
		
		internal ResolvedExpression(ELException exception) :this(){
			EvalException = exception;
			ResolvedValue = null;
			ResolvedType = null;
		}
	
		/// <summary>
		/// Type of the answer that has been resolved
		/// </summary>
		public Type ResolvedType
		{
			get; private set;
		}


		/// <summary>
		/// String representation of the answer
		/// </summary>
		public object ResolvedValue
		{
			get; private set;
		}

		/// <summary>
		/// Checks to see if the expression evaluation has generated an exception
		/// </summary>
		public bool HasException()
		{
			return (EvalException != null);
		}

		/// <summary>
		/// An evaluation exception if one occurred, or null
		/// </summary>
		public ELException EvalException
		{
			get; private set;
		}
	}
}
