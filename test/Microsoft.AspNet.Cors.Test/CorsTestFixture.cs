// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.PlatformAbstractions;

namespace Microsoft.AspNet.Cors.Infrastructure
{
    public class CorsTestFixture : IDisposable
    {
        private readonly TestServer _server;

        public CorsTestFixture(object startupInstance)
        {
            var startupTypeInfo = startupInstance.GetType().GetTypeInfo();
            var configureApplication = (Action<IApplicationBuilder>)startupTypeInfo
                .DeclaredMethods
                .FirstOrDefault(m => m.Name == "Configure" && m.GetParameters().Length == 1)
                ?.CreateDelegate(typeof(Action<IApplicationBuilder>), startupInstance);
            if (configureApplication == null)
            {
                var configureWithLogger = (Action<IApplicationBuilder, ILoggerFactory>)startupTypeInfo
                    .DeclaredMethods
                    .FirstOrDefault(m => m.Name == "Configure" && m.GetParameters().Length == 2)
                    ?.CreateDelegate(typeof(Action<IApplicationBuilder, ILoggerFactory>), startupInstance);
                Debug.Assert(configureWithLogger != null);

                configureApplication = application => configureWithLogger(application, NullLoggerFactory.Instance);
            }

            var buildServices = (Func<IServiceCollection, IServiceProvider>)startupTypeInfo
                .DeclaredMethods
                .FirstOrDefault(m => m.Name == "ConfigureServices" && m.ReturnType == typeof(IServiceProvider))
                ?.CreateDelegate(typeof(Func<IServiceCollection, IServiceProvider>), startupInstance);
            if (buildServices == null)
            {
                var configureServices = (Action<IServiceCollection>)startupTypeInfo
                    .DeclaredMethods
                    .FirstOrDefault(m => m.Name == "ConfigureServices" && m.ReturnType == typeof(void))
                    ?.CreateDelegate(typeof(Action<IServiceCollection>), startupInstance);
                Debug.Assert(configureServices != null);

                buildServices = services =>
                {
                    configureServices(services);
                    return services.BuildServiceProvider();
                };
            }

            _server = TestServer.Create(
                configureApplication,
                configureServices: InitializeServices(startupTypeInfo.Assembly, buildServices));

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }

        private Func<IServiceCollection, IServiceProvider> InitializeServices(
            Assembly startupAssembly,
            Func<IServiceCollection, IServiceProvider> buildServices)
        {
            var libraryManager = PlatformServices.Default.LibraryManager;

            // When an application executes in a regular context, the application base path points to the root
            // directory where the application is located, for example .../test/WebSites/CorsMiddlewareWebSite/.
            // However, when executing an application as part of a test, the ApplicationBasePath of the
            // IApplicationEnvironment points to the root folder of the test project and not the loaded application.
            //
            // To compensate, we need to calculate the correct project path and override the application
            // environment value so that components like the view engine work properly in the context of the test.
            var applicationName = startupAssembly.GetName().Name;
            var library = libraryManager.GetLibrary(applicationName);
            var applicationRoot = Path.GetDirectoryName(library.Path);

            var applicationEnvironment = PlatformServices.Default.Application;

            return (services) =>
            {
                services.AddSingleton<IApplicationEnvironment>(
                    new TestApplicationEnvironment(applicationEnvironment, applicationName, applicationRoot));

                var hostingEnvironment = new HostingEnvironment();
                hostingEnvironment.Initialize(applicationRoot, new WebHostOptions(), configuration: null);
                services.AddSingleton<IHostingEnvironment>(hostingEnvironment);

                return buildServices(services);
            };
        }

        // An application environment that overrides the base path of the original application environment in order to
        // make it point to the folder of the original web application. All components can thus act as if they were
        // executing in a regular context.
        private class TestApplicationEnvironment : IApplicationEnvironment
        {
            private readonly IApplicationEnvironment _original;

            public TestApplicationEnvironment(IApplicationEnvironment original, string name, string basePath)
            {
                _original = original;
                ApplicationName = name;
                ApplicationBasePath = basePath;
            }

            public string ApplicationName { get; }

            public string ApplicationVersion
            {
                get
                {
                    return _original.ApplicationVersion;
                }
            }

            public string ApplicationBasePath { get; }

            public FrameworkName RuntimeFramework
            {
                get
                {
                    return _original.RuntimeFramework;
                }
            }
        }
    }
}
