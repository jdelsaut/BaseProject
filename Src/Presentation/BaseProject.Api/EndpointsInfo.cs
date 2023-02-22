using BaseProject.Api.Functions;
using BaseProject.Infrastructure.Scopes;
using System;
using System.Collections.Concurrent;

namespace BaseProject.Api
{
    public static class EndpointsInfo
    {
        private readonly static string ScopesKeyPrefix = "type";

        public static ConcurrentDictionary<string, string> Endpoints { get; } = new ConcurrentDictionary<string, string>();

        static EndpointsInfo()
        {
            //Common endpoints authentication information
            Endpoints.TryAdd(nameof(VersionFunctions.Version), BuildScopeList());

            //Specific Roadside scopes
            Endpoints.TryAdd(BuildAccessKey(nameof(BaseProjectFunctions.BaseProject_Starter), ScopesKeyPrefix), Scopes.BaseProjectWrite);
            Endpoints.TryAdd(BuildAccessKey(nameof(BaseProjectFunctions.GetBaseProjectById), ScopesKeyPrefix), Scopes.BaseProjectRead);
        }

        public static string BuildAccessKey(string functionName, string keyPrefix)
        {
            if (string.IsNullOrEmpty(keyPrefix))
                return BuildKeyWithFunctionName(functionName);

            return $"{keyPrefix.ToLowerInvariant()}/{functionName}";
        }

        public static string BuildKeyWithFunctionName(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException(nameof(functionName));

            return $"{functionName}";
        }

        private static string BuildScopeList()
        {
            return string.Join(' ', Scopes.RetrieveAll());
        }
    }
}
