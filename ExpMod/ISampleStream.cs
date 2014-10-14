using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{
    public interface ISampleStream
    {
        void Randomize();
        ISample Next();
        void Rewind();
        int GetSize();
    }
}
