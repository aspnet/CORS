// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Builder to support building of <see cref="CorsPolicy"/>.
    /// </summary>
    public class CorsPolicyBuilder
    {
        private readonly CorsPolicy _corsPolicy = new CorsPolicy();

        public CorsPolicy AddOrigin([NotNull]string origin)
        {
            if (origin == CorsConstants.AnyOrigin)
            {
                _corsPolicy.AllowAnyOrigin = true;
            }
            else
            {
                _corsPolicy.Origins.Add(origin);
            }

            return _corsPolicy;
        }

        public CorsPolicy AddAllowedHeader([NotNull] string header)
        {
            _corsPolicy.Headers.Add(header);
            return _corsPolicy;
        }

        public CorsPolicy AddAllowedMethod([NotNull]string method)
        {
            _corsPolicy.Methods.Add(method);
            return _corsPolicy;
        }

        public CorsPolicy AddExposedHeader([NotNull]string header)
        {
            _corsPolicy.ExposedHeaders.Add(header);
            return _corsPolicy;
        }

        public CorsPolicy Build()
        {
            return _corsPolicy;
        }
    }
}