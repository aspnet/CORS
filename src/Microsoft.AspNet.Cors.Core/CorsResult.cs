// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Result of a CORS request validation.
    /// </summary>
    public class CorsResult
    {
        private long? _preflightMaxAge;

        /// <summary>
        /// Indicates the headers safe to expose to the API of a CORS API specification.
        /// Corresponds to Access-Control-Expose-Headers CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-expose-headers-response-header
        /// </summary>
        public IList<string> AllowedExposedHeaders { get; } = new List<string>();

        /// <summary>
        /// Indicates as part of response to a preflight request, which header field names can be used during actual request. 
        /// Corresponds to Access-Control-Allow-Headers CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-headers-response-header
        /// </summary>
        public IList<string> AllowedHeaders { get; } = new List<string>();

        /// <summary>
        /// Indicates as part of response to a preflight request, which HTTP methods can be used during actual request.
        /// Corresponds to Access-Control-Allow-Methods CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-methods-response-header
        /// </summary>
        public IList<string> AllowedMethods { get; } = new List<string>();

        /// <summary>
        /// Indicates whether a resource can be shared based by returning the value of the 'Origin' request header, '*' or "null" in the response.
        /// Corresponds to Access-Control-Allow-Origin CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-origin-response-header
        /// </summary>
        public string AllowedOrigin { get; set; }

        /// <summary>
        /// Indicates the errors encountered while validating the CORS request.
        /// </summary>
        public IList<string> ErrorMessages { get; } = new List<string>();

        /// <summary>
        /// Indicates if a CORS request is valid per the <see cref="ICorsPolicy"/> defined.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return ErrorMessages.Count == 0;
            }
        }

        /// <summary>
        /// Indicates how long the results of a preflight request can be cached in a preflight result cache.
        /// Corresponds to Access-Control-Max-Age CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-max-age-response-header
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
        /// Indicates as part of a response if the browser can send or receive credentials (cookies or authorization headers).
        /// Corresponds to Access-Control-Allow-Credentials CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-credentials-response-header
        /// </summary>
        public bool SupportsCredentials { get; set; }

        /// <summary>
        /// Gets all the headers that needs to be set to the CORS response.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetResponseHeaders()
        {
            var headers = new Dictionary<string, string>();

            if (!IsValid)
            {
                return headers;
            }

            if (AllowedOrigin != null)
            {
                headers.Add(CorsConstants.AccessControlAllowOrigin, this.AllowedOrigin);
            }

            if (SupportsCredentials)
            {
                headers.Add(CorsConstants.AccessControlAllowCredentials, "true");
            }

            if (AllowedMethods.Count > 0)
            {
                var allowedMethods =
                    from m in AllowedMethods
                    where !CorsConstants.SimpleMethods.Contains(m, StringComparer.OrdinalIgnoreCase)
                    select m;

                AddHeader(headers, CorsConstants.AccessControlAllowMethods, allowedMethods);
            }

            if (AllowedHeaders.Count > 0)
            {
                var allowedHeaders =
                    from header in AllowedHeaders
                    where !CorsConstants.SimpleRequestHeaders.Contains(header, StringComparer.OrdinalIgnoreCase)
                    select header;

                AddHeader(headers, CorsConstants.AccessControlAllowHeaders, allowedHeaders);
            }

            if (AllowedExposedHeaders.Count > 0)
            {
                var allowedExposedHeaders =
                    from header in AllowedExposedHeaders
                    where !CorsConstants.SimpleResponseHeaders.Contains(header, StringComparer.OrdinalIgnoreCase)
                    select header;

                AddHeader(headers, CorsConstants.AccessControlExposeHeaders, allowedExposedHeaders);
            }

            if (PreflightMaxAge.HasValue)
            {
                headers.Add(CorsConstants.AccessControlMaxAge, PreflightMaxAge.ToString());
            }

            return headers;
        }

        private static void AddHeader(IDictionary<string, string> headers, string headerName, IEnumerable<string> headerValues)
        {
            var value = string.Join(",", headerValues);
            if (!string.IsNullOrEmpty(value))
            {
                headers.Add(headerName, value);
            }
        }
    }
}