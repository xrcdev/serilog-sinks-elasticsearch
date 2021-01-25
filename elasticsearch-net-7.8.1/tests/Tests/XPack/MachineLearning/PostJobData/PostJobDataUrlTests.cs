// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Threading.Tasks;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using Nest;
using Tests.Framework.EndpointTests;
using static Tests.Framework.EndpointTests.UrlTester;

namespace Tests.XPack.MachineLearning.PostJobData
{
	public class PostJobDataUrlTests : UrlTestsBase
	{
		[U] public override async Task Urls() => await POST("/_ml/anomaly_detectors/job_id/_data")
			.Fluent(c => c.MachineLearning.PostJobData("job_id", p => p))
			.Request(c => c.MachineLearning.PostJobData(new PostJobDataRequest("job_id")))
			.FluentAsync(c => c.MachineLearning.PostJobDataAsync("job_id", p => p))
			.RequestAsync(c => c.MachineLearning.PostJobDataAsync(new PostJobDataRequest("job_id")));
	}
}
