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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenBots.UI.Forms
{
    public partial class frmGalleryPackageManagerV2 : UIForm
    {
        private string _packageLocation;
        private List<IPackageSearchMetadata> _allResults;
        private List<IPackageSearchMetadata> _allGalleryResults;
        private List<IPackageSearchMetadata> _allNugetResults;
        private List<IPackageSearchMetadata> _projectDependencies;
        private Dictionary<string, string> _projectDependenciesDict;
        private IPackageSearchMetadata _catalog;
        private List<NuGetVersion> _projectVersions;
        private List<IPackageSearchMetadata> _selectedPackageMetaData;
        private bool _isInstalled;
        private string _installedVersion;
        private bool _includePrerelease;

        private string _gallerySourceUrl = "https://dev.gallery.openbots.io/v3/index.json";
        private string _nugetSourceUrl = "https://api.nuget.org/v3/index.json";

        public frmGalleryPackageManagerV2(Dictionary<string, string> projectDependenciesDict, 
            string packageLocation = "")
        {
            InitializeComponent();
            _projectDependenciesDict = projectDependenciesDict;
            _packageLocation = packageLocation;
            _allResults = new List<IPackageSearchMetadata>();
            _projectDependencies = new List<IPackageSearchMetadata>();
            _projectVersions = new List<NuGetVersion>();
        }

        private async void frmGalleryProject_LoadAsync(object sender, EventArgs e)
        {            
            tvPackageFeeds.ExpandAll();
            pnlProjectVersion.Hide();
            pnlProjectDetails.Hide();
            uiBtnOpen.Enabled = false;
            try
            {
                _allGalleryResults = await NugetPackageManagerV2.SearchPackages("", _gallerySourceUrl, _includePrerelease);
                _allNugetResults = await NugetPackageManagerV2.SearchPackages("", _nugetSourceUrl, _includePrerelease);
                _allResults.AddRange(_allGalleryResults);
                _allResults.AddRange(_allNugetResults);
                await GetCurrentDepencies();
                PopulateListBox(_projectDependencies);
            }
            catch (Exception ex)
            {
                //not connected to internet
                lblError.Text = "Error: " + ex.Message;
            }
        } 
        
        private void PopulateListBox(List<IPackageSearchMetadata> searchresults)
        {
            lbxNugetPackages.Visible = false;
            tpbLoadingSpinner.Visible = true;

            lbxNugetPackages.Clear();
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

                lbxNugetPackages.Add(result.Identity.Id, result.Identity.Id, result.Description, img, result.Identity.Version.ToString());               
            }

            tpbLoadingSpinner.Visible = false;
            lbxNugetPackages.Visible = true;
        }

        private async void lbxNugetPackages_ItemClick(object sender, int index)
        {
            try
            {
                string projectId = lbxNugetPackages.ClickedItem.Id;
                List<IPackageSearchMetadata> metadata = new List<IPackageSearchMetadata>();

                metadata.AddRange(await NugetPackageManagerV2.GetPackageMetadata(projectId, _gallerySourceUrl, _includePrerelease));
                metadata.AddRange(await NugetPackageManagerV2.GetPackageMetadata(projectId, _nugetSourceUrl, _includePrerelease));

                string latestVersion = metadata.LastOrDefault().Identity.Version.ToString();

                _projectVersions.Clear();
                _projectVersions.AddRange(await NugetPackageManagerV2.GetPackageVersions(projectId, _gallerySourceUrl, _includePrerelease));
                _projectVersions.AddRange(await NugetPackageManagerV2.GetPackageVersions(projectId, _nugetSourceUrl, _includePrerelease));

                List<string> versionList = _projectVersions.Select(x => x.ToString()).ToList();
                versionList.Reverse();

                cbxVersion.Items.Clear();
                foreach (var version in versionList)
                    cbxVersion.Items.Add(version);

                _selectedPackageMetaData = metadata;

                var installedPackage = _projectDependencies.Where(x => x.Identity.Id == projectId).FirstOrDefault();

                if (installedPackage != null)
                {
                    _isInstalled = true;
                    _installedVersion = installedPackage.Identity.Version.ToString();
                    PopulateProjectDetails(_installedVersion);
                    cbxVersion.SelectedItem = _installedVersion;
                }
                else
                {
                    _isInstalled = false;
                    _installedVersion = "";
                    PopulateProjectDetails(latestVersion);
                    cbxVersion.SelectedIndex = 0;
                }

                pnlProjectVersion.Show();
                pnlProjectDetails.Show();
                uiBtnOpen.Enabled = true;
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

            if (_isInstalled)
            {
                btnInstall.Text = "Update";
                lblInstalled.Visible = true;
                txtInstalled.Text = _installedVersion;
                txtInstalled.Visible = true;
                btnUninstall.Visible = true;

                if (_installedVersion == version)
                    btnInstall.Enabled = false;
                else
                    btnInstall.Enabled = true;
            }
                              
            else
            {
                btnInstall.Text = "Install";
                lblInstalled.Visible = false;
                txtInstalled.Visible = false;
                btnUninstall.Visible = false;
                btnInstall.Enabled = true;
            }               
        }

        private void llblLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_catalog.LicenseUrl.ToString()))
            {
                llblLicenseURL.LinkVisited = true;
                Process.Start(_catalog.LicenseUrl.ToString());
            }
        }

        private void llblProjectURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_catalog.ProjectUrl.ToString()))
            {
                llblProjectURL.LinkVisited = true;
                Process.Start(_catalog.ProjectUrl.ToString());
            }
        }

        private void cbxVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateProjectDetails(cbxVersion.SelectedItem.ToString());
        }

        private async void DownloadAndOpenPackage(string packageId, string version)
        {            
            try
            {
                string packageName = $"{packageId}.{version}";
                Cursor.Current = Cursors.WaitCursor;
                lblError.Text = $"Installing {packageName}";

                await NugetPackageManagerV2.InstallPackage(packageId, version, _projectDependenciesDict);                   

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
            DownloadAndOpenPackage(_catalog.Identity.Id, _catalog.Identity.Version.ToString());
        }

        private void uiBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


        private async void tvPackageFeeds_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (tvPackageFeeds.SelectedNode != null)
            {
                if (tvPackageFeeds.SelectedNode.Name == "projectDependencies")
                {
                    lblPackageCategory.Text = "Project Dependencies";
                    pbxPackageCategory.Image = Resources.OpenBots_icon;
                    await GetCurrentDepencies();
                    PopulateListBox(_projectDependencies);
                }
                else if (tvPackageFeeds.SelectedNode.Name == "allPackages")
                {
                    lblPackageCategory.Text = "All Packages";
                    pbxPackageCategory.Image = Resources.OpenBots_icon;
                    PopulateListBox(_allResults);
                }
                else if (tvPackageFeeds.SelectedNode.Name == "gallery")
                {
                    lblPackageCategory.Text = "Gallery";
                    pbxPackageCategory.Image = Resources.openbots_gallery_icon;
                    PopulateListBox(_allGalleryResults);
                }
                else if (tvPackageFeeds.SelectedNode.Name == "nuget")
                {
                    lblPackageCategory.Text = "Nuget";
                    pbxPackageCategory.Image = Resources.nuget_icon;
                    PopulateListBox(_allNugetResults);
                }
            }
        }

        private async Task GetCurrentDepencies()
        {
            List<string> nugetDirectoryList = Directory.GetDirectories(_packageLocation).ToList();
            _projectDependencies.Clear();
            foreach(var pair in _projectDependenciesDict)
            {
                var galleryDependency = (await NugetPackageManagerV2.GetPackageMetadata(pair.Key, _gallerySourceUrl, _includePrerelease))
                    .Where(x => x.Identity.Version.ToString() == pair.Value).FirstOrDefault();
                if (galleryDependency != null && nugetDirectoryList.Where(x => x.EndsWith($"{pair.Key}.{pair.Value}")).FirstOrDefault() != null)
                    _projectDependencies.Add(galleryDependency);
                var nugetDependency = (await NugetPackageManagerV2.GetPackageMetadata(pair.Key, _nugetSourceUrl, _includePrerelease))
                    .Where(x => x.Identity.Version.ToString() == pair.Value).FirstOrDefault();
                if (nugetDependency != null && nugetDirectoryList.Where(x => x.EndsWith($"{pair.Key}.{pair.Value}")).FirstOrDefault() != null)
                    _projectDependencies.Add(nugetDependency);
            }
            //string pattern = @"^(.*?)\.(?=(?:[0-9]+\.){2,}[0-9]+(?:-[a-z]+)?)(.*?)$";
        }

        private async void txtSampleSearch_KeyDown(object sender, KeyEventArgs e)
        {
            var searchResults = new List<IPackageSearchMetadata>();
            if (e.KeyCode == Keys.Enter)
            {               
                if (lblPackageCategory.Text == "Project Dependencies")
                {
                    await GetCurrentDepencies();
                    searchResults.AddRange(_projectDependencies.Where(x => x.Title.ToLower().Contains(txtSampleSearch.Text.ToLower())));
                }
                else if (lblPackageCategory.Text == "All Packages")
                {
                    searchResults.AddRange(await NugetPackageManagerV2.SearchPackages(txtSampleSearch.Text, _gallerySourceUrl, _includePrerelease));
                    searchResults.AddRange(await NugetPackageManagerV2.SearchPackages(txtSampleSearch.Text, _nugetSourceUrl, _includePrerelease));
                }
                else if (lblPackageCategory.Text == "Gallery")
                {
                    searchResults.AddRange(await NugetPackageManagerV2.SearchPackages(txtSampleSearch.Text, _gallerySourceUrl, _includePrerelease));
                }
                else if (lblPackageCategory.Text == "Nuget")
                {
                    searchResults.AddRange(await NugetPackageManagerV2.SearchPackages(txtSampleSearch.Text, _nugetSourceUrl, _includePrerelease));
                }
               
                PopulateListBox(searchResults);
            }
        }

        private void tvPackageFeeds_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (btnInstall.Text == "Install")
                DownloadAndOpenPackage(_catalog.Identity.Id, _catalog.Identity.Version.ToString());
            else if (btnInstall.Text == "Update")
            {
                UninstallPackage(_catalog.Identity.Id, txtInstalled.Text);
                DownloadAndOpenPackage(_catalog.Identity.Id, _catalog.Identity.Version.ToString());
            }
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            UninstallPackage(_catalog.Identity.Id, txtInstalled.Text);
        }

        private void UninstallPackage(string packageId, string version)
        {
            _projectDependenciesDict.Remove(packageId);
            DialogResult = DialogResult.OK;
        }

        private void chbxIncludePrerelease_CheckedChanged(object sender, EventArgs e)
        {
            _includePrerelease = chbxIncludePrerelease.Checked;
        }
    }
}
