using BaseProject.Infrastructure.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Infrastructure
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsReader _settingsReader;
        private readonly string RootAuthEndpoint = AppSettingsKeys.RootAuthEndpoint;

        public TokenGenerator(ISettingsReader settingsReader, HttpClient httpClient)
        {
            _settingsReader = settingsReader;
            _httpClient = httpClient;
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderValueAsync(string clientId, string clientSecret)
        {
            _ = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _ = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));

            //scopes related to all the clients use by Assignment to call external apis (eg. "scope scope scope").
            //empty space is use to separate scopes within the "scope" strings here. Be aware of this point when modifying this string.

            string scope = $@"{Scopes.Scopes.BaseProjectWrite} {Scopes.Scopes.BaseProjectRead}";

            var tokenEndpoint = new Uri(_settingsReader.ReadSetting(RootAuthEndpoint) + "/token");

            using (var req = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint))
            {
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                byte[] byteArray = Encoding.ASCII.GetBytes(
                    $"{_settingsReader.ReadSetting(clientId)}:{_settingsReader.ReadSetting(clientSecret)}");

                req.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", $"{scope}"),
                });

                req.Content = content;

                using (HttpResponseMessage resp = await _httpClient.SendAsync(req))
                {
                    string tokenResponse = await resp.Content.ReadAsStringAsync();

                    const string errorMessage = "An error occured while authenticating client '{0}' " +
                            "against '{1}'. ";

                    if (resp.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                errorMessage + "Unexpected status code '{2} {3}'. Response body = {4}",
                                _settingsReader.ReadSetting(clientId),
                                tokenEndpoint,
                                (int)resp.StatusCode,
                                resp.StatusCode,
                                tokenResponse));
                    }

                    dynamic token = JsonConvert.DeserializeObject(tokenResponse);

                    dynamic accessToken = token.access_token;

                    if (accessToken == null)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                errorMessage + "Malformed reponse. Missing 'access_token' member.",
                                _settingsReader.ReadSetting(clientId),
                                tokenEndpoint));
                    }

                    dynamic tokenType = token.token_type;

                    if (tokenType == null)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                errorMessage + "Malformed reponse. Missing 'token_type' member.",
                                _settingsReader.ReadSetting(clientId),
                                tokenEndpoint));
                    }

                    return new AuthenticationHeaderValue((string)tokenType, (string)accessToken);
                }
            }
        }
    }
}
