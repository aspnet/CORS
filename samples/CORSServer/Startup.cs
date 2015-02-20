// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Cors.Core;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;

namespace CORSServer
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseErrorPage(ErrorPageOptions.ShowAll);

            var options = new CorsOptions()
            {
                PolicyProvider = new CorsPolicyProvider()
                {
                    PolicyResolver = context =>
                    {
                        var allowAllPolicy = new CorsPolicy()
                        {
                            AllowAnyOrigin = true,
                            AllowAnyMethod = true,
                            AllowAnyHeader = true
                        };

                        return Task.FromResult<ICorsPolicy>(allowAllPolicy);
                    }
                }
            };

            app.UseCors(options);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Cross origin response");
            });
        }
    }
}