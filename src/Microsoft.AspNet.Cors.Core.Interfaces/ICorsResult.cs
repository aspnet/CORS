// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Result of a CORS request validation.
    /// </summary>
    public interface ICorsResult
    {
        /// <summary>
        /// Indicates the headers safe to expose to the API of a CORS API specification.
        /// Corresponds to Access-Control-Expose-Headers CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-expose-headers-response-header
        /// </summary>
        IList<string> AllowedExposedHeaders { get; }

        /// <summary>
        /// Indicates as part of response to a preflight request, which header field names can be used during actual request. 
        /// Corresponds to Access-Control-Allow-Headers CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-headers-response-header
        /// </summary>
        IList<string> AllowedHeaders { get; }

        /// <summary>
        /// Indicates as part of response to a preflight request, which HTTP methods can be used during actual request.
        /// Corresponds to Access-Control-Allow-Methods CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-methods-response-header
        /// </summary>
        IList<string> AllowedMethods { get; }

        /// <summary>
        /// Indicates whether a resource can be shared based by returning the value of the 'Origin' request header, '*' or "null" in the response.
        /// Corresponds to Access-Control-Allow-Origin CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-origin-response-header
        /// </summary>
        string AllowedOrigin { get; set; }

        /// <summary>
        /// Indicates the errors encountered while validating the CORS request.
        /// </summary>
        IList<string> ErrorMessages { get; }

        /// <summary>
        /// Indicates if a CORS request is valid per the <see cref="ICorsPolicy"/> defined.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Indicates how long the results of a preflight request can be cached in a preflight result cache.
        /// Corresponds to Access-Control-Max-Age CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-max-age-response-header
        /// </summary>
        long? PreflightMaxAge { get; set; }

        /// <summary>
        /// Indicates as part of a response if the browser can send or receive credentials (cookies or authorization headers).
        /// Corresponds to Access-Control-Allow-Credentials CORS response header.
        /// More information: http://www.w3.org/TR/cors/#access-control-allow-credentials-response-header
        /// </summary>
        bool SupportsCredentials { get; set; }

        /// <summary>
        /// Gets all the headers that needs to be set to the CORS response.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, string> GetResponseHeaders();
    }
}