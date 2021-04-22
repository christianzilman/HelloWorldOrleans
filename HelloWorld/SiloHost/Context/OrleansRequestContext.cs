using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace SiloHost.Context
{
    public class OrleansRequestContext : IOrleansRequestContext
    {
        public Guid TraceId => RequestContext.Get("traceId") == null ? Guid.Empty : (Guid)RequestContext.Get("traceId");
    }
}
