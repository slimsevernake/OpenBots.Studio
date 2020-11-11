namespace OpenBots.UI.CustomControls.CustomUIControls
{
    partial class UIRatingBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UIRatingBar));
            this.Star1 = new System.Windows.Forms.PictureBox();
            this.ImageList1 = new System.Windows.Forms.ImageList(this.components);
            this.Star5 = new System.Windows.Forms.PictureBox();
            this.Star4 = new System.Windows.Forms.PictureBox();
            this.Star3 = new System.Windows.Forms.PictureBox();
            this.Star2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Star1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Star5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Star4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Star3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Star2)).BeginInit();
            this.SuspendLayout();
            // 
            // Star1
            // 
            this.Star1.BackColor = System.Drawing.Color.Transparent;
            this.Star1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Star1.Location = new System.Drawing.Point(7, 4);
            this.Star1.Margin = new System.Windows.Forms.Padding(0);
            this.Star1.Name = "Star1";
            this.Star1.Size = new System.Drawing.Size(17, 16);
            this.Star1.TabIndex = 1;
            this.Star1.TabStop = false;
            // 
            // ImageList1
            // 
            this.ImageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList1.ImageStream")));
            this.ImageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList1.Images.SetKeyName(0, "empty");
            this.ImageList1.Images.SetKeyName(1, "full");
            // 
            // Star5
            // 
            this.Star5.BackColor = System.Drawing.Color.Transparent;
            this.Star5.Cursor = System.Windows.Forms.Cursors.Default;
            this.Star5.Location = new System.Drawing.Point(76, 4);
            this.Star5.Margin = new System.Windows.Forms.Padding(0);
            this.Star5.Name = "Star5";
            this.Star5.Size = new System.Drawing.Size(17, 16);
            this.Star5.TabIndex = 2;
            this.Star5.TabStop = false;
            // 
            // Star4
            // 
            this.Star4.BackColor = System.Drawing.Color.Transparent;
            this.Star4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Star4.Location = new System.Drawing.Point(59, 4);
            this.Star4.Margin = new System.Windows.Forms.Padding(0);
            this.Star4.Name = "Star4";
            this.Star4.Size = new System.Drawing.Size(17, 16);
            this.Star4.TabIndex = 3;
            this.Star4.TabStop = false;
            // 
            // Star3
            // 
            this.Star3.BackColor = System.Drawing.Color.Transparent;
            this.Star3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Star3.Location = new System.Drawing.Point(42, 4);
            this.Star3.Margin = new System.Windows.Forms.Padding(0);
            this.Star3.Name = "Star3";
            this.Star3.Size = new System.Drawing.Size(17, 16);
            this.Star3.TabIndex = 4;
            this.Star3.TabStop = false;
            // 
            // Star2
            // 
            this.Star2.BackColor = System.Drawing.Color.Transparent;
            this.Star2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Star2.Location = new System.Drawing.Point(24, 4);
            this.Star2.Margin = new System.Windows.Forms.Padding(0);
            this.Star2.Name = "Star2";
            this.Star2.Size = new System.Drawing.Size(17, 16);
            this.Star2.TabIndex = 5;
            this.Star2.TabStop = false;
            // 
            // UIRatingBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.Star1);
            this.Controls.Add(this.Star5);
            this.Controls.Add(this.Star4);
            this.Controls.Add(this.Star3);
            this.Controls.Add(this.Star2);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(100, 18);
            this.MinimumSize = new System.Drawing.Size(100, 18);
            this.Name = "UIRatingBar";
            this.Size = new System.Drawing.Size(100, 18);
            ((System.ComponentModel.ISupportInitialize)(this.Star1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Star5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Star4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Star3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Star2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.PictureBox Star1;
        internal System.Windows.Forms.ImageList ImageList1;
        internal System.Windows.Forms.PictureBox Star5;
        internal System.Windows.Forms.PictureBox Star4;
        internal System.Windows.Forms.PictureBox Star3;
        internal System.Windows.Forms.PictureBox Star2;
    }
}
