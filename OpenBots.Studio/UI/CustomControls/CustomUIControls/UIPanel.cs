using OpenBots.Core.Utilities.FormsUtilities;
using System.Windows.Forms;

namespace OpenBots.UI.CustomControls.CustomUIControls
{
    public partial class UIPanel : Panel
    {
        private Theme _theme = new Theme();
        public Theme Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var brush = Theme.CreateGradient(ClientRectangle);
            e.Graphics.FillRectangle(brush, ClientRectangle);

            base.OnPaint(e);
        }
    }
}
