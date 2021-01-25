// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System;
using Nest;
using Tests.Core.ManagedElasticsearch.Clusters;
using Tests.Domain;
using Tests.Framework.EndpointTests.TestState;

namespace Tests.Mapping.Types.Core.Date
{
	public class DatePropertyTests : PropertyTestsBase
	{
		public DatePropertyTests(WritableCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override object ExpectJson => new
		{
			properties = new
			{
				lastActivity = new
				{
					type = "date",
					doc_values = false,
					similarity = "BM25",
					store = true,
					index = false,
					boost = 1.2,
					ignore_malformed = true,
					format = "yyyy-MM-dd'T'HH:mm[:ss][.S]",
					null_value = DateTime.MinValue
				}
			}
		};

		protected override Func<PropertiesDescriptor<Project>, IPromise<IProperties>> FluentProperties => f => f
			.Date(b => b
				.Name(p => p.LastActivity)
				.DocValues(false)
				.Similarity("BM25")
				.Store()
				.Index(false)
				.Boost(1.2)
				.IgnoreMalformed()
				.Format("yyyy-MM-dd'T'HH:mm[:ss][.S]")
				.NullValue(DateTime.MinValue)
			);

		protected override IProperties InitializerProperties => new Properties
		{
			{
				"lastActivity", new DateProperty
				{
					DocValues = false,
					Similarity = "BM25",
					Store = true,
					Index = false,
					Boost = 1.2,
					IgnoreMalformed = true,
					Format = "yyyy-MM-dd'T'HH:mm[:ss][.S]",
					NullValue = DateTime.MinValue
				}
			}
		};
	}
}
