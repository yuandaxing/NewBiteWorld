using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{
    public interface ISample
    {
        int GetSize();
        int[] GetStates();
        int[] GetObservation();
        List<string> GetObservationString();
        bool IsBoundary(int frame);
    }
}
