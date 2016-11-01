// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.AspNetCore.Cors.Infrastructure
{
    public sealed class CorsPolicyExtensionsTest
    {
        [Fact]
        public void IsOriginAnAllowedSubdomain_ReturnsTrueIfPolicyContainsOrigin()
        {
            // Arrange
            const string origin = "http://sub.domain";
            var policy = new CorsPolicy();
            policy.Origins.Add(origin);

            // Act
            var actual = policy.IsOriginAnAllowedSubdomain(origin);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("http://sub.domain", "http://*.domain", true)]
        [InlineData("http://sub.sub.domain", "http://*.domain", true)]
        [InlineData("http://sub.domain", "http://domain", false)]
        [InlineData("http://sub.domain", "http://domain.*", false)]
        [InlineData("http://sub.domain.hacker", "http://*.domain", false)]
        [InlineData("https://sub.domain", "http://*.domain", false)]
        public void IsOriginAnAllowedSubdomain(string origin, string allowedOrigin, bool expected)
        {
            // Arrange
            var policy = new CorsPolicy();
            policy.Origins.Add(allowedOrigin);

            // Act
            var actual = policy.IsOriginAnAllowedSubdomain(origin);

            // Assert
            Assert.Equal(actual, expected);
        }
    }
}