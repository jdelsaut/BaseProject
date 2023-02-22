using System;
using System.Collections.Generic;
using System.Text;

namespace BaseProject.Infrastructure.Logging
{
    public static class EventNames
    {
        public const string COSMOSDB_REQUEST_THROTTLED = "COSMOSDB_REQUEST_THROTTLED";

        public const string REQUEST_CREATED = "REQUEST_CREATED";
        public const string BaseProject_REQUEST_SENT = "BaseProject_REQUEST_SENT";
        public const string BaseProject_RESPONSE_RECEIVED = "BaseProject_RESPONSE_RECEIVED";
    }
}
