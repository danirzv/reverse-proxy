// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Moq;
using Xunit;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Yarp.ReverseProxy.SessionAffinity.Tests
{
    public class AffinitizeTransformProviderTests
    {
        [Fact]
        public void EnableSessionAffinity_AddsTransform()
        {
            var affinityProvider = new Mock<ISessionAffinityProvider>(MockBehavior.Strict);
            affinityProvider.SetupGet(p => p.Mode).Returns("Mode");

            var transformProvider = new AffinitizeTransformProvider(new[] { affinityProvider.Object });
            
            var cluster = new ClusterConfig
            {
                ClusterId = "cluster1",
                SessionAffinity = new SessionAffinityConfig
                {
                    Enabled = true,
                    Mode = "Mode",
                    AffinityKeyName = "Key1"
                }
            };

            var validationContext = new TransformClusterValidationContext()
            {
                Cluster = cluster,
            };
            transformProvider.ValidateCluster(validationContext);

            Assert.Empty(validationContext.Errors);

            var builderContext = new TransformBuilderContext()
            {
                Cluster = cluster,
            };
            transformProvider.Apply(builderContext);

            Assert.IsType<AffinitizeTransform>(builderContext.ResponseTransforms.Single());
        }

        [Fact]
        public void EnableSession_InvalidMode_Fails()
        {
            var affinityProvider = new Mock<ISessionAffinityProvider>(MockBehavior.Strict);
            affinityProvider.SetupGet(p => p.Mode).Returns("Mode");

            var transformProvider = new AffinitizeTransformProvider(new[] { affinityProvider.Object });

            var cluster = new ClusterConfig
            {
                ClusterId = "cluster1",
                SessionAffinity = new SessionAffinityConfig
                {
                    Enabled = true,
                    Mode = "Invalid",
                    AffinityKeyName = "Key1"
                }
            };

            var validationContext = new TransformClusterValidationContext()
            {
                Cluster = cluster,
            };
            transformProvider.ValidateCluster(validationContext);

            var ex = Assert.Single(validationContext.Errors);
            Assert.Equal("No matching ISessionAffinityProvider found for the session affinity mode 'Invalid' set on the cluster 'cluster1'.", ex.Message);

            var builderContext = new TransformBuilderContext()
            {
                Cluster = cluster,
            };

            ex = Assert.Throws<ArgumentException>(() => transformProvider.Apply(builderContext));
            Assert.Equal($"No {typeof(ISessionAffinityProvider).FullName} was found for the id 'Invalid'. (Parameter 'id')", ex.Message);
        }
    }
}
