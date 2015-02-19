// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Cors.Core;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.WebUtilities;

namespace Microsoft.AspNet.Cors
{
    /// <summary>
    /// An ASP.NET middleware for handling CORS.
    /// </summary>
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorsOptions _options;

        /// <summary>
        /// Instantiates a new <see cref="CorsMiddleware"/>.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        public CorsMiddleware(
            [NotNull]RequestDelegate next,
            [NotNull]CorsOptions options)
        {
            _next = next;
            _options = options;

            _options.PolicyProvider = _options.PolicyProvider ?? new CorsPolicyProvider();
            _options.CorsEngine = _options.CorsEngine ?? new CorsEngine();
        }

        public async Task Invoke(HttpContext context)
        {
            var corsRequestContext = new CorsRequestContext(context);

            if (corsRequestContext.IsCorsRequest)
            {
                var corsPolicy = await _options.PolicyProvider.GetCorsPolicyAsync(corsRequestContext);

                if (corsPolicy != null)
                {
                    var corsResult = _options.CorsEngine.EvaluatePolicy(corsRequestContext, corsPolicy);

                    if (corsRequestContext.IsPreflight)
                    {
                        // Always respond to preflight requests.
                        if (corsResult.IsValid)
                        {
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            WriteCorsHeaders(context, corsResult);
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        }

                        await Task.FromResult(0);
                    }
                    else
                    {
                        if (corsResult.IsValid)
                        {
                            WriteCorsHeaders(context, corsResult);
                            await _next(context);
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            await Task.FromResult(0);
                        }
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }

        private static void WriteCorsHeaders(HttpContext context, ICorsResult result)
        {
            foreach (var header in result.GetResponseHeaders())
            {
                context.Response.Headers.Set(header.Key, header.Value);
            }
        }
    }
}