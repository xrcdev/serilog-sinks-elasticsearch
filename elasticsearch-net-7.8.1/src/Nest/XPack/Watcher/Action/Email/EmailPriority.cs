// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Runtime.Serialization;
using Elasticsearch.Net;


namespace Nest
{
	[StringEnum]
	public enum EmailPriority
	{
		[EnumMember(Value = "lowest")]
		Lowest,

		[EnumMember(Value = "low")]
		Low,

		[EnumMember(Value = "normal")]
		Normal,

		[EnumMember(Value = "high")]
		High,

		[EnumMember(Value = "highest")]
		Highest
	}
}
