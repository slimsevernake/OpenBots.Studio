using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using OpenBots.Core.Gallery;
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

        private List<IPackageSearchMetadata> _searchresults;
        private IPackageSearchMetadata _catalog;
        private List<NuGetVersion> _projectVersions;
        private List<IPackageSearchMetadata> _selectedPackageMetaData;
        private string _gallerySourceUrl = "https://dev.gallery.openbots.io/v3/index.json";

        public frmGalleryProjectManager(string projectLocation, string projectName)
        {
            InitializeComponent();
            _projectLocation = projectLocation;
            _projectName = projectName;
            _searchresults = new List<IPackageSearchMetadata>();
            _projectVersions = new List<NuGetVersion>();
        }

        private async void frmGalleryProject_LoadAsync(object sender, EventArgs e)
        {
            pnlProjectVersion.Hide();
            pnlProjectDetails.Hide();
            uiBtnOpen.Enabled = false;
            try
            {           
                _searchresults = await NugetPackageManager.SearchPackages("", _gallerySourceUrl, true);
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
        
        private void PopulateListBox(List<IPackageSearchMetadata> searchresults)
        {
            lbxGalleryProjects.Visible = false;
            //tpbLoadingSpinner.Visible = true;

            lbxGalleryProjects.Clear();
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
                    img = Resources.nuget_icon;
                }

                lbxGalleryProjects.Add(result.Identity.Id, result.Identity.Id, result.Description, img, result.Identity.Version.ToString());
            }

            //tpbLoadingSpinner.Visible = false;
            lbxGalleryProjects.Visible = true;
        }

        private async void lbxGalleryProjects_ItemClick(object sender, int index)
        {
            try
            {
                string projectId = lbxGalleryProjects.ClickedItem.Id;
                List<IPackageSearchMetadata> metadata = new List<IPackageSearchMetadata>();

                metadata.AddRange(await NugetPackageManager.GetPackageMetadata(projectId, _gallerySourceUrl, true));

                string latestVersion = metadata.LastOrDefault().Identity.Version.ToString();

                _projectVersions.Clear();
                _projectVersions.AddRange(await NugetPackageManager.GetPackageVersions(projectId, _gallerySourceUrl, true));


                List<string> versionList = _projectVersions.Select(x => x.ToString()).ToList();
                versionList.Reverse();

                cbxVersion.Items.Clear();
                foreach (var version in versionList)
                    cbxVersion.Items.Add(version);
              
                _selectedPackageMetaData = metadata;

                pnlProjectVersion.Show();
                pnlProjectDetails.Show();
                uiBtnOpen.Enabled = true;
                cbxVersion.SelectedItem = latestVersion;
            }
            catch (Exception)
            {
                pnlProjectVersion.Hide();
                pnlProjectDetails.Hide();
                uiBtnOpen.Enabled = false;
            }
        }

        private void PopulateProjectDetails(string version)
        {
            _catalog = _selectedPackageMetaData.Where(x => x.Identity.Version.ToString() == version).SingleOrDefault();

            if (_catalog != null)
            {
                try
                {
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(_catalog.IconUrl);
                    MemoryStream ms = new MemoryStream(bytes);
                    pbxOBStudio.Image = Image.FromStream(ms);
                }
                catch (Exception)
                {
                    pbxOBStudio.Image = Resources.nuget_icon;
                }

                lblTitle.Text = _catalog.Title;
                lblAuthors.Text = _catalog.Authors;
                lblDescription.Text = _catalog.Description;
                lblDownloads.Text = _catalog.DownloadCount.ToString();
                lblVersion.Text = _catalog.Identity.Version.ToString();
                lblPublishDate.Text = DateTime.Parse(_catalog.Published.ToString()).ToString("g");
                llblProjectURL.LinkVisited = false;
                llblLicenseURL.LinkVisited = false;

                lvDependencies.Items.Clear();
                if (_catalog.DependencySets.ToList().Count > 0)
                {
                    foreach (var dependency in _catalog.DependencySets.FirstOrDefault().Packages)
                        lvDependencies.Items.Add(new ListViewItem(new string[] { dependency.Id, dependency.VersionRange.ToString() }));
                }
            }
            else
            {
                _catalog = _selectedPackageMetaData.Last();
                try
                {
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(_catalog.IconUrl);
                    MemoryStream ms = new MemoryStream(bytes);
                    pbxOBStudio.Image = Image.FromStream(ms);
                }
                catch (Exception)
                {
                    pbxOBStudio.Image = Resources.nuget_icon;
                }

                lblTitle.Text = _catalog.Title;
                lblAuthors.Text = "Unknown";
                lblDescription.Text = "Unknown";
                lblDownloads.Text = "Unknown";
                lblVersion.Text = version;
                lblPublishDate.Text = "Unknown";
                llblProjectURL.LinkVisited = false;
                llblLicenseURL.LinkVisited = false;

                lvDependencies.Items.Clear();
            }
        }

        private void llblLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_catalog.LicenseUrl != null)
            {
                llblLicenseURL.LinkVisited = true;
                Process.Start(_catalog.LicenseUrl.ToString());
            }
        }

        private void llblProjectURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_catalog.ProjectUrl != null)
            {
                llblProjectURL.LinkVisited = true;
                Process.Start(_catalog.ProjectUrl.ToString());
            }
        }

        private void cbxVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateProjectDetails(cbxVersion.SelectedItem.ToString());
        }

        private async void DownloadAndOpenProject(string projectId, string version)
        {
            try
            {
                string packageName = $"{projectId}.{version}";
                Cursor.Current = Cursors.WaitCursor;
                lblError.Text = $"Downloading {packageName}";

                await NugetPackageManager.DownloadPackage(projectId, version, _projectLocation, _projectName, _gallerySourceUrl);

                lblError.Text = string.Empty;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void uiBtnOpen_Click(object sender, EventArgs e)
        {
            DownloadAndOpenProject(_catalog.Identity.Id, cbxVersion.SelectedItem.ToString());
        }

        private void uiBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
