using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.Common
{
    public class ArrayUtil
    {
        public static void Fill<T>(T[] array, T value)
        {
            Fill(array, value, 0, array.Length);
        }
        public static void Fill<T>(T[] array, T value, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                array[i] = value;
            }
        }

    }
}
