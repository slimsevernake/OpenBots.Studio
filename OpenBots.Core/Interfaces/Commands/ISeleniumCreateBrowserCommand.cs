using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBots.Core.Infrastructure
{
    public interface ISeleniumCreateBrowserCommand
    {
        string v_InstanceName { get; set; }
        string v_EngineType { get; set; }
        string v_URL { get; set; }
        string v_InstanceTracking { get; set; }
        string v_BrowserWindowOption { get; set; }
        string v_SeleniumOptions { get; set; }
    }
}
