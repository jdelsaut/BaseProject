using BoxApi.Infrastructure.Logging;
using BoxApi.Infrastructure.Settings;
using BoxApi.Persistance.Base;
using Microsoft.Azure.Documents;
using Moq;
using System;

namespace BoxApi.Persistance.Tests.Fixtures
{
    public class BoxApiRepositoryFixture : IDisposable
    {
        public string CollectionName { get; } = "fakeBoxApiCollection";

        public Mock<IInsightsLogger> InsightsLoggerMock = new Mock<IInsightsLogger>();
        public Mock<ISettingsReader> SettingsReaderMock = new Mock<ISettingsReader>();

        public Mock<ICosmosDbClient> GetCosmosDbClientMockForTesting()
        {
            return new Mock<ICosmosDbClient>();
        }

        public Mock<BoxApiRepository> CreateBoxApiRepositoryMockForTesting(ICosmosDbClient cosmosDbClient)
        {
            var factoryStub = new Mock<ICosmosDbClientFactory>();
            factoryStub.Setup(x => x.GetClient(CollectionName)).Returns(cosmosDbClient);

            SettingsReaderMock.Setup(x => x.ReadSetting(AppSettingsKeys.ApiVersion))
                .Returns("01");

            var sut = new Mock<BoxApiRepository>(factoryStub.Object, InsightsLoggerMock.Object, SettingsReaderMock.Object);
            sut.Setup(x => x.CollectionName).Returns(CollectionName);
            sut.CallBase = true;

            return sut;
        }

        public Document CreateDocument()
        {

            var document = new Document();

            return document;
        }

        public void Dispose()
        {

        }
    }
}
