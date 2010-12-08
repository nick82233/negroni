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

using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;
using Negroni.OpenSocial.EL;


namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// Any element which as had a repeat attribute applied.
	/// </summary>
	[MarkupTag("os:If")]
	[AttributeTagAlternative("if")]
	public class OsIfTag : BaseContainerControl
	{
		public OsIfTag() { }

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			IfConditionExpression = GetAttribute("condition");
			IfConditionResult = null;
		}

		private string _ifConditionExpression = null;

		/// <summary>
		/// Actual expression language statement
		/// </summary>
		public string IfConditionExpression
		{
			get
			{
				return _ifConditionExpression;
			}
			set
			{
				_ifConditionExpression = value;
			}
		}


		private bool? _ifConditionResult = null;

		/// <summary>
		/// Accessor for ifConditionResult.
		/// Performs lazy load upon first request
		/// </summary>
		public bool? IfConditionResult
		{
			get
			{
				if (!_ifConditionResult.HasValue)
				{
					_ifConditionResult = EvaluateIfCondition();
				}
				return _ifConditionResult;
			}
			set
			{
				_ifConditionResult = value;
			}
		}

		/// <summary>
		/// Evaluates the current IfConditionExpression.
		/// Returns the result
		/// </summary>
		/// <returns></returns>
		protected bool EvaluateIfCondition()
		{
			if (String.IsNullOrEmpty(IfConditionExpression))
			{
				return false;
			}

			ResolvedExpression resolvedExpression = Engine.ResolveExpression(IfConditionExpression, MyDataContext);
			
			if (resolvedExpression.HasException()){
				return false;
			}
			if (resolvedExpression.ResolvedType != ResolvedExpression.booleanType){
				return false;
			}
			else{
				return (bool)resolvedExpression.ResolvedValue;
			}
		}


		public override void Render(System.IO.TextWriter writer)
		{
			IfConditionResult = EvaluateIfCondition();
			if (IfConditionResult == true)
			{
				base.Render(writer);
			}
		}



	}
}
