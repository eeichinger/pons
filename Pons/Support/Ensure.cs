using System;

namespace Pons.Util
{
    /// <summary>
    /// </summary>
    /// <author>Erich Eichinger</author>
    public static class Ensure
    {
        public static void IsTrue( bool condition, string message )
        {
            if (!condition)
            {
                throw new ArgumentException(message);
            }
        }

        public static void NotNull(object val, string argname)
        {
            if (val == null)
            {
                throw new ArgumentNullException(argname);
            }
        }

        public static void HasText(string val, string argname)
        {
            if (val == null || val.Length == 0 || val.Trim().Length == 0)
            {
                throw new ArgumentException(argname);
            }
        }

        public static void Range(double l, double r, double val, string argname)
        {
            if (val < l || val > r)
            {
                throw new ArgumentOutOfRangeException(argname, val, string.Format("Argument is not within the expected range of [{0},{1}]", l, r));
            }
        }

        public static void Range(long l, long r, long val, string argname)
        {
            if (val < l || val > r)
            {
                throw new ArgumentOutOfRangeException(argname, val, string.Format("Argument is not within the expected range of [{0},{1}]", l, r));
            }
        }
    }
}