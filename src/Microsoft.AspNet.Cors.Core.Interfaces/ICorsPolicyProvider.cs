// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Provider to choose a specific <see cref="ICorsPolicy"/> for a specific request.
    /// </summary>
    public interface ICorsPolicyProvider
    {
        /// <summary>
        /// Gets the <see cref="ICorsPolicy"/> to apply for the specified CORS request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<ICorsPolicy> GetCorsPolicyAsync([NotNull]ICorsRequestContext context);
    }
}