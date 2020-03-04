using System;
using System.Collections.Generic;
using System.Text;

namespace DmvAppointmentScheduler
{
    public class TellerDuration
    {
        public int index;
        public double multiplier;
        public double duration;

        public TellerDuration() { }
        public TellerDuration(int index, Teller teller)
        {
            this.index = index;
            this.multiplier = Convert.ToDouble(teller.multiplier);
            this.duration = 0;
        }
    }

}
