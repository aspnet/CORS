// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Microsoft.AspNet.Cors.Core.Test
{
    public class CorsResultTests
    {
        [Fact]
        public void Validate_CorsResult_Defaults()
        {
            var corsResult = new CorsResult();
            Assert.Empty(corsResult.AllowedExposedHeaders);
            Assert.Empty(corsResult.AllowedHeaders);
            Assert.Empty(corsResult.AllowedMethods);
            Assert.Null(corsResult.AllowedOrigin);
            Assert.Empty(corsResult.ErrorMessages);
            Assert.True(corsResult.IsValid);
            Assert.False(corsResult.PreflightMaxAge.HasValue);
            Assert.False(corsResult.SupportsCredentials);
            Assert.Empty(corsResult.GetResponseHeaders());
        }

        [Fact]
        public void Validate_CorsResult()
        {
            // TODO.
            var corsResult = new CorsResult();
            corsResult.AllowedOrigin = "*";
            corsResult.SupportsCredentials = false;
        }
    }
}