using OpenBots.Core.App;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Windows.Forms;

namespace OpenBots.Commands.IEBrowser
{
    [Serializable]
    [Category("IE Browser Commands")]
    [Description("This command creates a new IE Web Browser Session.")]
    public class IECreateBrowserCommand : ScriptCommand
    {
        [Required]
        [DisplayName("IE Browser Instance Name")]
        [Description("Enter a unique name that will represent the application instance.")]
        [SampleUsage("MyIEBrowserInstance")]
        [Remarks("This unique name allows you to refer to the instance by name in future commands, " +
                 "ensuring that the commands you specify run against the correct application.")]
        public string v_InstanceName { get; set; }

        [Required]
        [DisplayName("URL")]
        [Description("Enter a Web URL to navigate to.")]
        [SampleUsage("https://example.com/ || {vURL}")]
        [Remarks("This input is optional.")]
        [Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
        public string v_URL { get; set; }

        [Required]
        [DisplayName("Instance Tracking (after task ends)")]
        [PropertyUISelectionOption("Forget Instance")]
        [PropertyUISelectionOption("Keep Instance Alive")]
        [Description("Specify if OpenBots should remember this instance name after the script has finished executing.")]
        [SampleUsage("")]
        [Remarks("Calling the **Close Browser** command or closing the application will end the instance.")]
        public string v_InstanceTracking { get; set; }

        public IECreateBrowserCommand()
        {
            CommandName = "IECreateBrowserCommand";
            SelectionName = "Create IE Browser";           
            CommandEnabled = false;

            v_InstanceName = "DefaultIEBrowser";
            v_InstanceTracking = "Forget Instance";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var webURL = v_URL.ConvertUserVariableToString(engine);

            InternetExplorer newBrowserSession = new InternetExplorer();

            if (!string.IsNullOrEmpty(webURL.Trim()))
            {
                try
                {
                    newBrowserSession.Navigate(webURL);
                    WaitForReadyState(newBrowserSession);
                    newBrowserSession.Visible = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
                
            //add app instance
            newBrowserSession.AddAppInstance(engine, v_InstanceName);

            //handle app instance tracking
            if (v_InstanceTracking == "Keep Instance Alive")
                GlobalAppInstances.AddInstance(v_InstanceName, newBrowserSession);
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InstanceName", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_URL", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_InstanceTracking", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [New Instance Name '{v_InstanceName}']";
        }

        public static void WaitForReadyState(InternetExplorer ieInstance)
        {
            try
            {
                DateTime waitExpires = DateTime.Now.AddSeconds(15);

                do
                {
                    Thread.Sleep(500);
                }

                while ((ieInstance.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE) && (waitExpires > DateTime.Now));
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
    }
}