using System.Threading.Tasks;

namespace BaseProject.Persistance.Base
{
    public interface ICosmosDbClientFactory
    {
        Task CreateCollectionsIfNotExists();
        Task CreateDatabaseIfNotExists();
        ICosmosDbClient GetClient(string collectionName);
    }
}
