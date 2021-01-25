// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using Nest;
using Tests.Core.ManagedElasticsearch.Clusters;
using Tests.Domain;
using Tests.Framework.EndpointTests.TestState;

namespace Tests.QueryDsl.TermLevel.Range
{
	public class RangeQueryUsageTests : QueryDslUsageTestsBase
	{
		public RangeQueryUsageTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override ConditionlessWhen ConditionlessWhen => new ConditionlessWhen<INumericRangeQuery>(q => q.Range as INumericRangeQuery)
		{
			q => q.Field = null,
			q =>
			{
				q.GreaterThan = null;
				q.GreaterThanOrEqualTo = null;
				q.LessThan = null;
				q.LessThanOrEqualTo = null;
			}
		};

		protected override QueryContainer QueryInitializer => new NumericRangeQuery
		{
			Name = "named_query",
			Boost = 1.1,
			Field = "description",
			GreaterThan = 1.0,
			GreaterThanOrEqualTo = 1.1,
			LessThan = 2.1,
			LessThanOrEqualTo = 2.0,
			Relation = RangeRelation.Within
		};

		protected override object QueryJson => new
		{
			range = new
			{
				description = new
				{
					_name = "named_query",
					boost = 1.1,
					gt = 1.0,
					gte = 1.1,
					lt = 2.1,
					lte = 2.0,
					relation = "within"
				}
			}
		};

		protected override QueryContainer QueryFluent(QueryContainerDescriptor<Project> q) => q
			.Range(c => c
				.Name("named_query")
				.Boost(1.1)
				.Field(p => p.Description)
				.GreaterThan(1.0)
				.GreaterThanOrEquals(1.1)
				.LessThan(2.1)
				.LessThanOrEquals(2.0)
				.Relation(RangeRelation.Within)
			);
	}
}
