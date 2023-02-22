using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace BaseProject.Persistance.Tests.Helpers
{
    public static class DocumentExceptionsHelper
    {

        public static DocumentClientException CreateDocumentClientExceptionForTesting(
                                           Error error, HttpStatusCode httpStatusCode)
        {
            var type = typeof(DocumentClientException);

            var documentClientExceptionInstance = type.Assembly.CreateInstance(
                type.FullName,
                false, BindingFlags.Instance | BindingFlags.NonPublic, 
                null,
                new object[] { error, (HttpResponseHeaders)null, httpStatusCode }, 
                null, 
                null);

            return (DocumentClientException)documentClientExceptionInstance;
        }
    }
}
