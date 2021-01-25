// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Elasticsearch.Net;

namespace Nest
{
	public class InvalidateUserAccessTokenResponse : ResponseBase
	{
		/// <summary>
		/// The number of the tokens that were invalidated as part of this request.
		/// </summary>
		[DataMember(Name = "invalidated_tokens")]
		public long InvalidatedTokens { get; internal set;  }

		/// <summary>
		/// The number of tokens that were already invalidated.
		/// </summary>
		[DataMember(Name = "previously_invalidated_tokens")]
		public long PreviouslyInvalidatedTokens { get; internal set;  }

		/// <summary>
		/// The number of errors that were encountered when invalidating the tokens.
		/// </summary>
		[DataMember(Name = "error_count")]
		public long ErrorCount { get; internal set;  }

		/// <summary>
		/// Details about these errors. This field is not present in the response when there are no errors.
		/// </summary>
		[DataMember(Name = "error_details")]
		public IReadOnlyCollection<ErrorCause> ErrorDetails { get; internal set; } = EmptyReadOnly<ErrorCause>.Collection;
	}
}
