// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Indicates the data from a CORS request.
    /// </summary>
    public class CorsRequestContext
    {
        /// <summary>
        /// Instantiates an instance of <see cref="CorsRequestContext"/>.
        /// </summary>
        public CorsRequestContext()
        {
        }

        /// <summary>
        /// Instantiates an instance of <see cref="CorsRequestContext"/>.
        /// </summary>
        public CorsRequestContext(HttpContext context)
        {
            Origin = context.Request.Headers.Get(CorsConstants.Origin);
            RequestPath = context.Request.Path;
            HttpMethod = context.Request.Method;
            Host = context.Request.Host.Value;
            AccessControlRequestMethod = context.Request.Headers.Get(CorsConstants.AccessControlRequestMethod);
            AccessControlRequestHeaders = context.Request.Headers.GetCommaSeparatedValues(CorsConstants.AccessControlRequestHeaders);
        }

        /// <summary>
        /// Value of the 'Origin' header from the request.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Value of 'Access-Control-Request-Headers' header from the request.
        /// </summary>
        public IList<string> AccessControlRequestHeaders { get; set; }

        /// <summary>
        /// Value of 'Access-Control-Request-Method' header from the request.
        /// </summary>
        public string AccessControlRequestMethod { get; set; }

        /// <summary>
        /// Value of 'Host' header from the request.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// HTTP verb of the request.
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Path to which request was received.
        /// </summary>
        public PathString RequestPath { get; set; }

        /// <summary>
        /// Indicates if this is a CORS preflight request.
        /// </summary>
        public bool IsPreflight
        {
            get
            {
                return Origin != null &&
                    AccessControlRequestMethod != null &&
                    string.Equals(HttpMethod, CorsConstants.PreflightHttpMethod, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}