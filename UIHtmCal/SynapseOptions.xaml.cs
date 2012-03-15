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
    /// Interaktionslogik für SynapseOptions.xaml
    /// </summary>
    public partial class SynapseOptions : Window
    {
        public SynapseOptions()
        {
            InitializeComponent();
        }

        private void ok_Button_Click(object sender, RoutedEventArgs e)
        {
            UserInputSynapse uInput = this.Resources["MySynapseInput"] as UserInputSynapse;
            uInput.saveUserInput();
            this.Close();
        }
    }
}
