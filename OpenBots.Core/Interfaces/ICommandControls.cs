using OpenBots.Core.Command;
using OpenBots.Core.Script;
using OpenBots.Core.UI.Controls.CustomControls;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenBots.Core.Infrastructure
{
    public interface ICommandControls
    {
        List<Control> CreateDefaultInputGroupFor(string parameterName, ScriptCommand parent, IfrmCommandEditor editor, int height = 30, int width = 300);
        List<Control> CreateDefaultPasswordInputGroupFor(string parameterName, ScriptCommand parent, IfrmCommandEditor editor);
        List<Control> CreateDefaultOutputGroupFor(string parameterName, ScriptCommand parent, IfrmCommandEditor editor);
        List<Control> CreateDefaultDropdownGroupFor(string parameterName, ScriptCommand parent, IfrmCommandEditor editor);
        List<Control> CreateDataGridViewGroupFor(string parameterName, ScriptCommand parent, IfrmCommandEditor editor);
        List<Control> CreateDefaultWindowControlGroupFor(string parameterName, ScriptCommand parent, IfrmCommandEditor editor);
        Control CreateDefaultLabelFor(string parameterName, ScriptCommand parent);
        void CreateDefaultToolTipFor(string parameterName, ScriptCommand parent, Control label);
        Control CreateDefaultInputFor(string parameterName, ScriptCommand parent, int height = 30, int width = 300);

        CheckBox CreateCheckBoxFor(string parameterName, ScriptCommand parent);
        Control CreateDropdownFor(string parameterName, ScriptCommand parent);

        ComboBox CreateStandardComboboxFor(string parameterName, ScriptCommand parent);
        List<Control> CreateUIHelpersFor(string parameterName, ScriptCommand parent, Control[] targetControls, IfrmCommandEditor editor);

        string ShowCommandsElementRecorder(object sender, EventArgs e, IfrmCommandEditor editor);

        IfrmScriptEngine CreateScriptEngineForm(string pathToFile, string projectPath, IfrmScriptBuilder builderForm, List<ScriptVariable> variables, List<ScriptElement> elements, 
            Dictionary<string, object> appInstances, bool blnCloseWhenDone, bool isDebugMode);
        IfrmWebElementRecorder CreateWebElementRecorderForm(string startURL);
        IfrmAdvancedUIElementRecorder CreateAdvancedUIElementRecorderForm();
        IfrmCommandEditor CreateCommandEditorForm(List<AutomationCommand> commands, List<ScriptCommand> existingCommands);
        ScriptCommand CreateBeginIfCommand(string commandData = null);
        Type GetCommandType(string commandName);
    }
}
