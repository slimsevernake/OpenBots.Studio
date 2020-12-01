using System.Data;

namespace OpenBots.Core.Infrastructure
{
    public interface ISeleniumElementActionCommand
    {
        string v_InstanceName { get; set; }
        DataTable v_SeleniumSearchParameters { get; set; }
        string v_SeleniumSearchOption { get; set; }
        string v_SeleniumElementAction { get; set; }
        DataTable v_WebActionParameterTable { get; set; }
        string v_Timeout { get; set; }

    }
}
