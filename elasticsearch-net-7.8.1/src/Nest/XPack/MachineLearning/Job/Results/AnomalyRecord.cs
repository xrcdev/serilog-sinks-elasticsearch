// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;

namespace Nest
{
	/// <summary>
	/// Detailed analytical results of anomalous activity that has been
	/// identified in the input data based on the detector configuration.
	/// </summary>
	[DataContract]
	public class AnomalyRecord
	{
		/// <summary>
		/// The actual value for the bucket.
		/// </summary>
		[DataMember(Name = "actual")]
		public IReadOnlyCollection<double> Actual { get; internal set; } = EmptyReadOnly<double>.Collection;

		/// <summary>
		/// The length of the bucket. This value matches the <see cref="AnalysisConfig.BucketSpan" /> that is specified in the job.
		/// </summary>
		[DataMember(Name = "bucket_span")]
		public Time BucketSpan { get; internal set; }

		/// <summary>
		/// The name of the analyzed field. This value is present only if it is specified in the detector.
		/// </summary>
		[DataMember(Name = "by_field_name")]
		public string ByFieldName { get; internal set; }

		/// <summary>
		/// The value of <see cref="ByFieldName" />. This value is present only if it is specified in the detector.
		/// </summary>
		[DataMember(Name = "by_field_value")]
		public string ByFieldValue { get; internal set; }

		/// <summary>
		/// For population analysis, an over field must be specified in the detector.
		/// This property contains an array of anomaly records that are the causes for the anomaly that has been
		/// identified for the over field. If no over fields exist, this field is not present.
		/// Contains the most anomalous records for the <see cref="OverFieldName" />. For scalability reasons,
		/// a maximum of the 10 most significant causes of the anomaly are returned.
		/// As part of the core analytical modeling, these low-level anomaly records are aggregated for their
		/// parent over field record.
		/// </summary>
		[DataMember(Name = "causes")]
		public IReadOnlyCollection<AnomalyCause> Causes { get; internal set; } = EmptyReadOnly<AnomalyCause>.Collection;

		/// <summary>
		/// A unique identifier for the detector.
		/// </summary>
		[DataMember(Name = "detector_index")]
		public int DetectorIndex { get; internal set; }

		/// <summary>
		/// Certain functions require a field to operate on, for example, sum().
		/// For those functions, this value is the name of the field to be analyzed.
		/// </summary>
		[DataMember(Name = "field_name")]
		public string FieldName { get; internal set; }

		/// <summary>
		/// The function in which the anomaly occurs, as specified in the detector configuration.
		/// </summary>
		[DataMember(Name = "function")]
		public string Function { get; internal set; }

		/// <summary>
		/// The description of the function in which the anomaly occurs, as specified in the detector configuration.
		/// </summary>
		[DataMember(Name = "function_description")]
		public string FunctionDescription { get; internal set; }

		/// <summary>
		/// If influencers was specified in the detector configuration, then this
		/// contains influencers that contributed to or were to blame for an anomaly.
		/// </summary>
		[DataMember(Name = "influencers")]
		public IReadOnlyCollection<Influence> Influencers { get; internal set; } = EmptyReadOnly<Influence>.Collection;

		/// <summary>
		/// A normalized score between 0-100, which is based on the probability of the anomalousness of this record.
		/// This is the initial value that was calculated at the time the bucket was processed.
		/// </summary>
		[DataMember(Name = "initial_record_score")]
		public double InitialRecordScore { get; internal set; }

		/// <summary>
		/// If true, this is an interim result. In other words, the anomaly record is calculated
		/// based on partial input data.
		/// </summary>
		[DataMember(Name = "is_interim")]
		public bool IsInterim { get; internal set; }

		/// <summary>
		/// The unique identifier for the job that these results belong to.
		/// </summary>
		[DataMember(Name = "job_id")]
		public string JobId { get; internal set; }

		/// <summary>
		/// The name of the over field that was used in the analysis. This value is present only if it was
		/// specified in the detector. Over fields are used in population analysis.
		/// </summary>
		[DataMember(Name = "over_field_name")]
		public string OverFieldName { get; internal set; }

		/// <summary>
		/// The value of <see cref="OverFieldName" />. This value is present only if it is specified in the detector.
		/// </summary>
		[DataMember(Name = "over_field_value")]
		public string OverFieldValue { get; internal set; }

		/// <summary>
		/// The name of the partition field that was used in the analysis.
		/// This value is present only if it was specified in the detector.
		/// </summary>
		[DataMember(Name = "partition_field_name")]
		public string PartitionFieldName { get; internal set; }

		/// <summary>
		/// The value of <see cref="PartitionFieldName" />. This value is present only if it is specified in the detector.
		/// </summary>
		[DataMember(Name = "partition_field_value")]
		public string PartitionFieldValue { get; internal set; }

		/// <summary>
		/// The probability of the individual anomaly occurring, in the range 0 to 1.
		/// This value can be held to a high precision of over 300 decimal places, so the <see cref="RecordScore" />
		/// is provided as a human-readable and friendly interpretation of this.
		/// </summary>
		/// <example>0.0000772031</example>
		[DataMember(Name = "probability")]
		public double Probability { get; internal set; }

		/// <summary>
		/// A normalized score between 0-100, which is based on the probability of the anomalousness of this record.
		/// Unlike <see cref="InitialRecordScore" />, this value will be updated by a re-normalization process
		/// as new data is analyzed.
		/// </summary>
		[DataMember(Name = "record_score")]
		public double RecordScore { get; internal set; }

		/// <summary>
		/// Internal. This is always set to record.
		/// </summary>
		[DataMember(Name = "result_type")]
		public string ResultType { get; internal set; }

		/// <summary>
		/// The start time of the bucket for which these results were calculated.
		/// </summary>
		[DataMember(Name = "timestamp")]
		[JsonFormatter(typeof(DateTimeOffsetEpochMillisecondsFormatter))]
		public DateTimeOffset Timestamp { get; internal set; }

		/// <summary>
		/// The typical value for the bucket, according to analytical modeling.
		/// </summary>
		/// <remarks>
		/// Additional record properties are added, depending on the fields being analyzed.
		/// For example, if it’s analyzing hostname as a by field, then a field hostname is added to the
		/// result document. This information enables you to filter the anomaly results more easily.
		/// </remarks>
		[DataMember(Name = "typical")]
		public IReadOnlyCollection<double> Typical { get; internal set; } = EmptyReadOnly<double>.Collection;
	}
}
