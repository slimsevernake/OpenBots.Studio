using NuGet;
using OpenBots.Core.Gallery;
using OpenBots.Core.Gallery.Models;
using OpenBots.Core.Properties;
using OpenBots.Core.UI.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace OpenBots.UI.Forms
{
    public partial class frmGalleryProjectManager : UIForm
    {
        private string _projectLocation;
        private string _projectName;
        private List<SearchResultPackage> _searchresults;
        private CatalogEntry _catalog;
        private List<RegistrationItemVersion> _projectVersions;
        private NugetPackageManger _manager;

        public frmGalleryProjectManager(string projectLocation, string projectName)
        {
            InitializeComponent();
            _projectLocation = projectLocation;
            _projectName = projectName;
            _manager = new NugetPackageManger();
        }

        private async void frmGalleryProject_LoadAsync(object sender, EventArgs e)
        {
            pnlProjectVersion.Hide();
            pnlProjectDetails.Hide();
            uiBtnOpen.Enabled = false;
            try
            {           
                _searchresults = await _manager.GetAllLatestPackagesAsync(NugetPackageManger.PackageType.Automation);
                PopulateListBox(_searchresults);
            }
            catch (Exception ex)
            {
                //not connected to internet
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void txtSampleSearch_TextChanged(object sender, EventArgs e)
        {      
            if (_searchresults != null)
            {
                lbxGalleryProjects.Clear();

                if (!string.IsNullOrEmpty(txtSampleSearch.Text))
                {
                    var filteredResult = _searchresults.Where(x => x.Title.ToLower().Contains(txtSampleSearch.Text.ToLower())).ToList();
                    PopulateListBox(filteredResult);
                }
                else
                    PopulateListBox(_searchresults);
            }                                          
        }  
        
        private void PopulateListBox(List<SearchResultPackage> searchresults)
        {
            foreach (var result in searchresults)
            {
                Image img;

                try
                {
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(result.IconUrl);
                    MemoryStream ms = new MemoryStream(bytes);
                    img = Image.FromStream(ms);
                }
                catch (Exception)
                {
                    img = Resources.OpenBots_icon;
                }

                lbxGalleryProjects.Add(result.Id, result.Title, result.Description, img, result.Version);
            }
        }

        private async void lbxGalleryProjects_ItemDoubleClick(object sender, int Index)
        {
            string projectId = lbxGalleryProjects.DoubleClickedItem.Id;
            var version = await _manager.GetLatestPackageVersionAsync(projectId);
            DownloadAndOpenProject(projectId, version);
        }

        private async void lbxGalleryProjects_ItemClick(object sender, int index)
        {
            string projectId = lbxGalleryProjects.ClickedItem.Id;
            List<RegistrationItem> registration = await _manager.GetPackageRegistrationAsync(projectId);
            string latestVersion = registration.FirstOrDefault().Upper;
            _projectVersions = registration.FirstOrDefault().Items;
            List<string> versionList = _projectVersions.Select(x => x.Catalog.Version).OrderByDescending(x => x).ToList();

            cbxVersion.Items.Clear();
            foreach (var version in versionList)
                cbxVersion.Items.Add(version);

            cbxVersion.SelectedIndex = 0;

            PopulateProjectDetails(latestVersion);

            pnlProjectVersion.Show();
            pnlProjectDetails.Show();
            uiBtnOpen.Enabled = true;
        }

        private void PopulateProjectDetails(string version)
        {
            _catalog = _projectVersions.Where(x => x.Catalog.Version == version).SingleOrDefault().Catalog;

            try
            {
                WebClient wc = new WebClient();
                byte[] bytes = wc.DownloadData(_catalog.IconUrl);
                MemoryStream ms = new MemoryStream(bytes);
                pbxOBStudio.Image = Image.FromStream(ms);
            }
            catch (Exception)
            {
                pbxOBStudio.Image = Resources.OpenBots_icon;
            }

            lblTitle.Text = _catalog.Title;
            lblAuthors.Text = _catalog.Authors;
            lblDescription.Text = _catalog.Description;
            lblDownloads.Text = _catalog.Downloads.ToString();
            lblVersion.Text = _catalog.Version;
            lblPublishDate.Text = DateTime.Parse(_catalog.Published).ToString("g");
            llblProjectURL.LinkVisited = false;
            llblLicenseURL.LinkVisited = false;

            lvDependencies.Items.Clear();
            foreach (var dependency in _catalog.DependencyGroups.FirstOrDefault().ProjectDependencies)
                lvDependencies.Items.Add(new ListViewItem(new string[] { dependency.Id, dependency.Range }));
        }

        private void llblLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_catalog.LicenseUrl))
            {
                llblLicenseURL.LinkVisited = true;
                Process.Start(_catalog.LicenseUrl);
            }
        }

        private void llblProjectURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_catalog.ProjectUrl))
            {
                llblProjectURL.LinkVisited = true;
                Process.Start(_catalog.ProjectUrl);
            }
        }

        private void cbxVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateProjectDetails(cbxVersion.SelectedItem.ToString());
        }

        private async void DownloadAndOpenProject(string projectId, SemanticVersion version)
        {
            await _manager.DownloadPackageAsync(projectId, version, _projectLocation, _projectName);
            DialogResult = DialogResult.OK;
        }

        private void uiBtnOpen_Click(object sender, EventArgs e)
        {
            DownloadAndOpenProject(_catalog.Id, SemanticVersion.Parse(_catalog.Version));
        }

        private void uiBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
