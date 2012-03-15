using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Linq;

namespace HTM.UIHtmCal
{
    /// <summary>
    /// Layer the represents the User Input as specified by Barry. This are the necessary parameters for creating
    /// the Regions. Regions are created as the User presses the Save-Button in the UI
    /// </summary>

    public class UserInputRegion : INotifyPropertyChanged
    {

        #region Properties Getters and Setters

        /// <summary>
        /// As multiple refControler can be activated switch checks for activation
        /// </summary>
        bool regionActivated;

        public bool RegionActivated
        {
            get { return regionActivated; }
            set 
            {
                Console.WriteLine("CheckBox activated " + value);
                this.regionActivated = value; 
            }
        }

        /// <summary>
        /// Switch for spatial learning
        /// </summary>
        bool spatialLearning;

        public bool SpatialLearning
        {
            get { return spatialLearning; }
            set { spatialLearning = value; }
        }

        /// <summary>
        /// Switch for temporal learning
        /// </summary>
        bool temporalLearning;

        public bool TemporalLearning
        {
            get { return temporalLearning; }
            set { temporalLearning = value; }
        }
        
        /// <summary>
        /// Input Parameter: InputSize -> Size of underlying sensor array or region
        /// </summary>
        Point inputSize;
        public Point InputSize
        {
            get { return inputSize; }
            set
            {
                this.inputSize = value;
            }
        }

        /// <summary>
        /// Input Parameter: ColSize determines width and height of region. For UI splitted into X,Y values
        /// </summary>
        Point colSize;
        public int ColSizeX
        {
            get { return colSize.X; }
            set
            {
                this.colSize.X = value;
            }
        }

        public int ColSizeY
        {
            get { return colSize.Y; }
            set
            {
                this.colSize.Y = value;
            }
        }


       
        /// <summary>
        /// Input Parameter: Amount of input from underlying region for feed-forward column in next region
        /// </summary>
        float pctInputCol;
        public float PctInputCol
        {
            get { return pctInputCol; }
            set { this.pctInputCol = value; }
        }

        /// <summary>
        /// Input Parameter: The minimum number of inputs that must be active for a column to be 
        /// considered during the inhibition step.
        /// </summary>
        float pctMiniOlap;
        public float PctMiniOlap
        {
            get { return pctMiniOlap; }
            set
            {
                this.pctMiniOlap = value;
            }
        }

        /// <summary>
        /// Input Parameter: A parameter controlling the number of columns that will be 
        /// winners after the inhibition step.
        /// </summary>
        float pctLocalActivity;
        public float PctLocalActivity
        {
            get { return pctLocalActivity; }
            set { this.pctLocalActivity = value; }
        }

        /// <summary>
        /// Input Parameter: Introduced by Barry to control the area from which feed-forward connections can be made
        /// </summary>
        int localityRad;
        public int LocalityRad
        {
            get { return localityRad; }
            set
            {
                Console.WriteLine("localityRad changed");
                this.localityRad = value;
            }
        }

        /// <summary>
        /// Input Parameter: CellsPColl, cell per column. More cells, more contextual information
        /// </summary>
        int cellsPCol;
        public int CellsPCol
        {
            get { return cellsPCol; }
            set { this.cellsPCol = value; }
        }

        /// <summary>
        /// Input Parameter: Number of cells from input area to be active for feed-forward activation
        /// </summary>
        int segActThreshold;
        public int SegActThreshold
        {
            get { return segActThreshold; }
            set { this.segActThreshold = value; }
        }

        /// <summary>
        /// Input Parameter: Number  of new synapses created by temporal learning
        /// </summary>
        int newSynapseCount;
        public int NewSynapseCount
        {
            get { return newSynapseCount; }
            set { this.newSynapseCount = value; }
        }
        #endregion

        #region INotifyPropertyChanged Members


        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region PropertyChanged
        /// <summary> 
        /// Fire the property changed event 
        /// </summary> 
        /// <param name="propertyName">Name of the property.</param> 
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        //Konstruktor mit Default-Werten:
        public UserInputRegion()
        {
            //Load XML-Config-File
            RegionConfLoad.LoadRegionConfigXML(MainWindow.CurrentlyActiveRegion);

            RegionActivated = RegionConfLoad.RegParams.regionOn;
            SpatialLearning = RegionConfLoad.RegParams.spatialOn;
            TemporalLearning = RegionConfLoad.RegParams.temporalOn;
            CellsPCol = RegionConfLoad.RegParams.cellPCol;
            ColSizeX =  RegionConfLoad.RegParams.colSizeX;
            ColSizeY =  RegionConfLoad.RegParams.colSizeY;
            InputSize =  RegionConfLoad.RegParams.inputSize;
            LocalityRad =  RegionConfLoad.RegParams.localityRad;
            SegActThreshold =  RegionConfLoad.RegParams.segActThreshold;
            NewSynapseCount =  RegionConfLoad.RegParams.newSynapseCount;
            PctInputCol =  RegionConfLoad.RegParams.pctInputCol;
            PctLocalActivity =  RegionConfLoad.RegParams.pctLocalActivity;
            PctMiniOlap = RegionConfLoad.RegParams.pctMiniOlap;
        }

        /// <summary>
        /// 
        /// </summary>
        public void saveUserInput()
        {
            RegionParams regionParams = new RegionParams();
            regionParams.regionOn = RegionActivated;
            regionParams.spatialOn = SpatialLearning;
            regionParams.temporalOn = TemporalLearning;
            regionParams.cellPCol = CellsPCol;
            regionParams.colSizeX = ColSizeX;
            regionParams.colSizeY = ColSizeY;
            regionParams.inputSize = InputSize;
            regionParams.localityRad = LocalityRad;
            regionParams.newSynapseCount = NewSynapseCount;
            regionParams.pctInputCol = PctInputCol;
            regionParams.pctLocalActivity = PctLocalActivity;
            regionParams.pctMiniOlap = PctMiniOlap;
            regionParams.segActThreshold = SegActThreshold;
            
            //Overwrite
            RegionConfLoad.RegParams = regionParams;

            //Save XML-File
            RegionConfLoad.SaveRegionConfiXML(MainWindow.CurrentlyActiveRegion);
        }
    }
}
