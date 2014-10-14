using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{
    public class Ln
    {
        public static readonly double lnZero = -(double.MaxValue / 4);
        //public static readonly double lnZero = 0.0 ;
        public static readonly double epsilon = 1e-7;
        public static readonly double lnOfHalf = -Math.Log(2.0);

        public static double Mult(double lnx1, double lnx2)
        {
            if (lnx1 <= lnZero || lnx2 <= lnZero)
                return lnZero;
            double r = lnx1 + lnx2;
            if (r <= lnZero)
                return lnZero;
            return r;
        }

        public static double Div(double lnx1, double lnx2) { return Mult(lnx1, -lnx2); }

        public static double Exp(double x1)
        {
            if (x1 < lnZero)
                return 0;
            return Math.Exp(x1);
        }
        public static double LogValue(double x1)
        {
            if (x1 == 0)
                return lnZero;
            double r = Math.Log(x1);
            if (r < lnZero)
                return lnZero;
            return r;
        }

        public static double Add(double lnx1, double lnx2)
        {
            if (lnx1 < lnx2)
            {
                double temp = lnx1;
                lnx1 = lnx2;
                lnx2 = temp;
            }

            if (lnx1 <= lnZero)
                return lnZero;
            if (lnx2 <= lnZero)
                return lnx1;
            return lnx1 + LogValue(1 + Exp(Div(lnx2, lnx1)));
        }

        public static double MultInPlace(ref double lnx1, double lnx2)
        {
            lnx1 = Mult(lnx1, lnx2);
            return lnx1;
        }
        public static double AddInPlace(ref double lnx1, double lnx2)
        {
            lnx1 = Add(lnx1, lnx2);
            return lnx1;
        }
        public static double DivInPlace(ref double lnx1, double lnx2)
        {
            lnx1 = Div(lnx1, lnx2);
            return lnx1;
        }
    }
}
