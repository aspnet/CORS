// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Defines a CORS policy. 
    /// </summary>
    public class CorsPolicy
    {
        private long? _preflightMaxAge;

        /// <summary>
        /// Indicates any header is allowed in the CORS request.
        /// </summary>
        public bool AllowAnyHeader { get; set; }

        /// <summary>
        /// Indicates any HTTP verb is allowed in the CORS request.
        /// </summary>
        public bool AllowAnyMethod { get; set; }

        /// <summary>
        /// Indicates all origins are allowed to access the resource.
        /// </summary>
        public bool AllowAnyOrigin { get; set; }

        /// <summary>
        /// Indicates the headers safe to expose to the API of a CORS API specification.
        /// More information: http://www.w3.org/TR/cors/#access-control-expose-headers-response-header
        /// </summary>
        public IList<string> ExposedHeaders { get; private set; } = new List<string>();

        /// <summary>
        /// Indicates the headers allowed in a CORS request.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-headers-response-header
        /// </summary>
        public IList<string> Headers { get; private set; } = new List<string>();

        /// <summary>
        /// Indicates the HTTP verbs allowed in a CORS request.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-methods-response-header
        /// </summary>
        public IList<string> Methods { get; private set; } = new List<string>();

        /// <summary>
        /// Indicates the list of permitted origins. '*' - indicates all origins.
        /// </summary>
        public IList<string> Origins { get; private set; } = new List<string>();

        /// <summary>
        /// Indicates in seconds how long the response to the preflight request can be cached and reused by the browser 
        /// before the next preflight request.
        /// </summary>
        public long? PreflightMaxAge
        {
            get
            {
                return _preflightMaxAge;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format("'{0}' cannot be less than zero.", nameof(PreflightMaxAge)));
                }

                _preflightMaxAge = value;
            }
        }

        /// <summary>
        /// To allow browser to send or receive credentials (cookies, HTTP authentication) during CORS.
        /// </summary>
        public bool SupportsCredentials { get; set; }
    }
}