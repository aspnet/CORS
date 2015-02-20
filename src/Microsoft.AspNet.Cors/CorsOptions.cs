// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    }
}