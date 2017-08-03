using System;
using System.Windows.Forms;

namespace ExternalGlasses
{
    public partial class MainWindow : Form
    {
        private bool _overlayStatus;
        private Overlay _overlay;

        public MainWindow()
        {
            InitializeComponent();
            _overlayStatus = false;
        }

        private void OverlayButton_Click(object sender, EventArgs e)
        {
            if(!_overlayStatus)
            {
                _overlay = new Overlay();
                _overlay.Show();
                _overlayStatus = true;

                OverlayButton.Text = "Close";
            }
            else if (_overlayStatus)
            {
                _overlay.Close();
                _overlayStatus = false;

                OverlayButton.Text = "Open";
            }
            else
            {
                Console.WriteLine("SOMETHING REALY BAD HAPPENED!");
            }
        }
    }
}
