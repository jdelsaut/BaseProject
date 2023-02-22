using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace BaseProject.Persistance.Base
{
    public class CosmosDbClient : ICosmosDbClient
    {
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly IDocumentClient _documentClient;

        public CosmosDbClient(string databaseName, string collectionName, IDocumentClient documentClient)
        {
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
            _documentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
            _documentClient.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 100;
            _documentClient.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 180;
        }

        public async Task<Document> CreateDocumentAsync(object document, RequestOptions options = null, bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _documentClient.CreateDocumentAsync(
               UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), document, options,
               disableAutomaticIdGeneration, cancellationToken);
        }

        public async Task<Document> DeleteDocumentAsync(string documentId, RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _documentClient.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId), options, cancellationToken);

        }

        public async Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(string collectionName, RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _documentClient.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, collectionName));
        }

        public async Task<Document> ReadDocumentAsync(string documentId, RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _documentClient.ReadDocumentAsync(
               UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId), options, cancellationToken);
        }

        public async Task<List<Document>> ReadDocumentsByQueryAsync(SqlQuerySpec sqlQuery, RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var documents = new List<Document>();

            var queryable = _documentClient.CreateDocumentQuery<Object>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName
                ), sqlQuery, new FeedOptions() { EnableCrossPartitionQuery = true }).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<Document>();
                foreach (var document in response)
                {
                    documents.Add(document);
                }                               
            }

            return documents.ToList();
        }

        public async Task<Document> ReplaceDocumentAsync(string documentId, object document, RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _documentClient.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId), document, options,
                cancellationToken);
        }

        public async Task<Document> CreateOrUpdateDocumentAsync(object document, RequestOptions options = null, bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _documentClient.UpsertDocumentAsync(
               UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), document, options,
               disableAutomaticIdGeneration, cancellationToken);
        }
    }
}
