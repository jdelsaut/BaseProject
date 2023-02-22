using BaseProject.Infrastructure.Claims;
using BaseProject.Infrastructure.Helpers;
using BaseProject.Infrastructure.Proxy;
using BaseProject.Infrastructure.Settings;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;

namespace BaseProject.Api.AccessToken
{
    public class AccessTokenValidateValueProvider : JwtSecurityTokenHandler
    {
        private const string DiscoveryEndpoint = AppSettingsKeys.DiscoveryEndpoint;
        private const string RequireSignedJwt = AppSettingsKeys.RequireSignedJwt;
        private const string RequireLifeTimeValidation = AppSettingsKeys.RequireLifetimeValidation;
        private static string HttpProxy = AppSettingsKeys.LocalHttpProxy;
        private const string ShowPII = AppSettingsKeys.ShowPII;

        private string accessToken { get; set; }
        private readonly ISettingsReader settingsReader;
        private readonly HttpClient httpClient = null;
        static IEnumerable<SecurityKey> keys = null;

        public AccessTokenValidateValueProvider(string accessToken, ISettingsReader settingsReader)
        {
            this.accessToken = accessToken;
            this.settingsReader = settingsReader;
            httpClient = new HttpClient(new ProxyAwareHttpClientHandler(settingsReader.ReadSetting(HttpProxy)));
        }

        public ClaimsPrincipal Validate()
        {
            try
            {
                IdentityModelEventSource.ShowPII = Convert.ToBoolean(settingsReader.ReadSetting(ShowPII));
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    IssuerSigningKeyResolver = (token, jwtToken, kid, param) =>
                    {
                        return GetSecurityKeysForSigningKeyResolver(kid);
                    },

                    RequireSignedTokens = Convert.ToBoolean(settingsReader.ReadSetting(RequireSignedJwt)),
                    ValidateLifetime = Convert.ToBoolean(settingsReader.ReadSetting(RequireLifeTimeValidation)),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };

                ClaimsPrincipal principal = ValidateToken(accessToken, parameters, out SecurityToken securityToken);

                return principal;
            }
            catch (Exception e)
            {
                return new ClaimsPrincipal();
            }
        }

        private IEnumerable<SecurityKey> GetSecurityKeysForSigningKeyResolver(string kid)
        {
            if (keys == null || !DoesKeyIdExist(kid, keys))
            {
                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                settingsReader.ReadSetting(DiscoveryEndpoint),
                new OpenIdConnectConfigurationRetriever(),
                httpClient);
                OpenIdConnectConfiguration openIdConfig = GetOpenIdConnectConfiguration(configurationManager);

                keys = openIdConfig.SigningKeys;
            }

            return keys;
        }

        private bool DoesKeyIdExist(string kid, IEnumerable<SecurityKey> keys)
        {
            return keys.Any(k => k.KeyId.Equals(kid));
        }

        private OpenIdConnectConfiguration GetOpenIdConnectConfiguration(ConfigurationManager<OpenIdConnectConfiguration> configurationManager)
        {
            return AsyncHelper.RunSync(async () => await configurationManager.GetConfigurationAsync(CancellationToken.None));
        }

        private void ValidateJwtFormat(JwtSecurityToken jwt)
        {
            if (jwt.Header.Alg != SecurityAlgorithms.RsaSha256)
            {
                string msg = $"Signing JWT is not supported for: Algorithm: '{jwt.Header.Alg}'";
                throw new SecurityTokenInvalidSignatureException(msg);
            }

            if (string.IsNullOrEmpty(jwt.Header.Kid))
            {
                string msg = $"JWT lacks '{JsonWebKeyParameterNames.Kid}' header claim.";
                throw new SecurityTokenInvalidSignatureException(msg);
            }

            var payloadClaimKeys = new List<string>(jwt.Payload.Keys);

            if (!payloadClaimKeys.Exists(c => c.Equals(JwtPrivateClaimNames.ClientId, StringComparison.InvariantCulture)))
            {
                string msg = $"JWT lacks '{JwtPrivateClaimNames.ClientId}' payload claim.";
                throw new SecurityTokenInvalidSignatureException(msg);
            }
        }

        protected override JwtSecurityToken ValidateSignature(string token, TokenValidationParameters validationParameters)
        {
            JwtSecurityToken jwt = base.ValidateSignature(token, validationParameters);

            ValidateJwtFormat(jwt);

            return jwt;
        }
    }
}
