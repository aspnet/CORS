// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNet.Cors.Core;

namespace Microsoft.AspNet.Cors
{
    /// <summary>
    /// Configuration options for <see cref="CorsMiddleware"/>.
    /// </summary>
    public class CorsOptions
    {
        /// <summary>
        /// The CORS policy to apply.
        /// </summary>
        public ICorsPolicyProvider PolicyProvider { get; set; }

        /// <summary>
        /// The CORS engine.
        /// </summary>
        public ICorsEngine CorsEngine { get; set; }

        /// <summary>
        /// A policy that allows all headers, all methods, any origin and supports credentials
        /// </summary>
        public static CorsOptions AllowAll
        {
            get
            {
                var policy = new CorsPolicy
                {
                    AllowAnyHeader = true,
                    AllowAnyMethod = true,
                    AllowAnyOrigin = true,
                    SupportsCredentials = true
                };

                // Since we can't prevent this from being mutable, just create a new one everytime.
                return new CorsOptions
                {
                    PolicyProvider = new CorsPolicyProvider
                    {
                        PolicyResolver = context => Task.FromResult<ICorsPolicy>(policy)
                    }
                };
            }
        }
    }
}