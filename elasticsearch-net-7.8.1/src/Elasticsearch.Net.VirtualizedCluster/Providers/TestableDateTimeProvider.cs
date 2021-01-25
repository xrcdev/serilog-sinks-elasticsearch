// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System;

namespace Elasticsearch.Net.VirtualizedCluster.Providers
{
	public class TestableDateTimeProvider : DateTimeProvider
	{
		private DateTime MutableNow { get; set; } = DateTime.UtcNow;

		public override DateTime Now() => MutableNow;

		public void ChangeTime(Func<DateTime, DateTime> change) => MutableNow = change(MutableNow);
	}
}
