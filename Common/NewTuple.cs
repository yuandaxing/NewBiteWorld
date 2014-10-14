using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.Common
{
    public class NewTuple<Type1>
    {
        public Type1 v1;
        public NewTuple(Type1 v1)
        {
            this.v1 = v1;
        }
        public override bool Equals(object obj)
        {
            NewTuple<Type1> other = obj as NewTuple<Type1>;
            if (other != null)
            {
                return this.v1.Equals(other.v1);
            }
            else
            {
                return base.Equals(obj);
            }
        }
        public override int GetHashCode()
        {
            return v1.GetHashCode();
        }
    }
    public class NewTuple<Type1, Type2>
    {
        public Type1 v1;
        public Type2 v2;
        public NewTuple(Type1 v1, Type2 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
        public override bool Equals(object obj)
        {
            NewTuple<Type1, Type2> other = obj as NewTuple<Type1, Type2>;
            if (other != null)
            {
                return this.v1.Equals(other.v1) && this.v2.Equals(other.v2);
            }
            else
            {
                return base.Equals(obj);
            }
        }
        public override int GetHashCode()
        {
            return v1.GetHashCode() * 31 + v1.GetHashCode() * v2.GetHashCode();
            //return v1.GetHashCode() ^ v2.GetHashCode();
        }
    }
    public class NewTuple<Type1, Type2, Type3>
    {
        public Type1 v1;
        public Type2 v2;
        public Type3 v3;
        public NewTuple(Type1 v1, Type2 v2, Type3 v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
        public override bool Equals(object obj)
        {
            NewTuple<Type1, Type2, Type3> other = obj as NewTuple<Type1, Type2, Type3>;
            if (other != null)
            {
                return this.v1.Equals(other.v1) && this.v2.Equals(other.v2)
                    && this.v3.Equals(other.v3); ;
            }
            else
            {
                return base.Equals(obj);
            }
        }
        public override int GetHashCode()
        {
            return v1.GetHashCode() * 31 + v2.GetHashCode() * 23 +
                v1.GetHashCode() * v2.GetHashCode() * v3.GetHashCode();
            //return v1.GetHashCode() ^ v2.GetHashCode() ^ v3.GetHashCode();
        }
    }
   
}
