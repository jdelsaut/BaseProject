using BaseProject.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Api.AccessToken
{
    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> instance for the supplied header and configuration values.
    /// </summary>
    /// <remarks>
    /// This is where the actual authentication happens - replace this code to implement a different authentication solution.
    /// </remarks>
    public class AccessTokenValueProvider : IValueProvider
    {
        private const string AuthHeaderName = AppSettingsKeys.AuthHeaderName;
        private const string BearerPrefix = AppSettingsKeys.BearerPrefix;
        private HttpRequest _request;
        private readonly ISettingsReader _settingsReader;

        public AccessTokenValueProvider(HttpRequest request, ISettingsReader settingReader)
        {
            _request = request;
            _settingsReader = settingReader;
        }

        public Task<object> GetValueAsync()
        {
            var _authHeaderName = _settingsReader.ReadSetting(AuthHeaderName);
            var _bearerPrefix = _settingsReader.ReadSetting(BearerPrefix);
            // Get the token from the header
            if (_request.Headers.ContainsKey(_authHeaderName) &&
               _request.Headers[_authHeaderName].ToString().StartsWith(_bearerPrefix))
            {
                var token = _request.Headers["Authorization"].ToString().Substring(_bearerPrefix.Length);

                // Validate the token
                var result = new AccessTokenValidateValueProvider(token.Trim(), _settingsReader).Validate();

                return Task.FromResult<object>(result);
            }
            else
            {
                return Task.FromResult<object>(new ClaimsPrincipal());
            }
        }

        public Type Type => typeof(ClaimsPrincipal);

        public string ToInvokeString() => GetValueAsync().Result.ToString();
    }
}
