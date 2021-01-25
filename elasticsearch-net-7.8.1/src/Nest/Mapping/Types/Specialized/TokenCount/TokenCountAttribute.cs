// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿namespace Nest
{
	public class TokenCountAttribute : ElasticsearchDocValuesPropertyAttributeBase, ITokenCountProperty
	{
		public TokenCountAttribute() : base(FieldType.TokenCount) { }

		public string Analyzer
		{
			get => Self.Analyzer;
			set => Self.Analyzer = value;
		}

		public double Boost
		{
			get => Self.Boost.GetValueOrDefault();
			set => Self.Boost = value;
		}

		public bool Index
		{
			get => Self.Index.GetValueOrDefault();
			set => Self.Index = value;
		}

		public double NullValue
		{
			get => Self.NullValue.GetValueOrDefault();
			set => Self.NullValue = value;
		}

		string ITokenCountProperty.Analyzer { get; set; }
		double? ITokenCountProperty.Boost { get; set; }
		bool? ITokenCountProperty.Index { get; set; }
		double? ITokenCountProperty.NullValue { get; set; }
		private ITokenCountProperty Self => this;
	}
}
