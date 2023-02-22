using Assignment.Persistance.Base;
using AutoMapper;
using BaseProject.Api;
using BaseProject.Api.AccessToken;
using BaseProject.Application.Infrastructure;
using BaseProject.Application.Interfaces;
using BaseProject.Infrastructure;
using BaseProject.Infrastructure.Logging;
using BaseProject.Infrastructure.Proxy;
using BaseProject.Infrastructure.Settings;
using BaseProject.Infrastructure.SmartID;
using BaseProject.Persistance;
using BaseProject.Persistance.Extensions;
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
namespace BaseProject.Api
{
    public class Startup : FunctionsStartup
    {
        private const string _BaseProjectCosmosDbEndPoint = AppSettingsKeys.BaseProjectCosmosDbEndpoint;
        private const string _BaseProjectCosmosDbPrimaryKey = AppSettingsKeys.BaseProjectCosmosDbPrimaryKey;
        private const string _cosmosDatabaseName = "BaseProject";
        private const string _BaseProjectsCollectionName = "BaseProjects";
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
            services.AddTransient(typeof(IRepository), typeof(BaseProjectRepository));
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
            collectionNames.Add(_BaseProjectsCollectionName);
            services.AddCosmosDb(serviceEndpoint, authKey, databaseName, collectionNames);

            services.AddHealthChecks(checks =>
            {
                checks.AddCosmosDbCheck(serviceEndpoint, authKey, TimeSpan.FromMinutes(1));
            });
        }

        private static (Uri cosmosEndpoint, string cosmosKey) RetrieveCosmosDBAccess(SettingsReader settingsReader)
        {
            var serviceEndpoint = settingsReader.ReadSetting(_BaseProjectCosmosDbEndPoint);
            var authKey = settingsReader.ReadSetting(_BaseProjectCosmosDbPrimaryKey);
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
