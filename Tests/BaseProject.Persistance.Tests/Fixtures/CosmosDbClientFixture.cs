using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using BoxApi.Persistance.Base;
using System;
using System.IO;

namespace BoxApi.Persistance.Tests.Fixtures
{
    public class CosmosDbClientFixture : IDisposable
    {
        public string DatabaseName { get; } = "foo";
        public string CollectionName { get; } = "bar";
        public string DocumentId { get; } = "foobar";
        public object Document { get; } = new { Id = "foobar", Note = "Note" };
        public Uri DocumentUri { get; }
        public ResourceResponse<Document> DocumentResponse { get; private set; }

        public SqlQuerySpec query { get; } = new SqlQuerySpec("SELECT * FROM c");

        public CosmosDbClientFixture()
        {
            DocumentUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, DocumentId);

            CreateDocumentResponse();
        }

        private void CreateDocumentResponse()
        {
            var documentJson = JsonConvert.SerializeObject(Document);
            var jsonReader = new JsonTextReader(new StringReader(documentJson));

            var document = new Document();
            document.LoadFrom(jsonReader);

            DocumentResponse = new ResourceResponse<Document>(document);
        }

        public CosmosDbClient CreateCosmosDbClientForTesting(IDocumentClient documentClient)
        {
            return new CosmosDbClient(DatabaseName, CollectionName, documentClient);
        }
        public void Dispose() { }
    }
}
