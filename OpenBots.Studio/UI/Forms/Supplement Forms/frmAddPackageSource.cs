using OpenBots.Core.Script;
using OpenBots.Core.UI.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    public partial class frmAddPackageSource : UIForm
    {
        public DataTable PackageSourceDT { get; set; }

        public frmAddPackageSource()
        {
            InitializeComponent();
            PackageSourceDT = new DataTable();
            PackageSourceDT.Columns.Add("Enabled");
            PackageSourceDT.Columns.Add("Package Name");
            PackageSourceDT.Columns.Add("Package Source");
            PackageSourceDT.TableName = DateTime.Now.ToString("PackageSourceDT" + DateTime.Now.ToString("MMddyy.hhmmss"));
        }

        public frmAddPackageSource(DataTable packageSourceDT)
        {
            InitializeComponent();

            PackageSourceDT = packageSourceDT;        
        }

        private void frmAddElement_Load(object sender, EventArgs e)
        {            
        }

        private void uiBtnOk_Click(object sender, EventArgs e)
        {


            dgvDefaultValue.EndEdit();
            DialogResult = DialogResult.OK;
        }

        private void uiBtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
