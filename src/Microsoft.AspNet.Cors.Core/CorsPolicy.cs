// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Cors.Core
{
    /// <inheritdoc />
    public class CorsPolicy : ICorsPolicy
    {
        /// <inheritdoc />
        public bool AllowAnyHeader { get; set; }

        /// <inheritdoc />
        public bool AllowAnyMethod { get; set; }

        /// <inheritdoc />
        public bool AllowAnyOrigin { get; set; }

        /// <inheritdoc />
        public IList<string> ExposedHeaders { get; private set; } = new List<string>();

        /// <inheritdoc />
        public IList<string> Headers { get; private set; } = new List<string>();

        /// <inheritdoc />
        public IList<string> Methods { get; private set; } = new List<string>();

        /// <inheritdoc />
        public IList<string> Origins { get; private set; } = new List<string>();

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
    }
}