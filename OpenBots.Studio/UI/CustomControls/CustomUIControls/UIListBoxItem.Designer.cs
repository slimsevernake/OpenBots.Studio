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
            this.lblVersion = new System.Windows.Forms.Label();
            this.RatingBar = new OpenBots.UI.CustomControls.CustomUIControls.UIRatingBar();
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
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Location = new System.Drawing.Point(433, 31);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(45, 23);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "0.0.0";
            // 
            // RatingBar
            // 
            this.RatingBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RatingBar.BackColor = System.Drawing.Color.Transparent;
            this.RatingBar.Location = new System.Drawing.Point(390, 7);
            this.RatingBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.RatingBar.MaximumSize = new System.Drawing.Size(90, 20);
            this.RatingBar.MinimumSize = new System.Drawing.Size(90, 20);
            this.RatingBar.Name = "RatingBar";
            this.RatingBar.Size = new System.Drawing.Size(90, 20);
            this.RatingBar.Stars = 3;
            this.RatingBar.TabIndex = 2;
            // 
            // UIListBoxItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RatingBar);
            this.Controls.Add(this.lblVersion);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI Light", 9.75F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UIListBoxItem";
            this.Size = new System.Drawing.Size(484, 75);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Paint += UIListBoxItem_Paint;
            this.MouseClick += UIListBoxItem_MouseClick;
            this.MouseDown += UIListBoxItem_MouseDown;
            this.MouseEnter += UIListBoxItem_MouseEnter;
            this.MouseUp += UIListBoxItem_MouseUp;

        }
        #endregion

        internal System.Windows.Forms.ImageList ImageList1;
        internal System.Windows.Forms.ImageList imageList2;
        internal System.Windows.Forms.Label lblVersion;
        internal UIRatingBar RatingBar;
    }
}
