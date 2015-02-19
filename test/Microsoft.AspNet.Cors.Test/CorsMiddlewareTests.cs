// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Cors.Core;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.TestHost;
using Xunit;

namespace Microsoft.AspNet.Cors.Test
{
    public class CorsMiddlewareTests
    {
        [Fact]
        public async Task Middleware_With_AllowAll()
        {
            using (var server = TestServer.Create(app =>
            {
                app.UseCors(CorsOptions.AllowAll);
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Cross origin response");
                });
            }))
            {
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5001/")
                    .AddHeader(CorsConstants.AccessControlRequestMethod, "PUT")
                    .SendAsync(CorsConstants.PreflightHttpMethod);

                response.EnsureSuccessStatusCode();
                Assert.Equal(3, response.Headers.Count());
                Assert.Equal("http://localhost:5001/", response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
                Assert.Equal("PUT", response.Headers.GetValues(CorsConstants.AccessControlAllowMethods).FirstOrDefault());
                Assert.Equal("true", response.Headers.GetValues(CorsConstants.AccessControlAllowCredentials).FirstOrDefault());

                // Actual request.
                response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5001/")
                    .SendAsync("PUT");

                response.EnsureSuccessStatusCode();
                Assert.Equal(2, response.Headers.Count());
                Assert.Equal("Cross origin response", await response.Content.ReadAsStringAsync());
                Assert.Equal("http://localhost:5001/", response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
                Assert.Equal("true", response.Headers.GetValues(CorsConstants.AccessControlAllowCredentials).FirstOrDefault());
            }
        }

        [Fact]
        public async Task Middleware_With_AllowRestricted()
        {
            var options = new CorsOptions()
            {
                PolicyProvider = new CorsPolicyProvider()
                {
                    PolicyResolver = context =>
                    {
                        var policy = new CorsPolicy();
                        policy.Origins.Add("http://localhost:5001/");
                        policy.Methods.Add("PUT");
                        policy.Headers.Add("Header1");
                        policy.ExposedHeaders.Add("AllowedHeader");
                        return Task.FromResult<ICorsPolicy>(policy);
                    }
                }
            };

            using (var server = TestServer.Create(app =>
            {
                app.UseCors(options);
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Cross origin response");
                });
            }))
            {
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5001/sub")
                    .AddHeader(CorsConstants.AccessControlRequestMethod, "PUT")
                    .SendAsync(CorsConstants.PreflightHttpMethod);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5001/")
                    .AddHeader(CorsConstants.AccessControlRequestMethod, "PUT")
                    .SendAsync(CorsConstants.PreflightHttpMethod);

                response.EnsureSuccessStatusCode();
                Assert.Equal(2, response.Headers.Count());
                Assert.Equal("http://localhost:5001/", response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
                Assert.Equal("PUT", response.Headers.GetValues(CorsConstants.AccessControlAllowMethods).FirstOrDefault());

                // Actual request.
                response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5001/")
                    .SendAsync("PUT");

                response.EnsureSuccessStatusCode();
                Assert.Equal(2, response.Headers.Count());
                Assert.Equal("Cross origin response", await response.Content.ReadAsStringAsync());
                Assert.Equal("http://localhost:5001/", response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
                Assert.Equal("AllowedHeader", response.Headers.GetValues(CorsConstants.AccessControlExposeHeaders).FirstOrDefault());
            }
        }
    }
}