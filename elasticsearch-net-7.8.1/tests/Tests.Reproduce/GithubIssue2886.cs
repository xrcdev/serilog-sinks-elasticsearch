// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Tests.Core.ManagedElasticsearch.Clusters;

namespace Tests.Reproduce
{
	public class GithubIssue2886 : IClusterFixture<WritableCluster>
	{
		private readonly WritableCluster _cluster;

		public GithubIssue2886(WritableCluster cluster) => _cluster = cluster;

		[I]
		public void CanReadSingleOrMultipleCommonGramsCommonWordsItem()
		{
			var client = _cluster.Client;

			var json = @"
				{
				  ""settings"": {
					""analysis"": {
					  ""filter"": {
						""single_common_words"": {
						  ""type"":         ""common_grams"",
						  ""common_words"": ""_english_""
						},
						""multiple_common_words"": {
						  ""type"":         ""common_grams"",
						  ""common_words"": [""_english_"", ""_french_""]
						}
					  }
					}
				  }
				}";

			var response = client.LowLevel.Indices.Create<StringResponse>("common_words_token_filter", json);
			response.Success.Should().BeTrue();

			var settingsResponse = client.Indices.Get("common_words_token_filter");

			var indexState = settingsResponse.Indices["common_words_token_filter"];
			indexState.Should().NotBeNull();

			var tokenFilters = indexState.Settings.Analysis.TokenFilters;
			tokenFilters.Should().HaveCount(2);

			var commonGramsTokenFilter = tokenFilters["single_common_words"] as ICommonGramsTokenFilter;
			commonGramsTokenFilter.Should().NotBeNull();
			commonGramsTokenFilter.CommonWords.Should().NotBeNull().And.HaveCount(1);

			commonGramsTokenFilter = tokenFilters["multiple_common_words"] as ICommonGramsTokenFilter;
			commonGramsTokenFilter.Should().NotBeNull();
			commonGramsTokenFilter.CommonWords.Should().NotBeNull().And.HaveCount(2);
		}
	}
}
