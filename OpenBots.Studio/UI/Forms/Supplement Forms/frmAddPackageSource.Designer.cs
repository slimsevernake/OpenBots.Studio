using System.Windows.Forms;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    partial class frmAddPackageSource
    {
        /// <summary>
        /// Required designer element.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddPackageSource));
            this.lblHeader = new System.Windows.Forms.Label();
            this.uiBtnOk = new OpenBots.Core.UI.Controls.UIPictureButton();
            this.uiBtnCancel = new OpenBots.Core.UI.Controls.UIPictureButton();
            this.dgvPackageSources = new System.Windows.Forms.DataGridView();
            this.enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.packageName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.packageSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.uiBtnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiBtnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPackageSources)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Semilight", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblHeader.Location = new System.Drawing.Point(8, 4);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(367, 54);
            this.lblHeader.TabIndex = 14;
            this.lblHeader.Text = "add package source";
            // 
            // uiBtnOk
            // 
            this.uiBtnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uiBtnOk.BackColor = System.Drawing.Color.Transparent;
            this.uiBtnOk.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.uiBtnOk.DisplayText = "Ok";
            this.uiBtnOk.DisplayTextBrush = System.Drawing.Color.White;
            this.uiBtnOk.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.uiBtnOk.Image = ((System.Drawing.Image)(resources.GetObject("uiBtnOk.Image")));
            this.uiBtnOk.IsMouseOver = false;
            this.uiBtnOk.Location = new System.Drawing.Point(20, 274);
            this.uiBtnOk.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.uiBtnOk.Name = "uiBtnOk";
            this.uiBtnOk.Size = new System.Drawing.Size(60, 60);
            this.uiBtnOk.TabIndex = 21;
            this.uiBtnOk.TabStop = false;
            this.uiBtnOk.Text = "Ok";
            this.uiBtnOk.Click += new System.EventHandler(this.uiBtnOk_Click);
            // 
            // uiBtnCancel
            // 
            this.uiBtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uiBtnCancel.BackColor = System.Drawing.Color.Transparent;
            this.uiBtnCancel.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.uiBtnCancel.DisplayText = "Cancel";
            this.uiBtnCancel.DisplayTextBrush = System.Drawing.Color.White;
            this.uiBtnCancel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.uiBtnCancel.Image = ((System.Drawing.Image)(resources.GetObject("uiBtnCancel.Image")));
            this.uiBtnCancel.IsMouseOver = false;
            this.uiBtnCancel.Location = new System.Drawing.Point(80, 274);
            this.uiBtnCancel.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.uiBtnCancel.Name = "uiBtnCancel";
            this.uiBtnCancel.Size = new System.Drawing.Size(60, 60);
            this.uiBtnCancel.TabIndex = 22;
            this.uiBtnCancel.TabStop = false;
            this.uiBtnCancel.Text = "Cancel";
            this.uiBtnCancel.Click += new System.EventHandler(this.uiBtnCancel_Click);
            // 
            // dgvPackageSources
            // 
            this.dgvPackageSources.AllowUserToResizeRows = false;
            this.dgvPackageSources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPackageSources.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPackageSources.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPackageSources.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.enabled,
            this.packageName,
            this.packageSource});
            this.dgvPackageSources.DataBindings.Add(new System.Windows.Forms.Binding("DataSource", this, "PackageSourceDT", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.dgvPackageSources.Location = new System.Drawing.Point(17, 61);
            this.dgvPackageSources.Name = "dgvPackageSources";
            this.dgvPackageSources.RowHeadersWidth = 51;
            this.dgvPackageSources.RowTemplate.Height = 24;
            this.dgvPackageSources.Size = new System.Drawing.Size(861, 204);
            this.dgvPackageSources.TabIndex = 28;
            // 
            // enabled
            // 
            this.enabled.DataPropertyName = "Enabled";
            this.enabled.FillWeight = 30F;
            this.enabled.HeaderText = "Enabled";
            this.enabled.MinimumWidth = 6;
            this.enabled.Name = "enabled";
            // 
            // packageName
            // 
            this.packageName.DataPropertyName = "Package Name";
            this.packageName.FillWeight = 50F;
            this.packageName.HeaderText = "Package Name";
            this.packageName.MinimumWidth = 6;
            this.packageName.Name = "packageName";
            this.packageName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.packageName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // packageSource
            // 
            this.packageSource.DataPropertyName = "Package Source";
            this.packageSource.HeaderText = "Package Source";
            this.packageSource.MinimumWidth = 6;
            this.packageSource.Name = "packageSource";
            this.packageSource.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.packageSource.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // frmAddPackageSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 350);
            this.Controls.Add(this.dgvPackageSources);
            this.Controls.Add(this.uiBtnOk);
            this.Controls.Add(this.uiBtnCancel);
            this.Controls.Add(this.lblHeader);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MinimumSize = new System.Drawing.Size(627, 330);
            this.Name = "frmAddPackageSource";
            this.Text = "Add Package Source";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmAddElement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.uiBtnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiBtnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPackageSources)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblHeader;
        private OpenBots.Core.UI.Controls.UIPictureButton uiBtnOk;
        private OpenBots.Core.UI.Controls.UIPictureButton uiBtnCancel;
        private System.Windows.Forms.DataGridView dgvPackageSources;
        private DataGridViewCheckBoxColumn enabled;
        private DataGridViewTextBoxColumn packageName;
        private DataGridViewTextBoxColumn packageSource;
    }
}