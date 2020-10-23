using OpenBots.Core.UI.Forms;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    [ComVisible(true)]
    public partial class frmHTMLBuilder : UIForm
    {
        public frmHTMLBuilder()
        {
            InitializeComponent();
        }

        private void uiBtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void uiBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void rtbHTML_TextChanged(object sender, EventArgs e)
        {
            webBrowserHTML.ScriptErrorsSuppressed = true;
            webBrowserHTML.DocumentText = rtbHTML.Text;
        }
    }
}
