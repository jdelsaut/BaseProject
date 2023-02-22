using AutoMapper;
using BoxApi.Api.AccessToken;
using BoxApi.Api.Constants;
using BoxApi.Api.DTOs;
using BoxApi.Api.Exceptions;
using BoxApi.Application.Exceptions;
using BoxApi.Common.CustomSerializer;
using BoxApi.Infrastructure.HttpHelper;
using BoxApi.Infrastructure.Logging;
using BoxApi.Infrastructure.Settings;
using BoxApi.Infrastructure.SmartID;
using BoxApi.Persistance.Exceptions;
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

namespace BoxApi.Api.Functions
{
    public class BoxApiFunctions
    {
        private readonly IInsightsLogger _insightsLogger;
        private readonly ISettingsReader _settingsReader;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ISmartIDGenerator _smartIDGenerator;

        public BoxApiFunctions(IInsightsLogger insightsLogger, ISettingsReader settingsReader, IMapper mapper, IMediator mediator, ISmartIDGenerator smartIDGenerator)
        {
            _insightsLogger = insightsLogger;
            _settingsReader = settingsReader;
            _mapper = mapper;
            _mediator = mediator;
            _smartIDGenerator = smartIDGenerator;
        }

        [FunctionName("BoxApi_Starter")]
        public async Task<HttpResponseMessage> BoxApi_Starter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{BoxApi_type}/BoxApis")] HttpRequestMessage req,
            string BoxApi_type,
            [AccessToken] ClaimsPrincipal principal)
        {
            #region Starting Global Operation logging
            (var telemetryRequest, var operation) = _insightsLogger.StartOperation(
                new MicroserviceRequestTelemetry(
                Guid.NewGuid().ToString(),
                "/BoxApi_Starter",
                string.Empty,
                string.Empty)
                );
            #endregion

            try
            {
                var authorization = principal.GetAuthorizationInformation(nameof(BoxApiFunctions.BoxApi_Starter));

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

        [FunctionName("BoxApi_GET")]
        public async Task<HttpResponseMessage> GetBoxApiById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{BoxApi_type}/BoxApis/{BoxApiId}")]HttpRequestMessage req,
            string BoxApi_type,
            string BoxApiId,
            [AccessToken] ClaimsPrincipal principal)
        {
            (var telemetryRequest, var operation) = _insightsLogger.StartOperation(
                new MicroserviceRequestTelemetry(
                Guid.NewGuid().ToString(),
                "/GetBoxApiByid",
                string.Empty,
                string.Empty)
                );

            try
            {
                var authorization = principal.GetAuthorizationInformation(nameof(BoxApiFunctions.GetBoxApiById));
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
                return CreateErrorResponse(req, HttpStatusCode.InternalServerError, e, "BoxApi process failed due to an unexpected error.");
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
                    return HttpResponsesMessageContents.BoxApi_NOT_FOUND;

                case HttpStatusCode.InternalServerError:
                    return HttpResponsesMessageContents.InternalServerError;
                default:
                    return string.Empty;
            }
        }
    }
}
