using System.Windows.Forms;

namespace OpenBots.Core.UI.Controls
{
    public class UIPictureBox : PictureBox
    {
        private string _encodedimage;
        public string EncodedImage
        {
            get
            {
                return _encodedimage;
            }
            set
            {
                _encodedimage = value;
            }
        }
    }
}
