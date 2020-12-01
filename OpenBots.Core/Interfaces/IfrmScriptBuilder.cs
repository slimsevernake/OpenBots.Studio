using Serilog.Core;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBots.Core.Infrastructure
{
    public interface IfrmScriptBuilder
    {
        string ScriptFilePath { get; set; }
        string ScriptProjectPath { get; }
        int DebugLine { get; set; }
        IfrmScriptEngine CurrentEngine { get; set; }
        bool IsScriptRunning { get; set; }
        bool IsScriptPaused { get; set; }
        bool IsScriptSteppedOver { get; set; }
        bool IsScriptSteppedInto { get; set; }
        bool IsUnhandledException { get; set; }
        Logger EngineLogger { get; set; }
        string HTMLElementRecorderURL { get; set; }
        void OpenScriptFile(string scriptFilePath);
        void Notify(string notificationText, Color notificationColor);
        void RemoveDebugTab();
        DialogResult LoadErrorForm(string errorMessage);
    }
}
