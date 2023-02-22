using BaseProject.Infrastructure.HttpHelper;
using BaseProject.Infrastructure.Settings;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseProject.Infrastructure
{
    public class HttpRequestRunner<TContent, TResponse> : IHttpRequestRunner<TContent, TResponse>
    {
        public Func<TContent> Content { get; set; }

        private readonly Lazy<TContent> _content;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly HttpClient _httpClient;
        private readonly ISettingsReader _settingsReader;

        private const int RetryCount = 5;
        private const int WaitSecondsBeforeRetry = 2;

        internal Type Type => typeof(TResponse);
        public (string ClientIdKey, string ClientSecretKey) ClientCredentialsKeys { get; set; }

        internal AuthenticationHeaderValue AuthenticationHeaderValue
        {
            get
            {
                return _tokenGenerator?.GetAuthenticationHeaderValueAsync(ClientCredentialsKeys.ClientIdKey, ClientCredentialsKeys.ClientSecretKey).GetAwaiter().GetResult();
            }
        }

        private static HttpStatusCode[] TransientHttpStatusCodes =
            {
                HttpStatusCode.RequestTimeout,
                HttpStatusCode.InternalServerError,
                HttpStatusCode.BadGateway,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.GatewayTimeout
            };

        public HttpRequestRunner(
            ISettingsReader settingsReader,
            HttpClient httpClient,
            ITokenGenerator tokenGenerator)
        {
            _content = new Lazy<TContent>(() => Content());
            _httpClient = httpClient;
            _settingsReader = settingsReader;
            _tokenGenerator = tokenGenerator;
        }

        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.ffZ",
            NullValueHandling = NullValueHandling.Ignore,
        };

        private HttpContent BuildContent()
        {
            if (typeof(TContent) == typeof(string))
            {
                return new StringContent(_content.Value as string, Encoding.UTF8);
            }

            return new StringContent(JsonConvert.SerializeObject(_content.Value, _serializerSettings), Encoding.UTF8, "application/json");
        }

        private async Task<HttpResponseMessage> PostAsync(string endpoint, AuthenticationHeaderValue token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = token;
            var response = await _httpClient.PostAsync(endpoint, BuildContent());
            return response;
        }

        public async Task<TResponse> ExecutePostWithRetryAsync(string endpoint)
        {
            try
            {
                AsyncRetryPolicy<HttpResponseMessage> retryPolicy = BuildRetryPolicy();
                var response = await retryPolicy.ExecuteAsync(
                    async () => await PostAsync(endpoint, AuthenticationHeaderValue));

                return await GetTResponse(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static AsyncRetryPolicy<HttpResponseMessage> BuildRetryPolicy()
        {
            return Policy
                 .Handle<HttpRequestException>()
                 .OrResult<HttpResponseMessage>(r => TransientHttpStatusCodes.Contains(r.StatusCode))
                 .WaitAndRetryAsync(RetryCount, i => TimeSpan.FromSeconds(WaitSecondsBeforeRetry));
        }

        private async Task<TResponse> GetTResponse(HttpResponseMessage response)
        {
            string value = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    throw new BadRequestException(value);
                }

                throw new HttpRequestException(value);
            }

            object result;

            if (Type != typeof(string))
            {
                result = (TResponse)JsonConvert.DeserializeObject(
                    value,
                    Type,
                    _serializerSettings);
            }
            else
            {
                result = value;
            }

            return (TResponse)result;
        }
    }
}
