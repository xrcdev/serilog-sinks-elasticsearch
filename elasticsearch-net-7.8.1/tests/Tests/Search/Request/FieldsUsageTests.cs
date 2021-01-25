// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using FluentAssertions;
using Nest;
using Tests.Core.ManagedElasticsearch.Clusters;
using Tests.Domain;
using Tests.Framework.EndpointTests.TestState;
using static Nest.Infer;

namespace Tests.Search.Request
{
	/** Allows to selectively load specific stored fields for each document represented by a search hit.
	*
	* WARNING: The `fields` parameter is about fields that are explicitly marked as stored in the mapping,
	* which is off by default and generally not recommended.
	* Use <<source-filtering-usage,source filtering>> instead to select subsets of the original source document to be returned.
	*
	* See the Elasticsearch documentation on {ref_current}/search-request-body.html#request-body-search-stored-fields[Fields] for more detail.
	*/
	public class FieldsUsageTests : SearchUsageTestBase
	{
		public FieldsUsageTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override object ExpectJson => new
		{
			query = ProjectFilterExpectedJson,
			stored_fields = new[] { "name", "startedOn", "numberOfCommits", "numberOfContributors", "dateString" }
		};

		protected override Func<SearchDescriptor<Project>, ISearchRequest> Fluent => s => s
			.Query(q => ProjectFilter)
			.StoredFields(fs => fs
				.Field(p => p.Name)
				.Field(p => p.StartedOn)
				.Field(p => p.NumberOfCommits)
				.Field(p => p.NumberOfContributors)
				.Field(p => p.DateString)
			);

		protected override SearchRequest<Project> Initializer =>
			new SearchRequest<Project>
			{
				Query = ProjectFilter,
				StoredFields = Fields<Project>(
					p => p.Name,
					p => p.StartedOn,
					p => p.NumberOfCommits,
					p => p.NumberOfContributors,
					p => p.DateString)
			};

		[I] protected Task FieldsAreReturned() => AssertOnAllResponses(r =>
		{
			r.Fields.Should().NotBeNull();
			r.Fields.Count().Should().BeGreaterThan(0);
			foreach (var fieldValues in r.Fields)
			{
				fieldValues.Should().NotBeNull("FieldValues on hits is null");
				fieldValues.Count().Should().Be(5);
				var name = fieldValues.Value<string>(Field<Project>(p => p.Name));
				name.Should().NotBeNullOrWhiteSpace();

				var commits = fieldValues.ValueOf<Project, float?>(p => p.NumberOfCommits);
				commits.Should().BeGreaterThan(0);

				var commitsAsNullableInt = fieldValues.ValueOf<Project, int?>(p => p.NumberOfCommits);
				commitsAsNullableInt.Should().BeGreaterThan(0);

				var contributors = fieldValues.ValueOf<Project, int>(p => p.NumberOfContributors);
				contributors.Should().BeGreaterThan(0);

				var dateTime = fieldValues.ValueOf<Project, DateTime>(p => p.StartedOn);
				dateTime.Should().BeAfter(default(DateTime));

				//Date strings should come out verbatim
				var dateTimeAsString = fieldValues.ValueOf<Project, string>(p => p.DateString);
				dateTimeAsString.Should()
					.NotContain("/")
					.And.MatchRegex(@"^\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d.*$");
			}
		});
	}
}
