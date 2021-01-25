// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using Elasticsearch.Net.Utf8Json;

namespace Nest
{
	[InterfaceDataContract]
	public interface ISchedule { }

	public abstract class ScheduleBase : ISchedule
	{
		internal abstract void WrapInContainer(IScheduleContainer container);
	}
}
