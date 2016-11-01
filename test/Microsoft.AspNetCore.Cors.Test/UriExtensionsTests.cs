// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.AspNetCore.Cors.Infrastructure
{
    public sealed class UriExtensionsTests
    {
        [Theory]
        [MemberData(nameof(IsSubdomainOfTestData))]
        public void TestIsSubdomainOf(Uri subdomain, Uri domain, bool expectedResult)
        {
            Assert.Equal(subdomain.IsSubdomainOf(domain), expectedResult);
        }

        public static IEnumerable<object[]> IsSubdomainOfTestData
        {
            get
            {
                return new[]
                {
                    new object[] {new Uri("http://sub.domain"), new Uri("http://domain"), true},
                    new object[] {new Uri("http://sub.sub.domain"), new Uri("http://domain"), true},
                    new object[] {new Uri("https://sub.domain"), new Uri("http://domain"), false},
                    new object[] {new Uri("http://domain.tld"), new Uri("http://domain"), false},
                    new object[] {new Uri("http://sub.domain.tld"), new Uri("http://domain"), false},
                    new object[] {new Uri("/relativeUri", UriKind.Relative), new Uri("http://domain"), false},
                    new object[] {new Uri("http://sub.domain"), new Uri("/relative", UriKind.Relative), false}
                };
            }
        }
    }
}