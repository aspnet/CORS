// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Cors.Core;
using Microsoft.AspNet.Http;
using Xunit;

namespace Microsoft.AspNet.Cors.Test
{
    public class CorsPolicyProviderTests
    {
        [Fact]
        public async Task Verify_Defaults()
        {
            var policyProvider = new CorsPolicyProvider();
            var policy = await policyProvider.GetCorsPolicyAsync(new CorsRequestContext());
            Assert.Null(policy);
        }

        [Fact]
        public async Task Custom_Policy_Resolver()
        {
            var policyCache = new Dictionary<string, CorsPolicy>();
            policyCache.Add("policy1", new CorsPolicy { PreflightMaxAge = 1 });
            policyCache.Add("policy2", new CorsPolicy { PreflightMaxAge = 2 });
            policyCache.Add("policy3", new CorsPolicy { PreflightMaxAge = 3 });
            policyCache.Add("policy4", new CorsPolicy { PreflightMaxAge = 4 });

            var policyProvider = new CorsPolicyProvider()
            {
                PolicyResolver = context =>
                {
                    var policyName = context.RequestPath.Value.Substring(1);
                    CorsPolicy appliedPolicy;
                    policyCache.TryGetValue(policyName, out appliedPolicy);
                    return Task.FromResult(appliedPolicy);
                }
            };

            for (int index = 1; index <= 5; index++)
            {
                var policyName = "policy" + index.ToString();
                var corsRequestContext = new CorsRequestContext() { RequestPath = new PathString("/" + policyName) };
                var appliedPolicy = await policyProvider.GetCorsPolicyAsync(corsRequestContext);
                CorsPolicy expectedPolicy;
                policyCache.TryGetValue(policyName, out expectedPolicy);
                Assert.Equal(expectedPolicy, appliedPolicy);
                if (expectedPolicy != null)
                {
                    Assert.Equal(expectedPolicy.PreflightMaxAge, appliedPolicy.PreflightMaxAge);
                }
            }
        }
    }
}