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
using OpenBots.Core.Utilities.FormsUtilities;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBots.Core.UI.Forms
{
    public class UIForm : Form
    {
        public Theme Theme { get; set; } = new Theme();

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UIForm));
            this.SuspendLayout();
            // 
            // UIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 554);
            this.Icon = global::OpenBots.Core.Properties.Resources.OpenBots_ico;
            this.Name = "UIForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.UIForm_Load);
            this.ResumeLayout(false);

        }

        public UIForm()
        {
            InitializeComponent();
        }

        private void UIForm_Load(object sender, EventArgs e)
        {

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Resizes if too tall to fit
            if (ClientSize.Height > Screen.PrimaryScreen.WorkingArea.Height - CurrentAutoScaleDimensions.Height)
            {
                int width = ClientSize.Width;
                int height = Screen.PrimaryScreen.WorkingArea.Height - (int)CurrentAutoScaleDimensions.Height;

                ClientSize = new Size(width, height);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ClientRectangle.Width != 0 && ClientRectangle.Height != 0)
                e.Graphics.FillRectangle(Theme.CreateGradient(ClientRectangle), ClientRectangle);
            base.OnPaint(e);           
        }

        public static void MoveFormToBottomRight(Form sender)
        {
            int x = Screen.FromPoint(sender.Location).WorkingArea.Right - sender.Width;
            int y = Screen.FromPoint(sender.Location).WorkingArea.Bottom - sender.Height;
            sender.Location = new Point(x, y);
        }
    }
}