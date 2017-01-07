using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Core
{
    public static class Extension
    {
        public static void ReverseForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable.Reverse())
            {
                action.Invoke(item);
            }
        }
    }

    public class LambdaEqualityComparer<T> : EqualityComparer<T>
    {
        public Func<T, T, bool> EqualsPredicate { get; private set; }
        public Func<T, int> GetHashFunction { get; private set; }

        public LambdaEqualityComparer(Func<T, T, bool> equalPredicate, Func<T, int> getHashFunc)
        {            
            EqualsPredicate = equalPredicate;
            GetHashFunction = getHashFunc;
        }

        public override bool Equals(T x, T y)
        {
            return EqualsPredicate.Invoke(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return GetHashFunction.Invoke(obj);
        }
    }
}
