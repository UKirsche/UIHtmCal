using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTM.HTMLibrary;
using System.Drawing;

namespace HTM.UIHtmCal
{
    /// <summary>
    /// Wrapper classe for creating HTM Region depending on parameters entered by user in Custom Dialog
    /// </summary>
    public class RegionWrapper
    {

        /// <summary>
        /// Params to use for Region Construction:
        /// </summary>
        Point inputSize;
        Point colGridSize; 
        float pctInputPCol,pctMinOlap, pctLocActivity;
        int localityRad, cellsPCol, segActThreshold, newSynCount;


        /// <summary>
        /// RegionParams for retrieving region parameters from XML file
        /// </summary>
        public RegionParams RegsParamsXML
        {
            get { return RegsParamsXML; }
            set {RegsParamsXML = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int[,] PredictedCols
        {
            get { return PredictedCols; }
            set { PredictedCols = value; }
        }

        public int[,] ActiveCols
        {
            get { return ActiveCols; }
            set { ActiveCols = value; }
        }

        //Input Size: 
        public Point InputSize
        {
            get { return InputSize; }
            set { InputSize = value; }
        }

        public int RegionID
        {
            get { return RegionID; }
            set { RegionID = value; } 
        }

        public HTM.HTMLibrary.Region RegionChosen
        {
            get { return RegionChosen; }
            set { RegionChosen = value; }
        }

        /// <summary>
        /// Create a new RegionWrapper with the standard regionId to identify the edited Region layer 
        /// plus the initial size of the input that is to be fed into the HTM Region.
        /// </summary>
        /// <param name="regionID"></param>
        /// <param name="inputsize"></param>
        public RegionWrapper(int regionID, Point inputsize)
        {
            RegionID = regionID;
            InputSize = inputsize;
        }


       /// <summary>
       /// Create the Region using the currently set UI parameters. The region
       /// is only created new if this is first run or if the UI parameters have
       /// changed since Region was last created. 
       /// </summary>
        private void checkCreateRegion()
        {
            //Todo: a. Parameterübernahme aus Eingabemaske bzw. XML-File

            //Get Params from XML-File:
            RegionConfLoad.LoadRegionConfigXML(RegionID);
            RegsParamsXML = RegionConfLoad.RegParams;

            //Modify params
            colGridSize  = new Point(RegsParamsXML.colSizeX, RegsParamsXML.colSizeY);
            pctInputPCol = RegsParamsXML.pctInputCol/100;
            pctMinOlap = RegsParamsXML.pctMiniOlap / 100;
            localityRad = RegsParamsXML.localityRad;
            pctLocActivity = RegsParamsXML.pctLocalActivity / 100;
            cellsPCol = RegsParamsXML.cellPCol;
            segActThreshold = RegsParamsXML.segActThreshold;
            newSynCount = RegsParamsXML.newSynapseCount;


            //Check for Rebuild or uncreated Region:
            if (checkForRebuild() || RegionChosen == null)
            {
                RegionChosen = new HTM.HTMLibrary.Region(inputSize, colGridSize, pctInputPCol, pctMinOlap, localityRad,
                    pctLocActivity, cellsPCol, segActThreshold, newSynCount);
            }


        }

        /// <summary>
        /// Check 
        /// </summary>
        /// <returns></returns>
        private bool checkForRebuild()
        {
            if(RegionChosen.cellsPerCol != cellsPCol || RegionChosen.pctLocalActivity != pctLocActivity ||
                RegionChosen.pctInputPerCol != pctInputPCol || RegionChosen.pctMinOverlap != pctMinOlap ||
                RegionChosen.localityRadius != localityRad || RegionChosen.segActiveThreshold != segActThreshold ||
                RegionChosen.newSynapseCount != newSynCount || RegionChosen.inputWidth != inputSize.X ||
                RegionChosen.inputHeight != inputSize.Y || RegionChosen.width != colGridSize.X ||
                RegionChosen.height != colGridSize.Y)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// If this Region is enabled to run, then run the Region for one time step
        /// using the last processed video frame (or last processed output from the
        /// previous Region in the hierarchy).  Set the Region to learn based on
        /// checkbox enablement in the UI.
        /// </summary>
        public void runRegionOnce()
        {

        }
    }
}
