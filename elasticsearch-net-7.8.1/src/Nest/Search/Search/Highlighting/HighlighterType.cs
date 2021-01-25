// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Runtime.Serialization;
using Elasticsearch.Net;


namespace Nest
{
	/// <summary>
	/// Type of highlighter
	/// </summary>
	[StringEnum]
	public enum HighlighterType
	{
		/// <summary>
		/// Plain Highlighter.
		/// Uses the Lucene highlighter.
		/// It tries hard to reflect the query matching logic in terms of understanding word
		/// importance and any word positioning criteria in phrase queries.
		/// </summary>
		[EnumMember(Value = "plain")]
		Plain,

		/// <summary>
		/// Fast Vector Highlighter.
		/// If term_vector information is provided by setting term_vector to with_positions_offsets
		/// in the mapping then the fast vector highlighter will be used instead of the plain highlighter
		/// </summary>
		[EnumMember(Value = "fvh")]
		Fvh,

		/// <summary>
		/// Unified Highlighter.
		/// The default choice.
		/// The unified highlighter can extract offsets from either term vectors, or via re-analyzing text.
		/// Under the hood it uses Lucene UnifiedHighlighter which picks its strategy depending on the field and the query to highlight.
		/// Independently of the strategy this highlighter breaks the text into sentences and scores individual sentences as if
		/// they were documents in this corpus, using the BM25 algorithm. It supports accurate phrase and multi-term
		/// (fuzzy, prefix, regex) highlighting
		/// </summary>
		[EnumMember(Value = "unified")]
		Unified
	}
}
