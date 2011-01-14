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

namespace Negroni.OpenSocial.EL.Elements
{

	public enum TokenType
	{
		None = 0,
		Start = 1,
		End = 2,
		BinaryOperator = 3,
		UnitaryOperator = 4,
		StringLiteral = 5,
		DecimalLiteral = 6,
		IntegerLiteral = 7,
		BooleanLiteral = 8,
		Variable = 9,
		OpenBracket = 10,
		CloseBracket = 11,
		OpenParenthesis = 12,
		CloseParenthesis = 13,
		TernaryIf = 14,
		TernaryElse = 15,
		Dot = 16,
		Object = 17,
		Selector = 18,
		Function = 19,
	}
}
