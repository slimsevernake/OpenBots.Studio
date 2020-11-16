using OpenBots.Core.Enums;
using OpenBots.Core.IO;
using OpenBots.Core.Project;
using OpenBots.Core.UI.Forms;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.UI.Forms
{
    public partial class frmProjectBuilder : UIForm
    {
        public string NewProjectName { get; private set; }        
        public string NewProjectPath { get; private set; }
        public string ExistingProjectPath { get; private set; }
        public string ExistingConfigPath { get; private set; }
        public ProjectAction Action { get; private set; }
        private string _newProjectLocation;

        public enum ProjectAction
        {
            CreateProject,
            OpenProject
        }

        public frmProjectBuilder()
        {
            InitializeComponent();
            txtNewProjectLocation.Text = Folders.GetFolder(FolderType.ScriptsFolder);
        }

        private void btnCreateProject_Click(object sender, EventArgs e)
        {
            CreateProject(DialogResult.OK);            
        }

        private void btnCreateGalleryProject_Click(object sender, EventArgs e)
        {
            CreateProject(DialogResult.None);

            if (string.IsNullOrEmpty(lblError.Text))
            {
                frmGalleryProjectManager gallery = new frmGalleryProjectManager(_newProjectLocation, NewProjectName);
                gallery.ShowDialog();

                if (gallery.DialogResult == DialogResult.OK)
                {
                    try
                    {
                        ExistingConfigPath = Project.ExtractGalleryProject(NewProjectPath);
                        ExistingProjectPath = Directory.GetParent(ExistingConfigPath).ToString();
                        Action = ProjectAction.OpenProject;
                        DialogResult = DialogResult.OK;
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = "Error: " + ex.Message;
                        Directory.Delete(NewProjectPath, true);
                    }
                }
                else
                {
                    Directory.Delete(NewProjectPath, true);
                }
            }           
        }

        private void CreateProject(DialogResult result)
        {
            lblError.Text = "";
            _newProjectLocation = txtNewProjectLocation.Text.Trim();
            NewProjectName = txtNewProjectName.Text.Trim();

            if (string.IsNullOrEmpty(NewProjectName) || string.IsNullOrEmpty(_newProjectLocation) || !Directory.Exists(_newProjectLocation))
            {
                lblError.Text = "Error: Please enter a valid project name and location";
            }
            else
            {
                try
                {
                    NewProjectPath = Path.Combine(_newProjectLocation, NewProjectName);
                    bool isInvalidProjectName = new[] { @"/", @"\" }.Any(c => NewProjectName.Contains(c));
                    if (isInvalidProjectName)
                        throw new Exception("Illegal characters in path");

                    if (!Directory.Exists(NewProjectPath))
                    {
                        Directory.CreateDirectory(NewProjectPath);
                        Action = ProjectAction.CreateProject;
                        DialogResult = result;
                    }
                    else
                        lblError.Text = "Error: Project already exists";
                }
                catch(Exception ex)
                {
                    lblError.Text = "Error: " + ex.Message;
                }
            }
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            ExistingConfigPath = txtExistingProjectLocation.Text.Trim();
            if (ExistingConfigPath == string.Empty || !File.Exists(ExistingConfigPath) ||
                Path.GetFileName(ExistingConfigPath) != "project.config")
            {
                lblError.Text = "Error: Please enter a valid project.config path";
            }
            else
            {
                ExistingProjectPath = Directory.GetParent(ExistingConfigPath).ToString();
                Action = ProjectAction.OpenProject;
                DialogResult = DialogResult.OK;
            }
        }

        private void btnFolderManager_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtNewProjectLocation.Text = fbd.SelectedPath;
                txtNewProjectLocation.Focus();
            }
        }

        private void btnFileManager_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtExistingProjectLocation.Text = ofd.FileName;
                txtExistingProjectLocation.Focus();
            }
        }

        private void txtNewProjectName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnCreateProject_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }           
        }

        private void txtNewProjectLocation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnCreateProject_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtExistingProjectLocation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOpenProject_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }      
    }
}
