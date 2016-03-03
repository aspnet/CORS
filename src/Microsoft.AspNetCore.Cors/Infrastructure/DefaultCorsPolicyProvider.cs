// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Cors.Infrastructure
{
    /// <inheritdoc />
    public class DefaultCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly CorsOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultCorsPolicyProvider"/>.
        /// </summary>
        /// <param name="options">The options configured for the application.</param>
        public DefaultCorsPolicyProvider(CorsOptions options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return Task.FromResult(_options.GetPolicy(policyName ?? _options.DefaultPolicyName));
        }
    }
}