// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Threading.Tasks;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using Nest;
using Tests.Framework.EndpointTests;
using static Tests.Framework.EndpointTests.UrlTester;

namespace Tests.XPack.License.DeleteLicense
{
	public class DeleteLicenseUrlTests : UrlTestsBase
	{
		[U] public override async Task Urls() => await DELETE("/_license")
			.Fluent(c => c.License.Delete())
			.Request(c => c.License.Delete(new DeleteLicenseRequest()))
			.FluentAsync(c => c.License.DeleteAsync())
			.RequestAsync(c => c.License.DeleteAsync(new DeleteLicenseRequest()));
	}
}
