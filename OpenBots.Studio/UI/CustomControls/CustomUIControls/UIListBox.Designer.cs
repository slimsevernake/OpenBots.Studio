namespace OpenBots.UI.CustomControls.CustomUIControls
{
    partial class UIListBox
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
            this.flpListBox = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flpListBox
            // 
            this.flpListBox.AutoScroll = true;
            this.flpListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpListBox.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpListBox.Location = new System.Drawing.Point(0, 0);
            this.flpListBox.Margin = new System.Windows.Forms.Padding(0);
            this.flpListBox.Name = "flpListBox";
            this.flpListBox.Size = new System.Drawing.Size(825, 481);
            this.flpListBox.TabIndex = 0;
            this.flpListBox.WrapContents = false;
            // 
            // UIListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.flpListBox);
            this.Name = "UIListBox";
            this.Size = new System.Drawing.Size(825, 481);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpListBox;
    }
}
