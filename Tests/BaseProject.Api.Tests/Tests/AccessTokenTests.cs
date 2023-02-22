using BoxApi.Api.AccessToken;
using BoxApi.Api.Tests.Fixtures;
using BoxApi.Api.Tests.Helpers;
using BoxApi.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Xunit;

namespace BoxApi.Api.Tests.Tests
{
    public class AccessTokenTests : IClassFixture<AccessTokenFixture>
    {
        private readonly AccessTokenFixture _accessTokenFixture;

        public AccessTokenTests(AccessTokenFixture accessTokenFixture)
        {
            this._accessTokenFixture = accessTokenFixture;
        }

        [Fact]
        public async void Should_return_claimsPrincipal_with_bearer_auth_in_header_of_http_request()
        {
            #region Arrange

            HeaderDictionary headers = new HeaderDictionary()
            {
                ["Authorization"] = new StringValues("Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkFUMjAxODA1MDkifQ.eyJybG0iOiJjbGllbnQiLCJjZWkiOiI3MzBhZmNmOCIsInNjb3BlIjoidXJuOmF4YS5wYXJ0bmVycy5iYWNrZW5kcy5wcm92aWRlcl9zZWFyY2guZ2VuZXJpYy5wcm92aWRlcnMucmVhZF9vbmx5IiwiaXNzIjoiaHR0cHM6XC9cL21hYW0tc3RnLmF4YS5jb21cL21hYW1cL3YyIiwiZXhwIjoxNTYyNjkzODc1LCJpYXQiOjE1NjI2OTAyNzUsImp0aSI6ImZiZWE1MzdhLWNkYmUtNGM1OS04YWU5LTgyNzE2ZTcwZWRjYiIsImNsaWVudF9pZCI6Im1pLWUxMGRjNTkyIn0.ctTUirkiGdHLdUX6Q7fqucYyjMd9RSFbObolIbhGnQSiAh9ZLaWkqnwxSreXV2_nf1kPvJXFIKNHMcGev0yC65Y-4EbwEY5rjmCosERL_UKnA4B91dZcW65urROuQ1fBN4W6Csvhxa6MGO7iic8kH47EOS2dVtycTRJPa_UazVFL1-N6wD1ZHtxIy0N_o-0F4fKl1-3vMC3L-PAPsjNdES4TYnpWEz-obLWdWWAJHyQd4zXc8kVxHOK2wsanQiY1pCP2LIqo8mybJD2PaQce7J3g5cN7f3YXXWJ0m5vSKrRS0BQFIgLJ0VDcgQz2vJMgQ51Wvqs7RBI04JQFnPhytQ")
            };

            BindingContext context = new BindingContext(
                new ValueBindingContext(new FunctionBindingContext(Guid.NewGuid(), CancellationToken.None), CancellationToken.None),
                new Dictionary<string, object>()
                {
                    ["req"] = HttpHelpers.CreateMockHttpRequest(null, headers).Object
                });
            #endregion

            #region Act
            var res = await _accessTokenFixture.accessTokenBinding.BindAsync(context);

            #endregion
            var response = res.GetValueAsync().Result;
            #region Assert
            Assert.NotNull(response);
            Assert.Contains("ClaimsPrincipal", response.ToString());
            #endregion
        }

        [Fact]
        public async System.Threading.Tasks.Task AccessToken_WithNullHeader_ShouldThrowSecurityTokenException()
        {
            #region Arrange

            HeaderDictionary headers = new HeaderDictionary() { };
            var mockSettingReader = new Mock<ISettingsReader>();

            #endregion

            #region Act
            var provider = await new AccessTokenValueProvider(HttpHelpers.CreateMockHttpRequest(null, headers).Object, mockSettingReader.Object).GetValueAsync();
            var claimsPrincipal = provider as ClaimsPrincipal;
            Assert.Empty(claimsPrincipal.Identities);
            #endregion
        }
    }
}
