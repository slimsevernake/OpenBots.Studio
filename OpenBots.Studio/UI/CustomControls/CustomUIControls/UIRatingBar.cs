using System;
using System.Windows.Forms;

namespace OpenBots.UI.CustomControls.CustomUIControls
{
    public partial class UIRatingBar : UserControl
    {
        public UIRatingBar()
        {
            InitializeComponent();
        }

        private int _stars = 3;
        public int Stars
        {
            get
            {
                return _stars;
            }
            set
            {
                _stars = value;
                SetupStars();
            }
        }

        private void SetupStars()
        {
            Star1.Image = ImageList1.Images[_stars >= 1 ? "full" : "empty"];
            Star2.Image = ImageList1.Images[_stars >= 2 ? "full" : "empty"];
            Star3.Image = ImageList1.Images[_stars >= 3 ? "full" : "empty"];
            Star4.Image = ImageList1.Images[_stars >= 4 ? "full" : "empty"];
            Star5.Image = ImageList1.Images[_stars >= 5 ? "full" : "empty"];
        }

        private void Star1_Click(object sender, EventArgs e)
        {
            _stars = 1;
            SetupStars();
        }

        private void Star2_Click(object sender, EventArgs e)
        {
            _stars = 2;
            SetupStars();
        }

        private void Star3_Click(object sender, EventArgs e)
        {
            _stars = 3;
            SetupStars();
        }

        private void Star4_Click(object sender, EventArgs e)
        {
            _stars = 4;
            SetupStars();
        }

        private void Star5_Click(object sender, EventArgs e)
        {
            _stars = 5;
            SetupStars();
        }
    }
}
