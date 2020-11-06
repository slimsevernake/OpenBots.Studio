using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace OpenBots.UI.CustomControls.CustomUIControls
{
    public partial class UIListBoxItem : UserControl
    {
        private Timer _tmrMouseLeave;
        internal Timer tmrMouseLeave
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _tmrMouseLeave;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_tmrMouseLeave != null)
                {
                    _tmrMouseLeave.Tick -= tmrMouseLeave_Tick;
                }

                _tmrMouseLeave = value;
                if (_tmrMouseLeave != null)
                {
                    _tmrMouseLeave.Tick += tmrMouseLeave_Tick;
                }
            }
        }       

        public string Id { get; set; }

        private string _title = "[Title]";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Refresh();
            }
        }

        private string _description = "[Description]";
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                Refresh();
            }
        }        

        public string Version
        {
            get
            {
                return lblVersion.Text;
            }
            set
            {
                lblVersion.Text = value;
            }
        }

        private bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                Refresh();
            }
        }

        public int Rating
        {
            get
            {
                return RatingBar1.Stars;
            }
            set
            {
                RatingBar1.Stars = value;
            }
        }

        private Image _image;
        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                Refresh();
            }
        }

        private enum MouseCapture
        {
            Outside,
            Inside
        }
        private enum ButtonState
        {
            ButtonUp,
            ButtonDown,
            Disabled
        }

        private ButtonState _bState;
        private MouseCapture _bMouse;

        public event SelectionChangedEventHandler SelectionChanged;
        public delegate void SelectionChangedEventHandler(object sender);

        public UIListBoxItem()
        {
            InitializeComponent();
            tmrMouseLeave = new Timer() { Interval = 10 };
        }

        private void UIListBoxItem_MouseClick(object sender, MouseEventArgs e)
        {
            if (Selected == false)
            {
                Selected = true;
                SelectionChanged?.Invoke(this);
            }
        }

        private void UIListBoxItem_MouseUp(object sender, MouseEventArgs e)
        {
            _bState = ButtonState.ButtonUp;
            Refresh();
        }

        private void UIListBoxItem_MouseEnter(object sender, EventArgs e)
        {
            _bMouse = MouseCapture.Inside;
            tmrMouseLeave.Start();
            Refresh();
        }

        private void UIListBoxItem_MouseDown(object sender, MouseEventArgs e)
        {
            _bState = ButtonState.ButtonDown;
            Refresh();
        }        

        private void tmrMouseLeave_Tick(object sender, EventArgs e)
        {
            try
            {
                var scrPT = MousePosition;
                Point ctlPT = PointToClient(scrPT);
                // 
                if (ctlPT.X < 0 | ctlPT.Y < 0 | ctlPT.X > Width | ctlPT.Y > Height)
                {
                    // Stop timer
                    tmrMouseLeave.Stop();
                    _bMouse = MouseCapture.Outside;
                    Refresh();
                }
                else
                    _bMouse = MouseCapture.Inside;
            }
            catch (Exception)
            {
                //item has already been disposed
            }
            
        }

        private void Paint_DrawBackground(Graphics gfx)
        {
            // 
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // /// Build a rounded rectangle
            GraphicsPath p = new GraphicsPath();
            const int Roundness = 5;
            p.StartFigure();
            p.AddArc(new Rectangle(rect.Left, rect.Top, Roundness, Roundness), 180, 90);
            p.AddLine(rect.Left + Roundness, 0, rect.Right - Roundness, 0);
            p.AddArc(new Rectangle(rect.Right - Roundness, 0, Roundness, Roundness), -90, 90);
            p.AddLine(rect.Right, Roundness, rect.Right, rect.Bottom - Roundness);
            p.AddArc(new Rectangle(rect.Right - Roundness, rect.Bottom - Roundness, Roundness, Roundness), 0, 90);
            p.AddLine(rect.Right - Roundness, rect.Bottom, rect.Left + Roundness, rect.Bottom);
            p.AddArc(new Rectangle(rect.Left, rect.Height - Roundness, Roundness, Roundness), 90, 90);
            p.CloseFigure();

            // /// Draw the background ///
            Color[] ColorScheme = null;
            SolidBrush brdr/* TODO Change to default(_) if this is not a reference type */;

            if (_bState == ButtonState.Disabled)
            {
                // normal
                brdr = UIListBoxItemColorSchemes.DisabledBorder;
                ColorScheme = UIListBoxItemColorSchemes.DisabledAllColor;
            }
            else if (_selected)
            {
                // Selected
                brdr = UIListBoxItemColorSchemes.SelectedBorder;

                if (_bState == ButtonState.ButtonUp & _bMouse == MouseCapture.Outside)
                    // normal
                    ColorScheme = UIListBoxItemColorSchemes.SelectedNormal;
                else if (_bState == ButtonState.ButtonUp & _bMouse == MouseCapture.Inside)
                    // hover 
                    ColorScheme = UIListBoxItemColorSchemes.SelectedHover;
                else if (_bState == ButtonState.ButtonDown & _bMouse == MouseCapture.Outside)
                    // no one cares!
                    return;
                else if (_bState == ButtonState.ButtonDown & _bMouse == MouseCapture.Inside)
                    // pressed
                    ColorScheme = UIListBoxItemColorSchemes.SelectedPressed;
            }
            else
            {
                // Not selected
                brdr = UIListBoxItemColorSchemes.UnSelectedBorder;

                if (_bState == ButtonState.ButtonUp & _bMouse == MouseCapture.Outside)
                {
                    // normal
                    brdr = UIListBoxItemColorSchemes.DisabledBorder;
                    ColorScheme = UIListBoxItemColorSchemes.UnSelectedNormal;
                }
                else if (_bState == ButtonState.ButtonUp & _bMouse == MouseCapture.Inside)
                    // hover 
                    ColorScheme = UIListBoxItemColorSchemes.UnSelectedHover;
                else if (_bState == ButtonState.ButtonDown & _bMouse == MouseCapture.Outside)
                    // no one cares!
                    return;
                else if (_bState == ButtonState.ButtonDown & _bMouse == MouseCapture.Inside)
                    // pressed
                    ColorScheme = UIListBoxItemColorSchemes.UnSelectedPressed;
            }

            // Draw
            LinearGradientBrush b = new LinearGradientBrush(rect, Color.White, Color.Black, LinearGradientMode.Vertical);
            ColorBlend blend = new ColorBlend();
            blend.Colors = ColorScheme;
            blend.Positions = new float[] { 0.0F, 0.1F, 0.9F, 0.95F, 1.0F };
            b.InterpolationColors = blend;
            gfx.FillPath(b, p);

            // // Draw border
            gfx.DrawPath(new Pen(brdr), p);

            // // Draw bottom border if Normal state (not hovered)
            if (_bMouse == MouseCapture.Outside)
            {
                rect = new Rectangle(rect.Left, Height - 1, rect.Width, 1);
                b = new LinearGradientBrush(rect, Color.Blue, Color.Yellow, LinearGradientMode.Horizontal);
                blend = new ColorBlend();
                blend.Colors = new Color[] { Color.White, Color.LightGray, Color.White };
                blend.Positions = new float[] { 0.0F, 0.5F, 1.0F };
                b.InterpolationColors = blend;
                // 
                gfx.FillRectangle(b, rect);
            }
        }

        private void Paint_DrawButton(Graphics gfx)
        {
            Font fnt;
            SizeF sz;
            RectangleF layoutRect;
            StringFormat SF = new StringFormat() { Trimming = StringTrimming.EllipsisCharacter };
            Rectangle workingRect = new Rectangle(40, 0, RatingBar1.Left - 40 - 6, Height);

            // Draw title name
            fnt = new Font("Segoe UI Bold", 14);
            sz = gfx.MeasureString(_title, fnt);
            layoutRect = new RectangleF(40, 0, workingRect.Width, sz.Height);
            gfx.DrawString(_title, fnt, Brushes.Black, layoutRect, SF);

            // Draw description name
            fnt = new Font("Segoe UI", 10);
            sz = gfx.MeasureString(_description, fnt);
            layoutRect = new RectangleF(42, 30, workingRect.Width, sz.Height);
            gfx.DrawString(_description, fnt, Brushes.Black, layoutRect, SF);

            // Icon Image
            if (_image != null)
                gfx.DrawImage(_image, 7, 7, workingRect.Left - 10, workingRect.Left - 10);
            else
                gfx.DrawImage(ImageList1.Images[0], 7, 7, workingRect.Left - 10, workingRect.Left - 10);
        }

        private void UIListBoxItem_Paint(object sender, PaintEventArgs e)
        {
            var gfx = e.Graphics;

            Paint_DrawBackground(gfx);
            Paint_DrawButton(gfx);
        }

    }

}