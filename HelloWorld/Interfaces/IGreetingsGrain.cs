using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IGreetingsGrain: IGrainWithIntegerKey
    {
        Task<string> SendGreetings(string greeting);
    }
}
