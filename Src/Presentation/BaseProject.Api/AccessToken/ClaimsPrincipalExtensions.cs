using BaseProject.Api.Constants;
using BaseProject.Infrastructure.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace BaseProject.Api.AccessToken
{
    public static class ClaimsPrincipalExtensions
    {
        public static KeyValuePair<HttpStatusCode, string> GetAuthorizationInformation(this ClaimsPrincipal claimsPrincipal, string methodName, string BaseProjectType)
        {
            if (!claimsPrincipal.Identities.Any() || !claimsPrincipal.Claims.Any())
            {
                return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, HttpResponsesMessageContents.NOT_AUTHORIZED);
            }

            var scopeClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtPrivateClaimNames.Scope)?.Value;

            if (string.IsNullOrEmpty(scopeClaim))
            {
                return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, HttpResponsesMessageContents.NOT_AUTHORIZED);
            }
            var scopes = scopeClaim.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var endpointInfo = new KeyValuePair<string, string>();

            if (!EndpointsInfo.Endpoints.IsEmpty)
            {
                var key = EndpointsInfo.BuildAccessKey(methodName, BaseProjectType);
                endpointInfo = EndpointsInfo.Endpoints.Where(e => e.Key.Equals(key)).FirstOrDefault();
            }

            string[] allowedScopes = endpointInfo.Value?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (allowedScopes != null && scopes.Any(s => allowedScopes.Contains(s, StringComparer.Ordinal)))
            {
                return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.OK, string.Empty);
            }

            return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, HttpResponsesMessageContents.Forbidden);
        }

        public static KeyValuePair<HttpStatusCode, string> GetAuthorizationInformation(this ClaimsPrincipal claimsPrincipal, string methodName)
        {
            return claimsPrincipal.GetAuthorizationInformation(methodName, null);
        }
    }
}
