using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using OpenBots.Core.Gallery;
using OpenBots.Core.Properties;
using OpenBots.Core.Settings;
using OpenBots.Core.UI.Forms;
using OpenBots.UI.Forms.Supplement_Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenBots.UI.Forms
{
    public partial class frmGalleryPackageManager : UIForm
    {
        private string _packageLocation;
        private List<IPackageSearchMetadata> _allResults;
        private List<IPackageSearchMetadata> _projectDependencies;
        private Dictionary<string, string> _projectDependenciesDict;
        private IPackageSearchMetadata _catalog;
        private List<NuGetVersion> _projectVersions;
        private List<IPackageSearchMetadata> _selectedPackageMetaData;
        private bool _isInstalled;
        private string _installedVersion;
        private bool _includePrerelease;
        private DataTable _packageSourceDT { get; set; }
        private string _appDataPackagePath;

        private ApplicationSettings _settings;

        public frmGalleryPackageManager(Dictionary<string, string> projectDependenciesDict, 
            string packageLocation = "")
        {
            InitializeComponent();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _appDataPackagePath = Path.Combine(appDataPath, "OpenBots Inc", "packages");

            _settings = new ApplicationSettings().GetOrCreateApplicationSettings();
            _packageSourceDT = _settings.EngineSettings.PackageSourceDT;

            _projectDependenciesDict = projectDependenciesDict;
            _packageLocation = packageLocation;
            _allResults = new List<IPackageSearchMetadata>();
            _projectDependencies = new List<IPackageSearchMetadata>();
            _projectVersions = new List<NuGetVersion>();
        }

        private async void frmGalleryProject_LoadAsync(object sender, EventArgs e)
        {
            PopulatetvPackageFeeds();
            pnlProjectVersion.Hide();
            pnlProjectDetails.Hide();

            try
            {
                foreach (DataRow row in _packageSourceDT.Rows)
                {
                    if (row[0].ToString() == "True")
                    {
                        try
                        {
                            _allResults.AddRange(await NugetPackageManager.SearchPackages("", row[2].ToString(), _includePrerelease));
                        }
                        catch (Exception ex)
                        {
                            //invalid source
                            lblError.Text = "Error: " + ex.Message;
                        }                        
                    }
                }

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
                    img = Resources.nuget_icon;
                }

                lbxNugetPackages.Add(result.Identity.Id, result.Identity.Id, result.Description, img, result.Identity.Version.ToString());               
            }

            tpbLoadingSpinner.Visible = false;
            lbxNugetPackages.Visible = true;
        }

        private async void lbxNugetPackages_ItemClick(object sender, int index)
        {
            lblError.Text = "";
            try
            {
                string projectId = lbxNugetPackages.ClickedItem.Id;
                List<IPackageSearchMetadata> metadata = new List<IPackageSearchMetadata>();

                if (lblPackageCategory.Text == "Project Dependencies")
                {
                    metadata.AddRange(await NugetPackageManager.GetPackageMetadata(projectId, _appDataPackagePath, _includePrerelease));
                }
                else if (lblPackageCategory.Text == "All Packages")
                {
                    foreach (DataRow row in _packageSourceDT.Rows)
                    {
                        if (row[0].ToString() == "True")
                        {
                            metadata.AddRange(await NugetPackageManager.GetPackageMetadata(projectId, row[2].ToString(), _includePrerelease));
                        }
                    }
                }
                else
                {
                    var sourceURL = _packageSourceDT.AsEnumerable().Where(r => r.Field<string>("Enabled") == "True" && r.Field<string>("Package Name") == lblPackageCategory.Text)
                        .Select(r => r.Field<string>("Package Source")).FirstOrDefault();
                    metadata.AddRange(await NugetPackageManager.GetPackageMetadata(projectId, sourceURL, _includePrerelease));
                }              

                string latestVersion = metadata.LastOrDefault().Identity.Version.ToString();

                _projectVersions.Clear();

                if (lblPackageCategory.Text == "Project Dependencies")
                {
                    _projectVersions.AddRange(await NugetPackageManager.GetPackageVersions(projectId, _appDataPackagePath, _includePrerelease));
                }
                else if (lblPackageCategory.Text == "All Packages")
                {
                    foreach (DataRow row in _packageSourceDT.Rows)
                    {
                        if (row[0].ToString() == "True")
                        {
                            _projectVersions.AddRange(await NugetPackageManager.GetPackageVersions(projectId, row[2].ToString(), _includePrerelease));
                        }
                    }
                }
                else
                {
                    var sourceURL = _packageSourceDT.AsEnumerable().Where(r => r.Field<string>("Enabled") == "True" && r.Field<string>("Package Name") == lblPackageCategory.Text)
                        .Select(r => r.Field<string>("Package Source")).FirstOrDefault();
                    _projectVersions.AddRange(await NugetPackageManager.GetPackageVersions(projectId, sourceURL, _includePrerelease));
                }

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
            }
            catch (Exception)
            {
                pnlProjectVersion.Hide();
                pnlProjectDetails.Hide();
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
            lblError.Text = "";
            if (!string.IsNullOrEmpty(_catalog.LicenseUrl.ToString()))
            {
                llblLicenseURL.LinkVisited = true;
                Process.Start(_catalog.LicenseUrl.ToString());
            }
        }

        private void llblProjectURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lblError.Text = "";
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

                await NugetPackageManager.InstallPackage(packageId, version, _projectDependenciesDict);                   

                lblError.Text = string.Empty;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void uiBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void PopulatetvPackageFeeds()
        {
            tvPackageFeeds.Nodes.Clear();

            TreeNode settingsNode = new TreeNode("Settings");
            settingsNode.Name = "Settings";
            settingsNode.ImageIndex = 3;
            settingsNode.SelectedImageIndex = 3;
            tvPackageFeeds.Nodes.Add(settingsNode);

            TreeNode projectDepNode = new TreeNode("Project Dependencies");
            projectDepNode.Name = "ProjectDependencies";
            tvPackageFeeds.Nodes.Add(projectDepNode);

            TreeNode allPackagesNode = new TreeNode("All Packages");
            allPackagesNode.Name = "AllPackages";
            tvPackageFeeds.Nodes.Add(allPackagesNode);

            foreach(DataRow row in _packageSourceDT.Rows)
            {
                if (row[0].ToString() == "True")
                {
                    string sourceName = row[1].ToString();
                    TreeNode sourceNode = new TreeNode(sourceName);
                    sourceNode.Name = sourceName.Replace(" ", "");
                    sourceNode.ToolTipText = row[2].ToString();
                    switch (sourceName)
                    {
                        case "Gallery":
                            sourceNode.ImageIndex = 1;
                            sourceNode.SelectedImageIndex = 1;
                            break;
                        case "Nuget":
                            sourceNode.ImageIndex = 2;
                            sourceNode.SelectedImageIndex = 2;
                            break;
                        default:
                            sourceNode.ImageIndex = 2;
                            sourceNode.SelectedImageIndex = 2;
                            break;
                    }
                    allPackagesNode.Nodes.Add(sourceNode);
                }                
            }
            tvPackageFeeds.ExpandAll();
        }

        private async void tvPackageFeeds_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            lblError.Text = "";
            try
            {
                if (tvPackageFeeds.SelectedNode != null)
                {
                    lbxNugetPackages.Clear();
                    lbxNugetPackages.Visible = false;
                    tpbLoadingSpinner.Visible = true;

                    if (tvPackageFeeds.SelectedNode.Name == "Settings")
                    {
                        var addPackageSource = new frmAddPackageSource(_packageSourceDT);
                        addPackageSource.ShowDialog();
                        if (addPackageSource.DialogResult == DialogResult.OK)
                        {
                            _packageSourceDT = addPackageSource.PackageSourceDT;
                            PopulatetvPackageFeeds();
                            _settings.Save(_settings);                           
                        }
                        tpbLoadingSpinner.Visible = false;
                    }
                    else if (tvPackageFeeds.SelectedNode.Name == "ProjectDependencies")
                    {
                        lblPackageCategory.Text = "Project Dependencies";
                        pbxPackageCategory.Image = Resources.OpenBots_icon;
                        await GetCurrentDepencies();
                        PopulateListBox(_projectDependencies);
                    }
                    else if (tvPackageFeeds.SelectedNode.Name == "AllPackages")
                    {
                        lblPackageCategory.Text = "All Packages";
                        pbxPackageCategory.Image = Resources.OpenBots_icon;
                        PopulateListBox(_allResults);
                    }
                    else if (tvPackageFeeds.SelectedNode.Name == "Gallery")
                    {
                        lblPackageCategory.Text = "Gallery";
                        pbxPackageCategory.Image = Resources.openbots_gallery_icon;
                        var sourceResults = await NugetPackageManager.SearchPackages("", tvPackageFeeds.SelectedNode.ToolTipText, _includePrerelease);
                        PopulateListBox(sourceResults);
                    }
                    else
                    {
                        lblPackageCategory.Text = tvPackageFeeds.SelectedNode.Text;
                        pbxPackageCategory.Image = Resources.nuget_icon;
                        var sourceResults = await NugetPackageManager.SearchPackages("", tvPackageFeeds.SelectedNode.ToolTipText, _includePrerelease);
                        PopulateListBox(sourceResults);
                    }
                }
            }
            catch (Exception)
            {
                lblPackageCategory.Text = "Source Not Found";
                tpbLoadingSpinner.Visible = false;
            }           
        }

        private async Task GetCurrentDepencies()
        {
            List<string> nugetDirectoryList = Directory.GetDirectories(_packageLocation).ToList();
            
            _projectDependencies.Clear();
            foreach(var pair in _projectDependenciesDict)
            {
                var dependency = (await NugetPackageManager.GetPackageMetadata(pair.Key, _appDataPackagePath, _includePrerelease))
                    .Where(x => x.Identity.Version.ToString() == pair.Value).FirstOrDefault();
                if (dependency != null && nugetDirectoryList.Where(x => x.EndsWith($"{pair.Key}.{pair.Value}")).FirstOrDefault() != null)
                    _projectDependencies.Add(dependency);

            }
        }

        private async void txtSampleSearch_KeyDown(object sender, KeyEventArgs e)
        {
            lblError.Text = "";
            var searchResults = new List<IPackageSearchMetadata>();

            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (lblPackageCategory.Text == "Project Dependencies")
                    {
                        await GetCurrentDepencies();
                        searchResults.AddRange(_projectDependencies.Where(x => x.Title.ToLower().Contains(txtSampleSearch.Text.ToLower())));
                    }
                    else if (lblPackageCategory.Text == "All Packages")
                    {
                        foreach (DataRow row in _packageSourceDT.Rows)
                        {
                            if (row[0].ToString() == "True")
                            {
                                try
                                {
                                    searchResults.AddRange(await NugetPackageManager.SearchPackages(txtSampleSearch.Text, row[2].ToString(), _includePrerelease));
                                } 
                                catch (Exception ex)
                                {
                                    lblError.Text = "Error: " + ex.Message;
                                }                                                           
                            }
                        }
                    }
                    else
                    {
                        var sourceURL = _packageSourceDT.AsEnumerable().Where(r => r.Field<string>("Enabled") == "True" && r.Field<string>("Package Name") == lblPackageCategory.Text)
                            .Select(r => r.Field<string>("Package Source")).FirstOrDefault();
                        searchResults.AddRange(await NugetPackageManager.SearchPackages(txtSampleSearch.Text, sourceURL, _includePrerelease));
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Error: " + ex.Message;
                }

                PopulateListBox(searchResults);
                e.Handled = true;
            }
        }

        private void txtSampleSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                e.Handled = true;
        }

        private void tvPackageFeeds_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            if (btnInstall.Text == "Install")
                DownloadAndOpenPackage(_catalog.Identity.Id, cbxVersion.SelectedItem.ToString());
            else if (btnInstall.Text == "Update")
            {
                _projectDependenciesDict.Remove(_catalog.Identity.Id);
                DownloadAndOpenPackage(_catalog.Identity.Id, cbxVersion.SelectedItem.ToString());
            }
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            _projectDependenciesDict.Remove(_catalog.Identity.Id);
            DialogResult = DialogResult.OK;
        }

        private void chbxIncludePrerelease_CheckedChanged(object sender, EventArgs e)
        {
            _includePrerelease = chbxIncludePrerelease.Checked;
        }
    }
}
