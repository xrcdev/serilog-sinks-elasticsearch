// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
	public class ActivateWatchResponse : ResponseBase
	{
		[DataMember(Name ="status")]
		public ActivationStatus Status { get; internal set; }
	}

	[DataContract]
	public class ActivationStatus
	{
		[DataMember(Name ="actions")]
		public IReadOnlyDictionary<string, ActionStatus> Actions { get; set; }

		[DataMember(Name ="state")]
		public ActivationState State { get; internal set; }
	}
}
