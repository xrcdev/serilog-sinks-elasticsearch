// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System;
using FluentAssertions;
using Nest;
using Tests.Core.Extensions;
using Tests.Core.ManagedElasticsearch.Clusters;
using Tests.Domain;
using Tests.Framework.EndpointTests.TestState;

namespace Tests.Aggregations.Bucket.AdjacencyMatrix
{
	public class AdjacencyMatrixUsageTests : AggregationUsageTestBase
	{
		public AdjacencyMatrixUsageTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override object AggregationJson => new
		{
			interactions = new
			{
				adjacency_matrix = new
				{
					filters = new
					{
						grpA = new { term = new { state = new { value = "BellyUp" } } },
						grpB = new { term = new { state = new { value = "Stable" } } },
						grpC = new { term = new { state = new { value = "VeryActive" } } }
					}
				}
			}
		};

		protected override Func<AggregationContainerDescriptor<Project>, IAggregationContainer> FluentAggs => a => a
			.AdjacencyMatrix("interactions", am => am
				.Filters(fs => fs
					.Filter("grpA", f => f.Term(p => p.State, StateOfBeing.BellyUp))
					.Filter("grpB", f => f.Term(p => p.State, StateOfBeing.Stable))
					.Filter("grpC", f => f.Term(p => p.State, StateOfBeing.VeryActive))
				)
			);

		protected override AggregationDictionary InitializerAggs =>
			new AdjacencyMatrixAggregation("interactions")
			{
				Filters = new NamedFiltersContainer
				{
					{ "grpA", new TermQuery { Field = "state", Value = StateOfBeing.BellyUp } },
					{ "grpB", new TermQuery { Field = "state", Value = StateOfBeing.Stable } },
					{ "grpC", new TermQuery { Field = "state", Value = StateOfBeing.VeryActive } },
				}
			};

		protected override void ExpectResponse(ISearchResponse<Project> response)
		{
			response.ShouldBeValid();
			var interactions = response.Aggregations.AdjacencyMatrix("interactions");
			interactions.Should().NotBeNull();
			var buckets = interactions.Buckets;
			buckets.Should().NotBeNullOrEmpty();
			foreach (var bucket in buckets)
			{
				bucket.Key.Should().NotBeNullOrEmpty();
				bucket.DocCount.Should().BeGreaterThan(0);
			}
		}
	}
}
