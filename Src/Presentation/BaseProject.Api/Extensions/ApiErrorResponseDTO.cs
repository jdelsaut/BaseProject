using System.Net;

namespace BoxApi.Api.Extensions
{
    internal class ApiErrorResponseDTO
    {
        private string error;
        private string errorDescription;
        private HttpStatusCode httpStatusCode;

        public ApiErrorResponseDTO(string error, string errorDescription, HttpStatusCode httpStatusCode)
        {
            this.error = error;
            this.errorDescription = errorDescription;
            this.httpStatusCode = httpStatusCode;
        }
    }
}