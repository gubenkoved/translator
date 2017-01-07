using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer.Core
{
    public static class Extensions
    {
        public static string Escape(this string str)
        {
            string tmp = str;

            tmp = tmp.Replace("\r", "\\r");
            tmp = tmp.Replace("\n", "\\n");

            return tmp;
        }

         public static int GetPosition(this Token token, StringList list)
         {
             return list.Find(token.Value);
         }

         public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
         {
             foreach (T item in enumerable)
             {
                 action.Invoke(item);
             }

             return enumerable;
         }

         public static string SubstringFromTo(this string s, int from, int to)
         {
             return s.Substring(from, to - from + 1);
         }
    }
}