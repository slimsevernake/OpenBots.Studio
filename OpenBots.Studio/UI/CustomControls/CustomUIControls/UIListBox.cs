using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenBots.UI.CustomControls.CustomUIControls
{
    public partial class UIListBox : UserControl
    {
        public UIListBoxItem LastSelectedItem;
        public UIListBoxItem ClickedItem;
        public UIListBoxItem DoubleClickedItem;

        public event ItemClickEventHandler ItemClick;
        public delegate void ItemClickEventHandler(object sender, int Index);

        public event MouseEventHandler ItemDoubleClick;
        public delegate void MouseEventHandler(object sender, int Index);

        public UIListBox()
        {
            InitializeComponent();
            flpListBox.Layout += flpListBox_Resize;
        }

        public void Add(Control control)
        {
            flpListBox.Controls.Add(control);
            SetupAnchors();
        }

        public void Add(string id, string title, string description, string version, Image image, int rating = 3)
        {
            UIListBoxItem c = new UIListBoxItem();
            {
                var withBlock = c;
                // Assign an auto generated name
                withBlock.Id = id;
                withBlock.Name = "item" + flpListBox.Controls.Count + 1;
                withBlock.Margin = new Padding(0);
                // set properties
                withBlock.Title = title;
                withBlock.Description = description;
                withBlock.Version = version;
                withBlock.Image = image;
                withBlock.Rating = rating;
            }
            // To check when the selection is changed
            c.SelectionChanged += SelectionChanged;
            c.Click += ItemClicked;
            c.DoubleClick += ItemDoubleClicked;
            // 
            flpListBox.Controls.Add(c);
            SetupAnchors();
        }

        public void Remove(int Index)
        {
            UIListBoxItem c = (UIListBoxItem)flpListBox.Controls[Index];
            Remove(c.Name);  // call the below sub
        }

        public void Remove(string name)
        {
            // grab which control is being removed
            UIListBoxItem c = (UIListBoxItem)flpListBox.Controls[name];
            flpListBox.Controls.Remove(c);
            // remove the event hook
            c.SelectionChanged -= SelectionChanged;
            c.Click -= ItemClicked;
            c.DoubleClick -= ItemDoubleClicked;
            // now dispose off properly
            c.Dispose();
            SetupAnchors();
        }

        public void Clear()
        {
            do
            {
                if (flpListBox.Controls.Count == 0)
                    break;
                UIListBoxItem c = (UIListBoxItem)flpListBox.Controls[0];
                flpListBox.Controls.Remove(c);
                // remove the event hook
                c.SelectionChanged -= SelectionChanged;
                c.Click -= ItemClicked;
                c.DoubleClick -= ItemDoubleClicked;
                // now dispose off properly
                c.Dispose();
            }
            while (true);
            LastSelectedItem = null;
        }

        public int Count()
        {
            return flpListBox.Controls.Count;
        }

        private void SetupAnchors()
        {
            if(flpListBox.Controls.Count > 0)
            {
                for(int i = 0; i < flpListBox.Controls.Count; i++)
                {
                    Control control = flpListBox.Controls[i];
                    if (i == 0)
                    {
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        control.Width = flpListBox.Width - SystemInformation.VerticalScrollBarWidth;
                    }
                    else
                    {
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    }
                }
            }
        }

        private void flpListBox_Resize(object sender, EventArgs e)
        {
            if (flpListBox.Controls.Count > 0)
                flpListBox.Controls[0].Width = flpListBox.Width - SystemInformation.VerticalScrollBarWidth;
        }

        
        private void SelectionChanged(object sender)
        {
            if (LastSelectedItem != null)
                LastSelectedItem.Selected = false;
            LastSelectedItem = (UIListBoxItem)sender;
        }

        private void ItemClicked(object sender, EventArgs e)
        {
            ClickedItem = (UIListBoxItem)sender;
            ItemClick?.Invoke(this, flpListBox.Controls.IndexOfKey(((UIListBoxItem)sender).Name));
        }

        private void ItemDoubleClicked(object sender, EventArgs e)
        {
            DoubleClickedItem = (UIListBoxItem)sender;
            ItemDoubleClick?.Invoke(this, flpListBox.Controls.IndexOfKey(((UIListBoxItem)sender).Name));           
        }
    }
}
