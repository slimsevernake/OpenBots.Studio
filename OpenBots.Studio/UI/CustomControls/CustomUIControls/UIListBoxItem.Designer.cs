namespace OpenBots.UI.CustomControls.CustomUIControls
{
    partial class UIListBoxItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UIListBoxItem));
            this.ImageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.lblLatestVersion = new System.Windows.Forms.Label();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ImageList1
            // 
            this.ImageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList1.ImageStream")));
            this.ImageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList1.Images.SetKeyName(0, "default");
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "default");
            // 
            // lblLatestVersion
            // 
            this.lblLatestVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLatestVersion.AutoSize = true;
            this.lblLatestVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblLatestVersion.Location = new System.Drawing.Point(433, 31);
            this.lblLatestVersion.Name = "lblLatestVersion";
            this.lblLatestVersion.Size = new System.Drawing.Size(45, 23);
            this.lblLatestVersion.TabIndex = 4;
            this.lblLatestVersion.Text = "0.0.0";
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentVersion.Location = new System.Drawing.Point(433, 8);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(45, 23);
            this.lblCurrentVersion.TabIndex = 5;
            this.lblCurrentVersion.Text = "0.0.0";
            // 
            // UIListBoxItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.lblLatestVersion);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI Light", 9.75F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UIListBoxItem";
            this.Size = new System.Drawing.Size(484, 75);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UIListBoxItem_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UIListBoxItem_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UIListBoxItem_MouseDown);
            this.MouseEnter += new System.EventHandler(this.UIListBoxItem_MouseEnter);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UIListBoxItem_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        internal System.Windows.Forms.ImageList ImageList1;
        internal System.Windows.Forms.ImageList imageList2;
        internal System.Windows.Forms.Label lblLatestVersion;
        internal System.Windows.Forms.Label lblCurrentVersion;
    }
}
