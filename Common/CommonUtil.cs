using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.Common
{
    public class CommonUtil
    {
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
