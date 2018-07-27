using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using TestGrainInterfaces;

namespace TestGrains
{
    public class SimpleGrain : Grain, ISimpleGrain
    {
        public async Task DoSomething()
        {
            this.GetLogger().Info("xxxxxxxxxxxxxxxxxx");
        }

        public async Task DoSomethingElse()
        {
            var logger = this.GetLogger();

            logger.Error(100, "sfgsgsgsgsg", new Exception());
        }
    }
}
