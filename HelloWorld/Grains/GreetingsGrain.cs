using Interfaces;
using Orleans.EventSourcing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans.Providers;

namespace Grains
{
    [LogConsistencyProvider(ProviderName = "StateStorage")]
    public class GreetingsGrain : JournaledGrain<GreetingState, GreetingEvent>, IGreetingsGrain
    {
        public async Task<string> SendGreetings(string greeting)
        {
            var state = State.Gretting;           

            RaiseEvent(new GreetingEvent { Gretting = greeting });

            await ConfirmEvents();

            return greeting;
        }
    }

    public class GreetingEvent
    {
        public string Gretting { get; set; }
    }

    public class GreetingState
    {
        public string Gretting { get; set; }

        public GreetingState Apply(GreetingEvent @event)
        {
            Gretting = @event.Gretting;

            return this;
        }
    }
}
