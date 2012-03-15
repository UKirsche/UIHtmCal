using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace HTM.UIHtmCal
{

    struct SynapseParams
    {
        public int connPerm;
        public int incPerm;
        public int intPerm;
        public int decPerm;

    }

    /// <summary>
    /// Loads and saves the Elements of the RegionConf.xml file, that contains user entries for RegionChosen configuration
    /// Provides loaded elements as a structure to return
    /// </summary>
    static class SynapseConfLoad
    {

        private const string SYNAPSENAME = "SynapseConf.xml";
        private static SynapseParams synParams;
        public static SynapseParams SynParams
        {
            get { return synParams; }
            set { synParams = value; }
        }

        public static void LoadSynapseConfigXML()
        {

            XElement docReg = XElement.Load(SYNAPSENAME);
            var cpm = from synapse in docReg.Descendants("Synapse")
                      select (int)synapse.Element("ConnectedPermanence");
            var inc = from synapse in docReg.Descendants("Synapse")
                      select (int)synapse.Element("PermanenceIncrease");
            var dec = from synapse in docReg.Descendants("Synapse")
                      select (int)synapse.Element("PermanenceDecrease");
            var init = from synapse in docReg.Descendants("Synapse")
                       select (int)synapse.Element("InitialPermanence");
          

            //Create Structure
            synParams = new SynapseParams();

            //Fill structure
            synParams.connPerm = Convert.ToInt32(cpm.First());
            synParams.incPerm = Convert.ToInt32(inc.First());
            synParams.decPerm = Convert.ToInt32(dec.First());
            synParams.intPerm = Convert.ToInt32(init.First());
        }

        public static void SaveSynapseConfiXML()
        {
            //Get Synape:
            XElement docReg = XElement.Load(SYNAPSENAME);
            var root = from synapse in docReg.Descendants("Synapse")
                       select new
                       {
                           ConnectedPermanence = synapse.Element("ConnectedPermanence"),
                           PermanenceIncrease = synapse.Element("PermanenceIncrease"),
                           PermanenceDecrease = synapse.Element("PermanenceDecrease"),
                           InitialPermanence  = synapse.Element("InitialPermanence")
                       };
            //Just 1 Item
            foreach (var item in root)
            {
                item.ConnectedPermanence.Value = Convert.ToString(synParams.connPerm);
                item.PermanenceIncrease.Value = Convert.ToString(synParams.incPerm);
                item.PermanenceDecrease.Value = Convert.ToString(synParams.decPerm);
                item.InitialPermanence.Value= Convert.ToString(synParams.intPerm);
                
            }

            docReg.Save(SYNAPSENAME);
            
        }
    }


}
