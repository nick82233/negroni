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
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Generic IExperssionEvaluator object wrapper that exposes
	/// property values to the Expression Language
	/// </summary>
	public class GenericExpressionEvalWrapper : IExpressionEvaluator
	{
		internal static void ClearRegisteredTypes()
		{
			registeredDataContracts.Clear();
			registeredDataObjects.Clear();
		}

		/// <summary>
		/// DataContract-based data object types registered to allow DataMember Fields invokation
		/// </summary>
		static Dictionary<Type, Dictionary<string, FieldInfo>> registeredDataContracts = null;

		/// <summary>
		/// Data object types registered to allow invokation of Property values
		/// </summary>
		static Dictionary<Type, Dictionary<string, PropertyInfo>> registeredDataObjects = null;


		static GenericExpressionEvalWrapper()
		{
			registeredDataContracts = new Dictionary<Type, Dictionary<string, FieldInfo>>();
			registeredDataObjects = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
		}

		/// <summary>
		/// Registers a DataContract as a valid type for Generic Expression evalution
		/// </summary>
		/// <param name="contractType"></param>
		public static Dictionary<string, FieldInfo> GetDataContractEvaluator(Type contractType)
		{
			if (null == contractType) return null;
			if (!registeredDataContracts.ContainsKey(contractType))
			{
				lock (registeredDataContracts)
				{
					//check again now that we've aquired the lock
					if (!registeredDataContracts.ContainsKey(contractType))
					{
						registeredDataContracts.Add(contractType, WrapperUtility.LoadKeyInvokers(contractType));
					}
				}
			}
			return registeredDataContracts[contractType];
		}
		/// <summary>
		/// Registers a Property-based data object type as valid for Generic Expression evalution
		/// </summary>
		/// <param name="contractType"></param>
		public static Dictionary<string, PropertyInfo> GetDataPropertyEvaluator(Type contractType)
		{
			if (null == contractType) return null;
			if (!registeredDataObjects.ContainsKey(contractType))
			{
				lock (registeredDataObjects)
				{
					if (!registeredDataObjects.ContainsKey(contractType))
					{
						registeredDataObjects.Add(contractType, WrapperUtility.LoadPropertyInvokers(contractType));
					}
				}
			}

			return registeredDataObjects[contractType];
		}


		public GenericExpressionEvalWrapper() { }

		/// <summary>
		/// Loads the object.
		/// </summary>
		/// <param name="dataObject"></param>
		public GenericExpressionEvalWrapper(object dataObject)
		{
			if (dataObject == null)
			{
				return;
			}
			MyWrappedObject = dataObject;
			Type t = dataObject.GetType();
			if(registeredDataContracts.ContainsKey(t)){
				isDataContract = true;
			}
			else if(registeredDataObjects.ContainsKey(t)){
				isDataContract = false;
			}
			else{			
				object[] attrs = t.GetCustomAttributes(typeof(DataContractAttribute), true);
				isDataContract = attrs.Length > 0;
			}
		}

		public GenericExpressionEvalWrapper(object dataObject, bool isDataContract)
		{
			MyWrappedObject = dataObject;
			this.isDataContract = isDataContract;
		}

		/// <summary>
		/// Object being wrapped
		/// </summary>
		public object MyWrappedObject
		{
			get;
			set;
		}

		private bool isDataContract = true;


		#region IExpressionEvaluator Members

		public object ResolveExpressionValue(string expression)
		{
			if (MyWrappedObject == null)
			{
				return null;
			}
			if (isDataContract)
			{
				return WrapperUtility.ResolveExpressionValue(expression, MyWrappedObject, GetDataContractEvaluator(MyWrappedObject.GetType()));
			}
			else
			{
				return WrapperUtility.ResolveExpressionValue(expression, MyWrappedObject, GetDataPropertyEvaluator(MyWrappedObject.GetType()));
			}
		}

		#endregion
	}
}
