using BoxApi.Infrastructure.Logging;
using BoxApi.Infrastructure.Settings;
using BoxApi.Persistance.Base;
using BoxApi.Persistance.Exceptions;
using BoxApi.Persistance.Interfaces;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BoxApi.Persistance
{
    public class BoxApiRepository : CosmosDbRepository<object>, IRepository
    {
        private readonly IInsightsLogger insightsLogger;
        private readonly ISettingsReader settings;

        public BoxApiRepository(
            ICosmosDbClientFactory factory,
            IInsightsLogger insightsLogger,
            ISettingsReader settings) : base(factory) {
            this.insightsLogger = insightsLogger;
            this.settings = settings;
        }

        public override string CollectionName { get; } = "BoxApis";
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':')[0]);


    }
}
