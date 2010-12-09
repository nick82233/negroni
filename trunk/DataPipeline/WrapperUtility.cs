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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Reflection;
using System.Text;

namespace Negroni.DataPipeline
{
    /// <summary>
    /// Utility methods for initializing DataContract wrappers.
    /// </summary>
    public static class WrapperUtility
    {


        /// <summary>
        /// Converts a DataContract-based object from one type to another.
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static object TransformObject(object sourceObject, Type from, Type to)
        {
            DataContractJsonSerializer fromSer = new DataContractJsonSerializer(from);
            DataContractJsonSerializer toSer = new DataContractJsonSerializer(to);

            object result = null;
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    fromSer.WriteObject(stream, sourceObject);
                    stream.Seek(0, SeekOrigin.Begin);
                    result = toSer.ReadObject(stream);
                }
                catch { }
            }

            return result;
        }


        /// <summary>
        /// Interrogates a DataContract and returns a dictionary of all field members
        /// exposed for use in the expression language.
        /// </summary>
        /// <remarks>This only loads "Field" values (not properties) that are
        /// tagged with the DataMember attribute.</remarks>
        /// <returns></returns>
        public static Dictionary<string, FieldInfo> LoadKeyInvokers(Type dataContract)
        {
            Dictionary<string, FieldInfo> invokers = new Dictionary<string, FieldInfo>();

            if (null == dataContract)
            {
                return invokers;
            }

            FieldInfo[] fields = dataContract.GetFields();

            for (int i = 0; i < fields.Length; i++)
            {
                object[] attrs = fields[i].GetCustomAttributes(typeof(DataMemberAttribute), true);
                if (attrs.Length > 0)
                {
                    DataMemberAttribute fieldDef = attrs[0] as DataMemberAttribute;
                    if (fieldDef == null) continue; //shouldn't need this line

                    invokers.Add(fieldDef.Name, fields[i]);
                }
            }

            return invokers;
        }

        /// <summary>
        /// Interrogates a DataContract and returns a dictionary of all field members
        /// exposed for use in the expression language
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> LoadPropertyInvokers(Type dataContract)
        {
            Dictionary<string, PropertyInfo> invokers = new Dictionary<string, PropertyInfo>();

            if (null == dataContract)
            {
                return invokers;
            }

            PropertyInfo[] fields = dataContract.GetProperties();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].CanRead)
                {
                    object[] attrs = fields[i].GetCustomAttributes(typeof(DataMemberAttribute), true);
                    DataMemberAttribute fieldDef = null;
                    if (attrs.Length > 0)
                    {
                        fieldDef = attrs[0] as DataMemberAttribute;
                    }
                    if (fieldDef != null && !string.IsNullOrEmpty(fieldDef.Name))
                    {
                        invokers.Add(fieldDef.Name, fields[i]);
                    }
                    else
                    {
                        invokers.Add(fields[i].Name, fields[i]);
                    }
                }
            }

            return invokers;
        }


        /// <summary>
        /// Resolves an expression for the supplied object
        /// </summary>
        /// <param name="expression">string expression to evaluate</param>
        /// <param name="dataObject">DataContract object (or wrapper)</param>
        /// <param name="fieldInvokers"></param>
        /// <returns></returns>
        static public object ResolveExpressionValue(string expression, object dataObject, Dictionary<string, FieldInfo> fieldInvokers)
        {
            if (null == dataObject)
            {
                return null;
            }

            if (fieldInvokers.ContainsKey(expression))
            {
                //fieldInvokers[expression].
                FieldInfo f = fieldInvokers[expression];
                return f.GetValue(dataObject);
            }
            return null;
        }

        /// <summary>
        /// Resolves an expression for the supplied object
        /// </summary>
        /// <param name="expression">string expression to evaluate</param>
        /// <param name="dataObject">DataContract object (or wrapper)</param>
        /// <param name="fieldInvokers"></param>
        /// <returns></returns>
        static public object ResolveExpressionValue(string expression, object dataObject, Dictionary<string, PropertyInfo> propertyInvokers)
        {
            if (null == dataObject)
            {
                return null;
            }

            if (propertyInvokers.ContainsKey(expression))
            {
                PropertyInfo p = propertyInvokers[expression];
                return p.GetValue(dataObject, null);
            }
            return null;
        }

    }
}
