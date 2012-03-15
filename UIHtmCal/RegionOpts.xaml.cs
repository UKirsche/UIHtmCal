using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HTM.UIHtmCal
{
    /// <summary>
    /// Interaktionslogik für Regions (Options)-Fenster
    /// </summary>
    public partial class RegOpts : Window
    {
        public RegOpts()
        {
            InitializeComponent();
        }

        private void ok_Button_Click(object sender, RoutedEventArgs e)
        {
            UserInputRegion uInput = this.Resources["MyRegionInput"] as UserInputRegion;
            uInput.saveUserInput();
            this.Close();
        }

    }
}
