// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.RuntimeModel;
using Yarp.ReverseProxy.Service.Proxy;

namespace Yarp.ReverseProxy.Health
{
    /// <summary>
    /// Passive health check evaluation policy.
    /// </summary>
    public interface IPassiveHealthCheckPolicy
    {
        /// <summary>
        /// Policy's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Registers a successful or failed request and evaluates a new <see cref="DestinationHealthState.Passive"/> value.
        /// </summary>
        /// <param name="cluster">Request's cluster.</param>
        /// <param name="destination">Request's destination.</param>
        /// <param name="context">Context.</param>
        void RequestProxied(ClusterState cluster, DestinationState destination, HttpContext context);
    }
}