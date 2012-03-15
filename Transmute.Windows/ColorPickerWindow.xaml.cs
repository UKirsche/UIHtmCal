using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using xColor = Microsoft.Xna.Framework.Color;

namespace Transmute.Windows
{
    public partial class ColorPickerWindow : Window
    {
        #region Fields

        Brush borderBrush;

        #endregion

        #region Properties

        public int R = 0;
        public int G = 0;
        public int B = 0;
        public int A = 0;

        public Color Color
        {
            get
            {
                return Color.FromArgb((byte)A, (byte)R, (byte)G, (byte)B);
            }
        }

        public xColor XnaColor
        {
            get
            {
                return new xColor((byte)R, (byte)G, (byte)B, (byte)A);
            }
        }

        #endregion

        #region Constructor

        public ColorPickerWindow()
        {
            InitializeComponent();
            borderBrush = txtR.BorderBrush;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(@"Data\TransparencyBackground.png", UriKind.Relative));
            canvasBackground.Background = brush;
        }

        public ColorPickerWindow(int r, int g, int b, int a)
            : this()
        {
            R = r;
            G = g;
            B = b;
            A = a;
            scrollR.Value = r;
            scrollG.Value = g;
            scrollB.Value = b;
            scrollA.Value = a;
        }

        #endregion

        #region Events

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void scrollR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            R = Convert.ToInt32(scrollR.Value);
            SetPreviewColor();
        }

        private void scrollG_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            G = Convert.ToInt32(scrollG.Value);
            SetPreviewColor();
        }

        private void scrollB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            B = Convert.ToInt32(scrollB.Value);
            SetPreviewColor();
        }

        private void scrollA_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            A = Convert.ToInt32(scrollA.Value);
            SetPreviewColor();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetPreviewColor();
        }

        private void txtR_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtR.BorderBrush = borderBrush;
            if (!string.IsNullOrEmpty(txtR.Text))
            {
                if (txtR.Text.IsNumeric())
                {
                    R = Convert.ToInt32(txtR.Text);
                    if (R > 255) R = 255;
                    if (R < 0) R = 0;
                    SetPreviewColor(false);
                }
                else
                {
                    txtR.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                txtR.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }

        private void txtG_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtG.BorderBrush = borderBrush;
            if (!string.IsNullOrEmpty(txtG.Text))
            {
                if (txtG.Text.IsNumeric())
                {
                    G = Convert.ToInt32(txtG.Text);
                    if (G > 255) G = 255;
                    if (G < 0) G = 0;
                    SetPreviewColor(false);
                }
                else
                {
                    txtG.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                txtG.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }

        private void txtB_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtB.BorderBrush = borderBrush;
            if (!string.IsNullOrEmpty(txtB.Text))
            {
                if (txtB.Text.IsNumeric())
                {
                    B = Convert.ToInt32(txtB.Text);
                    if (B > 255) B = 255;
                    if (B < 0) B = 0;
                    SetPreviewColor(false);
                }
                else
                {
                    txtB.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                txtB.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }

        private void txtA_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtA.BorderBrush = borderBrush;
            if (!string.IsNullOrEmpty(txtA.Text))
            {
                if (txtA.Text.IsNumeric())
                {
                    A = Convert.ToInt32(txtA.Text);
                    if (A > 255) A = 255;
                    if (A < 0) A = 0;
                    SetPreviewColor(false);
                }
                else
                {
                    txtA.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                txtA.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }

        #endregion

        #region Methods

        private void SetPreviewColor(bool setText = true)
        {
            if (canvasBackground != null)
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color;
                canvasPreview.Background = brush;
            }

            if (setText)
            {
                txtA.Text = A.ToString();
                txtR.Text = R.ToString();
                txtG.Text = G.ToString();
                txtB.Text = B.ToString();
            }
        }

        public void Load(Microsoft.Xna.Framework.Color xnaColor)
        {
            R = xnaColor.R;
            G = xnaColor.G;
            B = xnaColor.B;
            A = xnaColor.A;

            scrollR.Value = R;
            scrollG.Value = G;
            scrollB.Value = B;
            scrollA.Value = A;

            SetPreviewColor();
        }

        #endregion


    }
}
