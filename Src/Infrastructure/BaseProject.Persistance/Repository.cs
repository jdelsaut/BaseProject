using BaseProject.Infrastructure.Logging;
using BaseProject.Infrastructure.Settings;
using BaseProject.Persistance.Base;
using BaseProject.Persistance.Exceptions;
using BaseProject.Persistance.Interfaces;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BaseProject.Persistance
{
    public class BaseProjectRepository : CosmosDbRepository<object>, IRepository
    {
        private readonly IInsightsLogger insightsLogger;
        private readonly ISettingsReader settings;

        public BaseProjectRepository(
            ICosmosDbClientFactory factory,
            IInsightsLogger insightsLogger,
            ISettingsReader settings) : base(factory) {
            this.insightsLogger = insightsLogger;
            this.settings = settings;
        }

        public override string CollectionName { get; } = "BaseProjects";
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':')[0]);


    }
}
