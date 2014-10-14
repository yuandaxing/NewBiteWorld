using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.Common.test
{
    public class TestNewTuple
    {
        public static void Main(string[] args)
        {
            Dictionary<NewTuple<int, int>, int> dictionary = new Dictionary<NewTuple<int, int>, int>();
            Dictionary<NewTuple<string, string, string>, int> dict2 = new Dictionary<NewTuple<string, string, string>, int>();
            NewTuple<int, int> a = new NewTuple<int, int>(1, 2);
            NewTuple<int, int> b = new NewTuple<int, int>(1, 2);
            NewTuple<int, int> c = new NewTuple<int, int>(2, 3);
            dictionary[a] = 2;
            Console.WriteLine("NewTuple<int, int>(1, 2) is exist: {0}", dictionary.ContainsKey(b));
            Console.WriteLine("NewTuple<int, int>(2, 3) is exist: {0}", dictionary.ContainsKey(c));

            NewTuple<string, string, string> d = new NewTuple<string, string, string>("a", "b", "c");
            NewTuple<string, string, string> e = new NewTuple<string, string, string>("a", "b", "c");
            Console.WriteLine("d == e : {0}", d.Equals(e));
            

        }
    }
}
