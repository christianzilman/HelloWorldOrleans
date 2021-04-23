using System;

namespace SiloHostClustering.Context
{
    public interface IOrleansRequestContext
    {
        Guid TraceId { get; }
    }
}