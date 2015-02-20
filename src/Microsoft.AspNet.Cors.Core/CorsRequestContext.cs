// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Cors.Core
{
    /// <inheritdoc />
    public class CorsRequestContext : ICorsRequestContext
    {
        public CorsRequestContext()
        {
        }

        public CorsRequestContext(HttpContext context)
        {
            Origin = context.Request.Headers.Get(CorsConstants.Origin);
            RequestPath = context.Request.Path;
            HttpMethod = context.Request.Method;
            Host = context.Request.Host.Value;
            AccessControlRequestMethod = context.Request.Headers.Get(CorsConstants.AccessControlRequestMethod);
            AccessControlRequestHeaders = context.Request.Headers.GetCommaSeparatedValues(CorsConstants.AccessControlRequestHeaders);
        }

        /// <inheritdoc />
        public IList<string> AccessControlRequestHeaders { get; set; }

        /// <inheritdoc />
        public string AccessControlRequestMethod { get; set; }

        /// <inheritdoc />
        public string Host { get; set; }

        /// <inheritdoc />
        public string HttpMethod { get; set; }

        /// <inheritdoc />
        public bool IsPreflight
        {
            get
            {
                return Origin != null &&
                    AccessControlRequestMethod != null &&
                    string.Equals(HttpMethod, CorsConstants.PreflightHttpMethod, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <inheritdoc />
        public string Origin { get; set; }

        /// <inheritdoc />
        public PathString RequestPath { get; set; }
    }
}