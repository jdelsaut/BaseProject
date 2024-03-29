﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using BaseProject.Persistance.Base;
using System;
using System.Threading;
using Xunit;
using BaseProject.Persistance.Tests.Fixtures;

namespace BaseProject.Persistance.Tests.Tests.Base
{
    public class CosmosDbClientTests : IClassFixture<CosmosDbClientFixture>
    {
        private readonly CosmosDbClientFixture _fixture;

        public CosmosDbClientTests(CosmosDbClientFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(null, null, null, "databaseName")]
        [InlineData("foo", null, null, "collectionName")]
        [InlineData("foo", "bar", null, "documentClient")]
        public void CosmosDbClient_WithNullArgument_ShouldThrowArgumentNullException(string databaseName,
           string collectionName, IDocumentClient documentClient, string paramName)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new CosmosDbClient(databaseName, collectionName, documentClient));

            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void CosmosDbClient_WithNonNullArguments_ShouldReturnNewInstance()
        {
            var documentClientStub = new Mock<IDocumentClient>();
            documentClientStub.Setup(x => x.ConnectionPolicy)
                .Returns(new ConnectionPolicy());

            var sut = _fixture.CreateCosmosDbClientForTesting(documentClientStub.Object);

            Assert.NotNull(sut);
        }

        [Fact]
        public async void ReadDocumentAsync_WhenCalled_ShouldCallReadDocumentAsyncOnDocumentClient()
        {
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(x => x.ConnectionPolicy)
                .Returns(new ConnectionPolicy());

            documentClientMock.Setup(x => x.ReadDocumentAsync(It.IsAny<Uri>(), null, default(CancellationToken)))
                .ReturnsAsync(_fixture.DocumentResponse);
            var sut = _fixture.CreateCosmosDbClientForTesting(documentClientMock.Object);

            await sut.ReadDocumentAsync(_fixture.DocumentId);

            documentClientMock.Verify(
                x => x.ReadDocumentAsync(
                    It.Is<Uri>(uri => uri == _fixture.DocumentUri),
                    null,
                    default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public async void CreateDocumentAsync_WhenCalled_ShouldCallCreateDocumentAsyncOnDocumentClient()
        {
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(x => x.ConnectionPolicy)
                .Returns(new ConnectionPolicy());

            documentClientMock.Setup(x =>
                    x.CreateDocumentAsync(It.IsAny<Uri>(), It.IsAny<object>(), null, false, default(CancellationToken)))
                .ReturnsAsync(_fixture.DocumentResponse);
            var sut = _fixture.CreateCosmosDbClientForTesting(documentClientMock.Object);

            await sut.CreateDocumentAsync(_fixture.Document);

            documentClientMock.Verify(
                x => x.CreateDocumentAsync(
                    It.Is<Uri>(uri =>
                        uri == UriFactory.CreateDocumentCollectionUri(_fixture.DatabaseName, _fixture.CollectionName)),
                    It.Is<object>(document => document == _fixture.Document),
                    null,
                    false,
                    default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public async void ReplaceAsync_WhenCalled_ShouldCallReplaceDocumentAsyncOnDocumentClient()
        {
            var documentClientMock = new Mock<IDocumentClient>();

            documentClientMock.Setup(x => x.ConnectionPolicy)
                .Returns(new ConnectionPolicy());

            documentClientMock.Setup(x =>
                    x.ReplaceDocumentAsync(It.IsAny<Uri>(), It.IsAny<object>(), null, default(CancellationToken)))
                .ReturnsAsync(_fixture.DocumentResponse);
            var sut = _fixture.CreateCosmosDbClientForTesting(documentClientMock.Object);

            await sut.ReplaceDocumentAsync(_fixture.DocumentId, _fixture.Document);

            documentClientMock.Verify(
                x => x.ReplaceDocumentAsync(
                    It.Is<Uri>(uri => uri == _fixture.DocumentUri),
                    It.Is<object>(document => document == _fixture.Document),
                    null,
                    default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public async void DeleteAsync_WhenCalled_ShouldCallDeleteDocumentAsyncOnDocumentClient()
        {
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(x => x.ConnectionPolicy)
                .Returns(new ConnectionPolicy());

            documentClientMock.Setup(x => x.DeleteDocumentAsync(It.IsAny<Uri>(), null, default(CancellationToken)))
                .ReturnsAsync(_fixture.DocumentResponse);
            var sut = _fixture.CreateCosmosDbClientForTesting(documentClientMock.Object);

            await sut.DeleteDocumentAsync(_fixture.DocumentId);

            documentClientMock.Verify(
                x => x.DeleteDocumentAsync(
                    It.Is<Uri>(uri => uri == _fixture.DocumentUri),
                    null,
                    default(CancellationToken)),
                Times.Once);
        }

    }
}
