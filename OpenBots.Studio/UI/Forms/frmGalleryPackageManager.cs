using NuGet;
using OpenBots.Core.Gallery;
using OpenBots.Core.Gallery.Models;
using OpenBots.Core.Properties;
using OpenBots.Core.UI.Forms;
using OpenBots.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OpenBots.UI.Forms
{
    public partial class frmGalleryPackageManager : UIForm
    {
        private string _packageLocation;
        private List<SearchResultPackage> _allResults;
        private List<SearchResultPackage> _allGalleryResults;
        private List<SearchResultPackage> _allNugetResults;
        private List<SearchResultPackage> _projectDependencies;
        private Dictionary<string,string> _projectDependenciesDict;
        private CatalogEntry _catalog;
        private List<RegistrationItemVersion> _projectVersions;
        private NugetPackageManger _galleryManager;
        private NugetPackageManger _nugetManager;

        public frmGalleryPackageManager(Dictionary<string, string> projectDependenciesDict, 
            string packageLocation = "")
        {
            InitializeComponent();
            _projectDependenciesDict = projectDependenciesDict;
            _packageLocation = packageLocation;
            _allResults = new List<SearchResultPackage>();
            _allGalleryResults = new List<SearchResultPackage>();
            _allNugetResults = new List<SearchResultPackage>();
            _galleryManager = new NugetPackageManger();
            _nugetManager = new NugetPackageManger(new Uri("https://api.nuget.org/v3/index.json"));
        }

        private async void frmGalleryProject_LoadAsync(object sender, EventArgs e)
        {
            tvPackageFeeds.ExpandAll();
            pnlProjectVersion.Hide();
            pnlProjectDetails.Hide();
            uiBtnOpen.Enabled = false;
            try
            {
                _allGalleryResults = await _galleryManager.GetAllPackagesAsync(NugetPackageManger.PackageType.Command.ToString());
                _allNugetResults = await _nugetManager.GetAllPackagesAsync();
                _allResults.AddRange(_allGalleryResults);
                _allResults.AddRange(_allNugetResults);
                _projectDependencies = GetCurrentDepencies();
                PopulateListBox(_allResults);
            }
            catch (Exception ex)
            {
                //not connected to internet
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void txtSampleSearch_TextChanged(object sender, EventArgs e)
        {      
            if (_allResults != null)
            {
                if (!string.IsNullOrEmpty(txtSampleSearch.Text))
                {
                    var filteredResult = _allResults.Where(x => x.Id.ToLower().Contains(txtSampleSearch.Text.ToLower())).ToList();
                    PopulateListBox(filteredResult);
                }
                else
                    PopulateListBox(_allResults);
            }                                          
        }  
        
        private void PopulateListBox(List<SearchResultPackage> searchresults)
        {
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
                    img = Resources.OpenBots_icon;
                }

                lbxGalleryProjects.Add(result.Id, result.Id, result.Description, img, result.Version);
            }
        }

        private async void lbxGalleryProjects_ItemDoubleClick(object sender, int Index)
        {
            string projectId = lbxGalleryProjects.DoubleClickedItem.Id;
            var version = await _galleryManager.GetLatestPackageVersionAsync(projectId);
            DownloadAndOpenPackage(projectId, version);
            
        }

        private async void lbxGalleryProjects_ItemClick(object sender, int index)
        {
            try
            {
                string projectId = lbxGalleryProjects.ClickedItem.Id;
                List<RegistrationItem> registration = new List<RegistrationItem>();
                try
                {
                    registration = await _galleryManager.GetPackageRegistrationAsync(projectId);

                }
                catch (Exception)
                {
                    registration = await _nugetManager.GetPackageRegistrationAsync(projectId);
                }

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
            catch (Exception)
            {
                pnlProjectVersion.Hide();
                pnlProjectDetails.Hide();
                uiBtnOpen.Enabled = false;
            }
            
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
            //foreach (var dependency in _catalog.DependencyGroups.FirstOrDefault().ProjectDependencies)
                //lvDependencies.Items.Add(new ListViewItem(new string[] { dependency.Id, dependency.Range }));
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

        private async void DownloadAndOpenPackage(string packageId, SemanticVersion version)
        {            
            try
            {
                var packageDirectoryList = Directory.GetDirectories(_packageLocation).ToList();
                string existingPackageDirectory = packageDirectoryList.Where(x => new DirectoryInfo(x).Name.StartsWith(packageId)).SingleOrDefault();

                if (!string.IsNullOrEmpty(existingPackageDirectory))
                    Directory.Delete(existingPackageDirectory, true);

                string packageName = $"{packageId}.{version}";
                Cursor.Current = Cursors.WaitCursor;
                lblError.Text = $"Installing {packageName}";

                try
                {
                    await _galleryManager.DownloadPackageAsync(packageId, version, _packageLocation, packageName);
                }
                catch (Exception)
                {
                    await _nugetManager.DownloadPackageAsync(packageId, version, _packageLocation, packageName);
                }
                
                ExtractGalleryPackage(Path.Combine(_packageLocation, packageName));

                if (_projectDependenciesDict.ContainsKey(packageId))
                    _projectDependenciesDict.Remove(packageId);

                _projectDependenciesDict.Add(packageId, version.ToString());

                
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
            DownloadAndOpenPackage(_catalog.Id, SemanticVersion.Parse(_catalog.Version));
        }

        private void uiBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public static void ExtractGalleryPackage(string projectDirectory)
        {
            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            var processNugetFilePath = projectDirectory + ".nupkg";
            var processZipFilePath = projectDirectory + ".zip";

            // Create .zip file
            File.Copy(processNugetFilePath, processZipFilePath, true);

            // Extract Files/Folders from (.zip) file
            Project.DecompressFile(processZipFilePath, projectDirectory);

            // Delete .zip File
            File.Delete(processZipFilePath);
            File.Move(processNugetFilePath, Path.Combine(projectDirectory, new FileInfo(processNugetFilePath).Name));
        }


        private void tvPackageFeeds_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (tvPackageFeeds.SelectedNode != null)
            {
                if (tvPackageFeeds.SelectedNode.Name == "projectDependencies")
                {
                    lblPackageCategory.Text = "Project Dependencies";
                    PopulateListBox(GetCurrentDepencies());
                }
                else if (tvPackageFeeds.SelectedNode.Name == "allPackages")
                {
                    lblPackageCategory.Text = "All Packages";
                    PopulateListBox(_allResults);
                }
                else if (tvPackageFeeds.SelectedNode.Name == "gallery")
                {
                    lblPackageCategory.Text = "Gallery";
                    PopulateListBox(_allGalleryResults);
                }
                else if (tvPackageFeeds.SelectedNode.Name == "nuget")
                {
                    lblPackageCategory.Text = "Nuget";
                    PopulateListBox(_allNugetResults);
                }
            }
            
        }

        private List<SearchResultPackage> GetCurrentDepencies()
        {
            List<string> nugetDirectoryList = Directory.GetDirectories(_packageLocation).ToList();
            Dictionary<string, string> nugetDirectoryDict = new Dictionary<string, string>();
            string pattern = @"^(.*?)\.(?=(?:[0-9]+\.){2,}[0-9]+(?:-[a-z]+)?)(.*?)$";

            foreach (string nugetDirectory in nugetDirectoryList)
            {
                MatchCollection matches = Regex.Matches(new DirectoryInfo(nugetDirectory).Name, pattern);
                nugetDirectoryDict.Add(matches[0].Groups[1].Value, matches[0].Groups[2].Value);
            }

            var filteredResult = _allResults.Where(x => nugetDirectoryDict.ContainsKey(x.Id) && nugetDirectoryDict[x.Id] == x.Version &&
                                                        _projectDependenciesDict.ContainsKey(x.Id)).ToList();
            return filteredResult;
        }

        private async void txtSampleSearch_KeyDown(object sender, KeyEventArgs e)
        {
            var searchResults = new List<SearchResultPackage>();
            if (e.KeyCode == Keys.Enter)
            {               
                if (lblPackageCategory.Text == "Project Dependencies")
                {
                    searchResults.AddRange(GetCurrentDepencies().Where(x => x.Title.ToLower().Contains(txtSampleSearch.Text.ToLower())));
                }
                else if (lblPackageCategory.Text == "All Packages")
                {
                    searchResults.AddRange(await _galleryManager.GetPackagesByIdAsync(txtSampleSearch.Text));
                    searchResults.AddRange(await _nugetManager.GetPackagesByIdAsync(txtSampleSearch.Text));
                }
                else if (lblPackageCategory.Text == "Gallery")
                {
                    searchResults.AddRange(await _galleryManager.GetPackagesByIdAsync(txtSampleSearch.Text));
                }
                else if (lblPackageCategory.Text == "Nuget")
                {
                    searchResults.AddRange(await _nugetManager.GetPackagesByIdAsync(txtSampleSearch.Text));
                }
               
                PopulateListBox(searchResults);
            }
        }

        private void tvPackageFeeds_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
