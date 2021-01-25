
// Copyright 2013-2016 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using BenchmarkDotNet.Attributes;
using System;
using Serilog.PerformanceTests.Support;

namespace Serilog.PerformanceTests
{
    /// <summary>
    /// Tests the cost of writing through the logging pipeline.
    /// </summary>
    [MemoryDiagnoser]
    public class PipelineBenchmark
    {
        ILogger _log;
        Exception _exception;
        
        [Setup]
        public void Setup()
        { 
            _exception = new Exception("An Error");
            _log = new LoggerConfiguration()
                .WriteTo.Sink(new NullSink())
                .CreateLogger();

            // Ensure template is cached
            _log.Information(_exception, "Hello, {Name}!", "World");
        }

        [Benchmark]
        public void EmitLogEvent()
        {
            _log.Information(_exception, "Hello, {Name}!", "World");
        }
    }
}
  