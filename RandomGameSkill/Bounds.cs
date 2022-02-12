using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGameSkill
{
    class Bounds
    {
        public Bounds(int low, int high)
        {
            this.Low = low;
            this.High = high;
        }
        public int Low { get;  private set; }
        public int High { get; private set; }
    }
}
