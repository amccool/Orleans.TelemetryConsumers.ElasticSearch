using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTester
{
    public abstract class OrleansTestingBase
    {
        protected static readonly Random random = new Random();

        protected static long GetRandomGrainId()
        {
            return random.Next();
        }
    }
}
