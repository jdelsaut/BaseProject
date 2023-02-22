using System;

namespace BaseProject.Persistance.Options
{
    public class ConnectionStringOptions
    {
        public Uri ServiceEndpoint { get; set; }
        public string AuthKey { get; set; }

        public ConnectionStringOptions(Uri serviceEndpoint, string authKey)
        {
            ServiceEndpoint = serviceEndpoint;
            AuthKey = authKey;
        }

        public void Deconstruct(out Uri serviceEndpoint, out string authKey)
        {
            serviceEndpoint = ServiceEndpoint;
            authKey = AuthKey;
        }
    }
}
