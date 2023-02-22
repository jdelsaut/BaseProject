using System.Threading.Tasks;

namespace BoxApi.Persistance.Base
{
    public interface ICosmosDbClientFactory
    {
        Task CreateCollectionsIfNotExists();
        Task CreateDatabaseIfNotExists();
        ICosmosDbClient GetClient(string collectionName);
    }
}
