// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Collections.Generic;
using Elasticsearch.Net;

namespace Nest
{
	public class NodesHotThreadsResponse : ResponseBase
	{
		public NodesHotThreadsResponse() { }

		internal NodesHotThreadsResponse(IReadOnlyCollection<HotThreadInformation> threadInfo) => HotThreads = threadInfo;

		public IReadOnlyCollection<HotThreadInformation> HotThreads { get; internal set; } = EmptyReadOnly<HotThreadInformation>.Collection;
	}

	public class HotThreadInformation
	{
		public IReadOnlyCollection<string> Hosts { get; internal set; } = EmptyReadOnly<string>.Collection;
		public string NodeId { get; internal set; }
		public string NodeName { get; internal set; }
		public IReadOnlyCollection<string> Threads { get; internal set; } = EmptyReadOnly<string>.Collection;
	}
}
