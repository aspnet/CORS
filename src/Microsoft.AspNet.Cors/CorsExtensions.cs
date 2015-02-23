// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Cors.Core;

namespace Microsoft.AspNet.Builder
{
    public static class CorsExtensions
    {
        /// <summary>
        /// Adds a CORS middleware to your web application pipeline to allow cross domain requests.
        /// </summary>
        /// <param name="app">The IApplicationBuilder passed to your Configure method</param>
        /// <param name="options">An options class that controls the middleware behavior</param>
        /// <returns>The original app parameter</returns>
        public static IApplicationBuilder UseCors([NotNull]this IApplicationBuilder app, [NotNull]CorsOptions options)
        {
            return app.UseMiddleware<CorsMiddleware>(options);
        }

        /// <summary>
        /// Adds a CORS middleware to your web application pipeline to allow cross domain requests
        /// from the specified origins.
        /// </summary>
        /// <param name="app">The IApplicationBuilder passed to your Configure method</param>
        /// <param name="origins"></param>
        /// <returns>The original app parameter</returns>
        public static IApplicationBuilder UseCors([NotNull]this IApplicationBuilder app, [NotNull]params string[] origins)
        {
            var options = new CorsOptions()
            {
                PolicyProvider = new CorsPolicyProvider()
                {
                    PolicyResolver = context =>
                    {
                        var singleOriginPolicy = new CorsPolicy()
                        {
                            AllowAnyMethod = true,
                            AllowAnyHeader = true
                        };

                        foreach (var origin in origins)
                        {
                            singleOriginPolicy.Origins.Add(origin);
                        }

                        return Task.FromResult(singleOriginPolicy);
                    }
                }
            };

            return app.UseMiddleware<CorsMiddleware>(options);
        }
    }
}