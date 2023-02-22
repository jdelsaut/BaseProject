using Microsoft.Azure.Documents;
using BaseProject.Persistance.Base;
using System;
using System.Collections.Generic;

namespace BaseProject.Persistance.Tests.Fixtures
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
