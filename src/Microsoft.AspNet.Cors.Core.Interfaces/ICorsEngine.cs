// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Engine to evaluate a CORS policy against a CORS request.
    /// </summary>
    public interface ICorsEngine
    {
        /// <summary>
        /// Evaluates a CORS request against a CORS policy.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        ICorsResult EvaluatePolicy([NotNull]ICorsRequestContext requestContext, [NotNull]ICorsPolicy policy);
    }
}