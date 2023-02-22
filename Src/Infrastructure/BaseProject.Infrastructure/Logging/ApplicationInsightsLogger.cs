using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using BaseProject.Infrastructure.Settings;
using System.Collections.Generic;
using System.Diagnostics;

namespace BaseProject.Infrastructure.Logging
{
    public class ApplicationInsightsLogger : IInsightsLogger
    {
        private readonly TelemetryClient telemetryClient = new TelemetryClient();
        private readonly ISettingsReader settingsReader;

        public ApplicationInsightsLogger(ISettingsReader settingsReader)
        {
            telemetryClient.InstrumentationKey = settingsReader.ReadSetting(AppSettingsKeys.AppInsightsInstrumentationKey);
            this.settingsReader = settingsReader;
        }


        public (RequestTelemetry, IOperationHolder<RequestTelemetry>) StartOperation(MicroserviceRequestTelemetry microserviceRequestTelemetry)
        {
            if (microserviceRequestTelemetry == null)
            {
                telemetryClient.TrackException(new ArgumentNullException(nameof(microserviceRequestTelemetry)));
                return (null, null);
            }

            var requestTelemetry = new RequestTelemetry
            {
                Name = microserviceRequestTelemetry.Name
            };

            // Get the operation ID from the Request-Id (if you follow the HTTP Protocol for Correlation).
            requestTelemetry.Context.Operation.Id = microserviceRequestTelemetry?.RootId;
            requestTelemetry.Context.Operation.ParentId = microserviceRequestTelemetry?.ParentId;

            // StartOperation is a helper method that allows correlation of 
            // current operations with nested operations/telemetry
            // and initializes start time and duration on telemetry items.
            var operation = telemetryClient.StartOperation(requestTelemetry);
            return (requestTelemetry, operation);
        }

        public void StopOperation(IOperationHolder<RequestTelemetry> operation)
        {
            telemetryClient.StopOperation<RequestTelemetry>(operation);
            telemetryClient.Flush();
        }

        public void TrackMicroserviceEvent(MicroServiceEventLog microserviceEvent, IDictionary<string, double> metrics = null)
        {
            telemetryClient.TrackEvent(microserviceEvent.Name, microserviceEvent, metrics);
        }

        public void TrackException(Exception e)
        {
            telemetryClient.TrackException(e);
        }

        public void TrackTrace(string message, SeverityLevel level)
        {
            telemetryClient.TrackTrace(message, level);
        }

        public void TrackMicroserviceCosmosDbThrottlingEvent(string BaseProjectId, string functionName, IDictionary<string, double> metrics = null)
        {
            this.TrackMicroserviceEvent(
                        new MicroServiceEventLog(
                            EventNames.COSMOSDB_REQUEST_THROTTLED,
                            functionName,
                            BaseProjectId,
                            "Critical",
                            "Request rate on ComsmosDB is getting too high, try increasing throughput",
                            settingsReader));
        }
    }
}
