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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline;
using Negroni.OpenSocial.EL;

namespace Negroni.TemplateFramework
{
	public class BaseRepeaterControl : BaseContainerControl
	{

		string _loopItemKey = null;

		/// <summary>
		/// Internal DataContext key used for repeated data item within the loop
		/// </summary>
		public string LoopItemKey
		{
			get
			{
				return _loopItemKey;
			}
			set
			{
				_loopItemKey = value;
			}
		}



		/// <summary>
		/// Condition to evaluate with each loop to emit results or not.
		/// If this condition is null each loop evaluates.
		/// </summary>
		/// <remarks>This is here to support attribute-based OSML repeaters
		/// with an additional IF statement.</remarks>
		public string LoopConditionalExpression{
			get; set;
		}

		/// <summary>
		/// Evaluates the current LoopConditionalExpression.
		/// Returns the result
		/// </summary>
		/// <returns></returns>
		protected bool EvaluateLoopConditionalExpressionPasses()
		{
			if (String.IsNullOrEmpty(LoopConditionalExpression))
			{
				return true;
			}
			ResolvedExpression resolvedExpression = Engine.ResolveExpression(LoopConditionalExpression, MyDataContext);
			if (resolvedExpression.HasException()){
				return false;
			}
			if (resolvedExpression.ResolvedType != ResolvedExpression.booleanType){
				return false;
			}
			
			return (bool)resolvedExpression.ResolvedValue;
		}



		string _repeatedDataKey = null;

		/// <summary>
		/// DataContext key used for repeater collection value
		/// </summary>
		public string RepeatedDataKey
		{
			get
			{
				return _repeatedDataKey;
			}
			set
			{
				_repeatedDataKey = CleanVariableKey(value);
			}
		}

		string _loopContextVariableKey = null;

		/// <summary>
		/// Context built-in variable key.
		/// </summary>
		public string LoopContextVariableKey
		{
			get
			{
				return _loopContextVariableKey;
			}
			set
			{
				_loopContextVariableKey = value;
			}
		}


		/// <summary>
		/// Content prior to each loop
		/// </summary>
		public string LoopPrequel { get; set; }

		/// <summary>
		/// Content after each loop
		/// </summary>
		public string LoopSequel { get; set; }


		public override void Render(System.IO.TextWriter writer)
		{

			IEnumerable simpleList = MyDataContext.GetEnumerableVariableObject(RepeatedDataKey);

			if (null == simpleList)
			{
				return;
			}

			//TODO - hunt down why repeat nested in templates not parsed.
			if (Controls.Count == 0 && MyOffset.ChildOffsets.Count > 0)
			{
				BuildControlTreeFromOffsets();
			}


			bool hasPrequel = (!string.IsNullOrEmpty(LoopPrequel));
			bool hasSequel = (!string.IsNullOrEmpty(LoopSequel));


			//System.Collections.Hashtable
			LoopContext context = new LoopContext(0, 0);

			//resolve the count
			if (simpleList is IList)
			{
				context.Count = ((IList)simpleList).Count;
			}
			else
			{
				//enumerate to count items
				int count = 0;
				foreach (object item in simpleList)
				{
					count++;
				}
				context.Count = count;
			}
			//saftey check on LoopContextVariableKey
			if (string.IsNullOrEmpty(LoopContextVariableKey))
			{
				LoopContextVariableKey = "Context";
			}


			foreach (object item in simpleList)
			{
				object realItem = item;
				if (item is DictionaryEntry)
				{
					realItem = ((DictionaryEntry)item).Value;
				}
				MyDataContext.RegisterLocalValue(this.LoopItemKey, realItem);

				if (MyDataContext.HasVariable(this.LoopContextVariableKey))
				{
					MyDataContext.RemoveLocalValue(this.LoopContextVariableKey);
				}
				MyDataContext.RegisterLocalValue(this.LoopContextVariableKey, context);

				//Evaluate any conditionals
				if (EvaluateLoopConditionalExpressionPasses())
				{
					if (hasPrequel)
					{
						writer.Write(MyDataContext.ResolveVariables(LoopPrequel));
					}

					RenderTemplateInstance(writer);

					if (hasSequel)
					{
						writer.Write(MyDataContext.ResolveVariables(LoopSequel));
					}
				}
				context.Index++;
				MyDataContext.RemoveLocalValue(this.LoopItemKey);
			}
			//remove the final context variable
			if (MyDataContext.HasVariable(this.LoopContextVariableKey))
			{
				MyDataContext.RemoveLocalValue(this.LoopContextVariableKey);
			}
		}



		/// <summary>
		/// Resolves template variables to current var and writes results
		/// for current item
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private void RenderTemplateInstance(System.IO.TextWriter writer)
		{
			if (Controls.Count > 0)
			{
				foreach (BaseGadgetControl control in Controls)
				{
					control.Render(writer);
				}
			}
		}

	}
}
