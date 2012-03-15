using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HTM.HTMLibrary;

namespace HTM.UIHtmCal
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Fields,Properties



        /// <summary>
        /// Holds information about chosen data input source
        /// </summary>
        public InputSource chosenDataSource { get; set; }
        private RegOpts region1Opts, region2Opts, region3Opts;
        private const string ResourceUI = "ResourceUI";

        public static int currentlyActiveRegion;
        public static int CurrentlyActiveRegion
        {
            get { return currentlyActiveRegion; }
            set { currentlyActiveRegion = value; }
        }

        public HTMController HtmProcessor { get; set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuRegion1Click(object sender, RoutedEventArgs e)
        {

            // Create a window
            openRegionOptsWindow(region1Opts, 1);
        }

        private void MenuRegion2Click(object sender, RoutedEventArgs e)
        {
            // Create a window and name it
            openRegionOptsWindow(region2Opts, 2);
        }

        private void MenuRegion3Click(object sender, RoutedEventArgs e)
        {
            // Create a window and name it
            openRegionOptsWindow(region3Opts, 3);
        }

        private void openRegionOptsWindow(RegOpts regOptsDlg, int intReg)
        {

            //set active RegionNumber
            CurrentlyActiveRegion = intReg;

            // Create a modal dialog and name it
            if (regOptsDlg == null || regOptsDlg.IsVisible == false)
            {
                regOptsDlg = new RegOpts();
                regOptsDlg.Owner = this;
                regOptsDlg.Title = Utilities.GetResourceValue(ResourceUI, "RegionTitlePre") +
                    intReg + Utilities.GetResourceValue(ResourceUI, "RegionTitlePost");
                regOptsDlg.Name = Utilities.GetResourceValue(ResourceUI, "RegionName");

                // Open a window
                regOptsDlg.ShowDialog();
            }
        }

        private void MenuSynapseClick(object sender, RoutedEventArgs e)
        {
            SynapseOptions synOptsDlg= new SynapseOptions();
            synOptsDlg.Owner = this;
            synOptsDlg.Title = "Synapse Options";
            synOptsDlg.Name = "Uwe";

            // Open a window
            synOptsDlg.ShowDialog();
        }

        /// <summary>
        /// Open context dictionary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuShowContextDictionary(object sender, RoutedEventArgs e)
        {
            //1. Lade Dialog-Fenster
            Lexikon contextDictionary = new Lexikon();
            contextDictionary.Owner = this;
            contextDictionary.Title = "Context Dictionary";

            // Open a window
            contextDictionary.ShowDialog();
        }

        /// <summary>
        /// Open visualisation of HTM-Network 1: temporal pooling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuShowTempLearning(object sender, RoutedEventArgs e)
        {

            if (HtmProcessor != null)
            { 
                VisualizeTemporalPooler visTemporalPooler = new VisualizeTemporalPooler();
                visTemporalPooler.Owner = this;
                visTemporalPooler.Title = "Visualize Temporal Pooler";

                //Hand over HTMProcessor:
                visTemporalPooler.Controler = HtmProcessor;


                //Open 
                visTemporalPooler.Show();
            }
        }

        #region DataGrid SelectionChanged-Methods

        private void RegionMasterView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Region selRegion = e.AddedItems[0] as Region;

            //Bind the ColumnMasterView
            ColumnMasterView.DataContext = selRegion.columns;
        }

        private void ColumnMasterView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Retrieve selected Column
            Column selColumn = e.AddedItems[0] as Column;
            selColumn.IsDataGridSelected = true;

            //Get old selection
            if (e.RemovedItems.Count > 0)
            {
                Column oldSelColumn = e.RemovedItems[0] as Column;
                oldSelColumn.IsDataGridSelected = false;
            }

            //Bind the column to the datagrid ColumnDetailView for Spatial Learning
            ColumnDetailView.DataContext = selColumn;
            //Bind the cells of Column to CellDetail for TemporalLearning-Information
            CellDetailView.DataContext = selColumn.cells;
        }

        private void CellDetailView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //Retrieve selected Column
            if (e.AddedItems.Count > 0)
            {
                Cell selCell = e.AddedItems[0] as Cell;
                selCell.IsDataGridSelected = true;

                //Deactivate Column selection:
                Column outColumn = selCell.column;
                outColumn.IsDataGridSelected = false;
            }

            //Get old selection
            if (e.RemovedItems.Count > 0)
            {
                Cell oldSelCell = e.RemovedItems[0] as Cell;
                oldSelCell.IsDataGridSelected = false;
            }
        }

        #endregion



        /// <summary>
        /// Initialzes the HTM-Network by creating the htm-controller to connect to events database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitializeHTM(object sender, RoutedEventArgs e)
        {
            //Disable relevant buttons:
            InitHTM.IsEnabled = false;
            EnableSourceButtons(false);

            HtmProcessor = HTMController.Instance;

            //Initialize HTM Processor
            HtmProcessor.Initialize(chosenDataSource);
            
            //Bind Region
            RegionMasterView.DataContext = HtmProcessor.LogRegions;

            //Bind the LogView
            SentencesLog.DataContext = HtmProcessor.LogContextEntries;

            EnableSteeringButtons(true);
        }

        /// <summary>
        /// Enable or disable buttons to choose input source
        /// </summary>
        /// <param name="value"></param>
        public void EnableSourceButtons(bool value)
        {
            Button_DB_Connect.IsEnabled = value;
            Button_Sentence_Connect.IsEnabled = value;
        }

        /// <summary>
        /// Enables or disable buttons in toolbar
        /// </summary>
        /// <param name="value"></param>
        public void EnableSteeringButtons(bool value)
        {
            //Enable other control buttons:
            HTMButtonStep.IsEnabled = value;
            HTMButtonFastStep.IsEnabled = value;
            HTMButtonStop.IsEnabled = value;
            HTMButtonShow.IsEnabled = value;
        }

        /// <summary>
        /// Initialzes the HTM-Network by creating the htm-controller to sentences created in dialog window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitalizeAlphaBeta(object sender, RoutedEventArgs e)
        {
            SentenceParams confSentences = new SentenceParams();
            confSentences.Owner = this;
            confSentences.Title = "Create sentences to learn by HTM";

            //Hand over HTMProcessor:
            //confSentences.Controler = HtmProcessor;

            //Open 
            confSentences.ShowDialog();
        }

        private void StepHTMEvents(object sender, RoutedEventArgs e)
        {
            if (HtmProcessor != null)
            {
                //Start stepping in HTMProcessor:
                HtmProcessor.PlayStep();

                //Update LogStatistics for Logger in UI
                HtmProcessor.UpdateLogStatistics();
                //Scroll down to logged information
                ScrollHTMLogView();

            }
        }


        private void FastThroughHTMEvents(object sender, RoutedEventArgs e)
        {
            if (HtmProcessor != null)
            {
                System.Diagnostics.Trace.WriteLine("Start...");
                for (int i = 0; i < 1000; i++)
                {
                    //Start stepping in HTMProcessor:
                    HtmProcessor.PlayStep();


                    //Update LogStatistics:
                    HtmProcessor.UpdateLogStatistics();
                }
                System.Diagnostics.Trace.WriteLine("Stop...");

                ScrollHTMLogView();

            }
        }


        private void ScrollHTMLogView()
        {

            if (SentencesLog.Items != null)
            {
                int noEntries = SentencesLog.Items.Count;
                SentencesLog.ScrollIntoView(SentencesLog.Items[noEntries - 1], SentencesLog.Columns[0]);
            }
        }


        private void StopUIAutomationListen(object sender, RoutedEventArgs e)
        {
            //Disable relevant buttons to reset
            InitHTM.IsEnabled = false;
            EnableSteeringButtons(false);
            EnableSourceButtons(true);
        }

        private void DBConnect_Click(object sender, RoutedEventArgs e)
        {
            //set input source
            chosenDataSource = InputSource.DataBase;

            //Activate Initialize Button
            InitHTM.IsEnabled = true;
        }

        //Testwiese Implementierung von Hilfe-Datei
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\Users\Kirschenmann\Documents\Visual Studio 2010\Projects\Winhelptest\TestHelpProject.chm");
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
