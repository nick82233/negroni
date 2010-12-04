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
using System.Web;

namespace Negroni.OpenSocial.EL.Operators
{
	public class HtmlDecodeOperator : FunctionOperator
	{
		private static readonly HtmlDecodeOperator instance = new HtmlDecodeOperator();

		private HtmlDecodeOperator() : base(OperatorType.HtmlDecode, AssignmentEnum.Right, 10) { }

		public static HtmlDecodeOperator Instance
		{
			get{return instance;}
		}


		public override IElementValue Apply(IElementValue[] parms)
		{
			if (parms == null || parms.Length == 0)
			{
				throw new ELOperationException("Invalid number of parameter for function HtmlDecode");
			}

			IElementValue variable = parms[0];
			string strVal = variable.ToString();
			return new ElementLiteral(TokenType.StringLiteral, HttpUtility.HtmlDecode(strVal));
		}
	}
}
