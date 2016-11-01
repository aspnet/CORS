// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Cors.Infrastructure
{
    internal static class UriExtensions
    {
        public static bool IsSubdomainOf(this Uri subdomain, Uri domain)
        {
            return subdomain.IsAbsoluteUri 
                && domain.IsAbsoluteUri
                && subdomain.Scheme == domain.Scheme
                && subdomain.Host.EndsWith(domain.Host);
        }
    }
}