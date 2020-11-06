using OpenBots.Core.Gallery;
using OpenBots.Core.Gallery.Models;
using OpenBots.Core.Properties;
using OpenBots.Core.UI.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    public partial class frmGalleryProject : UIForm
    {
        private string _projectLocation;
        private string _projectName;

        public frmGalleryProject(string projectLocation, string projectName)
        {
            InitializeComponent();
            _projectLocation = projectLocation;
            _projectName = projectName;
        }

        private async void txtSampleSearch_TextChangedAsync(object sender, EventArgs e)
        {
            try
            {
                List<SearchResultPackage> searchresults = await new NugetPackageManger().GetAllVersionsByTitleAsync(txtSampleSearch.Text, 
                                                                                                NugetPackageManger.PackageType.Automation);
                lbxGalleryProjects.Clear();

                if (!string.IsNullOrEmpty(txtSampleSearch.Text))
                {
                    foreach (var result in searchresults)
                    {
                        if (result.Title.ToLower().Contains(txtSampleSearch.Text.ToLower()))
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
                }
                else
                    lbxGalleryProjects.Clear();
            }
            catch (Exception)
            {
                //not connected to internet
            }
        }       

        private async void lbxGalleryProjects_ItemDoubleClick(object sender, int Index)
        {
            string projectId = lbxGalleryProjects.DoubleClickedItem.Id;
            var manager = new NugetPackageManger();
            var version = await manager.GetLatestVersionAsync(projectId);
            await new NugetPackageManger().DownloadAsync(projectId, version, _projectLocation, _projectName);
            DialogResult = DialogResult.OK;
        }
    }
}
