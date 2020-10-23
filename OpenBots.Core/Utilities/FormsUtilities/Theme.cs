using System.Drawing;
using System.Drawing.Drawing2D;

namespace OpenBots.Core.Utilities.FormsUtilities
{
    public class Theme
    {
        public Color BgGradientStartColor { get; set; } = Color.FromArgb(49, 49, 49);
        public Color BgGradientEndColor { get; set; } = Color.FromArgb(49, 49, 49);

        public LinearGradientBrush CreateGradient(Rectangle rect)
        {
            return new LinearGradientBrush(rect, BgGradientStartColor, BgGradientEndColor, 180);
        }
    }
}
