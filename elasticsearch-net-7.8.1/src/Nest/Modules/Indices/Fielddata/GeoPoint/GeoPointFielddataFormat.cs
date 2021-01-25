// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Runtime.Serialization;
using Elasticsearch.Net;


namespace Nest
{
	[StringEnum]
	public enum GeoPointFielddataFormat
	{
		[EnumMember(Value = "array")]
		Array,

		[EnumMember(Value = "doc_values")]
		DocValues,

		[EnumMember(Value = "compressed")]
		Compressed,

		[EnumMember(Value = "disabled")]
		Disabled
	}
}
