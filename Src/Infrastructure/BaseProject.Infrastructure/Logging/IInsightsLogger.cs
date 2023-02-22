using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BoxApi.Infrastructure.Logging
{
    public interface IInsightsLogger
    {
        (RequestTelemetry, IOperationHolder<RequestTelemetry>) StartOperation(MicroserviceRequestTelemetry microserviceRequestTelemetry);
        void StopOperation(IOperationHolder<RequestTelemetry> operation);
        void TrackException(Exception e);
        void TrackMicroserviceEvent(MicroServiceEventLog microserviceEvent, IDictionary<string, double> metrics = null);
        void TrackMicroserviceCosmosDbThrottlingEvent(string BoxApiId, string functionName, IDictionary<string, double> metrics = null);
        void TrackTrace(string message, SeverityLevel level);
    }
}
