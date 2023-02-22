using BaseProject.Acceptance.Exceptions;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BaseProject.Acceptance.Helpers
{
    public class HttpRequestWrapper
    {
        private const int RetryCount = 30;
        private const int WaitSecondsBeforeRetry = 4;
        private RestRequest _restRequest;
        private RestClient _restClient;
        private readonly string _endpoint;
        private readonly bool _uselocalProxy;
        private readonly string _bearerToken;

        public HttpRequestWrapper(string endPoint, bool useLocalProxy, string barearToken)
        {
            _restRequest = new RestRequest();
            _endpoint = endPoint;
            _uselocalProxy = useLocalProxy;
            this._bearerToken = barearToken;
        }

        public HttpRequestWrapper SetResourse(string resource)
        {
            _restRequest.Resource = resource;
            return this;
        }

        public HttpRequestWrapper SetMethod(Method method)
        {
            _restRequest.Method = method;
            return this;
        }

        public HttpRequestWrapper AddHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                _restRequest.AddParameter(header.Key, header.Value, ParameterType.HttpHeader);
            }
            return this;
        }

        public HttpRequestWrapper AddJsonContent(object data)
        {
            _restRequest.RequestFormat = DataFormat.Json;
            _restRequest.AddHeader("Content-Type", "application/json");

            _restRequest.AddJsonBody(data);
            return this;
        }

        public HttpRequestWrapper AddEtagHeader(string value)
        {
            _restRequest.AddHeader("If-None-Match", value);
            return this;
        }


        public HttpRequestWrapper AddParameter(string name, object value)
        {
            _restRequest.AddParameter(name, value);
            return this;
        }

        public HttpRequestWrapper AddParameters(IDictionary<string, object> parameters)
        {
            foreach (var item in parameters)
            {
                _restRequest.AddParameter(item.Key, item.Value);
            }
            return this;
        }

        public IRestResponse Execute()
        {
            try
            {
                LogRequest(_restRequest);

                _restClient = CreateCustomClient();

                var response = _restClient.Execute(_restRequest);
                HandleHttpErrorsCode(response);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public T Execute<T>()
        {
            LogRequest(_restRequest);
            _restClient = CreateCustomClient();

            var response = _restClient.Execute(_restRequest);
            HandleHttpErrorsCode(response);
            var data = JsonConvert.DeserializeObject<T>(response.Content);
            return data;
        }

        public T Execute<T>(Func<T, bool> executeWhile)
        {
            LogRequest(_restRequest);

            _restClient = CreateCustomClient();

            var retryPolicy = Policy
                .Handle<TransientHttpCallException>()
                .OrResult<bool>(result => result == false)
                .WaitAndRetryAsync(RetryCount, i => TimeSpan.FromSeconds(WaitSecondsBeforeRetry));

            T data = (T)typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]);

            retryPolicy.ExecuteAsync(async () =>
            {
                var response = _restClient.Execute(_restRequest);
                HandleHttpErrorsCode(response);

                data = JsonConvert.DeserializeObject<T>(response.Content);
                return executeWhile.Invoke(data);
            }).GetAwaiter().GetResult();

            return data;
        }

        private RestClient CreateCustomClient()
        {
            if (_restClient != null)
                return _restClient;

            _restClient = new RestClient(_endpoint);
            if (_uselocalProxy)
                _restClient.Proxy = new WebProxy("127.0.0.1", 3128);

            _restClient.AddDefaultHeader("Authorization", string.Format("Bearer {0}", _bearerToken));
            _restClient.UseSerializer(() => new JsonNetSerializer());

            return _restClient;
        }

        private void HandleHttpErrorsCode(IRestResponse responseReceived)
        {
            var validHttpResponseCode = new List<HttpStatusCode>
            {
                HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted, HttpStatusCode.NoContent, HttpStatusCode.Found
            };

            if (validHttpResponseCode.Contains(responseReceived.StatusCode))
            {
                return;
            }

            if (responseReceived.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new NoTransientHttpCallException(responseReceived.Content);
            }

            throw new Exception(responseReceived.Content);
        }

        private void LogRequest(RestRequest request)
        {
            var rq = $"{request?.Method} : {this._endpoint}{request?.Resource}";
            Console.WriteLine(rq);
        }
    }
    
}
