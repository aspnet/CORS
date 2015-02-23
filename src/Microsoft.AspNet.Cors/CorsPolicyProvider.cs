// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Cors.Core;

namespace Microsoft.AspNet.Cors
{
    /// <inheritdoc />
    public class CorsPolicyProvider : ICorsPolicyProvider
    {
        /// <summary>
        /// Creates a new <see cref="CorsPolicyProvider"/> instance.
        /// </summary>
        public CorsPolicyProvider()
        {
            PolicyResolver = request => Task.FromResult<CorsPolicy>(null);
        }

        /// <summary>
        /// A pluggable callback that will be used to select the CORS policy for the given requests.
        /// </summary>
        public Func<CorsRequestContext, Task<CorsPolicy>> PolicyResolver { get; set; }

        /// <inheritdoc />
        public virtual Task<CorsPolicy> GetCorsPolicyAsync(CorsRequestContext context)
        {
            return PolicyResolver(context);
        }
    }
}