using System;
using System.Collections.Generic;
using System.Text;

namespace BoxApi.Infrastructure.Logging
{
    public static class EventNames
    {
        public const string COSMOSDB_REQUEST_THROTTLED = "COSMOSDB_REQUEST_THROTTLED";

        public const string REQUEST_CREATED = "REQUEST_CREATED";
        public const string BoxApi_REQUEST_SENT = "BoxApi_REQUEST_SENT";
        public const string BoxApi_RESPONSE_RECEIVED = "BoxApi_RESPONSE_RECEIVED";
    }
}
