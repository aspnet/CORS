// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.Http.Core;
using Xunit;

namespace Microsoft.AspNet.Cors.Core.Test
{
    public class CorsRequestContextTests
    {
        [Fact]
        public void Validate_CorsRequestContext_Defaults()
        {
            var corsRequestContext = new CorsRequestContext();
            Assert.False(corsRequestContext.IsPreflight);
            Assert.Null(corsRequestContext.AccessControlRequestMethod);
            Assert.Null(corsRequestContext.AccessControlRequestHeaders);
            Assert.Null(corsRequestContext.Host);
            Assert.Null(corsRequestContext.HttpMethod);
            Assert.Null(corsRequestContext.Origin);
            Assert.False(corsRequestContext.RequestPath.HasValue);
        }

        [Fact]
        public void Validate_Preflight_Request()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = CorsConstants.PreflightHttpMethod;
            httpContext.Request.Headers.Add(CorsConstants.Origin, new string[] { "http://localhost:5001/" });
            httpContext.Request.Headers.Add(CorsConstants.AccessControlRequestMethod, new string[] { "GET" });
            httpContext.Request.Headers.Add(CorsConstants.AccessControlRequestHeaders, new string[] { "Header1", "Header2" });
            var corsRequestContext = new CorsRequestContext(httpContext);
            Assert.True(corsRequestContext.IsPreflight);
            Assert.Equal("GET", corsRequestContext.AccessControlRequestMethod);
            Assert.Equal(new List<string>{ "Header1", "Header2" }, corsRequestContext.AccessControlRequestHeaders);
        }

        [Fact]
        public void Validate_Preflight_Request_Without_Method()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = CorsConstants.PreflightHttpMethod;
            httpContext.Request.Headers.Add(CorsConstants.Origin, new string[] { "http://localhost:5001/" });
            var corsRequestContext = new CorsRequestContext(httpContext);
            Assert.False(corsRequestContext.IsPreflight);
        }

        [Fact]
        public void Validate_Preflight_Request_Without_Headers()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = CorsConstants.PreflightHttpMethod;
            httpContext.Request.Headers.Add(CorsConstants.Origin, new string[] { "http://localhost:5001/" });
            httpContext.Request.Headers.Add(CorsConstants.AccessControlRequestMethod, new string[] { "GET" });
            var corsRequestContext = new CorsRequestContext(httpContext);
            Assert.True(corsRequestContext.IsPreflight);
        }

        [Fact]
        public void Validate_Non_Preflight_Request()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "GET";
            httpContext.Request.Headers.Add(CorsConstants.Origin, new string[] { "http://localhost:5001/" });
            var corsRequestContext = new CorsRequestContext(httpContext);
            Assert.False(corsRequestContext.IsPreflight);
        }
    }
}