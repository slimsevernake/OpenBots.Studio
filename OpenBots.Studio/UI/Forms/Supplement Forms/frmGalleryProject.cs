using OpenBots.Core.Gallery;
using OpenBots.Core.Gallery.Models;
using OpenBots.Core.Properties;
using OpenBots.Core.UI.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    public partial class frmGalleryProject : UIForm
    {
        private string _projectLocation;
        private string _projectName;
        private List<SearchResultPackage> _searchresults;

        public frmGalleryProject(string projectLocation, string projectName)
        {
            InitializeComponent();
            _projectLocation = projectLocation;
            _projectName = projectName;
        }

        private async void frmGalleryProject_LoadAsync(object sender, EventArgs e)
        {
            try
            {           
                _searchresults = await new NugetPackageManger().GetAllPackagesAsync(NugetPackageManger.PackageType.Automation);
                PopulateListBox(_searchresults);
            }
            catch (Exception)
            {
                //not connected to internet
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
                {
                    PopulateListBox(_searchresults);
                }
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

                lbxGalleryProjects.Add(result.Id, result.Title, result.Description, result.Version, img);
            }
        }

        private async void lbxGalleryProjects_ItemDoubleClick(object sender, int Index)
        {
            string projectId = lbxGalleryProjects.DoubleClickedItem.Id;
            var manager = new NugetPackageManger();
            var version = await manager.GetLatestPackageVersionAsync(projectId);
            await new NugetPackageManger().DownloadPackageAsync(projectId, version, _projectLocation, _projectName);
            DialogResult = DialogResult.OK;
        }
    }
}
