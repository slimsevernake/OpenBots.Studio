using OpenBots.UI.CustomControls.CustomUIControls;
using static OpenBots.UI.CustomControls.CustomUIControls.UIListBox;

namespace OpenBots.UI.Forms
{
    partial class frmGalleryProjectManager
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
            OpenBots.Core.Utilities.FormsUtilities.Theme theme1 = new OpenBots.Core.Utilities.FormsUtilities.Theme();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGalleryProjectManager));
            OpenBots.Core.Utilities.FormsUtilities.Theme theme2 = new OpenBots.Core.Utilities.FormsUtilities.Theme();
            OpenBots.Core.Utilities.FormsUtilities.Theme theme3 = new OpenBots.Core.Utilities.FormsUtilities.Theme();
            OpenBots.Core.Utilities.FormsUtilities.Theme theme4 = new OpenBots.Core.Utilities.FormsUtilities.Theme();
            this.tlpProjectLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lbxGalleryProjects = new OpenBots.UI.CustomControls.CustomUIControls.UIListBox();
            this.pnlProjectVersion = new OpenBots.UI.CustomControls.CustomUIControls.UIPanel();
            this.pbxOBStudio = new System.Windows.Forms.PictureBox();
            this.cbxVersion = new System.Windows.Forms.ComboBox();
            this.lblVersionTitleLabel = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlProjectSearch = new OpenBots.UI.CustomControls.CustomUIControls.UIPanel();
            this.pbxOBGallery = new System.Windows.Forms.PictureBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.lblGalleryProjects = new System.Windows.Forms.Label();
            this.txtSampleSearch = new System.Windows.Forms.TextBox();
            this.pnlProjectDetails = new OpenBots.UI.CustomControls.CustomUIControls.UIPanel();
            this.lblPublishDate = new System.Windows.Forms.Label();
            this.lblPublishDateLabel = new System.Windows.Forms.Label();
            this.lvDependencies = new System.Windows.Forms.ListView();
            this.DependencyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Range = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblAuthors = new System.Windows.Forms.Label();
            this.lblDownloads = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.llblProjectURL = new System.Windows.Forms.LinkLabel();
            this.llblLicenseURL = new System.Windows.Forms.LinkLabel();
            this.lblProjectURLLabel = new System.Windows.Forms.Label();
            this.lblLicenseURLLabel = new System.Windows.Forms.Label();
            this.lblDependenciesLabel = new System.Windows.Forms.Label();
            this.lblDescriptionLabel = new System.Windows.Forms.Label();
            this.lblAuthorsLabel = new System.Windows.Forms.Label();
            this.lblVersionLabel = new System.Windows.Forms.Label();
            this.lblDownloadsLabel = new System.Windows.Forms.Label();
            this.pnlFinishButtons = new OpenBots.UI.CustomControls.CustomUIControls.UIPanel();
            this.uiBtnOpen = new OpenBots.Core.UI.Controls.UIPictureButton();
            this.uiBtnCancel = new OpenBots.Core.UI.Controls.UIPictureButton();
            this.lblError = new System.Windows.Forms.Label();
            this.tlpProjectLayout.SuspendLayout();
            this.pnlProjectVersion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxOBStudio)).BeginInit();
            this.pnlProjectSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxOBGallery)).BeginInit();
            this.pnlProjectDetails.SuspendLayout();
            this.pnlFinishButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiBtnOpen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiBtnCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpProjectLayout
            // 
            this.tlpProjectLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.tlpProjectLayout.ColumnCount = 2;
            this.tlpProjectLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProjectLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProjectLayout.Controls.Add(this.lblError, 0, 2);
            this.tlpProjectLayout.Controls.Add(this.lbxGalleryProjects, 0, 1);
            this.tlpProjectLayout.Controls.Add(this.pnlProjectVersion, 1, 0);
            this.tlpProjectLayout.Controls.Add(this.pnlProjectSearch, 0, 0);
            this.tlpProjectLayout.Controls.Add(this.pnlProjectDetails, 1, 1);
            this.tlpProjectLayout.Controls.Add(this.pnlFinishButtons, 1, 2);
            this.tlpProjectLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProjectLayout.Location = new System.Drawing.Point(0, 0);
            this.tlpProjectLayout.Name = "tlpProjectLayout";
            this.tlpProjectLayout.RowCount = 3;
            this.tlpProjectLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpProjectLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProjectLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tlpProjectLayout.Size = new System.Drawing.Size(1057, 859);
            this.tlpProjectLayout.TabIndex = 36;
            // 
            // lbxGalleryProjects
            // 
            this.lbxGalleryProjects.AutoScroll = true;
            this.lbxGalleryProjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbxGalleryProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxGalleryProjects.Location = new System.Drawing.Point(3, 113);
            this.lbxGalleryProjects.Name = "lbxGalleryProjects";
            this.lbxGalleryProjects.Size = new System.Drawing.Size(522, 661);
            this.lbxGalleryProjects.TabIndex = 36;
            this.lbxGalleryProjects.ItemClick += new OpenBots.UI.CustomControls.CustomUIControls.UIListBox.ItemClickEventHandler(this.lbxGalleryProjects_ItemClick);
            // 
            // pnlProjectVersion
            // 
            this.pnlProjectVersion.Controls.Add(this.pbxOBStudio);
            this.pnlProjectVersion.Controls.Add(this.cbxVersion);
            this.pnlProjectVersion.Controls.Add(this.lblVersionTitleLabel);
            this.pnlProjectVersion.Controls.Add(this.lblTitle);
            this.pnlProjectVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProjectVersion.Location = new System.Drawing.Point(531, 3);
            this.pnlProjectVersion.Name = "pnlProjectVersion";
            this.pnlProjectVersion.Size = new System.Drawing.Size(523, 104);
            this.pnlProjectVersion.TabIndex = 2;
            theme1.BgGradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            theme1.BgGradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.pnlProjectVersion.Theme = theme1;
            // 
            // pbxOBStudio
            // 
            this.pbxOBStudio.Image = ((System.Drawing.Image)(resources.GetObject("pbxOBStudio.Image")));
            this.pbxOBStudio.Location = new System.Drawing.Point(14, 13);
            this.pbxOBStudio.Name = "pbxOBStudio";
            this.pbxOBStudio.Size = new System.Drawing.Size(50, 50);
            this.pbxOBStudio.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxOBStudio.TabIndex = 43;
            this.pbxOBStudio.TabStop = false;
            // 
            // cbxVersion
            // 
            this.cbxVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxVersion.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cbxVersion.ForeColor = System.Drawing.Color.SteelBlue;
            this.cbxVersion.FormattingEnabled = true;
            this.cbxVersion.Location = new System.Drawing.Point(111, 65);
            this.cbxVersion.Margin = new System.Windows.Forms.Padding(4);
            this.cbxVersion.Name = "cbxVersion";
            this.cbxVersion.Size = new System.Drawing.Size(412, 36);
            this.cbxVersion.TabIndex = 42;
            this.cbxVersion.SelectedIndexChanged += new System.EventHandler(this.cbxVersion_SelectedIndexChanged);
            // 
            // lblVersionTitleLabel
            // 
            this.lblVersionTitleLabel.AutoSize = true;
            this.lblVersionTitleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 14F);
            this.lblVersionTitleLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblVersionTitleLabel.Location = new System.Drawing.Point(8, 69);
            this.lblVersionTitleLabel.Name = "lblVersionTitleLabel";
            this.lblVersionTitleLabel.Size = new System.Drawing.Size(100, 32);
            this.lblVersionTitleLabel.TabIndex = 2;
            this.lblVersionTitleLabel.Text = "Version:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 20F);
            this.lblTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTitle.Location = new System.Drawing.Point(70, 17);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(75, 46);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "title";
            // 
            // pnlProjectSearch
            // 
            this.pnlProjectSearch.Controls.Add(this.pbxOBGallery);
            this.pnlProjectSearch.Controls.Add(this.lblSearch);
            this.pnlProjectSearch.Controls.Add(this.lblGalleryProjects);
            this.pnlProjectSearch.Controls.Add(this.txtSampleSearch);
            this.pnlProjectSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProjectSearch.Location = new System.Drawing.Point(3, 3);
            this.pnlProjectSearch.Name = "pnlProjectSearch";
            this.pnlProjectSearch.Size = new System.Drawing.Size(522, 104);
            this.pnlProjectSearch.TabIndex = 0;
            theme2.BgGradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            theme2.BgGradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.pnlProjectSearch.Theme = theme2;
            // 
            // pbxOBGallery
            // 
            this.pbxOBGallery.Image = ((System.Drawing.Image)(resources.GetObject("pbxOBGallery.Image")));
            this.pbxOBGallery.Location = new System.Drawing.Point(13, 13);
            this.pbxOBGallery.Name = "pbxOBGallery";
            this.pbxOBGallery.Size = new System.Drawing.Size(50, 50);
            this.pbxOBGallery.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxOBGallery.TabIndex = 44;
            this.pbxOBGallery.TabStop = false;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 14F);
            this.lblSearch.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblSearch.Location = new System.Drawing.Point(7, 71);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(94, 32);
            this.lblSearch.TabIndex = 35;
            this.lblSearch.Text = "Search:";
            // 
            // lblGalleryProjects
            // 
            this.lblGalleryProjects.AutoSize = true;
            this.lblGalleryProjects.BackColor = System.Drawing.Color.Transparent;
            this.lblGalleryProjects.Font = new System.Drawing.Font("Segoe UI Semilight", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGalleryProjects.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblGalleryProjects.Location = new System.Drawing.Point(70, 9);
            this.lblGalleryProjects.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGalleryProjects.Name = "lblGalleryProjects";
            this.lblGalleryProjects.Size = new System.Drawing.Size(287, 54);
            this.lblGalleryProjects.TabIndex = 33;
            this.lblGalleryProjects.Text = "gallery projects";
            // 
            // txtSampleSearch
            // 
            this.txtSampleSearch.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtSampleSearch.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtSampleSearch.Location = new System.Drawing.Point(101, 69);
            this.txtSampleSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtSampleSearch.Name = "txtSampleSearch";
            this.txtSampleSearch.Size = new System.Drawing.Size(420, 34);
            this.txtSampleSearch.TabIndex = 34;
            this.txtSampleSearch.TextChanged += new System.EventHandler(this.txtSampleSearch_TextChanged);
            // 
            // pnlProjectDetails
            // 
            this.pnlProjectDetails.Controls.Add(this.lblPublishDate);
            this.pnlProjectDetails.Controls.Add(this.lblPublishDateLabel);
            this.pnlProjectDetails.Controls.Add(this.lvDependencies);
            this.pnlProjectDetails.Controls.Add(this.lblVersion);
            this.pnlProjectDetails.Controls.Add(this.lblAuthors);
            this.pnlProjectDetails.Controls.Add(this.lblDownloads);
            this.pnlProjectDetails.Controls.Add(this.lblDescription);
            this.pnlProjectDetails.Controls.Add(this.llblProjectURL);
            this.pnlProjectDetails.Controls.Add(this.llblLicenseURL);
            this.pnlProjectDetails.Controls.Add(this.lblProjectURLLabel);
            this.pnlProjectDetails.Controls.Add(this.lblLicenseURLLabel);
            this.pnlProjectDetails.Controls.Add(this.lblDependenciesLabel);
            this.pnlProjectDetails.Controls.Add(this.lblDescriptionLabel);
            this.pnlProjectDetails.Controls.Add(this.lblAuthorsLabel);
            this.pnlProjectDetails.Controls.Add(this.lblVersionLabel);
            this.pnlProjectDetails.Controls.Add(this.lblDownloadsLabel);
            this.pnlProjectDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProjectDetails.Location = new System.Drawing.Point(531, 113);
            this.pnlProjectDetails.Name = "pnlProjectDetails";
            this.pnlProjectDetails.Size = new System.Drawing.Size(523, 661);
            this.pnlProjectDetails.TabIndex = 37;
            theme3.BgGradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            theme3.BgGradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.pnlProjectDetails.Theme = theme3;
            // 
            // lblPublishDate
            // 
            this.lblPublishDate.AutoSize = true;
            this.lblPublishDate.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.lblPublishDate.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblPublishDate.Location = new System.Drawing.Point(161, 305);
            this.lblPublishDate.Name = "lblPublishDate";
            this.lblPublishDate.Size = new System.Drawing.Size(114, 28);
            this.lblPublishDate.TabIndex = 60;
            this.lblPublishDate.Text = "publish date";
            // 
            // lblPublishDateLabel
            // 
            this.lblPublishDateLabel.AutoSize = true;
            this.lblPublishDateLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblPublishDateLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblPublishDateLabel.Location = new System.Drawing.Point(9, 305);
            this.lblPublishDateLabel.Name = "lblPublishDateLabel";
            this.lblPublishDateLabel.Size = new System.Drawing.Size(132, 28);
            this.lblPublishDateLabel.TabIndex = 59;
            this.lblPublishDateLabel.Text = "Publish Date:";
            // 
            // lvDependencies
            // 
            this.lvDependencies.AllowColumnReorder = true;
            this.lvDependencies.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.lvDependencies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DependencyName,
            this.Range});
            this.lvDependencies.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lvDependencies.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lvDependencies.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lvDependencies.FullRowSelect = true;
            this.lvDependencies.GridLines = true;
            this.lvDependencies.HideSelection = false;
            this.lvDependencies.Location = new System.Drawing.Point(0, 366);
            this.lvDependencies.Name = "lvDependencies";
            this.lvDependencies.Size = new System.Drawing.Size(523, 295);
            this.lvDependencies.TabIndex = 58;
            this.lvDependencies.UseCompatibleStateImageBehavior = false;
            this.lvDependencies.View = System.Windows.Forms.View.Details;
            // 
            // DependencyName
            // 
            this.DependencyName.Text = "Name";
            this.DependencyName.Width = 312;
            // 
            // Range
            // 
            this.Range.Text = "Range";
            this.Range.Width = 207;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.lblVersion.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblVersion.Location = new System.Drawing.Point(161, 165);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(72, 28);
            this.lblVersion.TabIndex = 57;
            this.lblVersion.Text = "version";
            // 
            // lblAuthors
            // 
            this.lblAuthors.AutoSize = true;
            this.lblAuthors.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.lblAuthors.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblAuthors.Location = new System.Drawing.Point(161, 193);
            this.lblAuthors.Name = "lblAuthors";
            this.lblAuthors.Size = new System.Drawing.Size(76, 28);
            this.lblAuthors.TabIndex = 56;
            this.lblAuthors.Text = "authors";
            // 
            // lblDownloads
            // 
            this.lblDownloads.AutoSize = true;
            this.lblDownloads.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.lblDownloads.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDownloads.Location = new System.Drawing.Point(161, 277);
            this.lblDownloads.Name = "lblDownloads";
            this.lblDownloads.Size = new System.Drawing.Size(103, 28);
            this.lblDownloads.TabIndex = 55;
            this.lblDownloads.Text = "downloads";
            // 
            // lblDescription
            // 
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.lblDescription.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDescription.Location = new System.Drawing.Point(9, 30);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(505, 135);
            this.lblDescription.TabIndex = 53;
            this.lblDescription.Text = "description";
            // 
            // llblProjectURL
            // 
            this.llblProjectURL.ActiveLinkColor = System.Drawing.Color.Coral;
            this.llblProjectURL.AutoSize = true;
            this.llblProjectURL.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.llblProjectURL.LinkColor = System.Drawing.Color.SteelBlue;
            this.llblProjectURL.Location = new System.Drawing.Point(161, 249);
            this.llblProjectURL.Name = "llblProjectURL";
            this.llblProjectURL.Size = new System.Drawing.Size(219, 28);
            this.llblProjectURL.TabIndex = 52;
            this.llblProjectURL.TabStop = true;
            this.llblProjectURL.Text = "View Project Information";
            this.llblProjectURL.VisitedLinkColor = System.Drawing.Color.Coral;
            this.llblProjectURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblProjectURL_LinkClicked);
            // 
            // llblLicenseURL
            // 
            this.llblLicenseURL.ActiveLinkColor = System.Drawing.Color.Coral;
            this.llblLicenseURL.AutoSize = true;
            this.llblLicenseURL.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.llblLicenseURL.LinkColor = System.Drawing.Color.SteelBlue;
            this.llblLicenseURL.Location = new System.Drawing.Point(161, 221);
            this.llblLicenseURL.Name = "llblLicenseURL";
            this.llblLicenseURL.Size = new System.Drawing.Size(222, 28);
            this.llblLicenseURL.TabIndex = 51;
            this.llblLicenseURL.TabStop = true;
            this.llblLicenseURL.Text = "View License Information";
            this.llblLicenseURL.VisitedLinkColor = System.Drawing.Color.Coral;
            this.llblLicenseURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblLicense_LinkClicked);
            // 
            // lblProjectURLLabel
            // 
            this.lblProjectURLLabel.AutoSize = true;
            this.lblProjectURLLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblProjectURLLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblProjectURLLabel.Location = new System.Drawing.Point(9, 249);
            this.lblProjectURLLabel.Name = "lblProjectURLLabel";
            this.lblProjectURLLabel.Size = new System.Drawing.Size(122, 28);
            this.lblProjectURLLabel.TabIndex = 50;
            this.lblProjectURLLabel.Text = "Project URL:";
            // 
            // lblLicenseURLLabel
            // 
            this.lblLicenseURLLabel.AutoSize = true;
            this.lblLicenseURLLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblLicenseURLLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblLicenseURLLabel.Location = new System.Drawing.Point(9, 221);
            this.lblLicenseURLLabel.Name = "lblLicenseURLLabel";
            this.lblLicenseURLLabel.Size = new System.Drawing.Size(126, 28);
            this.lblLicenseURLLabel.TabIndex = 49;
            this.lblLicenseURLLabel.Text = "License URL:";
            // 
            // lblDependenciesLabel
            // 
            this.lblDependenciesLabel.AutoSize = true;
            this.lblDependenciesLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblDependenciesLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDependenciesLabel.Location = new System.Drawing.Point(9, 333);
            this.lblDependenciesLabel.Name = "lblDependenciesLabel";
            this.lblDependenciesLabel.Size = new System.Drawing.Size(146, 28);
            this.lblDependenciesLabel.TabIndex = 48;
            this.lblDependenciesLabel.Text = "Dependencies:";
            // 
            // lblDescriptionLabel
            // 
            this.lblDescriptionLabel.AutoSize = true;
            this.lblDescriptionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblDescriptionLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDescriptionLabel.Location = new System.Drawing.Point(9, 2);
            this.lblDescriptionLabel.Name = "lblDescriptionLabel";
            this.lblDescriptionLabel.Size = new System.Drawing.Size(120, 28);
            this.lblDescriptionLabel.TabIndex = 46;
            this.lblDescriptionLabel.Text = "Description:";
            // 
            // lblAuthorsLabel
            // 
            this.lblAuthorsLabel.AutoSize = true;
            this.lblAuthorsLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblAuthorsLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblAuthorsLabel.Location = new System.Drawing.Point(9, 193);
            this.lblAuthorsLabel.Name = "lblAuthorsLabel";
            this.lblAuthorsLabel.Size = new System.Drawing.Size(103, 28);
            this.lblAuthorsLabel.TabIndex = 47;
            this.lblAuthorsLabel.Text = "Author(s):";
            // 
            // lblVersionLabel
            // 
            this.lblVersionLabel.AutoSize = true;
            this.lblVersionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblVersionLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblVersionLabel.Location = new System.Drawing.Point(9, 165);
            this.lblVersionLabel.Name = "lblVersionLabel";
            this.lblVersionLabel.Size = new System.Drawing.Size(85, 28);
            this.lblVersionLabel.TabIndex = 44;
            this.lblVersionLabel.Text = "Version:";
            // 
            // lblDownloadsLabel
            // 
            this.lblDownloadsLabel.AutoSize = true;
            this.lblDownloadsLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblDownloadsLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDownloadsLabel.Location = new System.Drawing.Point(9, 277);
            this.lblDownloadsLabel.Name = "lblDownloadsLabel";
            this.lblDownloadsLabel.Size = new System.Drawing.Size(118, 28);
            this.lblDownloadsLabel.TabIndex = 45;
            this.lblDownloadsLabel.Text = "Downloads:";
            // 
            // pnlFinishButtons
            // 
            this.pnlFinishButtons.Controls.Add(this.uiBtnOpen);
            this.pnlFinishButtons.Controls.Add(this.uiBtnCancel);
            this.pnlFinishButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFinishButtons.Location = new System.Drawing.Point(531, 780);
            this.pnlFinishButtons.Name = "pnlFinishButtons";
            this.pnlFinishButtons.Size = new System.Drawing.Size(523, 76);
            this.pnlFinishButtons.TabIndex = 38;
            theme4.BgGradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            theme4.BgGradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.pnlFinishButtons.Theme = theme4;
            // 
            // uiBtnOpen
            // 
            this.uiBtnOpen.BackColor = System.Drawing.Color.Transparent;
            this.uiBtnOpen.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.uiBtnOpen.DisplayText = "Open";
            this.uiBtnOpen.DisplayTextBrush = System.Drawing.Color.White;
            this.uiBtnOpen.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.uiBtnOpen.Image = ((System.Drawing.Image)(resources.GetObject("uiBtnOpen.Image")));
            this.uiBtnOpen.IsMouseOver = false;
            this.uiBtnOpen.Location = new System.Drawing.Point(395, 12);
            this.uiBtnOpen.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.uiBtnOpen.Name = "uiBtnOpen";
            this.uiBtnOpen.Size = new System.Drawing.Size(60, 58);
            this.uiBtnOpen.TabIndex = 39;
            this.uiBtnOpen.TabStop = false;
            this.uiBtnOpen.Text = "Open";
            this.uiBtnOpen.Click += new System.EventHandler(this.uiBtnOpen_Click);
            // 
            // uiBtnCancel
            // 
            this.uiBtnCancel.BackColor = System.Drawing.Color.Transparent;
            this.uiBtnCancel.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.uiBtnCancel.DisplayText = "Cancel";
            this.uiBtnCancel.DisplayTextBrush = System.Drawing.Color.White;
            this.uiBtnCancel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.uiBtnCancel.Image = ((System.Drawing.Image)(resources.GetObject("uiBtnCancel.Image")));
            this.uiBtnCancel.IsMouseOver = false;
            this.uiBtnCancel.Location = new System.Drawing.Point(455, 12);
            this.uiBtnCancel.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.uiBtnCancel.Name = "uiBtnCancel";
            this.uiBtnCancel.Size = new System.Drawing.Size(60, 58);
            this.uiBtnCancel.TabIndex = 40;
            this.uiBtnCancel.TabStop = false;
            this.uiBtnCancel.Text = "Cancel";
            this.uiBtnCancel.Click += new System.EventHandler(this.uiBtnCancel_Click);
            // 
            // lblError
            // 
            this.lblError.BackColor = System.Drawing.Color.Transparent;
            this.lblError.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblError.Location = new System.Drawing.Point(4, 777);
            this.lblError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(520, 28);
            this.lblError.TabIndex = 39;
            // 
            // frmGalleryProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(1057, 859);
            this.Controls.Add(this.tlpProjectLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmGalleryProject";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Gallery Project Manager";
            this.Load += new System.EventHandler(this.frmGalleryProject_LoadAsync);
            this.tlpProjectLayout.ResumeLayout(false);
            this.pnlProjectVersion.ResumeLayout(false);
            this.pnlProjectVersion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxOBStudio)).EndInit();
            this.pnlProjectSearch.ResumeLayout(false);
            this.pnlProjectSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxOBGallery)).EndInit();
            this.pnlProjectDetails.ResumeLayout(false);
            this.pnlProjectDetails.PerformLayout();
            this.pnlFinishButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiBtnOpen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiBtnCancel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.TextBox txtSampleSearch;
        private System.Windows.Forms.Label lblGalleryProjects;
        private System.Windows.Forms.TableLayoutPanel tlpProjectLayout;
        private UIPanel pnlProjectSearch;
        private UIListBox lbxGalleryProjects;
        private UIPanel pnlProjectVersion;
        private System.Windows.Forms.Label lblDependenciesLabel;
        private System.Windows.Forms.Label lblAuthorsLabel;
        private System.Windows.Forms.Label lblDescriptionLabel;
        private System.Windows.Forms.Label lblDownloadsLabel;
        private System.Windows.Forms.Label lblVersionLabel;
        private System.Windows.Forms.ComboBox cbxVersion;
        private System.Windows.Forms.Label lblVersionTitleLabel;
        private System.Windows.Forms.Label lblTitle;
        private UIPanel pnlProjectDetails;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblAuthors;
        private System.Windows.Forms.Label lblDownloads;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.LinkLabel llblProjectURL;
        private System.Windows.Forms.LinkLabel llblLicenseURL;
        private System.Windows.Forms.Label lblProjectURLLabel;
        private System.Windows.Forms.Label lblLicenseURLLabel;
        private System.Windows.Forms.ListView lvDependencies;
        private System.Windows.Forms.ColumnHeader DependencyName;
        private System.Windows.Forms.ColumnHeader Range;
        private System.Windows.Forms.PictureBox pbxOBStudio;
        private System.Windows.Forms.Label lblPublishDate;
        private System.Windows.Forms.Label lblPublishDateLabel;
        private UIPanel pnlFinishButtons;
        private Core.UI.Controls.UIPictureButton uiBtnOpen;
        private Core.UI.Controls.UIPictureButton uiBtnCancel;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.PictureBox pbxOBGallery;
        public System.Windows.Forms.Label lblError;
    }
}