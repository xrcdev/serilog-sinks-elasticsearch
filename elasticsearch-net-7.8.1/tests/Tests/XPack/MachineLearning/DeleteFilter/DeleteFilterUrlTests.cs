// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Threading.Tasks;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using Nest;
using Tests.Framework.EndpointTests;
using static Tests.Framework.EndpointTests.UrlTester;

namespace Tests.XPack.MachineLearning.DeleteFilter
{
	public class DeleteFilterUrlTests : UrlTestsBase
	{
		[U] public override async Task Urls() =>
			await DELETE("/_ml/filters/filter_id")
				.Fluent(c => c.MachineLearning.DeleteFilter("filter_id", p => p))
				.Request(c => c.MachineLearning.DeleteFilter(new DeleteFilterRequest("filter_id")))
				.FluentAsync(c => c.MachineLearning.DeleteFilterAsync("filter_id", p => p))
				.RequestAsync(c => c.MachineLearning.DeleteFilterAsync(new DeleteFilterRequest("filter_id")));
	}
}
