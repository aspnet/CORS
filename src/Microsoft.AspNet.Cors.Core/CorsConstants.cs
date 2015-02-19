// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Cors.Core
{
    public class CorsConstants
    {
        public const string PreflightHttpMethod = "OPTIONS";
        public const string Origin = "Origin";
        public const string AnyOrigin = "*";
        public const string AccessControlRequestMethod = "Access-Control-Request-Method";
        public const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
        public const string AccessControlMaxAge = "Access-Control-Max-Age";

        internal static readonly string[] SimpleRequestHeaders = new string[]
        {
            "Origin",
            "Accept",
            "Accept-Language",
            "Content-Language"
        };

        internal static readonly string[] SimpleResponseHeaders = new string[]
        {
            "Cache-Control",
            "Content-Language",
            "Content-Type",
            "Expires",
            "Last-Modified",
            "Pragma"
        };

        internal static readonly string[] SimpleMethods = new string[]
        {
            "GET",
            "HEAD",
            "POST"
        };
    }
}