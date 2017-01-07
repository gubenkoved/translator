using System.Collections.Generic;

namespace Lexer.Core
{
    public class StringList : List<string>
    {
        public string Description { get; private set; }

        public StringList(string description = null)
        {
            Description = description;
        }

         public int Find(string str)
         {
             for (int i = 0; i < Count; i++)
             {
                 if (this[i] == str)
                     return i;
             }

             return -1;
         }
    }
}