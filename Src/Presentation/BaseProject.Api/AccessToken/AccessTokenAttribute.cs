using Microsoft.Azure.WebJobs.Description;
using System;
using System.Reflection;

namespace BaseProject.Api.AccessToken
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class AccessTokenAttribute : Attribute
    {        
    }
}
