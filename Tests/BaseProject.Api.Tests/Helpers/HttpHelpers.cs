using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace BoxApi.Api.Tests.Helpers
{
    public static class HttpHelpers
    {
        public static Mock<HttpRequest> CreateMockHttpRequest(object body, HeaderDictionary headers)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);
            mockRequest.SetupGet(x => x.Headers)
                .Returns(headers);

            return mockRequest;
        }

        public static HttpRequestMessage CreateMockHttpRequestMessage(string body)
        {

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://not_relevant_uri.com"),
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            var services = new ServiceCollection()
                .AddMvc()
                .AddWebApiConventions()
                .Services
                .BuildServiceProvider();

            httpRequestMessage.Properties.Add(nameof(HttpContext), new DefaultHttpContext
            {
                RequestServices = services
            });

            return httpRequestMessage;
        }
    }
}
