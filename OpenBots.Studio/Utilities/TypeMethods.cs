using OpenBots.Core.Enums;
using OpenBots.UI.Forms.Supplement_Forms;
using System;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.Studio.Utilities
{
    public static class TypeMethods
    {
        public static Type GetTypeByName(AppDomain appDomain, string typeName)
        {
            var commandType = appDomain.GetAssemblies()
                .Where(a => a.FullName.Contains("OpenBots.Commands"))
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name.Equals(typeName));

            if (commandType == null)
            {
                var packageName = GetPackageName(typeName);
                ShowErrorDialog($"Missing {packageName}, please download the package from Package Manager and retry.");
                Application.Restart();
                Environment.Exit(0);
                // TODO
                // Cancel Execution 
                // Show Nuget Package Manager Window
            }

            return commandType;
        }

        public static object CreateTypeInstance(AppDomain appDomain, string typeName)
        {
            var commandType = GetTypeByName(appDomain, typeName);
            return Activator.CreateInstance(commandType);
        }

        private static void ShowErrorDialog(string message)
        {
            var confirmationForm = new frmDialog(message, "MessageBox", DialogType.OkOnly, 10);
            confirmationForm.ShowDialog();
        }
        private static string GetPackageName(string typeName)
        {
            string packageName = string.Empty;
            switch (typeName)
            {
                case "ExecuteDLLCommand":
                    packageName = "API Commands Package";
                    break;
                case "PauseScriptCommand":
                    packageName = "Engine Commands Package";
                    break;
                case "SendMouseMoveCommand":
                case "SendKeystrokesCommand":
                case "SendAdvancedKeystrokesCommand":
                case "UIAutomationCommand":
                    packageName = "Input Commands Package";
                    break;
                case "BeginSwitchCommand":
                case "CaseCommand":
                case "EndSwitchCommand":
                    packageName = "Switch Commands Package";
                    break;
                case "ActivateWindowCommand":
                case "MoveWindowCommand":
                case "ResizeWindowCommand":
                    packageName = "Window Commands Package";
                    break;
                case "SequenceCommand":
                case "AddCodeCommentCommand":
                case "BrokenCodeCommentCommand":
                case "ShowMessageCommand":
                    packageName = "Misc Commands Package";
                    break;
                case "SeleniumCreateBrowserCommand":
                case "SeleniumElementActionCommand":
                case "SeleniumRefreshCommand":
                case "SeleniumNavigateBackCommand":
                case "SeleniumNavigateForwardCommand":
                case "SeleniumNavigateToURLCommand":
                case "SeleniumCloseBrowserCommand":
                    packageName = "Web Browser Commands Package";
                    break;
                case "BeginIfCommand":
                case "EndIfCommand":
                    packageName = "If Commands Package";
                    break;
                case "EndLoopCommand":
                    packageName = "Loop Commands Package";
                    break;
                case "CatchCommand":
                case "EndTryCommand":
                case "EndRetryCommand":
                    packageName = "Error Handling Commands Package";
                    break;
            }
            return packageName;
        }
    }
}

