// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Indicates the data from a CORS request.
    /// </summary>
    public interface ICorsRequestContext
    {
        /// <summary>
        /// Value of the 'Origin' header from the request.
        /// </summary>
        string Origin { get; set; }

        /// <summary>
        /// Value of 'Access-Control-Request-Method' header from the request.
        /// </summary>
        string AccessControlRequestMethod { get; set; }

        /// <summary>
        /// Value of 'Access-Control-Request-Headers' header from the request.
        /// </summary>
        IList<string> AccessControlRequestHeaders { get; set; }

        /// <summary>
        /// Value of 'Host' header from the request.
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// HTTP verb of the request.
        /// </summary>
        string HttpMethod { get; set; }

        /// <summary>
        /// Indicates if this is a CORS preflight request.
        /// </summary>
        bool IsPreflight { get; }

        /// <summary>
        /// Path to which request was received.
        /// </summary>
        PathString RequestPath { get; set; }
    }
}