// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Defines a CORS policy. 
    /// </summary>
    public interface ICorsPolicy
    {
        /// <summary>
        /// Indicates any header is allowed in the CORS request.
        /// </summary>
        bool AllowAnyHeader { get; set; }

        /// <summary>
        /// Indicates any HTTP verb is allowed in the CORS request.
        /// </summary>
        bool AllowAnyMethod { get; set; }

        /// <summary>
        /// Indicates all origins are allowed to access the resource.
        /// </summary>
        bool AllowAnyOrigin { get; set; }

        /// <summary>
        /// Indicates the headers safe to expose to the API of a CORS API specification.
        /// More information: http://www.w3.org/TR/cors/#access-control-expose-headers-response-header
        /// </summary>
        IList<string> ExposedHeaders { get; }

        /// <summary>
        /// Indicates the headers allowed in a CORS request.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-headers-response-header
        /// </summary>
        IList<string> Headers { get; }

        /// <summary>
        /// Indicates the HTTP verbs allowed in a CORS request.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-methods-response-header
        /// </summary>
        IList<string> Methods { get; }

        /// <summary>
        /// Indicates the list of permitted origins. '*' - indicates all origins.
        /// </summary>
        IList<string> Origins { get; }

        /// <summary>
        /// Indicates in seconds how long the response to the preflight request can be cached and reused by the browser 
        /// before the next preflight request.
        /// </summary>
        long? PreflightMaxAge { get; set; }

        /// <summary>
        /// To allow browser to send or receive credentials (cookies, HTTP authentication) during CORS.
        /// </summary>
        bool SupportsCredentials { get; set; }
    }
}