using System;
using System.Net;
using System.Net.Http;

namespace BaseProject.Infrastructure.Proxy
{
    public class ProxyAwareHttpClientHandler : HttpClientHandler
    {
        public ProxyAwareHttpClientHandler(string proxyUrl)
        {
            if (proxyUrl != null)
            {
                Uri proxy = null;

                try
                {
                    proxy = new Uri(proxyUrl);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Invalid proxy url : {proxyUrl}", ex);
                }

                Proxy = new InternalProxy(proxy);
                UseProxy = true;
            }
            else
            {
                UseProxy = false;
                Proxy = null;
            }
        }

        private sealed class InternalProxy : IWebProxy
        {
            private readonly Uri _proxy;

            public InternalProxy(Uri proxy)
            {
                _proxy = proxy;
            }

            public ICredentials Credentials { get; set; }

            public Uri GetProxy(Uri destination)
            {
                return _proxy;
            }

            public bool IsBypassed(Uri host)
            {
                return GetProxy(host) == null;
            }
        }
    }
}

