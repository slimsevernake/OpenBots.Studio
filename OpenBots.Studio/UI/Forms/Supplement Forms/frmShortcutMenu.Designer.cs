namespace OpenBots.UI.Forms.Supplement_Forms
{
    partial class frmShortcutMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShortcutMenu));
            this.lblShortcutMenu = new System.Windows.Forms.Label();
            this.lvShortcutMenu = new System.Windows.Forms.ListView();
            this.Action = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Key = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lblShortcutMenu
            // 
            this.lblShortcutMenu.AutoSize = true;
            this.lblShortcutMenu.BackColor = System.Drawing.Color.Transparent;
            this.lblShortcutMenu.Font = new System.Drawing.Font("Segoe UI", 26.25F);
            this.lblShortcutMenu.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblShortcutMenu.Location = new System.Drawing.Point(1, -2);
            this.lblShortcutMenu.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblShortcutMenu.Name = "lblShortcutMenu";
            this.lblShortcutMenu.Size = new System.Drawing.Size(314, 60);
            this.lblShortcutMenu.TabIndex = 1;
            this.lblShortcutMenu.Text = "Shortcut Menu";
            // 
            // lvShortcutMenu
            // 
            this.lvShortcutMenu.AllowColumnReorder = true;
            this.lvShortcutMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvShortcutMenu.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Action,
            this.Key});
            this.lvShortcutMenu.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lvShortcutMenu.FullRowSelect = true;
            this.lvShortcutMenu.GridLines = true;
            this.lvShortcutMenu.HideSelection = false;
            this.lvShortcutMenu.Location = new System.Drawing.Point(12, 61);
            this.lvShortcutMenu.Name = "lvShortcutMenu";
            this.lvShortcutMenu.Size = new System.Drawing.Size(605, 545);
            this.lvShortcutMenu.TabIndex = 3;
            this.lvShortcutMenu.UseCompatibleStateImageBehavior = false;
            this.lvShortcutMenu.View = System.Windows.Forms.View.Details;
            // 
            // Action
            // 
            this.Action.Text = "Action";
            this.Action.Width = 393;
            // 
            // Key
            // 
            this.Key.Text = "Key";
            this.Key.Width = 206;
            // 
            // frmShortcutMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 618);
            this.Controls.Add(this.lvShortcutMenu);
            this.Controls.Add(this.lblShortcutMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmShortcutMenu";
            this.Text = "shortcut menu";
            this.Load += new System.EventHandler(this.frmShortcutMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblShortcutMenu;
        private System.Windows.Forms.ListView lvShortcutMenu;
        private System.Windows.Forms.ColumnHeader Action;
        private System.Windows.Forms.ColumnHeader Key;
    }
}