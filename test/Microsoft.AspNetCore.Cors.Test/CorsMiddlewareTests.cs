﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Cors.Infrastructure
{
    public class CorsMiddlewareTests
    {
        private const string OriginUrl = "http://api.example.com";

        [Theory]
        [InlineData("PuT")]
        [InlineData("PUT")]
        public async Task CorsRequest_MatchesPolicy_OnCaseInsensitiveAccessControlRequestMethod(string accessControlRequestMethod)
        {
            // Arrange
            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors(builder =>
                        builder.WithOrigins(OriginUrl)
                               .WithMethods("PUT"));
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services => services.AddCors());

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Actual request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, OriginUrl)
                    .SendAsync(accessControlRequestMethod);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Single(response.Headers);
                Assert.Equal("Cross origin response", await response.Content.ReadAsStringAsync());
                Assert.Equal(OriginUrl, response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
            }
        }

        [Fact]
        public async Task CorsRequest_MatchPolicy_SetsResponseHeaders()
        {
            // Arrange
            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors(builder =>
                        builder.WithOrigins(OriginUrl)
                               .WithMethods("PUT")
                               .WithHeaders("Header1")
                               .WithExposedHeaders("AllowedHeader"));
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services => services.AddCors());

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Actual request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, OriginUrl)
                    .SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Equal(2, response.Headers.Count());
                Assert.Equal("Cross origin response", await response.Content.ReadAsStringAsync());
                Assert.Equal(OriginUrl, response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
                Assert.Equal("AllowedHeader", response.Headers.GetValues(CorsConstants.AccessControlExposeHeaders).FirstOrDefault());
            }
        }

        [Theory]
        [InlineData("OpTions")]
        [InlineData("OPTIONS")]
        public async Task PreFlight_MatchesPolicy_OnCaseInsensitiveOptionsMethod(string preflightMethod)
        {
            // Arrange
            var policy = new CorsPolicy();
            policy.Origins.Add(OriginUrl);
            policy.Methods.Add("PUT");

            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors("customPolicy");
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("customPolicy", policy);
                    });
                });

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, OriginUrl)
                    .SendAsync(preflightMethod);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Single(response.Headers);
                Assert.Equal(OriginUrl, response.Headers.GetValues(CorsConstants.AccessControlAllowOrigin).FirstOrDefault());
            }
        }

        [Fact]
        public async Task PreFlight_MatchesPolicy_SetsResponseHeaders()
        {
            // Arrange
            var policy = new CorsPolicy();
            policy.Origins.Add(OriginUrl);
            policy.Methods.Add("PUT");
            policy.Headers.Add("Header1");
            policy.ExposedHeaders.Add("AllowedHeader");

            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors("customPolicy");
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("customPolicy", policy);
                    });
                });

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, OriginUrl)
                    .AddHeader(CorsConstants.AccessControlRequestMethod, "PUT")
                    .SendAsync(CorsConstants.PreflightHttpMethod);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Collection(
                    response.Headers.OrderBy(h => h.Key),
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowHeaders, kvp.Key);
                        Assert.Equal(new[] { "Header1" }, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowMethods, kvp.Key);
                        Assert.Equal(new[] { "PUT" }, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowOrigin, kvp.Key);
                        Assert.Equal(new[] { OriginUrl }, kvp.Value);
                    });
            }
        }

        [Fact]
        public async Task PreFlight_WithCredentialsAllowed_ReturnsWildcardValues()
        {
            // Arrange
            var policy = new CorsPolicyBuilder(OriginUrl)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .Build();

            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors("customPolicy");
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("customPolicy", policy);
                    });
                });

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, OriginUrl)
                    .AddHeader(CorsConstants.AccessControlRequestMethod, "PUT")
                    .AddHeader(CorsConstants.AccessControlRequestHeaders, "X-Test1,X-Test2")
                    .SendAsync(CorsConstants.PreflightHttpMethod);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Collection(
                    response.Headers.OrderBy(h => h.Key),
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowCredentials, kvp.Key);
                        Assert.Equal(new[] { "true" }, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowHeaders, kvp.Key);
                        Assert.Equal(new[] { "*" }, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowMethods, kvp.Key);
                        Assert.Equal(new[] { "*" }, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowOrigin, kvp.Key);
                        Assert.Equal(new[] { OriginUrl }, kvp.Value);
                    });
            }
        }

        [Fact]
        public async Task PreFlightRequest_DoesNotMatchPolicy_DoesNotSetHeadersAndReturnsForbidden()
        {
            // Arrange
            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors(builder =>
                        builder.WithOrigins(OriginUrl)
                               .WithMethods("PUT")
                               .WithHeaders("Header1")
                               .WithExposedHeaders("AllowedHeader"));
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services => services.AddCors());

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://test.example.com")
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
            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors(builder =>
                        builder.WithOrigins(OriginUrl)
                               .WithMethods("PUT")
                               .WithHeaders("Header1")
                               .WithExposedHeaders("AllowedHeader"));
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services => services.AddCors());

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Actual request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, "http://test.example.com")
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
            var loggerFactory = NullLoggerFactory.Instance;
            mockProvider.Setup(o => o.GetPolicyAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
                .Returns(Task.FromResult<CorsPolicy>(null))
                .Verifiable();

            var middleware = new CorsMiddleware(
                Mock.Of<RequestDelegate>(),
                corsService,
                mockProvider.Object,
                loggerFactory,
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
            var loggerFactory = NullLoggerFactory.Instance;
            mockProvider.Setup(o => o.GetPolicyAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
                .Returns(Task.FromResult<CorsPolicy>(null))
                .Verifiable();

            var middleware = new CorsMiddleware(
                Mock.Of<RequestDelegate>(),
                corsService,
                mockProvider.Object,
                loggerFactory,
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

        [Fact]
        public async Task PreFlight_MatchesDefaultPolicy_SetsResponseHeaders()
        {
            // Arrange
            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors();
                    app.Run(async context =>
                    {
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(policyBuilder =>
                        {
                            policyBuilder
                            .WithOrigins(OriginUrl)
                            .WithMethods("PUT")
                            .WithHeaders("Header1")
                            .WithExposedHeaders("AllowedHeader")
                            .Build();
                        });
                        options.AddPolicy("policy2", policyBuilder =>
                        {
                            policyBuilder
                            .WithOrigins("http://test.example.com")
                            .Build();
                        });
                    });
                });

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Preflight request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, OriginUrl)
                    .AddHeader(CorsConstants.AccessControlRequestMethod, "PUT")
                    .SendAsync(CorsConstants.PreflightHttpMethod);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Collection(
                    response.Headers.OrderBy(h => h.Key),
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowHeaders, kvp.Key);
                        Assert.Equal(new[] { "Header1" }, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowMethods, kvp.Key);
                        Assert.Equal(new[] { "PUT" }, kvp.Value);
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowOrigin, kvp.Key);
                        Assert.Equal(new[] { OriginUrl }, kvp.Value);
                    });
            }
        }

        [Fact]
        public async Task CorsRequest_SetsResponseHeaders()
        {
            // Arrange
            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseCors(builder =>
                        builder.WithOrigins(OriginUrl)
                            .WithMethods("PUT")
                            .WithHeaders("Header1")
                            .WithExposedHeaders("AllowedHeader"));
                    app.Run(async context =>
                    {
                        context.Response.Headers.Add("Test", "Should-Appear");
                        await context.Response.WriteAsync("Cross origin response");
                    });
                })
                .ConfigureServices(services => services.AddCors());

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Actual request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, OriginUrl)
                    .SendAsync("PUT");

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Collection(
                    response.Headers.OrderBy(o => o.Key),
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowOrigin, kvp.Key);
                        Assert.Equal(OriginUrl, Assert.Single(kvp.Value));
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlExposeHeaders, kvp.Key);
                        Assert.Equal("AllowedHeader", Assert.Single(kvp.Value));
                    },
                    kvp =>
                    {
                        Assert.Equal("Test", kvp.Key);
                        Assert.Equal("Should-Appear", Assert.Single(kvp.Value));
                    });

                Assert.Equal("Cross origin response", await response.Content.ReadAsStringAsync());
            }
        }

        [Fact]
        public async Task CorsRequest_SetsResponseHeader_IfExceptionHandlerClearsResponse()
        {
            // Arrange
            var exceptionSeen = true;
            var hostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    // Simulate ExceptionHandler middleware
                    app.Use(async (context, next) =>
                    {
                        try
                        {
                            await next();
                        }
                        catch (Exception)
                        {
                            exceptionSeen = true;
                            context.Response.Clear();
                            context.Response.StatusCode = 500;
                        }
                    });

                    app.UseCors(builder =>
                        builder.WithOrigins(OriginUrl)
                            .WithMethods("PUT")
                            .WithHeaders("Header1")
                            .WithExposedHeaders("AllowedHeader"));

                    app.Run(context =>
                    {
                        context.Response.Headers.Add("Test", "Should-Not-Exist");
                        throw new Exception("Runtime error");
                    });
                })
                .ConfigureServices(services => services.AddCors());

            using (var server = new TestServer(hostBuilder))
            {
                // Act
                // Actual request.
                var response = await server.CreateRequest("/")
                    .AddHeader(CorsConstants.Origin, OriginUrl)
                    .SendAsync("PUT");

                // Assert
                Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                Assert.True(exceptionSeen, "We expect exception middleware to have executed");

                Assert.Collection(
                    response.Headers.OrderBy(o => o.Key),
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlAllowOrigin, kvp.Key);
                        Assert.Equal(OriginUrl, Assert.Single(kvp.Value));
                    },
                    kvp =>
                    {
                        Assert.Equal(CorsConstants.AccessControlExposeHeaders, kvp.Key);
                        Assert.Equal("AllowedHeader", Assert.Single(kvp.Value));
                    });
            }
        }
    }
}
