//Copyright (c) 2019 Jason Bayldon
//Modifications - Copyright (c) 2020 OpenBots Inc.
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
using OpenBots.Core.UI.Forms;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    public partial class frmAbout : UIForm
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            DateTime buildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

            lblAppVersion.Text = "version: " + new Version(Application.ProductVersion);
            lblBuildDate.Text = "build date: " + buildDate.ToString("MM.dd.yyyy");
        }       
    }
}