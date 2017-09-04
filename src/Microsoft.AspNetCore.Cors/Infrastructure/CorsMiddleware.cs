// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Cors.Infrastructure
{
    /// <summary>
    /// An ASP.NET middleware for handling CORS.
    /// </summary>
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICorsService _corsService;
        private readonly CorsPolicy _policy;
        private readonly string _corsPolicyName;

        /// <summary>
        /// Instantiates a new <see cref="CorsMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="corsService">An instance of <see cref="ICorsService"/>.</param>
        /// <param name="policyName">An optional name of the policy to be fetched.</param>
        public CorsMiddleware(
            RequestDelegate next,
            ICorsService corsService,
            string policyName)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (corsService == null)
            {
                throw new ArgumentNullException(nameof(corsService));
            }

            _next = next;
            _corsService = corsService;
            _corsPolicyName = policyName;
        }

        /// <summary>
        /// Instantiates a new <see cref="CorsMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="corsService">An instance of <see cref="ICorsService"/>.</param>
        /// <param name="policy">An instance of the <see cref="CorsPolicy"/> which can be applied.</param>
        public CorsMiddleware(
           RequestDelegate next,
           ICorsService corsService,
           CorsPolicy policy)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (corsService == null)
            {
                throw new ArgumentNullException(nameof(corsService));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _next = next;
            _corsService = corsService;
            _policy = policy;
        }

        /// <inheritdoc />
        public async Task Invoke(HttpContext context, ICorsPolicyProvider corsPolicyProvider)
        {
            if (context.Request.Headers.ContainsKey(CorsConstants.Origin))
            {
                var corsPolicy = _policy ?? await corsPolicyProvider.GetPolicyAsync(context, _corsPolicyName);
                if (corsPolicy != null)
                {
                    var corsResult = _corsService.EvaluatePolicy(context, corsPolicy);
                    _corsService.ApplyResult(corsResult, context.Response);

                    var accessControlRequestMethod =
                        context.Request.Headers[CorsConstants.AccessControlRequestMethod];
                    if (string.Equals(
                            context.Request.Method,
                            CorsConstants.PreflightHttpMethod,
                            StringComparison.OrdinalIgnoreCase) &&
                            !StringValues.IsNullOrEmpty(accessControlRequestMethod))
                    {
                        // Since there is a policy which was identified,
                        // always respond to preflight requests.
                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}