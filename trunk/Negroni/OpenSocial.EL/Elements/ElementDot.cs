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
	public class ElementDot : Element
	{

		public  ElementDot(): base(TokenType.Dot){}

		internal static bool ParseDot(Parser.ParseContext context){
			char c = context.Expression[context.TokenPosition];
			if (c == '.'){
				ValidateTokenOrder(context, TokenType.Dot);
				ElementDot element = new ElementDot();
				context.Ouput.Push(element);
				context.TokenPosition++;
				return true;
			}
			return false;
		}

		public override void Eval(Stack<Element> output)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return ".";
		}

		public override bool Equals(object obj)
		{
			//TODO override correctly Equals
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return ".".GetHashCode();
		}
	}
}
