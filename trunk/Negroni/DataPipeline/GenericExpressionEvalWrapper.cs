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
            registeredDataFieldInvokers.Clear();
            registeredDataPropertyInvokers.Clear();
        }

        /// <summary>
        /// DataContract-based data object types registered to allow DataMember Fields invokation
        /// </summary>
        static Dictionary<Type, Dictionary<string, FieldInfo>> registeredDataFieldInvokers = null;

        /// <summary>
        /// Data object types registered to allow invokation of Property values
        /// </summary>
        static Dictionary<Type, Dictionary<string, PropertyInfo>> registeredDataPropertyInvokers = null;


        static GenericExpressionEvalWrapper()
        {
            registeredDataFieldInvokers = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            registeredDataPropertyInvokers = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        }

        /// <summary>
        /// Registers a DataContract as a valid type for Generic Expression evalution
        /// </summary>
        /// <param name="contractType"></param>
		public static Dictionary<string, FieldInfo> GetDataFieldEvaluator(Type contractType)
		{
			if (null == contractType) return null;
            if (!registeredDataFieldInvokers.ContainsKey(contractType))
            {
                lock (registeredDataFieldInvokers)
                {
                    //check again now that we've aquired the lock
                    if (!registeredDataFieldInvokers.ContainsKey(contractType))
                    {
						registeredDataFieldInvokers.Add(contractType, WrapperUtility.LoadFieldInvokers(contractType));
					}
                }
            }
            return registeredDataFieldInvokers[contractType];
        }
        /// <summary>
        /// Registers a Property-based data object type as valid for Generic Expression evalution
        /// </summary>
        /// <param name="contractType"></param>
        public static Dictionary<string, PropertyInfo> GetDataPropertyEvaluator(Type contractType)
        {
            if (null == contractType) return null;
            if (!registeredDataPropertyInvokers.ContainsKey(contractType))
            {
                lock (registeredDataPropertyInvokers)
                {
                    if (!registeredDataPropertyInvokers.ContainsKey(contractType))
                    {
                        registeredDataPropertyInvokers.Add(contractType, WrapperUtility.LoadPropertyInvokers(contractType));
                    }
                }
            }

            return registeredDataPropertyInvokers[contractType];
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
        }

        [Obsolete("No longer differentiating between field-based and property-based objects")]
        public GenericExpressionEvalWrapper(object dataObject, bool isDataContract)
        {
            MyWrappedObject = dataObject;
        }

        /// <summary>
        /// Object being wrapped
        /// </summary>
        public object MyWrappedObject
        {
            get;
            set;
        }

        #region IExpressionEvaluator Members

        public object ResolveExpressionValue(string expression)
        {
            if (MyWrappedObject == null)
            {
                return null;
            }
			Dictionary<string, PropertyInfo> propertyEvaluators = GetDataPropertyEvaluator(MyWrappedObject.GetType());
			if (propertyEvaluators.ContainsKey(expression))
			{
				return WrapperUtility.ResolveExpressionValue(expression, MyWrappedObject, propertyEvaluators);
			}
			else
			{
				Dictionary<string, FieldInfo> fieldEvaluators = GetDataFieldEvaluator(MyWrappedObject.GetType());
				return WrapperUtility.ResolveExpressionValue(expression, MyWrappedObject, fieldEvaluators);
			}
		}

        #endregion
    }
}
