using Newtonsoft.Json;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Script;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenBots.Core.Command
{
    public abstract class ScriptCommand
    {
        public string CommandID { get; set; }
        public string CommandName { get; set; }
        public string SelectionName { get; set; }
        public int LineNumber { get; set; }
        public bool IsCommented { get; set; }
        public bool PauseBeforeExecution { get; set; }
        public bool CommandEnabled { get; set; }

		[DisplayName("Private (Optional)")]
        [Description("Optional field to mark the command as private (data sensitive) in order to avoid its logging.")]
        [SampleUsage("")]
        [Remarks("")]
        public bool v_IsPrivate { get; set; }

		[DisplayName("Comment Field (Optional)")]
        [Description("Optional field to enter a custom comment which could potentially describe this command or the need for this command, if required.")]
        [SampleUsage("I am using this command to ...")]
        [Remarks("Optional")]
        public string v_Comment { get; set; }

        [JsonIgnore]
        public List<Control> RenderedControls;      

        [JsonIgnore]
        public bool IsSteppedInto { get; set; }

        [JsonIgnore]
        public IfrmScriptBuilder CurrentScriptBuilder { get; set; }

        [JsonIgnore]
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

        public ScriptCommand()
        {
            CommandEnabled = false;
            IsCommented = false;
            GenerateID();
        }

        public void GenerateID()
        {
            var id = Guid.NewGuid();
            CommandID = id.ToString();
        }

        public virtual void RunCommand(object sender)
        {
        }

        public virtual void RunCommand(object sender, ScriptAction command)
        {
        }

        public virtual string GetDisplayValue()
        {
            if (string.IsNullOrEmpty(v_Comment))
                return SelectionName;
            else
                return $"{v_Comment} - " + SelectionName;
        }

        public virtual List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            RenderedControls = new List<Control>();
            return RenderedControls;
        }
    }
}