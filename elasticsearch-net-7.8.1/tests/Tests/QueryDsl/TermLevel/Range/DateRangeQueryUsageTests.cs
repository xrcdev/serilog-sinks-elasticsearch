// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using Nest;
using Tests.Core.ManagedElasticsearch.Clusters;
using Tests.Domain;
using Tests.Framework.EndpointTests.TestState;
using static Tests.Domain.Helpers.TestValueHelper;

namespace Tests.QueryDsl.TermLevel.Range
{
	public class DateRangeQueryUsageTests : QueryDslUsageTestsBase
	{
		public DateRangeQueryUsageTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override ConditionlessWhen ConditionlessWhen => new ConditionlessWhen<IDateRangeQuery>(q => q.Range as IDateRangeQuery)
		{
			q => q.Field = null,
			q =>
			{
				q.GreaterThan = null;
				q.GreaterThanOrEqualTo = null;
				q.LessThan = null;
				q.LessThanOrEqualTo = null;
			},
			q =>
			{
				q.GreaterThan = default;
				q.GreaterThanOrEqualTo = default;
				q.LessThan = default;
				q.LessThanOrEqualTo = default;
			}
		};

		protected override QueryContainer QueryInitializer => new DateRangeQuery
		{
			Name = "named_query",
			Boost = 1.1,
			Field = "description",
			GreaterThan = FixedDate,
			GreaterThanOrEqualTo = DateMath.Anchored(FixedDate).RoundTo(DateMathTimeUnit.Month),
			LessThan = "01/01/2012",
			LessThanOrEqualTo = DateMath.Now,
			TimeZone = "+01:00",
			Format = "dd/MM/yyyy||yyyy"
		};

		protected override object QueryJson => new
		{
			range = new
			{
				description = new
				{
					_name = "named_query",
					boost = 1.1,
					format = "dd/MM/yyyy||yyyy",
					gt = "2015-06-06T12:01:02.123",
					gte = "2015-06-06T12:01:02.123||/M",
					lt = "01/01/2012",
					lte = "now",
					time_zone = "+01:00"
				}
			}
		};

		protected override QueryContainer QueryFluent(QueryContainerDescriptor<Project> q) => q
			.DateRange(c => c
				.Name("named_query")
				.Boost(1.1)
				.Field(p => p.Description)
				.GreaterThan(FixedDate)
				.GreaterThanOrEquals(DateMath.Anchored(FixedDate).RoundTo(DateMathTimeUnit.Month))
				.LessThan("01/01/2012")
				.LessThanOrEquals(DateMath.Now)
				.Format("dd/MM/yyyy||yyyy")
				.TimeZone("+01:00")
			);
	}

}
