using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxApi.Api.DTOs
{
    public class ErrorDTO
    {
        [JsonProperty("error")]
        public string ErrorLabel { get; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; }

        [JsonProperty("status_code")]
        public string StatusCode { get; }

        public ErrorDTO(string errorLabel, string errorDescription, string statusCode)
        {
            ErrorLabel = errorLabel ?? throw new ArgumentNullException(nameof(errorLabel));
            ErrorDescription = errorDescription ?? throw new ArgumentNullException(nameof(errorDescription));
            StatusCode = statusCode ?? throw new ArgumentNullException(nameof(statusCode));
        }
    }
}
