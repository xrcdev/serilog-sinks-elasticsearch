// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Linq;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using FluentAssertions;
using Nest;

namespace Tests.QueryDsl.BoolDsl.Operators
{
	public class OrOperatorUsageTests : OperatorUsageBase
	{
		private static readonly int Iterations = 10000;

		[U] public void Or()
		{
			ReturnsBool(Query || Query, q => q.Query() || q.Query(), b =>
			{
				b.Should.Should().NotBeEmpty().And.HaveCount(2);
				b.Must.Should().BeNull();
				b.MustNot.Should().BeNull();
				b.Filter.Should().BeNull();
			});

			ReturnsBool(Query || Query || ConditionlessQuery, q => q.Query() || q.Query() || q.ConditionlessQuery(), b =>
			{
				b.Should.Should().NotBeEmpty().And.HaveCount(2);
				b.Must.Should().BeNull();
				b.MustNot.Should().BeNull();
				b.Filter.Should().BeNull();
			});

			ReturnsBool(Query || Query || ConditionlessQuery, q => q.Query() || q.Query() || q.ConditionlessQuery(), b =>
			{
				b.Should.Should().NotBeEmpty().And.HaveCount(2);
				b.Must.Should().BeNull();
				b.MustNot.Should().BeNull();
				b.Filter.Should().BeNull();
			});

			ReturnsSingleQuery(Query || ConditionlessQuery, q => q.Query() || q.ConditionlessQuery(),
				c => c.Term.Value.Should().NotBeNull());

			ReturnsSingleQuery(ConditionlessQuery || Query, q => q.ConditionlessQuery() || q.Query(),
				c => c.Term.Value.Should().NotBeNull());

			ReturnsSingleQuery(Query || NullQuery, q => q.Query() || q.NullQuery(),
				c => c.Term.Value.Should().NotBeNull());

			ReturnsSingleQuery(NullQuery || Query, q => q.NullQuery() || q.Query(),
				c => c.Term.Value.Should().NotBeNull());

			ReturnsSingleQuery(ConditionlessQuery || ConditionlessQuery || ConditionlessQuery || Query,
				q => q.ConditionlessQuery() || q.ConditionlessQuery() || q.ConditionlessQuery() || q.Query(),
				c => c.Term.Value.Should().NotBeNull());

			ReturnsSingleQuery(
				NullQuery || NullQuery || ConditionlessQuery || Query,
				q => q.NullQuery() || q.NullQuery() || q.ConditionlessQuery() || q.Query(),
				c => c.Term.Value.Should().NotBeNull());

			ReturnsNull(NullQuery || ConditionlessQuery, q => q.NullQuery() || q.ConditionlessQuery());
			ReturnsNull(ConditionlessQuery || NullQuery, q => q.ConditionlessQuery() || q.NullQuery());
			ReturnsNull(ConditionlessQuery || ConditionlessQuery, q => q.ConditionlessQuery() || q.ConditionlessQuery());
			ReturnsNull(
				ConditionlessQuery || ConditionlessQuery || ConditionlessQuery || ConditionlessQuery,
				q => q.ConditionlessQuery() || q.ConditionlessQuery() || q.ConditionlessQuery() || q.ConditionlessQuery()
			);
			ReturnsNull(
				NullQuery || ConditionlessQuery || ConditionlessQuery || ConditionlessQuery,
				q => q.NullQuery() || q.ConditionlessQuery() || q.ConditionlessQuery() || q.ConditionlessQuery()
			);
		}

		[U] public void CombiningManyUsingAggregate()
		{
			var lotsOfOrs = Enumerable.Range(0, Iterations).Aggregate(new QueryContainer(), (q, c) => q || Query, q => q);
			LotsOfOrs(lotsOfOrs);
		}

		[U] public void CombiningManyUsingForeachInitializingWithNull()
		{
			QueryContainer container = null;
			foreach (var i in Enumerable.Range(0, Iterations))
				container |= Query;
			LotsOfOrs(container);
		}

		[U] public void CombiningManyUsingForeachInitializingWithDefault()
		{
			var container = new QueryContainer();
			foreach (var i in Enumerable.Range(0, Iterations))
				container |= Query;
			LotsOfOrs(container);
		}

		private void LotsOfOrs(IQueryContainer lotsOfOrs)
		{
			lotsOfOrs.Should().NotBeNull();
			lotsOfOrs.Bool.Should().NotBeNull();
			lotsOfOrs.Bool.Should.Should().NotBeEmpty().And.HaveCount(Iterations);
		}
	}
}
