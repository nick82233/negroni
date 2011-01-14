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

namespace Negroni.OpenSocial.EL.Operators
{
	public class JsStringEscapeOperator : FunctionOperator
	{
		private static readonly JsStringEscapeOperator instance = new JsStringEscapeOperator();
		
		public static JsStringEscapeOperator Instance{
			get{ return instance; }
		}

		private JsStringEscapeOperator() : base(OperatorType.JsStringEscape, AssignmentEnum.Right, 10) { }

		public override IElementValue Apply(IElementValue[] parms)
		{
			if (parms == null || parms.Length == 0 || parms.Length > 2){
				throw new ELOperationException("Invalid number of parameter for function JsStringEscapeOperator");
			}
			IElementValue o1 = parms[0];
			bool o2 = false;
			
			if (parms.Length == 2){
				if (parms[1].Type == TokenType.BooleanLiteral){
					o2 = (bool)parms[1].Value;
				}
			}
			if (o2){
				return new ElementLiteral(TokenType.StringLiteral, o1.ToString().Replace("\\", "\\\\").Replace("'", "\\'"));
			}
			else{
				return new ElementLiteral(TokenType.StringLiteral, o1.ToString().Replace("\\", "\\\\").Replace("\"", "\\\""));
			}
		}
	}
}
