using Microsoft.Azure.WebJobs;
using System;

namespace BaseProject.Api.AccessToken
{
    public static class AccessTokenExtensions
    {
        //create an extension method that lets add the binding to the host’s IWebJobsBuilde
        public static IWebJobsBuilder AddAccessTokenBinding(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<AccessTokenExtensionProvider>();
            return builder;
        }
    }
}
