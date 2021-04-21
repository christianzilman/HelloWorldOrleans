using Interfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider]
    public class HelloGrain : Grain<GreetingArchive>, IHello
    {
        public override Task OnActivateAsync()
        {
            Console.WriteLine("OnActivate is called");

            return base.OnActivateAsync();
        }
        public async Task<string> SayHello(string greeting)
        {
            State.Greetings.Add(greeting);

            await WriteStateAsync();

            var key = this.GetPrimaryKeyLong();
            //this.DeactivateOnIdle();
            return $"You said: {greeting}, I say: hello. the key  {key}";
        }

        public override Task OnDeactivateAsync()
        {
            Console.WriteLine("OnDeactivate is called");

            return base.OnDeactivateAsync();
        }
    }

    public class GreetingArchive
    {
        public List<string> Greetings { get; private set; } = new List<string>();
    }
}
