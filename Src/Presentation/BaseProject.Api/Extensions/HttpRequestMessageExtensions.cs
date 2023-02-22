using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using BoxApi.Api.Constants;
using BoxApi.Infrastructure.Logging;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using BoxApi.Application.Exceptions;

namespace BoxApi.Api.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpResponseMessage CreateApiResponse<T>(
           this HttpRequestMessage request,
           T value)
        {
            return request.CreateApiResponse(HttpStatusCode.OK, value);
        }

        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static HttpResponseMessage CreateApiResponse<T>(
           this HttpRequestMessage request,
           HttpStatusCode statusCode,
           T value)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = new HttpResponseMessage(statusCode)
            {
                RequestMessage = request,
            };

            if (typeof(T) == typeof(string))
            {
                response.Content = new StringContent(value as string, Encoding.UTF8);
            }
            else
            {
                response.Content = new StringContent(JsonConvert.SerializeObject(value, _serializerSettings), Encoding.UTF8, "application/json");
            }

            return response;
        }

        public static HttpResponseMessage CreateApiErrorResponse(
            this HttpRequestMessage httpRequestMessage,
            string error,
            string errorDescription,
            HttpStatusCode httpStatusCode)
            => httpRequestMessage.CreateApiResponse(httpStatusCode, new ApiErrorResponseDTO(error, errorDescription, httpStatusCode));

        public static HttpResponseMessage CreateApiErrorResponse(
            this HttpRequestMessage req,
            Exception exception,
            IInsightsLogger insightsLogger)
        {
            insightsLogger.TrackException(exception);

            return exception switch
            {
                JsonException e => req.CreateApiErrorResponse(HttpResponsesMessageContents.BadRequest, $"The input JSON format is incorrect : {e.Message}", HttpStatusCode.BadRequest),
                ValidationException e => req.CreateApiErrorResponse(HttpResponsesMessageContents.BadRequest, e.Message, HttpStatusCode.BadRequest),
                FormatException e => req.CreateApiErrorResponse(HttpResponsesMessageContents.BadRequest, "'id' expected format is not correct.", HttpStatusCode.BadRequest),
                Exception _ => req.CreateApiErrorResponse(HttpResponsesMessageContents.InternalServerError, "The request failed due to an unexpected service error.", HttpStatusCode.InternalServerError)
            };
        }
    }
}
