using System;
using System.Collections.Generic;
using System.Text;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false,Inherited = true)]
    public class Param : Attribute
    {
        public Param()
        {
        }
    }
}
