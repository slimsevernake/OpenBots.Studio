using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Word.Application;

namespace OpenBots.Commands.Word
{

    [Serializable]
    [Category("Word Commands")]
    [Description("This command creates a Word Instance.")]

    public class WordCreateApplicationCommand : ScriptCommand
    {

        [Required]
		[DisplayName("Word Instance Name")]
        [Description("Enter a unique name that will represent the application instance.")]
        [SampleUsage("MyWordInstance")]
        [Remarks("This unique name allows you to refer to the instance by name in future commands, " +
                 "ensuring that the commands you specify run against the correct application.")]
        public string v_InstanceName { get; set; }

        [Required]
		[DisplayName("New/Open Document")]
        [PropertyUISelectionOption("New Document")]
        [PropertyUISelectionOption("Open Document")]
        [Description("Indicate whether to create a new Document or to open an existing Document.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_NewOpenDocument { get; set; }

        [Required]
		[DisplayName("Document File Path")]
        [Description("Enter or Select the path to the Document file.")]
        [SampleUsage(@"C:\temp\myfile.docx || {vFilePath} || {ProjectPath}\myfile.docx")]
        [Remarks("This input should only be used for opening existing Documents.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFileSelectionHelper)]
        public string v_FilePath { get; set; }

        [Required]
		[DisplayName("Visible")]
        [PropertyUISelectionOption("Yes")]
        [PropertyUISelectionOption("No")]
        [Description("Indicate whether the Word automation should be visible or not.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_Visible { get; set; }

        [Required]
		[DisplayName("Close All Existing Word Instances")]
        [PropertyUISelectionOption("Yes")]
        [PropertyUISelectionOption("No")]
        [Description("Indicate whether to close any existing Word instances before executing Word Automation.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_CloseAllInstances { get; set; }

        [JsonIgnore]
		[Browsable(false)]
        private List<Control> _openFileControls;

        public WordCreateApplicationCommand()
        {
            CommandName = "WordCreateApplicationCommand";
            SelectionName = "Create Word Application";
            CommandEnabled = true;
            
            v_InstanceName = "DefaultWord";
            v_NewOpenDocument = "New Document";
            v_Visible = "No";
            v_CloseAllInstances = "Yes";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vFilePath = v_FilePath.ConvertUserVariableToString(engine);

            if (v_CloseAllInstances == "Yes")
            {
                var processes = Process.GetProcessesByName("winword");
                foreach (var prc in processes)
                {
                    prc.Kill();
                }
            }

            var newWordSession = new Application();

            if (v_Visible == "Yes")
                newWordSession.Visible = true;
            else
                newWordSession.Visible = false;

            newWordSession.AddAppInstance(engine, v_InstanceName);

            if (v_NewOpenDocument == "New Document")
            {
                if (!string.IsNullOrEmpty(vFilePath))
                    throw new InvalidDataException("File path should not be provided for a new Word Document");
                else
                    newWordSession.Documents.Add();
            }
            else if (v_NewOpenDocument == "Open Document")
            {
                if (string.IsNullOrEmpty(vFilePath))
                    throw new NullReferenceException("File path for Word Document not provided");
                else
                    newWordSession.Documents.Open(vFilePath);
            }
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InstanceName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_NewOpenDocument", this, editor));
            ((ComboBox)RenderedControls[3]).SelectedIndexChanged += OpenFileComboBox_SelectedValueChanged;

            _openFileControls = new List<Control>();
            _openFileControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_FilePath", this, editor));

            foreach (var ctrl in _openFileControls)
                ctrl.Visible = false;

            RenderedControls.AddRange(_openFileControls);

            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_Visible", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_CloseAllInstances", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [{v_NewOpenDocument} - Visible '{v_Visible}' - Close Instances '{v_CloseAllInstances}' - New Instance Name '{v_InstanceName}']";
        }

        private void OpenFileComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((ComboBox)RenderedControls[3]).Text == "Open Document")
            {
                foreach (var ctrl in _openFileControls)
                    ctrl.Visible = true;
            }
            else
            {
                foreach (var ctrl in _openFileControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }
            }
        }
    }
}
