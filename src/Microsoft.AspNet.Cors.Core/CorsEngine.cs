// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.Cors.Core
{
    /// <inheritdoc />
    public class CorsEngine : ICorsEngine
    {
        /// <inheritdoc />
        public CorsResult EvaluatePolicy([NotNull]CorsRequestContext requestContext, [NotNull]CorsPolicy policy)
        {
            var corsResult = new CorsResult();

            if (!TryValidateOrigin(requestContext, policy, corsResult))
            {
                return corsResult;
            }

            corsResult.SupportsCredentials = policy.SupportsCredentials;
            if (requestContext.IsPreflight)
            {
                if (!TryValidateMethod(requestContext, policy, corsResult))
                {
                    return corsResult;
                }
                if (!TryValidateHeaders(requestContext, policy, corsResult))
                {
                    return corsResult;
                }
                corsResult.PreflightMaxAge = policy.PreflightMaxAge;
            }
            else
            {
                AddHeaderValues(corsResult.AllowedExposedHeaders, policy.ExposedHeaders);
            }

            return corsResult;
        }

        private static bool TryValidateOrigin(
            [NotNull]CorsRequestContext requestContext,
            [NotNull]CorsPolicy policy,
            [NotNull]CorsResult result)
        {
            if (requestContext.Origin != null)
            {
                if (policy.AllowAnyOrigin)
                {
                    result.AllowedOrigin = policy.SupportsCredentials ?
                        requestContext.Origin :
                        CorsConstants.AnyOrigin;
                }
                else if (policy.Origins.Contains(requestContext.Origin))
                {
                    result.AllowedOrigin = requestContext.Origin;
                }
                else
                {
                    result.ErrorMessages.Add(string.Format("Origin '{0}' is not allowed.", requestContext.Origin));
                }
            }
            else
            {
                result.ErrorMessages.Add("Origin header missing in the request.");
            }

            return result.IsValid;
        }

        private static bool TryValidateMethod(
            [NotNull]CorsRequestContext requestContext,
            [NotNull]CorsPolicy policy,
            [NotNull]CorsResult result)
        {
            if (policy.AllowAnyMethod || policy.Methods.Contains(requestContext.AccessControlRequestMethod))
            {
                result.AllowedMethods.Add(requestContext.AccessControlRequestMethod);
            }
            else
            {
                result.ErrorMessages.Add(string.Format("Method '{0}' not allowed.", requestContext.AccessControlRequestMethod));
            }

            return result.IsValid;
        }

        private static bool TryValidateHeaders(
            [NotNull]CorsRequestContext requestContext,
            [NotNull]CorsPolicy policy,
            [NotNull]CorsResult result)
        {
            if (policy.AllowAnyHeader ||                                        // Any header allowed
                requestContext.AccessControlRequestHeaders == null ||           // No Access-Control-Request-Headers found in request
                (requestContext.AccessControlRequestHeaders != null &&          // Received headers found in allowed headers list
                requestContext.AccessControlRequestHeaders.All(a => policy.Headers.Contains(a))
                ))
            {
                AddHeaderValues(result.AllowedHeaders, requestContext.AccessControlRequestHeaders);
            }
            else
            {
                result.ErrorMessages.Add("Requested headers not allowed in the CORS request.");
            }

            return result.IsValid;
        }

        private static void AddHeaderValues(IList<string> target, IEnumerable<string> headerValues)
        {
            if (headerValues != null)
            {
                foreach (string current in headerValues)
                {
                    target.Add(current);
                }
            }
        }
    }
}