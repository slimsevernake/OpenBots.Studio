using Microsoft.Office.Interop.Outlook;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace OpenBots.Commands.Outlook
{
    [Serializable]
    [Category("Outlook Commands")]
    [Description("This command replies to a selected email in Outlook.")]

    public class ReplyToOutlookEmailCommand : ScriptCommand
    {

        [Required]
		[DisplayName("MailItem")]
        [Description("Enter the MailItem to reply to.")]
        [SampleUsage("{vMailItem}")]
        [Remarks("")]
        [Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
        public string v_MailItem { get; set; }

        [Required]
		[DisplayName("Mail Operation")]
        [PropertyUISelectionOption("Reply")]
        [PropertyUISelectionOption("Reply All")]
        [Description("Specify whether you intend to reply or reply all.")]
        [SampleUsage("")]
        [Remarks("Replying will reply to only the original sender. Reply all will reply to everyone in the recipient list.")]
        public string v_OperationType { get; set; }

        [Required]
		[DisplayName("Email Body")]
        [Description("Enter text to be used as the email body.")]
        [SampleUsage("Dear John, ... || {vBody}")]
        [Remarks("")]
        [Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
        public string v_Body { get; set; }

        [Required]
		[DisplayName("Email Body Type")]
        [PropertyUISelectionOption("Plain")]
        [PropertyUISelectionOption("HTML")]
        [Description("Select the email body format.")]
        [Remarks("")]
        public string v_BodyType { get; set; }

		[DisplayName("Attachment File Path(s) (Optional)")]
        [Description("Enter the file path(s) of the file(s) to attach.")]
        [SampleUsage(@"C:\temp\myFile.xlsx || {vFile} || C:\temp\myFile1.xlsx;C:\temp\myFile2.xlsx || {vFile1};{vFile2} || {vFiles}")]
        [Remarks("This input is optional. Multiple attachments should be delimited by a semicolon (;).")]
        [Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
        [Editor("ShowFileSelectionHelper", typeof(UIAdditionalHelperType))]
        public string v_Attachments { get; set; }

        public ReplyToOutlookEmailCommand()
        {
            CommandName = "ReplyToOutlookEmailCommand";
            SelectionName = "Reply To Outlook Email";
            CommandEnabled = true;
            
            v_OperationType = "Reply";
            v_BodyType = "Plain";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            MailItem vMailItem = (MailItem)v_MailItem.ConvertUserVariableToObject(engine);
            var vBody = v_Body.ConvertUserVariableToString(engine);
            var vAttachment = v_Attachments.ConvertUserVariableToString(engine);
           
            if (v_OperationType == "Reply")
            {
                MailItem newMail = vMailItem.Reply();
                Reply(newMail, vBody, vAttachment);
            }
            else if(v_OperationType == "Reply All")
            {
                MailItem newMail = vMailItem.ReplyAll();
                Reply(newMail, vBody, vAttachment);
            }                           
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_MailItem", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_OperationType", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Body", this, editor, 100, 300));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_BodyType", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Attachments", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [MailItem '{v_MailItem}']";
        }

        private void Reply(MailItem mail, string body, string attPath)
        {
            if (v_BodyType == "HTML")
                mail.HTMLBody = body;
            else 
                mail.Body = body;

            if (!string.IsNullOrEmpty(attPath))
            {
                var splitAttachments = attPath.Split(';');

                foreach (var attachment in splitAttachments)
                    mail.Attachments.Add(attachment);
            }
            mail.Send();
        }
    }
}
