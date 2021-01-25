// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System.Runtime.Serialization;

namespace Nest
{
	public class RecoveryOrigin
	{
		[DataMember(Name ="hostname")]
		public string HostName { get; internal set; }

		[DataMember(Name ="id")]
		public string Id { get; internal set; }

		[DataMember(Name ="ip")]
		public string Ip { get; internal set; }

		[DataMember(Name ="name")]
		public string Name { get; internal set; }
	}
}
