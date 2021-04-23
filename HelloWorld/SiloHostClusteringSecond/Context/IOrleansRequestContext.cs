using System;

namespace SiloHostClusteringSecond.Context
{
    public interface IOrleansRequestContext
    {
        Guid TraceId { get; }
    }
}