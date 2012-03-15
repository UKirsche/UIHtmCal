using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HTM.HTMInterfaces;
using HTM.HTMBitmapProcessor;
using HTM.HTMLibrary;
using HTM.HTMAnalytics;

namespace HTM.UIHtmCal
{
    /// <summary>
    /// Enum keeps track of chosen input source for HTM Processing
    /// </summary>
    public enum InputSource
    {
        /// <summary>
        /// Database from Event-Logger
        /// </summary>
        DataBase,
        /// <summary>
        /// Sentence from newly created sentence as sequence
        /// </summary>
        Sentences
    };

    /// <summary>
    /// Handles setup of HTM-Processing. I.e.reading parameters for configuring the regions and preparing outputs for logs observers
    /// in the UI.
    /// Handles input-output cycle of regions and input-region as well as the statistics updating for the created regions
    /// </summary>
    public class HTMController : IGamer
    {

        #region constants

        int NUMREGIONS = Convert.ToInt16(Properties.Settings.Default.MaxNumberHTMRegions);

        #endregion


        #region Fields, Properties


        /// <summary>
        /// Singleton for HTMController
        /// </summary>
        private static HTMController instance = null;
        public static HTMController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HTMController();
                }
                return instance;
            }
        }


        /// <summary>
        /// Enum to hold reference to chosen inputsource
        /// </summary>
        public InputSource InSource { get; set; }
        /// <summary>
        /// InputRegion as <see cref="IRegionProcessor"/> to help traverse the input and feed to HTM-Machine
        /// </summary>
        public IRegionProcessor InputRegion { get; set; }
        /// <summary>
        /// InputRegions as <see cref="IStatisticsLogSentence"/> to get additional logging information for the actual bitmap
        /// </summary>
        public IStatisticsLogSentence InputStatistics { get; set; }
        /// <summary>
        /// List of HTMRegions created for the non-input HTMRegions <see cref="Region"/>
        /// </summary>
        public List<IRegionProcessor> HTMRegions { get; set; }
        /// <summary>
        /// Paramters for the regions as defined by UI input dialog
        /// </summary>
        private List<RegionParams> RegionParamList { get; set; }
        /// <summary>
        /// Create observable collection for regions in order to log changes within for <see cref="IStatistics"/>
        /// </summary>
        public ObservableCollection<Region> LogRegions { get; set; }
        /// <summary>
        /// Create observable collection for <see cref="ContextStatisticEntry"/> that logs information from input-bitmap arrays
        /// </summary>
        public ListContextStatisticLog LogContextEntries { get; set; }
        
        #endregion

        /// <summary>
        /// Wrapper to initialize different HTMRegions
        /// </summary>
        public void Initialize(InputSource inSource=InputSource.DataBase)
        {
            //Hand over input Source (DB, Sentences...)
            InSource = inSource;

            //Create Lists
            RegionParamList = new List<RegionParams>();
            HTMRegions = new List<IRegionProcessor>();
            LogRegions = new ObservableCollection<Region>();
            LogContextEntries = new ListContextStatisticLog();

            //Load Region Parameters: Region start counting by 1
            for (int i = 1; i <= NUMREGIONS; i++)
            {
                RegionConfLoad.LoadRegionConfigXML(i);
                RegionParamList.Add(RegionConfLoad.RegParams);
            }


            //Initalize BMProcessor first. I.e. the input bitmap
            InitializeHTMBitMap();

            //Initalize Regions second
            InitializeHTMRegions();
            
        }

        private void InitializeHTMRegions()
        {      
            foreach (var param in RegionParamList)
            {
                if (param.regionOn == true)
                {
                    //Percentage calculations:
                    float floatPctInput = (float) (param.pctInputCol * 0.01);
                    float floatPctLocalActivity = (float) (param.pctLocalActivity * 0.01);
                    float floatPctMinOlap = (float) (param.pctMiniOlap * 0.01);

                    //Initalize HTMRegion with Params
                    Region newReg = new Region(param.inputSize, new System.Drawing.Point(param.colSizeX, param.colSizeY), 
                        floatPctInput,floatPctMinOlap, param.localityRad, floatPctLocalActivity,
                        param.cellPCol, param.segActThreshold, param.newSynapseCount);

                    //Add region to HTMRegionsList
                    HTMRegions.Add(newReg);
                    //Add region to Observable Collection
                    LogRegions.Add(newReg);
                }
            }
        }


        /// <summary>
        /// Prepare input region (Bitmap) for use
        /// </summary>
        private void InitializeHTMBitMap()
        {
            switch (InSource)
            {
                case InputSource.DataBase :
                    BitMapProcessor BMProcessor = new BitMapProcessor();
                    BMProcessor.Initialize();
                    InputRegion = BMProcessor as IRegionProcessor;
                    InputStatistics = BMProcessor as IStatisticsLogSentence;
                    break;
                case InputSource.Sentences :
                    SentenceProcessor STProcessor = SentenceProcessor.Instance;
                    STProcessor.Initialize();                  
                    InputRegion = STProcessor as IRegionProcessor;
                    InputStatistics = STProcessor as IStatisticsLogSentence;
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void PlayLoop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process refControler and set outputs for next refControler!
        /// </summary>
        public void PlayStep()
        {

            //Basic Region
            IRegionProcessor regionPrevious = InputRegion as IRegionProcessor;
            //First step
            if (regionPrevious != null)
            {
                //First step:
                regionPrevious.Step();
            }

            foreach (IRegionProcessor region in HTMRegions) //Run through Regions array
            {
                if (regionPrevious != null)
                {                 
                    //1. Update Input of region
                    region.SetInput(regionPrevious.GetOutPut());

                    //2. step region:
                    region.Step();

                    //3. Shift refControler
                    regionPrevious = region;
                }
            }

            //Get last output:
            int[,] lastOutput = regionPrevious.GetOutPut();
        }


        /// <summary>
        /// Collect log information from input bitmap and prepare to write to ui log
        /// </summary>
        public void UpdateLogStatistics()
        {
            //Fill this into data grid with relevant values!
            string logString = InputStatistics.LogSentence;
            if (logString != null && logString.Length > 0)
            {
                ContextStatisticEntry newEntry = new ContextStatisticEntry(logString, HTMRegions[0] as IStatistics);
                LogContextEntries.AddContextStatisticEntry(newEntry);
            }
        }
    }
}
