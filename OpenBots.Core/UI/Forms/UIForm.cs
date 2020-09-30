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
using OpenBots.Core.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OpenBots.Core.UI.Forms
{
    public class UIForm : Form
    {
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(UIForm));
            SuspendLayout();
            // 
            // UIForm
            // 
            ClientSize = new Size(284, 261);
            Icon = Resources.OpenBots_ico;
            Name = "UIForm";
            Load += new EventHandler(UIForm_Load);
            ResumeLayout(false);

        }
        private void UIForm_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            var topColor = Color.FromArgb(49, 49, 49);
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, Width, Height),
                topColor,
                topColor,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, 0, 0, Width, Height);
            }
        }

        public static void MoveFormToBottomRight(Form sender)
        {
            int x = Screen.FromPoint(sender.Location).WorkingArea.Right - sender.Width;
            int y = Screen.FromPoint(sender.Location).WorkingArea.Bottom - sender.Height;
            sender.Location = new Point(x, y);
        }
    }
}