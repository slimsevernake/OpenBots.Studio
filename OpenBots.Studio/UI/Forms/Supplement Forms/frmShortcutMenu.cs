using OpenBots.Core.UI.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    public partial class frmShortcutMenu : UIForm
    {
        public frmShortcutMenu()
        {            
            InitializeComponent();
        }

        private void frmShortcutMenu_Load(object sender, EventArgs e)
        {
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Enable Selected Code", "Ctrl + E" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Disable Selected Code", "Ctrl + D" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Add/Remove Breakpoint", "Ctrl + B" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Cut Selected Code", "Ctrl + X" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Copy Selected Code", "Ctrl + C" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Paste Selected Code", "Ctrl  +V" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Delete Selected Code", "Del" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Select All Code", "Ctrl + A" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Open Code Editor", "Enter" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Undo Changes", "Ctrl + Z" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Redo Changes", "Ctrl + R" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Save Script File", "Ctrl + S" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Save All Script Files", "Ctrl + Shift + S" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Open Variable Manager", "Ctrl + K" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Open Element Manager", "Ctrl + L" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Open About", "Ctrl + O" }));
            lvShortcutMenu.Items.Add(new ListViewItem(new string[] { "Open Shortcut Menu", "Ctrl + M" }));

            ColorListViewHeader(ref lvShortcutMenu, Color.FromArgb(20, 136, 204), Color.White, new Font("Segoe UI", 10, FontStyle.Bold));
        }

        private void ColorListViewHeader(ref ListView list, Color backColor, Color foreColor, Font font)
        {
            list.OwnerDraw = true;
            list.DrawColumnHeader +=
                new DrawListViewColumnHeaderEventHandler
                (
                    (sender, e) => HeaderDraw(sender, e, backColor, foreColor, font)
                );
            list.DrawItem += new DrawListViewItemEventHandler(BodyDraw);
        }

        private void HeaderDraw(object sender, DrawListViewColumnHeaderEventArgs e, Color backColor, Color foreColor, Font font)
        {
            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            using (SolidBrush foreBrush = new SolidBrush(foreColor))
            {
                e.Graphics.DrawString(e.Header.Text, font, foreBrush, e.Bounds);
            }
        }

        private void BodyDraw(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}