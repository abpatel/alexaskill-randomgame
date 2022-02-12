using System;
using System.Collections.Generic;
using System.Text;

namespace NumberGameSkill
{
    class MagicNumberGenerator
    {
      
        public MagicNumberGenerator(int low, int high)
        {
            this.Low = low;
            this.High = high;
        }

        public int Low { get; private set; }
        public int High { get; private set; }
        public int Generate()
        {
            Random rnd = new Random();
            int magicNumber = rnd.Next(Low, High);
            return magicNumber;
        }
    }
}
