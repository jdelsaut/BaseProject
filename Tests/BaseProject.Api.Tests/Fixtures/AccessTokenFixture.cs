using BaseProject.Api.AccessToken;
using BaseProject.Infrastructure.Claims;
using BaseProject.Infrastructure.Settings;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BaseProject.Api.Tests.Fixtures
{
    public class AccessTokenFixture : IDisposable
    {
        Mock<ISettingsReader> mockSettingsReader = new Mock<ISettingsReader>();
        public AccessTokenBinding accessTokenBinding { get; set; }

        public AccessTokenFixture()
        {
            mockSettingsReader.Setup(x => x.ReadSetting(AppSettingsKeys.JwtIssuer))
                .Returns("https://maam-stg.axa.com");
            mockSettingsReader.Setup(x => x.ReadSetting(AppSettingsKeys.DiscoveryEndpoint))
                .Returns("https://maam-stg.axa.com/.well-known/openid-configuration");
            mockSettingsReader.Setup(x => x.ReadSetting(AppSettingsKeys.RequireSignedJwt))
                .Returns("false");
            mockSettingsReader.Setup(x => x.ReadSetting(AppSettingsKeys.AuthHeaderName))
                            .Returns("Authorization");
            mockSettingsReader.Setup(x => x.ReadSetting(AppSettingsKeys.BearerPrefix))
                            .Returns("Bearer");

            accessTokenBinding = new AccessTokenBinding(mockSettingsReader.Object);
        }

        public static void SettingClaimsPrincipal(Mock<ClaimsPrincipal> claimsPrincipal, string scope)
        {
            IEnumerable<Claim> claims = new List<Claim>()
                {
                    new Claim(JwtPrivateClaimNames.Scope, scope)
                }.AsEnumerable();

            claimsPrincipal.Setup(x => x.Claims).Returns(claims);
        }
        public void Dispose()
        {
            accessTokenBinding = null;
        }
    }
}
