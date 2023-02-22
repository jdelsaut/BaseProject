using Assignment.Persistance.Base;
using AutoMapper;
using BoxApi.Api;
using BoxApi.Api.AccessToken;
using BoxApi.Application.Infrastructure;
using BoxApi.Application.Interfaces;
using BoxApi.Infrastructure;
using BoxApi.Infrastructure.Logging;
using BoxApi.Infrastructure.Proxy;
using BoxApi.Infrastructure.Settings;
using BoxApi.Infrastructure.SmartID;
using BoxApi.Persistance;
using BoxApi.Persistance.Extensions;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

[assembly: WebJobsStartup(typeof(Startup))]
[assembly: WebJobsStartup(typeof(StartupWebJob))]
namespace BoxApi.Api
{
    public class Startup : FunctionsStartup
    {
        private const string _BoxApiCosmosDbEndPoint = AppSettingsKeys.BoxApiCosmosDbEndpoint;
        private const string _BoxApiCosmosDbPrimaryKey = AppSettingsKeys.BoxApiCosmosDbPrimaryKey;
        private const string _cosmosDatabaseName = "BoxApi";
        private const string _BoxApisCollectionName = "BoxApis";
        private static string _proxyUrl = AppSettingsKeys.LocalHttpProxy;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            SettingsReader settingsReader = new SettingsReader();

            IServiceCollection services = builder.Services;
            RegisterRepository(services, settingsReader);
            RegisterMediatorInjections(services);
            RegisterValidators(services);
            RegisterApplicationServices(services);
            RegisterHttpRequestServices(services, settingsReader);
        }

        private static void RegisterRepository(IServiceCollection services, SettingsReader settingsReader)
        {
            RegisterCosmosDB(services, settingsReader);
            services.AddTransient(typeof(IRepository), typeof(BoxApiRepository));
        }

        private static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddTransient(typeof(ISettingsReader), typeof(SettingsReader));
            services.AddTransient(typeof(IInsightsLogger), typeof(ApplicationInsightsLogger));
            services.AddTransient(typeof(ISmartIDGenerator), typeof(SmartIDGenerator));
        }

        private static void RegisterValidators(IServiceCollection services)
        {
        }

        private static void RegisterMediatorInjections(IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        }

        private static void RegisterCosmosDB(IServiceCollection services, SettingsReader settingsReader)
        {
            (Uri serviceEndpoint, string authKey) = RetrieveCosmosDBAccess(settingsReader);
            var databaseName = _cosmosDatabaseName;
            var collectionNames = new List<string>();
            collectionNames.Add(_BoxApisCollectionName);
            services.AddCosmosDb(serviceEndpoint, authKey, databaseName, collectionNames);

            services.AddHealthChecks(checks =>
            {
                checks.AddCosmosDbCheck(serviceEndpoint, authKey, TimeSpan.FromMinutes(1));
            });
        }

        private static (Uri cosmosEndpoint, string cosmosKey) RetrieveCosmosDBAccess(SettingsReader settingsReader)
        {
            var serviceEndpoint = settingsReader.ReadSetting(_BoxApiCosmosDbEndPoint);
            var authKey = settingsReader.ReadSetting(_BoxApiCosmosDbPrimaryKey);
            return (new Uri(serviceEndpoint), authKey);
        }

        private static void RegisterHttpRequestServices(IServiceCollection services, SettingsReader settingsReader)
        {
            var proxy = settingsReader.ReadSetting(_proxyUrl);

            var handler = new ProxyAwareHttpClientHandler(proxy);
            var httpClient = new HttpClient(handler);
            var tokenGenerator = new TokenGenerator(settingsReader, httpClient);

            services.AddSingleton<ITokenGenerator>(tokenGenerator);
            services.AddSingleton(typeof(IHttpRequestRunner<,>), typeof(HttpRequestRunner<,>));
        }
    }

    internal class StartupWebJob : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddAccessTokenBinding();
        }
    }
}
