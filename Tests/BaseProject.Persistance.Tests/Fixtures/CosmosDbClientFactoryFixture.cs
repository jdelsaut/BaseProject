using Microsoft.Azure.Documents;
using BoxApi.Persistance.Base;
using System;
using System.Collections.Generic;

namespace BoxApi.Persistance.Tests.Fixtures
{
    public class CosmosDbClientFactoryFixture : IDisposable
    {
        public string DatabaseName { get; } = "foobar";
        public List<string> CollectionNames { get; } = new List<string> { "foo", "bar" };

        public CosmosDbClientFactory CreateCosmosDbClientFactoryForTesting(IDocumentClient documentClient)
        {
            return new CosmosDbClientFactory(DatabaseName, CollectionNames, documentClient);
        }

        public void Dispose() { }
    }
}
