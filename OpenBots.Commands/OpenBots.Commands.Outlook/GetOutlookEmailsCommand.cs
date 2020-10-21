using Microsoft.Office.Interop.Outlook;
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
using System.IO;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Outlook.Application;

namespace OpenBots.Commands.Outlook
{
    [Serializable]
    [Category("Outlook Commands")]
    [Description("This command gets selected emails and their attachments from Outlook.")]

    public class GetOutlookEmailsCommand : ScriptCommand
    {

        [Required]
		[DisplayName("Source Mail Folder Name")]
        [Description("Enter the name of the Outlook mail folder the emails are located in.")]
        [SampleUsage("Inbox || {vFolderName}")]
        [Remarks("Source folder cannot be a subfolder.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_SourceFolder { get; set; }

        [Required]
		[DisplayName("Filter")]
        [Description("Enter a valid Outlook filter string.")]
        [SampleUsage("[Subject] = 'Hello' || [Subject] = 'Hello' and [SenderName] = 'Jane Doe' || {vFilter} || None")]
        [Remarks("*Warning* Using 'None' as the Filter will return every email in the selected Mail Folder.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        public string v_Filter { get; set; }

        [Required]
		[DisplayName("Unread Only")]
        [PropertyUISelectionOption("Yes")]
        [PropertyUISelectionOption("No")]
        [Description("Specify whether to retrieve unread email messages only.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_GetUnreadOnly { get; set; }

        [Required]
		[DisplayName("Mark As Read")]
        [PropertyUISelectionOption("Yes")]
        [PropertyUISelectionOption("No")]
        [Description("Specify whether to mark retrieved emails as read.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_MarkAsRead { get; set; }

        [Required]
		[DisplayName("Save MailItems and Attachments")]
        [PropertyUISelectionOption("Yes")]
        [PropertyUISelectionOption("No")]
        [Description("Specify whether to save the email attachments to a local directory.")]
        [SampleUsage("")]
        [Remarks("")]
        public string v_SaveMessagesAndAttachments { get; set; }

        [Required]
		[DisplayName("Output MailItem Directory")]   
        [Description("Enter or Select the path of the directory to store the messages in.")]
        [SampleUsage(@"C:\temp\myfolder || {vFolderPath} || {ProjectPath}\myFolder")]
        [Remarks("This input is optional and will only be used if *Save MailItems and Attachments* is set to **Yes**.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFolderSelectionHelper)]
        public string v_MessageDirectory { get; set; }

        [Required]
		[DisplayName("Output Attachment Directory")]      
        [Description("Enter or Select the path to the directory to store the attachments in.")]
        [SampleUsage(@"C:\temp\myfolder\attachments || {vFolderPath} || {ProjectPath}\myFolder\attachments")]
        [Remarks("This input is optional and will only be used if *Save MailItems and Attachments* is set to **Yes**.")]
        [PropertyUIHelper(UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(UIAdditionalHelperType.ShowFolderSelectionHelper)]
        public string v_AttachmentDirectory { get; set; }

        [Required]
        [Editable(false)]
        [DisplayName("Output MailItem List Variable")]
        [Description("Create a new variable or select a variable from the list.")]
        [SampleUsage("{vUserVariable}")]
        [Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
        public string v_OutputUserVariableName { get; set; }

        [JsonIgnore]
		[Browsable(false)]
        private List<Control> _savingControls;

        public GetOutlookEmailsCommand()
        {
            CommandName = "GetOutlookEmailsCommand";
            SelectionName = "Get Outlook Emails";
            CommandEnabled = true;
            
            v_SourceFolder = "Inbox";
            v_GetUnreadOnly = "No";
            v_MarkAsRead = "Yes";
            v_SaveMessagesAndAttachments = "No";
        }

        public override void RunCommand(object sender)
        {
            var engine = (AutomationEngineInstance)sender;
            var vFolder = v_SourceFolder.ConvertUserVariableToString(engine);
            var vFilter = v_Filter.ConvertUserVariableToString(engine);
            var vAttachmentDirectory = v_AttachmentDirectory.ConvertUserVariableToString(engine);
            var vMessageDirectory = v_MessageDirectory.ConvertUserVariableToString(engine);

            if (vFolder == "") 
                vFolder = "Inbox";

            Application outlookApp = new Application();
            AddressEntry currentUser = outlookApp.Session.CurrentUser.AddressEntry;
            NameSpace test = outlookApp.GetNamespace("MAPI");

            if (currentUser.Type == "EX")
            {
                MAPIFolder inboxFolder = (MAPIFolder)test.GetDefaultFolder(OlDefaultFolders.olFolderInbox).Parent;
                MAPIFolder userFolder = inboxFolder.Folders[vFolder];
                Items filteredItems = null;

                if (string.IsNullOrEmpty(vFilter.Trim()))
                    throw new NullReferenceException("Outlook Filter not specified");
                else if (vFilter != "None")
                {
                    try
                    {
                        filteredItems = userFolder.Items.Restrict(vFilter);
                    }
                    catch(System.Exception)
                    {
                        throw new InvalidDataException("Outlook Filter is not valid");
                    }
                }                   
                else
                    filteredItems = userFolder.Items;

                List<MailItem> outMail = new List<MailItem>();

                foreach (object _obj in filteredItems)
                {
                    if (_obj is MailItem)
                    { 
                        MailItem tempMail = (MailItem)_obj;
                        if (v_GetUnreadOnly == "Yes")
                        {
                            if (tempMail.UnRead == true)
                            {
                                ProcessEmail(tempMail, vMessageDirectory, vAttachmentDirectory);
                                outMail.Add(tempMail);
                            }
                        }
                        else {
                            ProcessEmail(tempMail, vMessageDirectory, vAttachmentDirectory);
                            outMail.Add(tempMail);
                        }   
                    }
                }

                outMail.StoreInUserVariable(engine, v_OutputUserVariableName);
            }
        }

        public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
        {
            base.Render(editor, commandControls);

            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_SourceFolder", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Filter", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_GetUnreadOnly", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_MarkAsRead", this, editor));
            RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_SaveMessagesAndAttachments", this, editor));
            ((ComboBox)RenderedControls[11]).SelectedIndexChanged += SaveMailItemsComboBox_SelectedValueChanged;

            _savingControls = new List<Control>();
            _savingControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_MessageDirectory", this, editor));
            _savingControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_AttachmentDirectory", this, editor));

            foreach (var ctrl in _savingControls)
                ctrl.Visible = false;

            RenderedControls.AddRange(_savingControls);

            RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + $" [From '{v_SourceFolder}' - Filter by '{v_Filter}' - Store MailItem List in '{v_OutputUserVariableName}']";
        }

        private void ProcessEmail(MailItem mail, string msgDirectory, string attDirectory)
        {
            if (v_MarkAsRead == "Yes")
            {
                mail.UnRead = false;
            }
            if (v_SaveMessagesAndAttachments == "Yes")
            {
                if (Directory.Exists(msgDirectory))
                {
                    string mailFileName = string.Join("_", mail.Subject.Split(Path.GetInvalidFileNameChars()));
                    mail.SaveAs(Path.Combine(msgDirectory, mailFileName + ".msg"));
                }
                    
                if (Directory.Exists(attDirectory))
                {
                    foreach (Attachment attachment in mail.Attachments)
                    {
                        attachment.SaveAsFile(Path.Combine(attDirectory, attachment.FileName));
                    }
                }
            }
        }

        private void SaveMailItemsComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((ComboBox)RenderedControls[11]).Text == "Yes")
            {
                foreach (var ctrl in _savingControls)
                    ctrl.Visible = true;
            }
            else
            {
                foreach (var ctrl in _savingControls)
                {
                    ctrl.Visible = false;
                    if (ctrl is TextBox)
                        ((TextBox)ctrl).Clear();
                }
            }
        }
    }
}