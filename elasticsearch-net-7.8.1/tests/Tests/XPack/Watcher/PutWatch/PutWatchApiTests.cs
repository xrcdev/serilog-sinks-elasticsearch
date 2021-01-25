// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elastic.Elasticsearch.Xunit.XunitPlumbing;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Tests.Domain;
using Tests.Framework.EndpointTests;
using Tests.Framework.EndpointTests.TestState;

namespace Tests.XPack.Watcher.PutWatch
{
	public class PutWatchApiTests : ApiIntegrationTestBase<WatcherCluster, PutWatchResponse, IPutWatchRequest, PutWatchDescriptor, PutWatchRequest>
	{
		public PutWatchApiTests(WatcherCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override bool ExpectIsValid => true;

		protected override object ExpectJson =>
			new
			{
				input = new
				{
					chain = new
					{
						inputs = new object[]
						{
							new
							{
								simple = new
								{
									simple = new
									{
										str = "val1",
										num = 23,
										obj = new
										{
											str = "val2"
										}
									}
								}
							},
							new
							{
								http = new
								{
									http = new
									{
										request = new
										{
											host = "localhost",
											port = 8080,
											method = "post",
											path = "/path.html",
											proxy = new
											{
												host = "proxy",
												port = 6000
											},
											scheme = "https",
											auth = new
											{
												basic = new
												{
													username = "Username123",
													password = "Password123"
												}
											},
											body =
												"{\"query\" : {\"range\": {\"@timestamp\" : {\"from\": \"{{ctx.trigger.triggered_time}}||-5m\",\"to\": \"{{ctx.trigger.triggered_time}}\"}}}}",
											headers = new
											{
												header1 = "value1"
											},
											@params = new
											{
												lat = "52.374031",
												lon = "4.88969",
												appid = "appid"
											},
											connection_timeout = "3s",
											read_timeout = "500ms"
										},
										response_content_type = "text"
									}
								}
							},
							new
							{
								search = new
								{
									search = new
									{
										request = new
										{
											indices = new[] { "project" },
											body = new
											{
												size = 0,
												aggs = new
												{
													nested_tags = new
													{
														nested = new
														{
															path = "tags"
														},
														aggs = new
														{
															top_project_tags = new
															{
																terms = new
																{
																	field = "tags.name"
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				},
				transform = new
				{
					chain = new object[]
					{
						new
						{
							search = new
							{
								request = new
								{
									indices = new[] { "project" },
									indices_options = new
									{
										expand_wildcards = "open",
										ignore_unavailable = true
									},
									search_type = "dfs_query_then_fetch",
									body = new
									{
										query = new
										{
											match = new
											{
												state = new
												{
													query = "stable"
												}
											}
										}
									}
								},
								timeout = "10s"
							}
						},
						new
						{
							script = new
							{
								source = "return [ 'time' : ctx.trigger.scheduled_time ]",
							}
						}
					}
				},
				condition = new
				{
					array_compare = new Dictionary<string, object>
					{
						{
							"ctx.payload.search.aggregations.top_project_tags.buckets", new Dictionary<string, object>
							{
								{ "path", "doc_count" },
								{ "gte", new Dictionary<string, object> { { "value", 1 } } }
							}
						}
					}
				},
				trigger = new
				{
					schedule = new
					{
						weekly = new[]
						{
							new { on = new[] { "monday" }, at = new[] { "noon" } },
							new { on = new[] { "friday" }, at = new[] { "17:00" } }
						}
					}
				},
				actions = new
				{
					reminder_email = new
					{
						email = new
						{
							to = new[] { "me@example.com" },
							subject = "Something's strange in the neighbourhood",
							body = new
							{
								text = "Dear {{ctx.payload.name}}, by the time you read these lines, I'll be gone"
							},
							attachments = new
							{
								http_attachment = new
								{
									http = new
									{
										inline = true,
										content_type = RequestData.MimeType,
										request = new
										{
											url = "http://localhost:8080/http_attachment"
										}
									}
								},
								data_attachment = new
								{
									data = new
									{
										format = "json"
									}
								}
							}
						}
					},
					reminder_index = new
					{
						index = new
						{
							index = "put-watch-test-index",
							execution_time_field = "execution_time"
						}
					},
					reminder_pagerduty = new
					{
						pagerduty = new
						{
							account = "my_pagerduty_account",
							description = "pager duty description",
							attach_payload = true,
							event_type = "trigger",
							incident_key = "incident_key",
							context = new object[]
							{
								new
								{
									type = "image",
									src = "http://example.com/image"
								},
								new
								{
									type = "link",
									href = "http://example.com/link"
								}
							}
						}
					},
					reminder_slack = new
					{
						slack = new
						{
							account = "monitoring",
							message = new
							{
								from = "nest integration test",
								to = new[] { "#nest" },
								text = "slack message",
								attachments = new[]
								{
									new
									{
										title = "Attachment 1",
										author_name = "Russ Cam"
									}
								}
							}
						}
					},
					webhook = new
					{
						webhook = new
						{
							scheme = "https",
							host = "localhost",
							port = 9200,
							method = "post",
							path = "/_bulk",
							auth = new
							{
								basic = new
								{
									username = "username",
									password = "password"
								}
							},
							body = "{{ctx.payload._value}}"
						}
					}
				}
			};

		protected override int ExpectStatusCode => 201;

		protected override Func<PutWatchDescriptor, IPutWatchRequest> Fluent => p => p
			.Input(i => i
				.Chain(c => c
					.Input("simple", ci => ci
						.Simple(s => s
							.Add("str", "val1")
							.Add("num", 23)
							.Add("obj", new { str = "val2" })
						)
					)
					.Input("http", ci => ci
						.Http(h => h
							.Request(r => r
								.Host("localhost")
								.Port(8080)
								.Method(HttpInputMethod.Post)
								.Path("/path.html")
								.Proxy(pr => pr
									.Host("proxy")
									.Port(6000)
								)
								.Scheme(ConnectionScheme.Https)
								.Authentication(a => a
									.Basic(b => b
										.Username("Username123")
										.Password("Password123")
									)
								)
								.Body(
									"{\"query\" : {\"range\": {\"@timestamp\" : {\"from\": \"{{ctx.trigger.triggered_time}}||-5m\",\"to\": \"{{ctx.trigger.triggered_time}}\"}}}}")
								.Headers(he => he
									.Add("header1", "value1")
								)
								.Params(pa => pa
									.Add("lat", "52.374031")
									.Add("lon", "4.88969")
									.Add("appid", "appid")
								)
								.ConnectionTimeout("3s")
								.ReadTimeout(TimeSpan.FromMilliseconds(500))
							)
							.ResponseContentType(ResponseContentType.Text)
						)
					)
					.Input("search", ci => ci
						.Search(s => s
							.Request(si => si
								.Indices<Project>()
								.Body<Project>(b => b
									.Size(0)
									.Aggregations(a => a
										.Nested("nested_tags", n => n
											.Path(np => np.Tags)
											.Aggregations(aa => aa
												.Terms("top_project_tags", ta => ta
													.Field(f => f.Tags.First().Name)
												)
											)
										)
									)
								)
							)
						)
					)
				)
			)
			.Transform(tr => tr
				.Chain(ct => ct
					.Transform(ctt => ctt
						.Search(st => st
							.Request(str => str
								.Indices(typeof(Project))
								.SearchType(SearchType.DfsQueryThenFetch)
								.IndicesOptions(io => io
									.ExpandWildcards(ExpandWildcards.Open)
									.IgnoreUnavailable()
								)
								.Body<Project>(b => b
									.Query(q => q
										.Match(m => m
											.Field("state")
											.Query(StateOfBeing.Stable.ToString().ToLowerInvariant())
										)
									)
								)
							)
							.Timeout("10s")
						)
					)
					.Transform(ctt => ctt
						.Script(st => st
							.Source("return [ 'time' : ctx.trigger.scheduled_time ]")
						)
					)
				)
			)
			.Condition(co => co
				.ArrayCompare(ac => ac
					.GreaterThanOrEqualTo("ctx.payload.search.aggregations.top_project_tags.buckets", "doc_count", 1)
				)
			)
			.Trigger(t => t
				.Schedule(s => s
					.Weekly(w => w
						.Add(ti => ti
							.On(Day.Monday)
							.At("noon")
						)
						.Add(ti => ti
							.On(Day.Friday)
							.At("17:00")
						)
					)
				)
			)
			.Actions(a => a
				.Email("reminder_email", e => e
					.To("me@example.com")
					.Subject("Something's strange in the neighbourhood")
					.Body(b => b
						.Text("Dear {{ctx.payload.name}}, by the time you read these lines, I'll be gone")
					)
					.Attachments(ea => ea
						.HttpAttachment("http_attachment", ha => ha
							.Inline()
							.ContentType(RequestData.MimeType)
							.Request(r => r
								.Url("http://localhost:8080/http_attachment")
							)
						)
						.DataAttachment("data_attachment", da => da
							.Format(DataAttachmentFormat.Json)
						)
					)
				)
				.Index("reminder_index", i => i
					.Index("put-watch-test-index")
					.ExecutionTimeField("execution_time")
				)
				.PagerDuty("reminder_pagerduty", pd => pd
					.Account("my_pagerduty_account")
					.Description("pager duty description")
					.AttachPayload()
					.EventType(PagerDutyEventType.Trigger)
					.IncidentKey("incident_key")
					.Context(c => c
						.Context(PagerDutyContextType.Image, cd => cd
							.Src("http://example.com/image")
						)
						.Context(PagerDutyContextType.Link, cd => cd
							.Href("http://example.com/link")
						)
					)
				)
				.Slack("reminder_slack", sl => sl
					.Account("monitoring")
					.Message(sm => sm
						.From("nest integration test")
						.To("#nest")
						.Text("slack message")
						.Attachments(sa => sa
							.Attachment(saa => saa
								.Title("Attachment 1")
								.AuthorName("Russ Cam")
							)
						)
					)
				)
				.Webhook("webhook", w => w
					.Scheme(ConnectionScheme.Https)
					.Host("localhost")
					.Port(9200)
					.Method(HttpInputMethod.Post)
					.Path("/_bulk")
					.Authentication(au => au
						.Basic(b => b
							.Username("username")
							.Password("password")
						)
					)
					.Body("{{ctx.payload._value}}")
				)
			);

		protected override HttpMethod HttpMethod => HttpMethod.PUT;

		protected override PutWatchRequest Initializer =>
			new PutWatchRequest(CallIsolatedValue)
			{
				Input = new ChainInput
				{
					Inputs = new Dictionary<string, InputContainer>
					{
						{
							"simple", new SimpleInput
							{
								{ "str", "val1" },
								{ "num", 23 },
								{ "obj", new { str = "val2" } }
							}
						},
						{
							"http", new HttpInput
							{
								Request = new HttpInputRequest
								{
									Host = "localhost",
									Port = 8080,
									Method = HttpInputMethod.Post,
									Path = "/path.html",
									Proxy = new HttpInputProxy
									{
										Host = "proxy",
										Port = 6000
									},
									Scheme = ConnectionScheme.Https,
									Authentication = new HttpInputAuthentication
									{
										Basic = new HttpInputBasicAuthentication
										{
											Username = "Username123",
											Password = "Password123"
										}
									},
									Body =
										"{\"query\" : {\"range\": {\"@timestamp\" : {\"from\": \"{{ctx.trigger.triggered_time}}||-5m\",\"to\": \"{{ctx.trigger.triggered_time}}\"}}}}",
									Headers = new Dictionary<string, string>
									{
										{ "header1", "value1" }
									},
									Params = new Dictionary<string, string>
									{
										{ "lat", "52.374031" },
										{ "lon", "4.88969" },
										{ "appid", "appid" },
									},
									ConnectionTimeout = "3s",
									ReadTimeout = TimeSpan.FromMilliseconds(500)
								},
								ResponseContentType = ResponseContentType.Text
							}
						},
						{
							"search", new SearchInput
							{
								Request = new SearchInputRequest
								{
									Indices = new IndexName[] { typeof(Project) },
									Body = new SearchRequest<Project>
									{
										Size = 0,
										Aggregations = new NestedAggregation("nested_tags")
										{
											Path = Infer.Field<Project>(p => p.Tags),
											Aggregations = new TermsAggregation("top_project_tags")
											{
												Field = Infer.Field<Project>(p => p.Tags.First().Name)
											}
										}
									}
								}
							}
						}
					}
				},
				Transform = new ChainTransform
				{
					Transforms = new TransformContainer[]
					{
						new SearchTransform
						{
							Request = new SearchInputRequest
							{
								Indices = new IndexName[] { typeof(Project) },
								SearchType = SearchType.DfsQueryThenFetch,
								IndicesOptions = new IndicesOptions
								{
									ExpandWildcards = ExpandWildcards.Open,
									IgnoreUnavailable = true
								},
								Body = new SearchRequest<Project>(typeof(Project))
								{
									Query = new MatchQuery
									{
										Field = "state",
										Query = StateOfBeing.Stable.ToString().ToLowerInvariant()
									}
								}
							},
							Timeout = "10s",
						},
						new InlineScriptTransform("return [ 'time' : ctx.trigger.scheduled_time ]")
					}
				},
				Condition = new GreaterThanOrEqualArrayCondition("ctx.payload.search.aggregations.top_project_tags.buckets", "doc_count", 1),
				Trigger = new ScheduleContainer
				{
					Weekly = new WeeklySchedule
					{
						new TimeOfWeek(Day.Monday, "noon"),
						new TimeOfWeek(Day.Friday, "17:00"),
					}
				},
				Actions = new EmailAction("reminder_email")
				{
					To = new[] { "me@example.com" },
					Subject = "Something's strange in the neighbourhood",
					Body = new EmailBody
					{
						Text = "Dear {{ctx.payload.name}}, by the time you read these lines, I'll be gone"
					},
					Attachments = new EmailAttachments
					{
						{
							"http_attachment", new HttpAttachment
							{
								Inline = true,
								ContentType = RequestData.MimeType,
								Request = new HttpInputRequest
								{
									Url = "http://localhost:8080/http_attachment"
								}
							}
						},
						{
							"data_attachment", new DataAttachment
							{
								Format = DataAttachmentFormat.Json
							}
						}
					}
				} && new IndexAction("reminder_index")
				{
					Index = "put-watch-test-index",
					ExecutionTimeField = "execution_time"
				} && new PagerDutyAction("reminder_pagerduty")
				{
					Account = "my_pagerduty_account",
					Description = "pager duty description",
					AttachPayload = true,
					EventType = PagerDutyEventType.Trigger,
					IncidentKey = "incident_key",
					Context = new[]
					{
						new PagerDutyContext(PagerDutyContextType.Image) { Src = "http://example.com/image" },
						new PagerDutyContext(PagerDutyContextType.Link) { Href = "http://example.com/link" },
					}
				} && new SlackAction("reminder_slack")
				{
					Account = "monitoring",
					Message = new SlackMessage
					{
						From = "nest integration test",
						To = new[] { "#nest" },
						Text = "slack message",
						Attachments = new[]
						{
							new SlackAttachment
							{
								Title = "Attachment 1",
								AuthorName = "Russ Cam"
							}
						}
					}
				} && new WebhookAction("webhook")
				{
					Scheme = ConnectionScheme.Https,
					Host = "localhost",
					Port = 9200,
					Method = HttpInputMethod.Post,
					Path = "/_bulk",
					Authentication = new HttpInputAuthentication
					{
						Basic = new HttpInputBasicAuthentication
						{
							Username = "username",
							Password = "password"
						}
					},
					Body = "{{ctx.payload._value}}"
				}
			};

		protected override bool SupportsDeserialization => false;

		protected override string UrlPath => $"/_watcher/watch/{CallIsolatedValue}";

		protected override LazyResponses ClientUsage() => Calls(
			(client, f) => client.Watcher.Put(CallIsolatedValue, f),
			(client, f) => client.Watcher.PutAsync(CallIsolatedValue, f),
			(client, r) => client.Watcher.Put(r),
			(client, r) => client.Watcher.PutAsync(r)
		);

		protected override PutWatchDescriptor NewDescriptor() => new PutWatchDescriptor(CallIsolatedValue);

		protected override void ExpectResponse(PutWatchResponse response)
		{
			response.Created.Should().BeTrue();
			response.Version.Should().Be(1);
			response.Id.Should().Be(CallIsolatedValue);
		}
	}

	[SkipVersion("<7.4.0", "Foreach introduced in 7.3.0, max iterations in 7.4.0")]
	public class PutWatchApiWithForeachTests : ApiIntegrationTestBase<WatcherCluster, PutWatchResponse, IPutWatchRequest, PutWatchDescriptor, PutWatchRequest>
	{
		public PutWatchApiWithForeachTests(WatcherCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override bool ExpectIsValid => true;

		protected override object ExpectJson =>
			new
			{
				input = new
				{
					search = new
					{
						request = new
						{
							indices = new[] { "project" },
							body = new
							{
								query = new
								{
									range = new
									{
										numberOfCommits = new
										{
											gt = 10.0
										}
									}
								}
							}
						}
					}
				},
				trigger = new
				{
					schedule = new
					{
						interval = "5m"
					}
				},
				actions = new
				{
					log_hits = new
					{
						@foreach = "ctx.payload.hits.hits",
						max_iterations = 500,
						logging = new
						{
							text = "Found id {{ctx.payload._id}} with field {{ctx.payload._source.numberOfCommits}}"
						},
						transform = new
						{
							script = new
							{
								source = "return [ 'time' : ctx.trigger.scheduled_time ]"
							}
						},
						condition = new
						{
							always = new {}
						}
					}
				}
			};

		protected override int ExpectStatusCode => 201;

		protected override HttpMethod HttpMethod => HttpMethod.PUT;

		protected override Func<PutWatchDescriptor, IPutWatchRequest> Fluent => p => p
			.Input(i => i
				.Search(s => s
					.Request(si => si
						.Indices<Project>()
						.Body<Project>(b => b
							.Query(q => q
								.Range(r => r
									.Field(f => f.NumberOfCommits)
									.GreaterThan(10)
								)
							)
						)
					)
				)
			)
			.Trigger(t => t
				.Schedule(s => s
					.Interval(new Interval(5, IntervalUnit.Minute))
				)
			)
			.Actions(a => a
				.Logging("log_hits", i => i
					.Foreach("ctx.payload.hits.hits")
					.MaxIterations(500)
					.Text("Found id {{ctx.payload._id}} with field {{ctx.payload._source.numberOfCommits}}")
					.Transform(t => t
						.Script(st =>st
							.Source("return [ 'time' : ctx.trigger.scheduled_time ]")
						)
					)
					.Condition(c => c
						.Always()
					)
				)
			);

		protected override PutWatchRequest Initializer =>
			new PutWatchRequest(CallIsolatedValue)
			{
				Input = new SearchInput
				{
					Request = new SearchInputRequest
					{
						Indices = new IndexName[] { typeof(Project) },
						Body = new SearchRequest<Project>
						{
							Query = new NumericRangeQuery
							{
								Field = Infer.Field<Project>(f => f.NumberOfCommits),
								GreaterThan = 10
							}
						}
					}
				},
				Trigger = new ScheduleContainer
				{
					Interval = new Interval(5, IntervalUnit.Minute)
				},
				Actions = new LoggingAction("log_hits")
				{
					Foreach = "ctx.payload.hits.hits",
					MaxIterations = 500,
					Text = "Found id {{ctx.payload._id}} with field {{ctx.payload._source.numberOfCommits}}",
					Transform = new InlineScriptTransform("return [ 'time' : ctx.trigger.scheduled_time ]"),
					Condition = new AlwaysCondition()
				}
			};

		protected override bool SupportsDeserialization => false;

		protected override string UrlPath => $"/_watcher/watch/{CallIsolatedValue}";

		protected override LazyResponses ClientUsage() => Calls(
			(client, f) => client.Watcher.Put(CallIsolatedValue, f),
			(client, f) => client.Watcher.PutAsync(CallIsolatedValue, f),
			(client, r) => client.Watcher.Put(r),
			(client, r) => client.Watcher.PutAsync(r)
		);

		protected override PutWatchDescriptor NewDescriptor() => new PutWatchDescriptor(CallIsolatedValue);

		protected override void ExpectResponse(PutWatchResponse response)
		{
			response.Created.Should().BeTrue();
			response.Version.Should().Be(1);
			response.Id.Should().Be(CallIsolatedValue);
		}
	}
}
