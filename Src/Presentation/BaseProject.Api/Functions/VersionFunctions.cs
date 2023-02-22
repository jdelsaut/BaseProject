
using BoxApi.Api.AccessToken;
using BoxApi.Api.Messages;
using BoxApi.Infrastructure.Logging;
using BoxApi.Infrastructure.Settings;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BoxApi.Api.Functions
{
    public class VersionFunctions
    {
        private readonly IInsightsLogger _insightsLogger;
        private readonly ISettingsReader _settingsReader;

        public VersionFunctions(ISettingsReader settingsReader, IInsightsLogger insightsLogger)
        {
            _insightsLogger = insightsLogger;
            _settingsReader = settingsReader;
        }

        [FunctionName("version")]
        public async Task<HttpResponseMessage> Version(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req)
        {
            var (requestTelemetry, operation) = _insightsLogger.StartOperation(new MicroserviceRequestTelemetry() { Name = "/api/version" });

            try
            {
                var apireponseVersion = await Task.FromResult(new ApiVersionResponse()
                {
                    Version = _settingsReader.ReadSetting(AppSettingsKeys.ApiVersion),
                    Description = _settingsReader.ReadSetting(AppSettingsKeys.VersionDescription)
                });

                requestTelemetry.Success = true;
                return new HttpResponseMessage(HttpStatusCode.OK);
                //return req.CreateResponse(HttpStatusCode.OK, apireponseVersion);
            }
            catch (Exception e)
            {
                requestTelemetry.Success = false;
                _insightsLogger.TrackException(e);
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
            finally
            {
                _insightsLogger.StopOperation(operation);
            }
        }
    }
}
