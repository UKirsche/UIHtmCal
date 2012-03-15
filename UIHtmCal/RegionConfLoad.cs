using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace HTM.UIHtmCal
{

    public struct RegionParams
    {
        public bool regionOn;
        public bool spatialOn;
        public bool temporalOn;
        public int cellPCol;
        public int colSizeX;
        public int colSizeY;
        public Point inputSize;
        public int localityRad;
        public int segActThreshold;
        public int newSynapseCount;
        public float pctInputCol;
        public float pctLocalActivity;
        public float pctMiniOlap;

    }

    /// <summary>
    /// Loads and saves the Elements of the RegionConf.xml file, that contains user entries for Region configuration
    /// Provides loaded elements as a structure to return
    /// </summary>
    static class RegionConfLoad
    {

        private const string REGIONNAME = "RegionConf.xml";
        private static RegionParams regParams;
        public static RegionParams RegParams
        {
            get { return regParams; }
            set { regParams = value; }
        }



        public static void LoadRegionConfigXML(int regionID)
        {

            XElement docReg = XElement.Load(REGIONNAME);
            var cpcs = from cps in docReg.Descendants("Region")
                       where (int)cps.Attribute("Number") == regionID
                       select (int)cps.Element("CellsPerColumn");
            var csx = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (int)cps.Element("ColumnSizeX");
            var csy = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (int)cps.Element("ColumnSizeY");
            var isx = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (int)cps.Element("InputSizeX");
            var isy = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (int)cps.Element("InputSizeY");
            var lrad = from cps in docReg.Descendants("Region")
                       where (int)cps.Attribute("Number") == regionID
                       select (int)cps.Element("LocalityRad");
            var sat = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (int)cps.Element("SegmentActivateThreshold");
            var nsc = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (int)cps.Element("NewSynapseCount");
            var pic = from cps in docReg.Descendants("Region")
                      where (float)cps.Attribute("Number") == regionID
                      select (int)cps.Element("PctInputCol");
            var pla = from cps in docReg.Descendants("Region")
                      where (float)cps.Attribute("Number") == regionID
                      select (int)cps.Element("PctLocalActivity");
            var pmo = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (float)cps.Element("PctMinOverlap");
            var reg = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (bool)cps.Attribute("On");
            var spat = from cps in docReg.Descendants("Region")
                      where (int)cps.Attribute("Number") == regionID
                      select (bool)cps.Attribute("Spatial");
            var temp = from cps in docReg.Descendants("Region")
                       where (int)cps.Attribute("Number") == regionID
                       select (bool)cps.Attribute("Temporal");

            //Create Structure
            regParams = new RegionParams();

            //Fill structure
            regParams.cellPCol = Convert.ToInt32(cpcs.First());
            regParams.colSizeX = Convert.ToInt32(csx.First());
            regParams.colSizeY = Convert.ToInt32(csy.First());
            regParams.inputSize = new Point(Convert.ToInt32(isx.First()), Convert.ToInt32(isy.First()));
            regParams.localityRad = Convert.ToInt32(lrad.First());
            regParams.segActThreshold = Convert.ToInt32(sat.First());
            regParams.newSynapseCount = Convert.ToInt32(nsc.First());
            regParams.pctInputCol = (float)Convert.ToInt32(pic.First());
            regParams.pctLocalActivity = (float)Convert.ToInt32(pla.First());
            regParams.pctMiniOlap = (float)Convert.ToInt32(pmo.First());
            regParams.regionOn = Convert.ToBoolean(reg.First());
            regParams.spatialOn = Convert.ToBoolean(spat.First());
            regParams.temporalOn = Convert.ToBoolean(temp.First());

        }

        public static void SaveRegionConfiXML(int regionID)
        {
            //Get Region:
            XElement docReg = XElement.Load(REGIONNAME);
            var root = from region in docReg.Descendants("Region")
                                         where (int)region.Attribute("Number") == regionID
                                         select new {CellsPerColumn=region.Element("CellsPerColumn"),
                                                     ColumnSizeX = region.Element("ColumnSizeX"),
                                                     ColumnSizeY = region.Element("ColumnSizeY"),
                                                     InputSizeX = region.Element("InputSizeX"),
                                                     InputSizeY = region.Element("InputSizeY"),
                                                     LocalityRad = region.Element("LocalityRad"),
                                                     SegmentActivateThreshold = region.Element("SegmentActivateThreshold"),
                                                     NewSynapseCount = region.Element("NewSynapseCount"),
                                                     PctInputCol = region.Element("PctInputCol"),
                                                     PctLocalActivity = region.Element("PctLocalActivity"),
                                                     PctMinOverlap = region.Element("PctMinOverlap"),
                                                     RegionActivated = region.Attribute("On"),
                                                     SpatialLearning = region.Attribute("Spatial"),
                                                     TemporalLearning = region.Attribute("Temporal")

                                         };
            //Just 1 Item
            foreach (var item in root)
            {
                item.CellsPerColumn.Value = Convert.ToString(regParams.cellPCol);
                item.ColumnSizeX.Value = Convert.ToString(regParams.colSizeX);
                item.ColumnSizeY.Value = Convert.ToString(regParams.colSizeY);
                item.InputSizeX.Value = Convert.ToString(regParams.inputSize.X);
                item.InputSizeY.Value = Convert.ToString(regParams.inputSize.Y);
                item.LocalityRad.Value = Convert.ToString(regParams.localityRad);
                item.SegmentActivateThreshold.Value = Convert.ToString(regParams.segActThreshold);
                item.NewSynapseCount.Value = Convert.ToString(regParams.newSynapseCount);
                item.PctInputCol.Value = Convert.ToString(regParams.pctInputCol);
                item.PctLocalActivity.Value = Convert.ToString(regParams.pctLocalActivity);
                item.PctMinOverlap.Value = Convert.ToString(regParams.pctMiniOlap);
                item.RegionActivated.Value = Convert.ToString(regParams.regionOn);
                item.SpatialLearning.Value = Convert.ToString(regParams.spatialOn);
                item.TemporalLearning.Value = Convert.ToString(regParams.temporalOn);
            }

            docReg.Save(REGIONNAME);
            
        }
    }


}
