// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System;
using Nest;
using Tests.Core.ManagedElasticsearch.Clusters;
using Tests.Domain;
using Tests.Framework.EndpointTests.TestState;

namespace Tests.QueryDsl.TermLevel.Fuzzy
{
	public class FuzzyDateQueryUsageTests : QueryDslUsageTestsBase
	{
		public FuzzyDateQueryUsageTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override ConditionlessWhen ConditionlessWhen => new ConditionlessWhen<IFuzzyQuery<DateTime?, Time>>(
			a => a.Fuzzy as IFuzzyQuery<DateTime?, Time>
		)
		{
			q => q.Field = null,
			q => q.Value = null
		};

		protected override QueryContainer QueryInitializer => new FuzzyDateQuery
		{
			Name = "named_query",
			Boost = 1.1,
			Field = "description",
			Fuzziness = TimeSpan.FromDays(2),
			Value = Project.Instance.StartedOn,
			MaxExpansions = 100,
			PrefixLength = 3,
			Rewrite = MultiTermQueryRewrite.ConstantScore,
			Transpositions = true
		};

		protected override object QueryJson => new
		{
			fuzzy = new
			{
				description = new
				{
					_name = "named_query",
					boost = 1.1,
					fuzziness = "2d",
					max_expansions = 100,
					prefix_length = 3,
					rewrite = "constant_score",
					transpositions = true,
					value = "2015-01-01T00:00:00"
				}
			}
		};

		protected override QueryContainer QueryFluent(QueryContainerDescriptor<Project> q) => q
			.FuzzyDate(c => c
				.Name("named_query")
				.Boost(1.1)
				.Field(p => p.Description)
				.Fuzziness(TimeSpan.FromDays(2))
				.Value(Project.Instance.StartedOn)
				.MaxExpansions(100)
				.PrefixLength(3)
				.Rewrite(MultiTermQueryRewrite.ConstantScore)
				.Transpositions()
			);
	}
}
