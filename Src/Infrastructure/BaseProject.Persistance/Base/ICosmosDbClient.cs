using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BaseProject.Persistance.Base
{
    public interface ICosmosDbClient
    {
        Task<Document> ReadDocumentAsync(string documentId, RequestOptions options = null,
           CancellationToken cancellationToken = default(CancellationToken));

        Task<Document> CreateDocumentAsync(object document, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Document> ReplaceDocumentAsync(string documentId, object document, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Document> DeleteDocumentAsync(string documentId, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<List<Document>> ReadDocumentsByQueryAsync(SqlQuerySpec sqlQuery, RequestOptions options = null,
           CancellationToken cancellationToken = default(CancellationToken));

        Task<Document> CreateOrUpdateDocumentAsync(object document, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken));

        Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(string collectionName, RequestOptions options = null,
           CancellationToken cancellationToken = default(CancellationToken));
    }
}
