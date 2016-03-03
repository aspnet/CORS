// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Cors.Infrastructure
{
    /// <summary>
    /// Provides programmatic configuration for Cors.
    /// </summary>
    public class CorsOptions
    {
        private string _defaultPolicyName = "__DefaultCorsPolicy";
        private IDictionary<string, CorsPolicy> PolicyMap { get; } = new Dictionary<string, CorsPolicy>();

        /// <summary>
        /// Constructs a new CorsOptions.
        /// </summary>
        public CorsOptions() { }

        /// <summary>
        /// Constructs and configures a new CorsOptions.
        /// </summary>
        /// <param name="configureOptions">The configuration actions that will be applied.</param>
        public CorsOptions(IEnumerable<IConfigureOptions<CorsOptions>> configureOptions) : this()
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            foreach (var configure in configureOptions)
            {
                configure.Configure(this);
            }
        }

        public string DefaultPolicyName
        {
            get
            {
                return _defaultPolicyName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _defaultPolicyName = value;
            }
        }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="policy">The <see cref="CorsPolicy"/> policy to be added.</param>
        public void AddPolicy(string name, CorsPolicy policy)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            PolicyMap[name] = policy;
        }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="configurePolicy">A delegate which can use a policy builder to build a policy.</param>
        public void AddPolicy(string name, Action<CorsPolicyBuilder> configurePolicy)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configurePolicy == null)
            {
                throw new ArgumentNullException(nameof(configurePolicy));
            }

            var policyBuilder = new CorsPolicyBuilder();
            configurePolicy(policyBuilder);
            PolicyMap[name] = policyBuilder.Build();
        }

        /// <summary>
        /// Gets the policy based on the <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the policy to lookup.</param>
        /// <returns>The <see cref="CorsPolicy"/> if the policy was added.<c>null</c> otherwise.</returns>
        public CorsPolicy GetPolicy(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return PolicyMap.ContainsKey(name) ? PolicyMap[name] : null;
        }
    }
}