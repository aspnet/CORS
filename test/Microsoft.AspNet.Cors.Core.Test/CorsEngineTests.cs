// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Xunit;

namespace Microsoft.AspNet.Cors.Core.Test
{
    public class CorsEngineTests
    {
        public ICorsPolicy AllowAllPolicy
        {
            get
            {
                var policy = new CorsPolicy()
                {
                    AllowAnyOrigin = true,
                    AllowAnyHeader = true,
                    AllowAnyMethod = true,
                    PreflightMaxAge = 200
                };

                policy.ExposedHeaders.Add("AllowedHeader1");
                policy.ExposedHeaders.Add("AllowedHeader2");
                return policy;
            }
        }

        public ICorsPolicy AllowAllPolicyWithCredentials
        {
            get
            {
                var policy = AllowAllPolicy;
                policy.SupportsCredentials = true;
                return policy;
            }
        }

        public ICorsPolicy AllowSpecificUrlPolicy
        {
            get
            {
                var policy = new CorsPolicy();
                policy.Origins.Add("http://localhost:5001/sub");
                policy.Methods.Add("PUT");
                policy.Headers.Add("AllowedHeader1");
                policy.ExposedHeaders.Add("AllowedHeader3");
                policy.PreflightMaxAge = 300;
                return policy;
            }
        }

        [Fact]
        public void PreFlight_Request_With_AllowAll_Policy()
        {
            var engine = new CorsEngine();
            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/",
                AccessControlRequestMethod = "PUT"
            }, AllowAllPolicy);

            Assert.True(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Equal(3, headers.Count);
            Assert.Equal(CorsConstants.AnyOrigin, headers[CorsConstants.AccessControlAllowOrigin]);
            Assert.Equal("PUT", headers[CorsConstants.AccessControlAllowMethods]);
            Assert.Equal("200", headers[CorsConstants.AccessControlMaxAge]);
        }

        [Fact]
        public void Cors_Request_With_AllowAll_Policy()
        {
            var engine = new CorsEngine();

            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = "PUT",
                Origin = "http://localhost:5001/"
            }, AllowAllPolicy);

            Assert.True(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Equal(2, headers.Count);
            Assert.Equal(CorsConstants.AnyOrigin, headers[CorsConstants.AccessControlAllowOrigin]);
            Assert.Equal("AllowedHeader1,AllowedHeader2", headers[CorsConstants.AccessControlExposeHeaders]);
        }

        [Fact]
        public void PreFlight_Request_With_AllowAllWithCredentials_Policy()
        {
            var engine = new CorsEngine();

            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/",
                AccessControlRequestMethod = "PUT"
            }, AllowAllPolicyWithCredentials);

            Assert.True(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Equal(4, headers.Count);
            Assert.Equal("http://localhost:5001/", headers[CorsConstants.AccessControlAllowOrigin]);
            Assert.Equal("PUT", headers[CorsConstants.AccessControlAllowMethods]);
            Assert.Equal("true", headers[CorsConstants.AccessControlAllowCredentials]);
            Assert.Equal("200", headers[CorsConstants.AccessControlMaxAge]);
        }

        [Fact]
        public void Cors_Request_With_AllowAllWithCredentials_Policy()
        {
            var engine = new CorsEngine();

            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = "PUT",
                Origin = "http://localhost:5001/"
            }, AllowAllPolicyWithCredentials);

            Assert.True(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Equal(3, headers.Count);
            Assert.Equal("http://localhost:5001/", headers[CorsConstants.AccessControlAllowOrigin]);
            Assert.Equal("AllowedHeader1,AllowedHeader2", headers[CorsConstants.AccessControlExposeHeaders]);
            Assert.Equal("true", headers[CorsConstants.AccessControlAllowCredentials]);
        }

        [Fact]
        public void PreFlight_Request_With_SpecificUrl_Policy_For_Unallowed_Origin()
        {
            var engine = new CorsEngine();
            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/",
                AccessControlRequestMethod = "PUT"
            }, AllowSpecificUrlPolicy);

            Assert.False(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Empty(headers);
        }

        [Fact]
        public void Cors_Request_With_SpecificUrl_Policy_For_Unallowed_Origin()
        {
            var engine = new CorsEngine();
            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = "PUT",
                Origin = "http://localhost:5001/",
            }, AllowSpecificUrlPolicy);

            Assert.False(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Empty(headers);
        }

        [Fact]
        public void PreFlight_Request_With_SpecificUrl_Policy_For_Allowed_Origin()
        {
            var engine = new CorsEngine();
            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/sub",
                AccessControlRequestMethod = "PUT"
            }, AllowSpecificUrlPolicy);

            Assert.True(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Equal(3, headers.Count);
            Assert.Equal("http://localhost:5001/sub", headers[CorsConstants.AccessControlAllowOrigin]);
            Assert.Equal("PUT", headers[CorsConstants.AccessControlAllowMethods]);
            Assert.Equal("300", headers[CorsConstants.AccessControlMaxAge]);
        }

        [Fact]
        public void PreFlight_Request_With_SpecificUrl_Policy_For_Not_Allowed_Header()
        {
            var engine = new CorsEngine();
            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/sub",
                AccessControlRequestMethod = "PUT",
                AccessControlRequestHeaders = new List<string> { "Header1"}
            }, AllowSpecificUrlPolicy);

            Assert.False(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Empty(headers);
        }

        [Fact]
        public void PreFlight_Request_With_SpecificUrl_Policy_For_Allowed_Header()
        {
            var engine = new CorsEngine();
            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/sub",
                AccessControlRequestMethod = "PUT",
                AccessControlRequestHeaders = new List<string> { "AllowedHeader1" }
            }, AllowSpecificUrlPolicy);

            Assert.True(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Equal(4, headers.Count);
            Assert.Equal("http://localhost:5001/sub", headers[CorsConstants.AccessControlAllowOrigin]);
            Assert.Equal("PUT", headers[CorsConstants.AccessControlAllowMethods]);
            Assert.Equal("AllowedHeader1", headers[CorsConstants.AccessControlAllowHeaders]);
            Assert.Equal("300", headers[CorsConstants.AccessControlMaxAge]);
        }

        [Fact]
        public void PreFlight_Request_With_SpecificUrl_Policy_With_Both_Allowed_And_NotAllowed_Header()
        {
            var engine = new CorsEngine();
            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/sub",
                AccessControlRequestMethod = "PUT",
                AccessControlRequestHeaders = new List<string> { "AllowedHeader1", "AllowedHeader2" }
            }, AllowSpecificUrlPolicy);

            Assert.False(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Empty(headers);
        }

        [Fact]
        public void PreFlight_Request_With_SpecificUrl_Policy_For_NotAllowed_Method()
        {
            var engine = new CorsEngine();
            var policy = engine.EvaluatePolicy(new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/sub",
                AccessControlRequestMethod = "DELETE"
            }, AllowSpecificUrlPolicy);

            Assert.False(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Empty(headers);
        }

        [Fact]
        public void PreFlight_Request_With_SpecificUrl_Policy_For_NotAllowed_Header()
        {
            var engine = new CorsEngine();
            var corsRequestContext = new CorsRequestContext
            {
                HttpMethod = CorsConstants.PreflightHttpMethod,
                Origin = "http://localhost:5001/sub",
                AccessControlRequestMethod = "PUT",
                AccessControlRequestHeaders = new List<string> { "CustomHeader1" }
            };

            corsRequestContext.AccessControlRequestHeaders = new List<string> { "CustomHeader1" };

            var policy = engine.EvaluatePolicy(corsRequestContext, AllowSpecificUrlPolicy);

            Assert.False(policy.IsValid);
            var headers = policy.GetResponseHeaders();
            Assert.Empty(headers);
        }
    }
}