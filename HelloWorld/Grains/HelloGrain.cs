using Interfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    public class HelloGrain : Grain, IHello
    {
        public override Task OnActivateAsync()
        {
            Console.WriteLine("OnActivate is called");

            return base.OnActivateAsync();
        }
        public Task<string> SayHello(string greeting)
        {
            var key = this.GetPrimaryKey();
            //this.DeactivateOnIdle();
            return Task.FromResult($"You said: {greeting}, I say: hello. the key  {key}");
        }

        public override Task OnDeactivateAsync()
        {
            Console.WriteLine("OnDeactivate is called");

            return base.OnDeactivateAsync();
        }
    }
}
