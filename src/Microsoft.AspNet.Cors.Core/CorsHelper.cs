// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Cors.Core
{
    public class CorsHelper
    {
        /// <summary>
        /// Returns true if the current request is a CORS request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsCorsRequest([NotNull]HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request.Headers.Get(CorsConstants.Origin));
        }
    }
}