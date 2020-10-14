using NuGet;
using OpenBots.Core.Enums;
using OpenBots.Core.IO;
using OpenBots.Core.Server.API_Methods;
using OpenBots.Core.UI.Forms;
using OpenBots.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace OpenBots.UI.Supplement_Forms
{
    public partial class frmPublishProject : UIForm
    {
        public string NotificationMessage { get; set; }
        private string _projectPath;
        private string _projectName;
        private Guid _projectId;
        
        public frmPublishProject(string projectPath, Project project)
        {
            _projectPath = projectPath;
            _projectName = project.ProjectName;
            _projectId = project.ProjectID;
            InitializeComponent();           
        }

        private void frmPublishProject_Load(object sender, EventArgs e)
        {
            txtLocation.Text = Folders.GetFolder(FolderType.PublishedFolder);
            lblPublish.Text = $"publish {_projectName}";
            Text = $"publish {_projectName}";
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;         

            if (!PublishProject())
                return;

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private bool PublishProject()
        {
            try
            {
                string[] scriptFiles = Directory.GetFiles(_projectPath, "*.json", SearchOption.AllDirectories);
                List<ManifestContentFiles> manifestFiles = new List<ManifestContentFiles>();
                foreach (string file in scriptFiles)
                {
                    ManifestContentFiles manifestFile = new ManifestContentFiles
                    {
                        Include = file.Replace(_projectPath, "")
                    };
                    manifestFiles.Add(manifestFile);
                }

                ManifestMetadata metadata = new ManifestMetadata()
                {
                    Id = _projectId.ToString(),
                    Title = _projectName,
                    Authors = txtAuthorName.Text.Trim(),
                    Version = txtVersion.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    RequireLicenseAcceptance = false,
                    DependencySets = new List<ManifestDependencySet>()
                {
                    new ManifestDependencySet()
                    {
                        Dependencies = new List<ManifestDependency>()
                        {
                            new ManifestDependency()
                            {
                                Id = "OpenBots.Studio",
                                Version = new Version(Application.ProductVersion).ToString()
                            }
                        }
                    }
                },
                    ContentFiles = manifestFiles,
                };

                PackageBuilder builder = new PackageBuilder();
                builder.PopulateFiles(_projectPath, new[] { new ManifestFile() { Source = "**" } });
                builder.Populate(metadata);

                if (!Directory.Exists(txtLocation.Text))
                    Directory.CreateDirectory(txtLocation.Text);

                string nugetFilePath = Path.Combine(txtLocation.Text.Trim(), $"{_projectName}_{txtVersion.Text.Trim()}.nupkg");
                using (FileStream stream = File.Open(nugetFilePath, FileMode.OpenOrCreate))
                    builder.Save(stream);

                NotificationMessage = $"'{_projectName}' published successfully";

                try {
                    var client = AuthMethods.GetAuthToken();
                    ProcessMethods.UploadProcess(client, _projectName, nugetFilePath);
                }
                catch (Exception)
                {
                    NotificationMessage = $"'{_projectName}' was published locally. To publish to an OpenBots Server please install and connect the OpenBots Agent.";
                }

                return true;             
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                return false;
            }           
        }

        private void btnFolderManager_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtLocation.Text = fbd.SelectedPath;
                txtLocation.Focus();
            }
        }  
        
        private bool ValidateForm()
        {           
            if (string.IsNullOrEmpty(txtVersion.Text.Trim()))
            {
                lblError.Text = "Please provide a valid version";
                return false;
            }
            else if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
            {
                lblError.Text = "Please provide a valid description";
                return false;
            }
            else if (string.IsNullOrEmpty(txtLocation.Text.Trim()))
            {
                lblError.Text = "Please provide a valid output path";
                return false;
            }
            else return true;
        }
    }
}