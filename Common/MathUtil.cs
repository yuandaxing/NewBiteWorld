using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.Common
{
    public class MathUtil
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        public static int Max(params int[] array)
        {
            int max = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (max < array[i])
                    max = array[i];
            }
            return max;
        }
    }
}
