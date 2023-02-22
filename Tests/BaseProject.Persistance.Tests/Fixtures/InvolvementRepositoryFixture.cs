using BaseProject.Infrastructure.Logging;
using BaseProject.Infrastructure.Settings;
using BaseProject.Persistance.Base;
using Microsoft.Azure.Documents;
using Moq;
using System;

namespace BaseProject.Persistance.Tests.Fixtures
{
    public class BaseProjectRepositoryFixture : IDisposable
    {
        public string CollectionName { get; } = "fakeBaseProjectCollection";

        public Mock<IInsightsLogger> InsightsLoggerMock = new Mock<IInsightsLogger>();
        public Mock<ISettingsReader> SettingsReaderMock = new Mock<ISettingsReader>();

        public Mock<ICosmosDbClient> GetCosmosDbClientMockForTesting()
        {
            return new Mock<ICosmosDbClient>();
        }

        public Mock<BaseProjectRepository> CreateBaseProjectRepositoryMockForTesting(ICosmosDbClient cosmosDbClient)
        {
            var factoryStub = new Mock<ICosmosDbClientFactory>();
            factoryStub.Setup(x => x.GetClient(CollectionName)).Returns(cosmosDbClient);

            SettingsReaderMock.Setup(x => x.ReadSetting(AppSettingsKeys.ApiVersion))
                .Returns("01");

            var sut = new Mock<BaseProjectRepository>(factoryStub.Object, InsightsLoggerMock.Object, SettingsReaderMock.Object);
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
