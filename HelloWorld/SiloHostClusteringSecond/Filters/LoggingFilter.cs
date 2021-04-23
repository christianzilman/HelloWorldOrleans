using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using SiloHostClusteringSecond.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiloHostClusteringSecond.Filters
{
    public class LoggingFilter : IIncomingGrainCallFilter
    {
        private readonly GrainInfo _grainInfo;
        private readonly ILogger<LoggingFilter> _logger;
        JsonSerializerSettings _jsonSerializerSettings;
        private readonly IOrleansRequestContext _orleansRequestContext;

        public LoggingFilter(GrainInfo grainInfo, 
                             ILogger<LoggingFilter> logger, 
                             JsonSerializerSettings jsonSerializerSettings,
                             IOrleansRequestContext orleansRequestContext)
        {
            _grainInfo = grainInfo;
            _logger = logger;
            _jsonSerializerSettings = jsonSerializerSettings;
            _orleansRequestContext = orleansRequestContext;
        }

        private bool ShouldLod(string methodName)
        {
            return _grainInfo.Methods.Contains(methodName);
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            try
            {
                if (ShouldLod(context.InterfaceMethod.Name))
                {
                    var arguments = JsonConvert.SerializeObject(context.Arguments, _jsonSerializerSettings);

                    _logger.LogInformation($"LOGGINGFILTER TraceId: {_orleansRequestContext.TraceId} {context.Grain.GetType()}.{context.InterfaceMethod.Name}: arguments: {arguments} request");
                }

                await context.Invoke();

                if (ShouldLod(context.InterfaceMethod.Name))
                {
                    var result = JsonConvert.SerializeObject(context.Result, _jsonSerializerSettings);

                    _logger.LogInformation($"LOGGINGFILTER TraceId: {_orleansRequestContext.TraceId} {context.Grain.GetType()}.{context.InterfaceMethod.Name}: result: {result} request");
                }
            }
            catch(Exception ex)
            {
                var arguments = JsonConvert.SerializeObject(context.Arguments, _jsonSerializerSettings);
                var result = JsonConvert.SerializeObject(context.Result, _jsonSerializerSettings);
                _logger.LogError($"LOGGINGFILTER TraceId: {_orleansRequestContext.TraceId} {context.Grain.GetType()}.{context.InterfaceMethod.Name}: threw an exception: {nameof(ex)} request", ex);

                throw;
            }
        }
    }
}
