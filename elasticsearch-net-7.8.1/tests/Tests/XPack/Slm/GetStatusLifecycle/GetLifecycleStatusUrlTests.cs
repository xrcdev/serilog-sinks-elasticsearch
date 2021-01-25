// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Threading.Tasks;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using Nest;
using Tests.Framework.EndpointTests;
using static Tests.Framework.EndpointTests.UrlTester;

namespace Tests.XPack.Slm.GetStatusLifecycle
{
	public class GetLifecycleStatusUrlTests : UrlTestsBase
	{
		[U] public override async Task Urls() =>
			await GET("/_slm/status")
				.Fluent(c => c.SnapshotLifecycleManagement.GetStatus())
				.Request(c => c.SnapshotLifecycleManagement.GetStatus(new GetSnapshotLifecycleManagementStatusRequest()))
				.FluentAsync(c => c.SnapshotLifecycleManagement.GetStatusAsync())
				.RequestAsync(c => c.SnapshotLifecycleManagement.GetStatusAsync(new GetSnapshotLifecycleManagementStatusRequest()));
	}
}
