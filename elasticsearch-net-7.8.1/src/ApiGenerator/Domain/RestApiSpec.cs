// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using ApiGenerator.Configuration;
using ApiGenerator.Domain.Specification;

namespace ApiGenerator.Domain
{
	public class EnumDescription
	{
		public string Name { get; set; }
		public IEnumerable<string> Options { get; set; }
	}

	public class RestApiSpec
	{
		public string Commit { get; set; }

		public static SortedDictionary<string, QueryParameters> CommonApiQueryParameters { get; set; }

		public IDictionary<string, ApiEndpoint> Endpoints { get; set; }

		public ImmutableSortedDictionary<string, ReadOnlyCollection<ApiEndpoint>> EndpointsPerNamespaceLowLevel =>
			Endpoints.Values.GroupBy(e=>e.CsharpNames.Namespace)
				.ToImmutableSortedDictionary(kv => kv.Key, kv => kv.ToList().AsReadOnly());

		public ImmutableSortedDictionary<string, ReadOnlyCollection<ApiEndpoint>> EndpointsPerNamespaceHighLevel =>
			Endpoints.Values
				.Where(v => !CodeConfiguration.IgnoredApisHighLevel.Contains(v.FileName))
				.GroupBy(e => e.CsharpNames.Namespace)
				.ToImmutableSortedDictionary(kv => kv.Key, kv => kv.ToList().AsReadOnly());

		private IEnumerable<EnumDescription> _enumDescriptions;
		public IEnumerable<EnumDescription> EnumsInTheSpec
		{
			get
			{
				if (_enumDescriptions != null) return _enumDescriptions;

				string CreateName(string name, string methodName, string @namespace)
				{
					if (
						name.ToLowerInvariant().Contains("metric")
						 ||(name.ToLowerInvariant() == "status")
						 ||(name.ToLowerInvariant() == "format")
					)
					{
						if (methodName.StartsWith(@namespace))
							return methodName + name;
						else
							return @namespace + methodName + name;
					}

					return name;
				}

				var urlParameterEnums = (
					from e in Endpoints.Values
					from para in e.Url.Params.Values
					where para.Options != null && para.Options.Any()
					let name = CreateName(para.ClsName, e.CsharpNames.MethodName, e.CsharpNames.Namespace)
					where name != "Time"
					select new EnumDescription
					{
						Name = name,
						Options = para.Options
					}).ToList();

				var urlPartEnums = (
					from e in Endpoints.Values
					from part in e.Url.Parts
					where part.Options != null && part.Options.Any()
					select new EnumDescription
					{
						Name = CreateName(part.Name.ToPascalCase(), e.CsharpNames.MethodName, e.CsharpNames.Namespace),
						Options = part.Options
					}).ToList();

				_enumDescriptions = urlPartEnums
					.Concat(urlParameterEnums)
					.DistinctBy(e => e.Name)
					.ToList();

				//TODO can be removed in 8.x
				var versionType = _enumDescriptions.FirstOrDefault(f => f.Name == "VersionType");
				if (versionType != null)
				{
					var options = new List<string>(versionType.Options);
					options.Add("force");
					versionType.Options = options;
				}

				return _enumDescriptions;
			}
		}
	}
}
