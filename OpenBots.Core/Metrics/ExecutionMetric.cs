using OpenBots.Core.Model.EngineModel;
using System;
using System.Collections.Generic;

namespace OpenBots.Core.Metrics
{
    public class ExecutionMetric
    {
        public string FileName { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
        public List<ScriptFinishedEventArgs> ExecutionData { get; set; }
    }
}
