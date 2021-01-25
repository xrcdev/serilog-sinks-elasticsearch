// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System.Text;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Tests.Core.Client;

namespace Tests.CodeStandards.Serialization
{
	public class GeoLocationTests
	{
		[U]
		public void CanDeserializeAndSerializeToWellKnownText()
		{
			var wkt = "{\"location\":\"POINT (-90 90)\"}";
			var client = TestClient.DisabledStreaming;

			Doc deserialized;
			using (var stream = RecyclableMemoryStreamFactory.Default.Create(Encoding.UTF8.GetBytes(wkt)))
				deserialized = client.RequestResponseSerializer.Deserialize<Doc>(stream);

			deserialized.Location.Should().Be(new GeoLocation(90, -90));
			client.RequestResponseSerializer.SerializeToString(deserialized).Should().Be(wkt);
		}

		private class Doc
		{
			public GeoLocation Location { get; set; }
		}
	}
}
