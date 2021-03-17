using System;

namespace MTSTCKWrapper.Helpers
{
    public static class Operation
    {
        public static Func<T, T, bool> Greater<T>()
            where T : IComparable<T>
        {
            return delegate (T lhs, T rhs) { return lhs.CompareTo(rhs) > 0; };
        }

        public static Func<T, T, bool> Less<T>()
            where T : IComparable<T>
        {
            return delegate (T lhs, T rhs) { return lhs.CompareTo(rhs) < 0; };
        }

        public static Func<T, T, bool> GreaterOrEquel<T>()
        where T : IComparable<T>
        {
            return delegate (T lhs, T rhs) { return lhs.CompareTo(rhs) >= 0; };
        }

        public static Func<T, T, bool> LessOrEquel<T>()
            where T : IComparable<T>
        {
            return delegate (T lhs, T rhs) { return lhs.CompareTo(rhs) <= 0; };
        }
    }
}
