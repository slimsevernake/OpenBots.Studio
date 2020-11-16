using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using OpenBots.Core.Gallery;
using OpenBots.Core.Project;
using OpenBots.Core.Properties;
using OpenBots.Core.UI.Forms;
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
    public partial class frmGalleryPackageManagerV2 : UIForm
    {
        private string _packageLocation;
        private List<IPackageSearchMetadata> _allResults;
        private List<IPackageSearchMetadata> _allGalleryResults;
        private List<IPackageSearchMetadata> _allNugetResults;
        private List<IPackageSearchMetadata> _projectDependencies;
        private List<Dependency> _projectDependenciesList;
        private IPackageSearchMetadata _catalog;
        private List<NuGetVersion> _projectVersions;
        private List<IPackageSearchMetadata> _selectedPackageMetaData;
        //private NugetPackageManger _galleryManager;
        //private NugetPackageManger _nugetManager;

        private string _gallerySourceUrl = "https://dev.gallery.openbots.io/v3/index.json";
        private string _nugetSourceUrl = "https://api.nuget.org/v3/index.json";

        public frmGalleryPackageManagerV2(List<Dependency> projectDependenciesDict, 
            string packageLocation = "")
        {
            InitializeComponent();
            _projectDependenciesList = projectDependenciesDict;
            _packageLocation = packageLocation;
            _allResults = new List<IPackageSearchMetadata>();
            _projectVersions = new List<NuGetVersion>();
            //_allGalleryResults = new List<SearchResultPackage>();
            //_allNugetResults = new List<SearchResultPackage>();
            // _galleryManager = new NugetPackageManger();
            // _nugetManager = new NugetPackageManger(new Uri("https://api.nuget.org/v3/index.json"));
        }

        private async void frmGalleryProject_LoadAsync(object sender, EventArgs e)
        {            
            tvPackageFeeds.ExpandAll();
            pnlProjectVersion.Hide();
            pnlProjectDetails.Hide();
            uiBtnOpen.Enabled = false;
            try
            {
                _allGalleryResults = await NugetPackageManagerV2.SearchPackages("", _gallerySourceUrl); // _galleryManager.GetAllPackagesAsync(NugetPackageManger.PackageType.Command.ToString());
                _allNugetResults = await NugetPackageManagerV2.SearchPackages("", _nugetSourceUrl);
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
                    var filteredResult = _allResults.Where(x => x.Identity.Id.ToLower().Contains(txtSampleSearch.Text.ToLower())).ToList();
                    PopulateListBox(filteredResult);
                }
                else
                    PopulateListBox(_allResults);
            }                                          
        }  
        
        private void PopulateListBox(List<IPackageSearchMetadata> searchresults)
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

                lbxGalleryProjects.Add(result.Identity.Id, result.Identity.Id, result.Description, img, result.Identity.Version.ToString());
            }
        }

        private async void lbxGalleryProjects_ItemDoubleClick(object sender, int Index)
        {
            //string projectId = lbxGalleryProjects.DoubleClickedItem.Id;
            //var version = await _galleryManager.GetLatestPackageVersionAsync(projectId);
            //DownloadAndOpenPackage(projectId, version);
            
        }

        private async void lbxGalleryProjects_ItemClick(object sender, int index)
        {
            try
            {
                string projectId = lbxGalleryProjects.ClickedItem.Id;
                List<IPackageSearchMetadata> metadata = new List<IPackageSearchMetadata>();

                metadata.AddRange(await NugetPackageManagerV2.GetPackageMetadata(projectId, _gallerySourceUrl)); //_galleryManager.GetPackageRegistrationAsync(projectId);
                metadata.AddRange(await NugetPackageManagerV2.GetPackageMetadata(projectId, _nugetSourceUrl)); // _nugetManager.GetPackageRegistrationAsync(projectId);

                string latestVersion = metadata.LastOrDefault().Identity.Version.ToString(); //FirstOrDefault()..Upper;

                _projectVersions.AddRange(await NugetPackageManagerV2.GetPackageVersions(projectId, _gallerySourceUrl)); //_galleryManager.GetPackageRegistrationAsync(projectId);
                _projectVersions.AddRange(await NugetPackageManagerV2.GetPackageVersions(projectId, _nugetSourceUrl)); // _nugetManager.GetPackageRegistrationAsync(projectId);

                //_projectVersions = registration.getv
                List<string> versionList = _projectVersions.Select(x => x.ToString()).ToList();
                versionList.Reverse();

                cbxVersion.Items.Clear();
                foreach (var version in versionList)
                    cbxVersion.Items.Add(version);

                _selectedPackageMetaData = metadata;
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
            _catalog = _selectedPackageMetaData.Where(x => x.Identity.Version.ToString() == version).SingleOrDefault();//_projectVersions.Where(x => x.Catalog.Version == version).SingleOrDefault().Catalog;

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
                var packageDirectoryList = Directory.GetDirectories(_packageLocation).ToList();
                string existingPackageDirectory = packageDirectoryList.Where(x => new DirectoryInfo(x).Name.StartsWith(packageId)).SingleOrDefault();

                if (!string.IsNullOrEmpty(existingPackageDirectory))
                    Directory.Delete(existingPackageDirectory, true);

                string packageName = $"{packageId}.{version}";
                Cursor.Current = Cursors.WaitCursor;
                lblError.Text = $"Installing {packageName}";

                await NugetPackageManagerV2.InstallPackage(packageId, version, _projectDependenciesList);
                    
                //try
                //{
                //    await NugetPackageManagerV2.DownloadPackage(packageId, version.ToString(), _gallerySourceUrl);//_galleryManager.DownloadPackageAsync(packageId, version, _packageLocation, packageName);
                //}
                //catch (Exception)
                //{
                //    await NugetPackageManagerV2.DownloadPackage(packageId, version.ToString(), _nugetSourceUrl);
                //}
                
                //ExtractGalleryPackage(Path.Combine(_packageLocation, packageName));

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

        private List<IPackageSearchMetadata> GetCurrentDepencies()
        {
            List<string> nugetDirectoryList = Directory.GetDirectories(_packageLocation).ToList();
            Dictionary<string, string> nugetDirectoryDict = new Dictionary<string, string>();
            string pattern = @"^(.*?)\.(?=(?:[0-9]+\.){2,}[0-9]+(?:-[a-z]+)?)(.*?)$";

            foreach (string nugetDirectory in nugetDirectoryList)
            {
                MatchCollection matches = Regex.Matches(new DirectoryInfo(nugetDirectory).Name, pattern);
                nugetDirectoryDict.Add(matches[0].Groups[1].Value, matches[0].Groups[2].Value);
            }

            var filteredResult = _allResults.Where(x => nugetDirectoryDict.ContainsKey(x.Identity.Id) && nugetDirectoryDict[x.Identity.Id] == x.Identity.Version.ToString() &&
                                                        _projectDependenciesList.Where(p => p.PackageId == x.Identity.Id).FirstOrDefault() != null).ToList();
            return filteredResult;
        }

        private async void txtSampleSearch_KeyDown(object sender, KeyEventArgs e)
        {
            var searchResults = new List<IPackageSearchMetadata>();
            if (e.KeyCode == Keys.Enter)
            {               
                if (lblPackageCategory.Text == "Project Dependencies")
                {
                    searchResults.AddRange(GetCurrentDepencies().Where(x => x.Title.ToLower().Contains(txtSampleSearch.Text.ToLower())));
                }
                else if (lblPackageCategory.Text == "All Packages")
                {
                    searchResults.AddRange(await NugetPackageManagerV2.SearchPackages(txtSampleSearch.Text, _gallerySourceUrl));//_galleryManager.GetPackagesByIdAsync(txtSampleSearch.Text));
                    searchResults.AddRange(await NugetPackageManagerV2.SearchPackages(txtSampleSearch.Text, _nugetSourceUrl));
                }
                else if (lblPackageCategory.Text == "Gallery")
                {
                    searchResults.AddRange(await NugetPackageManagerV2.SearchPackages(txtSampleSearch.Text, _gallerySourceUrl));
                }
                else if (lblPackageCategory.Text == "Nuget")
                {
                    searchResults.AddRange(await NugetPackageManagerV2.SearchPackages(txtSampleSearch.Text, _nugetSourceUrl));
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
