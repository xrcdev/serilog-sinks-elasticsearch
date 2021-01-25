﻿using Serilog.Capturing;
using Xunit;
using System.Linq;

namespace Serilog.Tests.Capturing
{
    public class GetablePropertyFinderTests
    {
        [Fact]
        public void GetPropertiesRecursiveIntegerTypeYieldNoResult()
        {
            var result = (default(int)).GetType().GetPropertiesRecursive();
            Assert.Empty(result);
        }

        [Fact]
        public void GetPropertiesRecursiveBooleanTypeYieldNoResult()
        {
            var result = (default(bool)).GetType().GetPropertiesRecursive();
            Assert.Empty(result);
        }

        [Fact]
        public void GetPropertiesRecursiveCharTypeYieldNoResult()
        {
            var result = (default(char)).GetType().GetPropertiesRecursive();
            Assert.Empty(result);
        }

        [Fact]
        public void GetPropertiesRecursiveObjectTypeYieldNoResult()
        {
            var result = (new object()).GetType().GetPropertiesRecursive();
            Assert.Empty(result);
        }

        [Fact]
        public void GetPropertiesRecursiveStringTypeYieldResult()
        {
            var result = (string.Empty).GetType().GetPropertiesRecursive();
            Assert.NotEmpty(result);
        }

        [Fact]
        // https://github.com/serilog/serilog/issues/1235
        public void GetPropertiesRecursiveBaseTypeSucceedsOnWcfProxyType()
        {
            var remoteAddress = new System.ServiceModel.EndpointAddress("http://localhost");
            var binding = new System.ServiceModel.BasicHttpBinding();

            var myFactory = new System.ServiceModel.ChannelFactory<IMyChannel>(binding, remoteAddress);
            var channel = myFactory.CreateChannel();

            var _ = channel.GetType().GetPropertiesRecursive().ToList();
        }

        [System.ServiceModel.ServiceContract]
        interface IMyChannel
        {
            [System.ServiceModel.OperationContract]
            string Get();
        }
    }
}
