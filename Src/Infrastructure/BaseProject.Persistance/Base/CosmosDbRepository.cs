using Microsoft.Azure.Documents;
using Assignment.Persistance.Base;
using System.Threading.Tasks;

namespace BaseProject.Persistance.Base
{
    public abstract class CosmosDbRepository <T> : IRepository, IDocumentCollectionContext<T>
    {
        protected readonly ICosmosDbClientFactory _cosmosDbClientFactory;
        public abstract string CollectionName { get; }

        protected CosmosDbRepository(ICosmosDbClientFactory cosmosDbClientFactory)
        {
            _cosmosDbClientFactory = cosmosDbClientFactory;
        }

        public async Task ClearCollectionAsync()
        {
            await DeleteCollectionIfExists();
            await _cosmosDbClientFactory.CreateCollectionsIfNotExists();
        }

        private async Task DeleteCollectionIfExists()
        {
            var cosmosDbClient = _cosmosDbClientFactory.GetClient(CollectionName);
            try
            {
                await cosmosDbClient.DeleteDocumentCollectionAsync(CollectionName);
            }
            catch (DocumentClientException e)
            {

                if (e.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
        }

        public virtual PartitionKey ResolvePartitionKey(string entityId) => null;

    }

}
