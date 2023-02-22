using Microsoft.Azure.WebJobs.Description;
using System;
using System.Reflection;

namespace BoxApi.Api.AccessToken
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class AccessTokenAttribute : Attribute
    {        
    }
}
