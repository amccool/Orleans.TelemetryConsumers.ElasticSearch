using System;
using System.Threading.Tasks;
using Orleans;

namespace TestGrainInterfaces
{
    public interface ISimpleGrain : IGrainWithStringKey
    {
        Task DoSomething();

        Task DoSomethingElse();
    }
}
