// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.Cors.Core
{
    /// <inheritdoc />
    public class CorsResult : ICorsResult
    {
        /// <inheritdoc />
        public IList<string> AllowedExposedHeaders { get; } = new List<string>();

        /// <inheritdoc />
        public IList<string> AllowedHeaders { get; } = new List<string>();

        /// <inheritdoc />
        public IList<string> AllowedMethods { get; } = new List<string>();

        /// <inheritdoc />
        public string AllowedOrigin { get; set; }

        /// <inheritdoc />
        public IList<string> ErrorMessages { get; } = new List<string>();

        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                return ErrorMessages.Count == 0;
            }
        }

        private long? _preflightMaxAge;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool SupportsCredentials { get; set; }

        /// <inheritdoc />
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