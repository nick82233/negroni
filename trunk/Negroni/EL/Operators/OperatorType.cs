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

namespace Negroni.OpenSocial.EL.Operators
{
	public enum OperatorType
	{
		None = 0,
		
		Addition = 1,
		Substraction = 2,

		Multiplication = 3,
		Division = 4,
		Modulo = 5,

		Equals = 6,
		NotEquals = 7,

		LessThan = 8,
		LessThanEquals = 9,

		GreaterThan = 10,
		GreaterThanEquals = 11,

		Not = 12,
		Negative = 13,
		Empty = 14,
		ToNumber = 15,
		ToString = 16,
		ToBoolean = 17,
		MathRound = 18,
		MathFloor = 19,
		MathCeil = 20,

		And = 21,
		Or = 22,
		
		Dot = 23,

		HtmlEncode = 24,
		HtmlDecode = 25,
		UrlEncode = 26,
		UrlDecode = 27,
		JsStringEscape = 28,
		
		
		
	}
}
