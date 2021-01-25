// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Runtime.Serialization;
using System.Text;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using FluentAssertions;
using Tests.Core.Client;

namespace Tests.Reproduce
{
	public class GithubIssue3107
	{
		[U] public void FieldResolverRespectsDataMemberAttributes()
		{
			var client = TestClient.DefaultInMemoryClient;

			var document = new SourceEntity
			{
				Name = "name",
				DisplayName = "display name"
			};

			var indexResponse = client.IndexDocument(document);
			var requestJson = Encoding.UTF8.GetString(indexResponse.ApiCall.RequestBodyInBytes);
			requestJson.Should().Contain("display_name");

			var searchResponse = client.Search<SourceEntity>(s => s
				.Query(q => q
					.Terms(t => t
						.Field(f => f.DisplayName)
						.Terms("term")
					)
				)
			);

			requestJson = Encoding.UTF8.GetString(searchResponse.ApiCall.RequestBodyInBytes);
			requestJson.Should().Contain("display_name");
		}

		[DataContract(Name = "source_entity")]
		public class SourceEntity
		{
			[DataMember(Name = "display_name")]
			public string DisplayName { get; set; }

			[DataMember(Name = "name")]
			public string Name { get; set; }
		}
	}
}
