// Copyright (c) 2009 http://grommet.codeplex.com
// This source is subject to the Microsoft Public License.
// See http://www.opensource.org/licenses/ms-pl.html
// All other rights reserved.

using System;
using Microsoft.SPOT;

namespace Grommet.Ext
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return (s == null || s.Length == 0);
        }

        public static bool StartsWith(this string s, string value)
        {
            return s.IndexOf(value) == 0;
        }

        public static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) > 0;
        }

        public static float TotalSeconds(this TimeSpan ts)
        {
            return ((float)ts.Ticks) / TimeSpan.TicksPerSecond;
        }

        public static float TotalMilliseconds(this TimeSpan ts)
        {
            return ((float)ts.Ticks) / TimeSpan.TicksPerMillisecond;
        }

        public static float TotalMilliseconds(this DateTime dt)
        {
            return ((float)dt.Ticks) / TimeSpan.TicksPerMillisecond;
        }
    }
}
