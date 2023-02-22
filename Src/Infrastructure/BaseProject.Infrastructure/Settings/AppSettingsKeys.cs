namespace BaseProject.Infrastructure.Settings
{
    public static class AppSettingsKeys
    {
        //AppInsights instrumentation key
        public const string AppInsightsInstrumentationKey = "APPINSIGHTS_INSTRUMENTATIONKEY";

        public const string ApiVersion = "ApiVersion";
        public const string VersionDescription = "VersionDescription";
        public const string AuthHeaderName = "AUTH_HEADER_NAME";
        public const string BearerPrefix = "BEARER_PREFIX";
        public const string BaseProjectEndpoint = "BaseProject_ENDPOINT";

        //CosmosDB & Service bus settings keys
        public const string BaseProjectCosmosDbEndpoint = "BaseProjectCosmosDbEndpoint";
        public const string BaseProjectCosmosDbPrimaryKey = "BaseProjectCosmosDbPrimaryKey";
        public const string BaseProjectResponseListenerConnection = "BaseProjectResponseListenerConnection";

        //Authentication & Authorization related settings keys
        public const string RequireSignedJwt = "REQUIRE_SIGNED_JWT";
        public const string DiscoveryEndpoint = "DISCOVERY_ENDPOINT";
        public const string JwtIssuer = "JWT_ISSUER";
        public const string ShowPII = "SHOW_PII";
        public const string RequireLifetimeValidation = "REQUIRE_LIFETIME_VALIDATION";
        public const string RootAuthEndpoint = "ROOT_AUTH_ENDPOINT";
        public const string ClientId = "ClientId";
        public const string ClientSecret = "ClientSecret";

        //Static properties for local settings
        //Only local app setting for now is LOCAL_HTTP_PROXY
        public static string LocalHttpProxy => "LOCAL_HTTP_PROXY";
    }
}
