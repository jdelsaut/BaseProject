using System;
using System.Collections.Generic;
using System.Text;

namespace BoxApi.Persistance.Options
{
    public class CosmosDbOptions
    {
        public string DatabaseName { get; set; }
        public List<CollectionInfo> CollectionNames { get; set; }

        public void Deconstruct(out string databaseName, out List<CollectionInfo> collectionNames)
        {
            databaseName = DatabaseName;
            collectionNames = CollectionNames;
        }
    }
}
