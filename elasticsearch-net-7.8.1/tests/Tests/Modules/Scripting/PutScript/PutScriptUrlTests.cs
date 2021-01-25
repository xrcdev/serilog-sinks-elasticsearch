// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Threading.Tasks;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using Nest;
using Tests.Framework.EndpointTests;
using static Tests.Framework.EndpointTests.UrlTester;

namespace Tests.Modules.Scripting.PutScript
{
	public class PutScriptUrlTests
	{
		[U] public async Task Urls()
		{
			var id = "id";

			await PUT($"/_scripts/{id}")
					.Fluent(c => c.PutScript(id, s => s.Painless("")))
					.Request(c => c.PutScript(new PutScriptRequest(id)))
					.FluentAsync(c => c.PutScriptAsync(id, s => s.Painless("")))
					.RequestAsync(c => c.PutScriptAsync(new PutScriptRequest(id)))
				;
		}
	}
}
