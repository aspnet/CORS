// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Cors.Core;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.TestHost;
using Microsoft.Framework.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Cors.Tests
{
    public class CorsMiddlewareTests
    {
        [Fact]
        public async Task CorsRequest_MatchPolicy_SetsResponseHeaders()
        {
            // Arrange
            using (var server = TestServer.Create(app =>
            {
                app.UseCors(builder => 
                    builder.WithOrigins("http://localhost:5001")
                           .WithMethods("PUT")
                           .WithHeaders("Header1")
                           .WithExposedHeaders("AllowedHeader"));
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Cross origin response");
                });
            },
            services => services.AddCors()))
            {
                // Act
                // Actual request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5001")
                    .SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Equal(2, response.Headers.Count());
                Assert.Equal("Cross origin response", await response.Content.ReadAsStringAsync());
                Assert.Equal("http://localhost:5001", response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
                Assert.Equal("AllowedHeader", response.Headers.GetValues(CorsConstants.AccessControlExposeHeaders).FirstOrDefault());
            }
        }

        [Fact]
        public async Task PreFlight_MatchesPolicy_SetsResponseHeaders()
        {
            // Arrange
            var policy = new CorsPolicy();
            policy.Origins.Add("http://localhost:5001");
            policy.Methods.Add("PUT");
            policy.Headers.Add("Header1");
            policy.ExposedHeaders.Add("AllowedHeader");

            using (var server = TestServer.Create(app =>
            {
                app.UseCors("customPolicy");
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Cross origin response");
                });
            },
            services =>
            {
                services.AddCors();
                services.ConfigureCors(options =>
                {
                    options.AddPolicy("customPolicy", policy);
                });
            }))
            {
                // Act
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5001")
                    .AddHeader(CorsConstants.AccessControlRequestMethod, "PUT")
                    .SendAsync(CorsConstants.PreflightHttpMethod);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Equal(2, response.Headers.Count());
                Assert.Equal("http://localhost:5001", response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
                Assert.Equal("PUT", response.Headers.GetValues(CorsConstants.AccessControlAllowMethods).FirstOrDefault());
            }
        }

        [Fact]
        public async Task PreFlightRequest_DoesNotMatchPolicy_DoesNotSetHeaders()
        {
            // Arrange
            using (var server = TestServer.Create(app =>
            {
                app.UseCors(builder =>
                    builder.WithOrigins("http://localhost:5001")
                           .WithMethods("PUT")
                           .WithHeaders("Header1")
                           .WithExposedHeaders("AllowedHeader"));
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Cross origin response");
                });
            },
            services => services.AddCors()))
            {
                // Act
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5002")
                    .AddHeader(CorsConstants.AccessControlRequestMethod, "PUT")
                    .SendAsync(CorsConstants.PreflightHttpMethod);

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                Assert.Empty(response.Headers);
            }
        }

        [Fact]
        public async Task CorsRequest_DoesNotMatchPolicy_DoesNotSetHeaders()
        {
            // Arrange
            using (var server = TestServer.Create(app =>
            {
                app.UseCors(builder => 
                    builder.WithOrigins("http://localhost:5001")
                           .WithMethods("PUT")
                           .WithHeaders("Header1")
                           .WithExposedHeaders("AllowedHeader"));
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Cross origin response");
                });
            },
            services => services.AddCors()))
            {
                // Act
                // Actual request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://localhost:5002")
                    .SendAsync("PUT");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Empty(response.Headers);
            }
        }

        [Fact]
        public async Task Uses_PolicyProvider_AsFallback()
        {
            // Arrange 
            var corsService = Mock.Of<ICorsService>();
            var mockProvider = new Mock<ICorsPolicyProvider>();
            mockProvider.Setup(o => o.GetPolicyAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
                .Returns(Task.FromResult<CorsPolicy>(null))
                .Verifiable();

            var middleware = new CorsMiddleware(
                Mock.Of<RequestDelegate>(),
                corsService,
                mockProvider.Object,
                policyName: null);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add(CorsConstants.Origin, new[] { "http://example.com" });

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            mockProvider.Verify(
                o => o.GetPolicyAsync(It.IsAny<HttpContext>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task DoesNotSetHeaders_ForNoPolicy()
        {
            // Arrange 
            var corsService = Mock.Of<ICorsService>();
            var mockProvider = new Mock<ICorsPolicyProvider>();
            mockProvider.Setup(o => o.GetPolicyAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
                .Returns(Task.FromResult<CorsPolicy>(null))
                .Verifiable();

            var middleware = new CorsMiddleware(
                Mock.Of<RequestDelegate>(),
                corsService,
                mockProvider.Object,
                policyName: null);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add(CorsConstants.Origin, new[] { "http://example.com" });

            // Act
            await middleware.Invoke(httpContext);

            // Assert
            Assert.Equal(200, httpContext.Response.StatusCode);
            Assert.Empty(httpContext.Response.Headers);
            mockProvider.Verify(
                o => o.GetPolicyAsync(It.IsAny<HttpContext>(), It.IsAny<string>()),
                Times.Once);
        }
    }
}