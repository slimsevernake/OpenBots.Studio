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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using OpenBots.Core.Server.User;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            DateTime buildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

            lblAppVersion.Text = $"Version: {Application.ProductVersion}";
            lblBuildDate.Text = $"Build Date: {buildDate:MM-dd-yyyy}";
            lblMachineName.Text = $"Machine Name: {MachineInfo.MachineName}";
            lblIPAddress.Text = $"IP Address: {MachineInfo.IPAddress}";
            lblMacAddress.Text = $"Mac Address: {MachineInfo.GetMacAddress()}";

            string agentSettingsPath = Path.Combine(EnvironmentSettings.GetEnvironmentVariable(), EnvironmentSettings.SettingsFileName);

            if (agentSettingsPath == null)
                lblServer.Text = "Server: Agent settings file not found";

            else
            {
                string agentSettingsText = File.ReadAllText(agentSettingsPath);
                var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(agentSettingsText);

                string agentId = settings["AgentId"];
                string serverURL = settings["OpenBotsServerUrl"];

                if (string.IsNullOrEmpty(agentId))
                    lblServer.Text = "Server: Agent is not connected";
                else
                    lblServer.Text = $"Server: {serverURL}";
            }           
        }
    }
}