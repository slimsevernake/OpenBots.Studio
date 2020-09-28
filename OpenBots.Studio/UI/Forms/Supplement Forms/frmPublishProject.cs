using NuGet;
using OpenBots.Core.Enums;
using OpenBots.Core.IO;
using OpenBots.Core.UI.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.UI.Supplement_Forms
{
    public partial class frmPublishProject : UIForm
    {
        public string Version { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }

        private string _projectPath;
        private string _projectName;
        public frmPublishProject(string projectPath)
        {
            _projectPath = projectPath;
            _projectName = _projectPath.Split('\\').LastOrDefault();
            InitializeComponent();
            
        }

        private void frmPublishProject_Load(object sender, EventArgs e)
        {
            txtLocation.Text = Folders.GetFolder(FolderType.RootFolder);
            lblPublish.Text = $"publish {_projectName}";
            Text = $"publish {_projectName}";
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            PublishProject();
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void PublishProject()
        {
            
            string[] scriptFiles = Directory.GetFiles(_projectPath, "*.json", SearchOption.AllDirectories);
            List<ManifestContentFiles> manifestFiles = new List<ManifestContentFiles>();
            foreach (string file in scriptFiles)
            {
                ManifestContentFiles manifestFile = new ManifestContentFiles
                {

                };
            }

            ManifestMetadata metadata = new ManifestMetadata()
            {
                
                Id = Guid.NewGuid().ToString(),
                Title = _projectName,
                Authors = $"{txtFirstName.Text} {txtLastName.Text} <{txtEmail.Text}>",
                Version = txtVersion.Text,
                Description = txtDescription.Text,
                RequireLicenseAcceptance = false,

            };

            PackageBuilder builder = new PackageBuilder();
            builder.PopulateFiles(_projectPath, new[] { new ManifestFile() { Source = "**" } });
            builder.Populate(metadata);
            using (FileStream stream = File.Open(Path.Combine(txtLocation.Text, _projectName + ".nupkg"), FileMode.OpenOrCreate))
            {
                builder.Save(stream);
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

        
    }
}