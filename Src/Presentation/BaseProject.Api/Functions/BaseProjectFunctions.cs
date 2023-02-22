using AutoMapper;
using BaseProject.Api.AccessToken;
using BaseProject.Api.Constants;
using BaseProject.Api.DTOs;
using BaseProject.Api.Exceptions;
using BaseProject.Application.Exceptions;
using BaseProject.Common.CustomSerializer;
using BaseProject.Infrastructure.HttpHelper;
using BaseProject.Infrastructure.Logging;
using BaseProject.Infrastructure.Settings;
using BaseProject.Infrastructure.SmartID;
using BaseProject.Persistance.Exceptions;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseProject.Api.Functions
{
    public class BaseProjectFunctions
    {
        private readonly IInsightsLogger _insightsLogger;
        private readonly ISettingsReader _settingsReader;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ISmartIDGenerator _smartIDGenerator;

        public BaseProjectFunctions(IInsightsLogger insightsLogger, ISettingsReader settingsReader, IMapper mapper, IMediator mediator, ISmartIDGenerator smartIDGenerator)
        {
            _insightsLogger = insightsLogger;
            _settingsReader = settingsReader;
            _mapper = mapper;
            _mediator = mediator;
            _smartIDGenerator = smartIDGenerator;
        }

        [FunctionName("BaseProject_Starter")]
        public async Task<HttpResponseMessage> BaseProject_Starter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{BaseProject_type}/BaseProjects")] HttpRequestMessage req,
            string BaseProject_type,
            [AccessToken] ClaimsPrincipal principal)
        {
            #region Starting Global Operation logging
            (var telemetryRequest, var operation) = _insightsLogger.StartOperation(
                new MicroserviceRequestTelemetry(
                Guid.NewGuid().ToString(),
                "/BaseProject_Starter",
                string.Empty,
                string.Empty)
                );
            #endregion

            try
            {
                var authorization = principal.GetAuthorizationInformation(nameof(BaseProjectFunctions.BaseProject_Starter));

                if (authorization.Key != HttpStatusCode.OK)
                {
                    return CreateErrorResponse(req, authorization.Key, null, authorization.Value);
                }
               
                telemetryRequest.Success = true;
                telemetryRequest.ResponseCode = HttpStatusCode.Accepted.GetHashCode().ToString();

                return req.CreateResponse(HttpStatusCode.Accepted);
            }
            catch (BadRequestException e)
            {
                telemetryRequest.Success = false;
                _insightsLogger.TrackException(e);
                return CreateErrorResponse(req, HttpStatusCode.BadRequest, new Exception());
            }
            catch (ValidationException e)
            {
                telemetryRequest.Success = false;
                _insightsLogger.TrackException(e);
                return CreateErrorResponse(req, HttpStatusCode.BadRequest, e);
            }
            catch (Exception e)
            {
                telemetryRequest.Success = false;
                _insightsLogger.TrackException(e);
                return CreateErrorResponse(req, HttpStatusCode.InternalServerError, e);
            }
            finally
            {
                _insightsLogger.StopOperation(operation);
            }


        }

        [FunctionName("BaseProject_GET")]
        public async Task<HttpResponseMessage> GetBaseProjectById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{BaseProject_type}/BaseProjects/{BaseProjectId}")]HttpRequestMessage req,
            string BaseProject_type,
            string BaseProjectId,
            [AccessToken] ClaimsPrincipal principal)
        {
            (var telemetryRequest, var operation) = _insightsLogger.StartOperation(
                new MicroserviceRequestTelemetry(
                Guid.NewGuid().ToString(),
                "/GetBaseProjectByid",
                string.Empty,
                string.Empty)
                );

            try
            {
                var authorization = principal.GetAuthorizationInformation(nameof(BaseProjectFunctions.GetBaseProjectById));
                if (authorization.Key != HttpStatusCode.OK)
                {
                    return CreateErrorResponse(req, authorization.Key, null, authorization.Value);
                }


                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (NotFoundException e)
            {
                _insightsLogger.TrackException(e);
                telemetryRequest.Success = false;
                return CreateErrorResponse(req, HttpStatusCode.NotFound, e);
            }
            catch (ValidationException e)
            {
                _insightsLogger.TrackException(e);
                telemetryRequest.Success = false;
                return CreateErrorResponse(req, HttpStatusCode.BadRequest, e);
            }
            catch (Exception e)
            {
                _insightsLogger.TrackException(e);
                telemetryRequest.Success = false;
                return CreateErrorResponse(req, HttpStatusCode.InternalServerError, e, "BaseProject process failed due to an unexpected error.");
            }
            finally
            {
                _insightsLogger.StopOperation(operation);
            }
        }

        private static HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, HttpStatusCode statusCode, Exception exception, string value = null)
        {
            var _exception = string.IsNullOrEmpty(value) ? exception : new Exception(value);
            return request.CreateResponse(statusCode, RetrieveErrorMessageFromException(_exception, statusCode));
        }

        private static ErrorDTO RetrieveErrorMessageFromException(Exception e, HttpStatusCode statusCode)
        {
            var StatusCodeValue = statusCode.GetHashCode().ToString();

            if (e.GetType() == typeof(ValidationException))
            {
                return new ErrorDTO(GetErrorLabelByStatusCode(statusCode), e.ToString(), StatusCodeValue);
            }

            return new ErrorDTO(GetErrorLabelByStatusCode(statusCode), e.GetExceptionMessage(), StatusCodeValue);
        }

        private static string GetErrorLabelByStatusCode(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.BadRequest:
                    return HttpResponsesMessageContents.BadRequest;

                case HttpStatusCode.NotFound:
                    return HttpResponsesMessageContents.BaseProject_NOT_FOUND;

                case HttpStatusCode.InternalServerError:
                    return HttpResponsesMessageContents.InternalServerError;
                default:
                    return string.Empty;
            }
        }
    }
}
