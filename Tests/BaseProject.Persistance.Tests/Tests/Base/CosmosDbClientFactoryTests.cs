using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using BoxApi.Persistance.Base;
using BoxApi.Persistance.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BoxApi.Persistance.Tests.Tests.Base
{
    public class CosmosDbClientFactoryTests : IClassFixture<CosmosDbClientFactoryFixture>
    {
        private readonly CosmosDbClientFactoryFixture _fixture;

        public CosmosDbClientFactoryTests(CosmosDbClientFactoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(null, null, null, "databaseName")]
        [InlineData("foo", null, null, "collectionNames")]
        [InlineData("foo", new[] { "bar" }, null, "documentClient")]
        public void CosmosDbClientFactory_WithNullArgument_ShouldThrowArgumentNullException(string databaseName,
            IEnumerable<string> collectionNames, DocumentClient documentClient, string paramName)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new CosmosDbClientFactory(databaseName, collectionNames?.ToList(), documentClient));

            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void CosmosClientFactory_WithNonNullArguments_ShouldCreateNewInstance()
        {
            var documentClientStub = new Mock<IDocumentClient>();
            var sut = _fixture.CreateCosmosDbClientFactoryForTesting(documentClientStub.Object);

            Assert.NotNull(sut);
        }

        [Fact]
        public void GetClient_WithNonExistingCollectionName_ShouldThrowArgumentException()
        {
            var documentClientStub = new Mock<IDocumentClient>();
            var sut = _fixture.CreateCosmosDbClientFactoryForTesting(documentClientStub.Object);
            const string collectionName = "abc";

            var ex = Assert.Throws<ArgumentException>(() => sut.GetClient(collectionName));

            Assert.Equal($"Unable to find collection: {collectionName}", ex.Message);
        }

        [Fact]
        public void GetClient_WithExistingCollectionName_ShouldReturnNewCosmosClient()
        {
            var documentClientStub = new Mock<IDocumentClient>();
            documentClientStub.Setup(x => x.ConnectionPolicy)
                .Returns(new ConnectionPolicy());
            var sut = _fixture.CreateCosmosDbClientFactoryForTesting(documentClientStub.Object);
            var collectionName = _fixture.CollectionNames[0];

            var result = sut.GetClient(collectionName);

            Assert.NotNull(result);
        }

        [Fact]
        public async void EnsureDbSetupAsync_WhenCalled_ShouldVerifyDatabaseAndCollectionsExistence()
        {
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(x => x.ReadDatabaseAsync(It.IsAny<Uri>(), null))
                .ReturnsAsync(new ResourceResponse<Database>());
            documentClientMock.Setup(x => x.ReadDocumentCollectionAsync(It.IsAny<Uri>(), null))
                .ReturnsAsync(new ResourceResponse<DocumentCollection>());
            var sut = _fixture.CreateCosmosDbClientFactoryForTesting(documentClientMock.Object);

            await sut.EnsureDbSetupAsync();

            var databaseUri = UriFactory.CreateDatabaseUri(_fixture.DatabaseName);
            var collectionUris = _fixture.CollectionNames
                .Select(x => UriFactory.CreateDocumentCollectionUri(_fixture.DatabaseName, x))
                .ToList();
            var udfUri = UriFactory.CreateUserDefinedFunctionUri(_fixture.DatabaseName, _fixture.CollectionNames[0], "ProviderHasServices");

            documentClientMock.Verify(
                x => x.ReadDatabaseAsync(It.Is<Uri>(uri => uri.Equals(databaseUri)), null), Times.AtLeastOnce);
            documentClientMock.Verify(
                x => x.ReadDocumentCollectionAsync(It.Is<Uri>(uri => uri.Equals(collectionUris[0])), null), Times.AtLeastOnce);
            documentClientMock.Verify(
                x => x.ReadDocumentCollectionAsync(It.Is<Uri>(uri => uri.Equals(collectionUris[1])), null), Times.AtLeastOnce);
        }
    }
}
