using System.Data;

namespace OpenBots.Core.Infrastructure
{
    public interface IUIAutomationCommand
    {
        string v_WindowName { get; set; }
        string v_AutomationType { get; set; }
        DataTable v_UIASearchParameters { get; set; }
        DataTable v_UIAActionParameters { get; set; }
    }
}
