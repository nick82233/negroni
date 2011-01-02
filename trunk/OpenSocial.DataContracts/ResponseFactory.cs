/* *********************************************************************
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
using System.Linq;
using System.Text;

namespace Negroni.OpenSocial.DataContracts
{
    /// <summary>
    /// Creates and initializes new response instances of various types
    /// </summary>
    public static class ResponseFactory
    {

        static public GenericRestResponse<List<Person>> CreatePeopleResponse()
        {
            GenericRestResponse<List<Person>> retval = new GenericRestResponse<List<Person>>();
            retval.Entry = new List<Person>();
            return retval;
        }


    }
}
