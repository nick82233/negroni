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

namespace Negroni.OpenSocial.EL.Elements
{
	public class ElementObject : Element, IElementValue
	{
	
		public ElementObject(object value):base(TokenType.Object){
			Value = value;
		}
	
	
		public override void Eval(Stack<Element> output)
		{
			throw new NotImplementedException();
		}


		public object Value
		{
			get ;
			private set;
		}

		public T GetValue<T>()
		{
			return (T)Value;
		}

		public override string ToString()
		{
			if (Value == null) return "null";
			if (Value is IList){
				return "array";
			}
			return "object";
			
		}

		public override int GetHashCode()
		{
			if (Value == null) return "null".GetHashCode();
			return Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj  == null) return false;
			if (!(obj is ElementObject)){
				return false;
			}
			ElementObject element = (ElementObject)obj;
			
			if (this.Value == null && element.Value == null){
				return true;
			}
			else if (this.Value == null && element.Value != null){
				return false;
			}
			else{
				return object.ReferenceEquals(this.Value, element.Value);
			}
			
		}

	}
}
