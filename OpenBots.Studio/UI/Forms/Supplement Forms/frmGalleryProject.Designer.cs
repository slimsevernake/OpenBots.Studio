using OpenBots.UI.CustomControls.CustomUIControls;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    partial class frmGalleryProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGalleryProject));
            this.lbxGalleryProjects = new OpenBots.UI.CustomControls.CustomUIControls.UIListBox();
            this.txtSampleSearch = new System.Windows.Forms.TextBox();
            this.lblGalleryProjects = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbxGalleryProjects
            // 
            this.lbxGalleryProjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbxGalleryProjects.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbxGalleryProjects.Location = new System.Drawing.Point(0, 132);
            this.lbxGalleryProjects.Name = "lbxGalleryProjects";
            this.lbxGalleryProjects.Size = new System.Drawing.Size(657, 435);
            this.lbxGalleryProjects.TabIndex = 35;
            this.lbxGalleryProjects.ItemDoubleClick += new OpenBots.UI.CustomControls.CustomUIControls.UIListBox.MouseEventHandler(this.lbxGalleryProjects_ItemDoubleClick);
            // 
            // txtSampleSearch
            // 
            this.txtSampleSearch.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSampleSearch.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtSampleSearch.Location = new System.Drawing.Point(43, 77);
            this.txtSampleSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtSampleSearch.Name = "txtSampleSearch";
            this.txtSampleSearch.Size = new System.Drawing.Size(508, 32);
            this.txtSampleSearch.TabIndex = 34;
            this.txtSampleSearch.TextChanged += new System.EventHandler(this.txtSampleSearch_TextChanged);
            // 
            // lblGalleryProjects
            // 
            this.lblGalleryProjects.AutoSize = true;
            this.lblGalleryProjects.BackColor = System.Drawing.Color.Transparent;
            this.lblGalleryProjects.Font = new System.Drawing.Font("Segoe UI Semilight", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGalleryProjects.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblGalleryProjects.Location = new System.Drawing.Point(18, 5);
            this.lblGalleryProjects.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGalleryProjects.Name = "lblGalleryProjects";
            this.lblGalleryProjects.Size = new System.Drawing.Size(287, 54);
            this.lblGalleryProjects.TabIndex = 33;
            this.lblGalleryProjects.Text = "gallery projects";
            // 
            // frmGalleryProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(657, 567);
            this.Controls.Add(this.lbxGalleryProjects);
            this.Controls.Add(this.txtSampleSearch);
            this.Controls.Add(this.lblGalleryProjects);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmGalleryProject";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gallery Project Manager";
            this.Load += new System.EventHandler(this.frmGalleryProject_LoadAsync);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomControls.CustomUIControls.UIListBox lbxGalleryProjects;
        public System.Windows.Forms.TextBox txtSampleSearch;
        private System.Windows.Forms.Label lblGalleryProjects;
    }
}