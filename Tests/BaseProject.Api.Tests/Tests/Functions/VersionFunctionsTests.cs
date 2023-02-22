
using BoxApi.Api.Functions;
using BoxApi.Api.Messages;
using BoxApi.Api.Tests.Fixtures;
using BoxApi.Api.Tests.Helpers;
using BoxApi.Infrastructure.Logging;
using BoxApi.Infrastructure.Scopes;
using BoxApi.Infrastructure.Settings;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BoxApi.Api.Tests.Tests.Functions
{
    public class VersionFunctionsTests
    {
        private readonly Mock<ILogger> _logger;
        private readonly Mock<ISettingsReader> _settingsReaderMock;
        private readonly Mock<IInsightsLogger> _insightsLoggerMock;
        private readonly Mock<ClaimsPrincipal> _claimsPrincipal;
        private readonly VersionFunctions _VersionFunctions;

        public VersionFunctionsTests()
        {
            _logger = new Mock<ILogger>();
            _settingsReaderMock = new Mock<ISettingsReader>();
            _insightsLoggerMock = new Mock<IInsightsLogger>();
            _claimsPrincipal = new Mock<ClaimsPrincipal>();
            _claimsPrincipal.Setup(x => x.Identities)
                .Returns(new List<ClaimsIdentity>() { new ClaimsIdentity() });

            _insightsLoggerMock.Setup(x => x.StartOperation(It.IsAny<MicroserviceRequestTelemetry>()))
                .Returns((new RequestTelemetry(), new Mock<IOperationHolder<RequestTelemetry>>().Object));
            _insightsLoggerMock.Setup(x => x.StopOperation(It.IsAny<IOperationHolder<RequestTelemetry>>()));
            _insightsLoggerMock.Setup(x => x.TrackException(It.IsAny<Exception>()));

            _VersionFunctions = new VersionFunctions(_settingsReaderMock.Object, _insightsLoggerMock.Object);
        }

        [Fact]
        public void Should_return_a_OK_response_with_an_ApiVersionResponse_type()
        {
            #region Arrange
            var request = HttpHelpers.CreateMockHttpRequestMessage(string.Empty);

            AccessTokenFixture.SettingClaimsPrincipal(_claimsPrincipal, Scopes.BoxApiRead);
            #endregion

            #region Act
            var response = _VersionFunctions.Version(request).GetAwaiter().GetResult();
            #endregion

            #region Assert

            Assert.IsType<HttpResponseMessage>(response);
            //TODOAssert.IsType<ApiVersionResponse>(response.Content.ReadAsAsync<ApiVersionResponse>().GetAwaiter().GetResult());
            #endregion
        }

        //[Fact]
        //public void Should_return_the_versionNumber_and_description_given_specified_values_in_sesstings()
        //{
        //    #region Arrange
        //    var request = HttpHelpers.CreateMockHttpRequestMessage(string.Empty);
        //    var settingsReader = new Mock<ISettingsReader>();

        //    settingsReader.Setup(x => x.ReadSetting(AppSettingsKeys.ApiVersion))
        //        .Returns("0.0");

        //    settingsReader.Setup(x => x.ReadSetting(AppSettingsKeys.VersionDescription))
        //       .Returns("Not Stable");

        //    AccessTokenFixture.SettingClaimsPrincipal(_claimsPrincipal, Scopes.BoxApiRead);

        //    #endregion

        //    #region Act
        //    var response = new VersionFunctions(settingsReader.Object, _insightsLoggerMock.Object).Version(request).GetAwaiter().GetResult();
        //    var apiVersion = response.Content.ReadAsAsync<ApiVersionResponse>().GetAwaiter().GetResult();
        //    #endregion

        //    #region Assert
        //    Assert.Equal("0.0", apiVersion.Version);
        //    Assert.Equal("Not Stable", apiVersion.Description);
        //    #endregion
        //}

        [Fact]
        public async Task Should_trace_success_telemetry_insight_if_no_exception()
        {
            #region Arrange
            var request = HttpHelpers.CreateMockHttpRequestMessage(string.Empty);
            var settingsReaderMock = new Mock<ISettingsReader>();
            RequestTelemetry telemetryRequest = new RequestTelemetry() { Name = "test request" };

            Mock<IOperationHolder<RequestTelemetry>> operationMock = new Mock<IOperationHolder<RequestTelemetry>>();

            _insightsLoggerMock.Setup(x => x.StartOperation(It.IsAny<MicroserviceRequestTelemetry>()))
                .Returns((telemetryRequest, operationMock.Object));

            _insightsLoggerMock.Setup(x => x.StopOperation(It.IsAny<IOperationHolder<RequestTelemetry>>()));

            AccessTokenFixture.SettingClaimsPrincipal(_claimsPrincipal, Scopes.BoxApiRead);
            #endregion

            #region Act
            await _VersionFunctions.Version(request);
            #endregion

            #region Assert
            _insightsLoggerMock.Verify(mock => mock.StartOperation(It.IsAny<MicroserviceRequestTelemetry>()),
                Times.Exactly(1));

            _insightsLoggerMock.Verify(mock => mock.StopOperation(It.IsAny<IOperationHolder<RequestTelemetry>>()),
                Times.Exactly(1));

            Assert.True(telemetryRequest.Success);
            #endregion
        }

        [Fact]
        public async Task Should_return_BadRequest_and_log_a_failure_and_exception_when_an_exception_happends()
        {
            #region Arrange
            var request = HttpHelpers.CreateMockHttpRequestMessage(string.Empty);
            var settingsReader = new Mock<ISettingsReader>();

            RequestTelemetry telemetryRequest = new RequestTelemetry() { Name = "test request" };
            Mock<IOperationHolder<RequestTelemetry>> operationMock = new Mock<IOperationHolder<RequestTelemetry>>();

            _insightsLoggerMock.Setup(x => x.StartOperation(It.IsAny<MicroserviceRequestTelemetry>()))
                .Returns((telemetryRequest, operationMock.Object));

            _insightsLoggerMock.Setup(x => x.StopOperation(It.IsAny<IOperationHolder<RequestTelemetry>>()));

            _insightsLoggerMock.Setup(x => x.TrackException(It.IsAny<Exception>()));

            //Setting-up the Exception
            settingsReader.Setup(x => x.ReadSetting(It.IsAny<string>()))
                .Throws(new System.Exception("Error reading settings"));

            AccessTokenFixture.SettingClaimsPrincipal(_claimsPrincipal, Scopes.BoxApiRead);
            #endregion

            #region Act
            var response = await new VersionFunctions(settingsReader.Object, _insightsLoggerMock.Object).Version(request);
            #endregion

            #region Assert
            _insightsLoggerMock.Verify(mock => mock.StartOperation(It.IsAny<MicroserviceRequestTelemetry>()),
                Times.Exactly(1));
            _insightsLoggerMock.Verify(mock => mock.StopOperation(It.IsAny<IOperationHolder<RequestTelemetry>>()),
               Times.Exactly(1));

            _insightsLoggerMock.Verify(mock => mock.TrackException(It.IsAny<Exception>()),
               Times.Exactly(1));
            Assert.False(telemetryRequest.Success);

            Assert.IsType<HttpResponseMessage>(response);
            Assert.Contains("Bad Request", response.ReasonPhrase);
            #endregion
        }
    }
}
