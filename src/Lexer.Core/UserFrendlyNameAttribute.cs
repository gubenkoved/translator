using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer.Core
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class UserFrendlyNameAttribute : Attribute
    {
        public string Description { get; private set; }

        public UserFrendlyNameAttribute(string description)
        {
            Description = description;
        }
    }
}
