// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Runtime.Serialization;
using Elasticsearch.Net.Utf8Json;

namespace Nest
{
	/// <summary>
	/// The response from updating a datafeed
	/// </summary>
	public class UpdateDatafeedResponse : ResponseBase
	{
		/// <summary>
		/// The aggregation searches to perform for the datafeed.
		/// </summary>
		[DataMember(Name = "aggregations")]
		public AggregationDictionary Aggregations { get; internal set; }

		/// <summary>
		/// Specifies how data searches are split into time chunks.
		/// </summary>
		[DataMember(Name = "chunking_config")]
		public IChunkingConfig ChunkingConfig { get; internal set; }

		/// <summary>
		/// The datafeed id.
		/// </summary>
		[DataMember(Name = "datafeed_id")]
		public string DatafeedId { get; internal set; }

		/// <summary>
		/// The interval at which scheduled queries are made while the datafeed runs in real time.
		/// The default value is either the bucket span for short bucket spans, or, for longer bucket spans,
		/// a sensible fraction of the bucket span.
		/// </summary>
		[DataMember(Name = "frequency")]
		public Time Frequency { get; internal set; }

		///<summary>A list of index names to search within, wildcards are supported.</summary>
		[DataMember(Name = "indices")]
		[JsonFormatter(typeof(IndicesFormatter))]
		public Indices Indices { get; internal set; }

		/// <summary>
		/// A numerical character string that uniquely identifies the job.
		/// </summary>
		[DataMember(Name = "job_id")]
		public string JobId { get; internal set; }

		/// <summary>
		/// Describe the query to perform using a query descriptor lambda
		/// </summary>
		[DataMember(Name = "query")]
		public QueryContainer Query { get; internal set; }

		/// <summary>
		/// The number of seconds behind real time that data is queried.
		/// For example, if data from 10:04 a.m. might not be searchable until 10:06 a.m.,
		/// set this property to 120 seconds. The default value is 60s.
		/// </summary>
		[DataMember(Name = "query_delay")]
		public Time QueryDelay { get; internal set; }

		/// <summary>
		/// Specifies scripts that evaluate custom expressions and returns script fields to the datafeed.
		/// The detector configuration in a job can contain functions that use these script fields.
		/// </summary>
		[DataMember(Name = "script_fields")]
		public IScriptFields ScriptFields { get; internal set; }

		/// <summary>
		/// The size parameter that is used in Elasticsearch searches
		/// </summary>
		[DataMember(Name = "scroll_size")]
		public int? ScrollSize { get; internal set; }

		/// <summary>
		/// If a real-time datafeed has never seen any data (including during any initial training period) then it will automatically stop
		/// itself and close its associated job after this many real-time searches that return no documents. In other words, it will
		/// stop after <see cref="Frequency"/> times <see cref="MaximumEmptySearches"/> of real-time operation. If not set then a datafeed
		/// with no end time that sees no data will remain started until it is explicitly stopped.
		/// </summary>
		[DataMember(Name ="max_empty_searches")]
		public int? MaximumEmptySearches { get; set; }
	}
}
