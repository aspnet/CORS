// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.Cors.Core
{
    /// <inheritdoc />
    public class DefaultCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly CorsOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultCorsPolicyProvider"/>.
        /// </summary>
        /// <param name="options">The options configured for the application.</param>
        public DefaultCorsPolicyProvider(IOptions<CorsOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc />
        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            return Task.FromResult(_options.GetPolicy(policyName ?? _options.DefaultPolicyName));
        }
    }
}