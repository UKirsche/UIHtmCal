using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTM.HTMAnalytics;

namespace HTM.HTMLibrary
{
    /// <summary>
    /// This data structure holds three pieces of information required to update a given segment: 
    /// a) segment reference (None if it's a new segment), 
    /// b) a list of existing active synapses, and 
    /// c) a flag indicating whether this segment should be marked as a sequence
    /// segment (defaults to false).
    /// The structure also determines which learning cells (at this time step) are available to connect 
    /// (add synapses to) should the segment get updated. If there is a locality radius set on the Region, 
    /// the pool of learning cells is restricted to those with the radius.
    /// </summary>
    public class SegmentUpdateInfo
    {
        public Cell cell { get; set; }
        public Segment segment { get; set; }
        public List<DistalSynapse> activeSynapses { get; set; }
        public List<Cell> lcsChosen { get; set; }
        public bool addNewSynapses { get; set; }
        public bool isSequence { get; set; }
        public List<DistalSynapse> addedSynapses { get; set; }

        public SegmentUpdateInfo(Cell cell, Segment segment, List<DistalSynapse> activeSynapses,
            bool addNewSynapses=false)
        {
            //BMK Essential for temporal learning. Details specified 

            this.cell = cell;
            this.segment = segment;
            this.activeSynapses = activeSynapses;
            this.addNewSynapses = addNewSynapses; //once synapses added, store here to visualize later
            this.isSequence = false;
            this.addedSynapses = new List<DistalSynapse>();

            List<Cell> learningCells = new List<Cell>(); //capture learning cells at this time step
            
            //do not add >1 synapse to the same cell on a given segment
            Region region = this.cell.column.region;
            if(this.addNewSynapses)
            {
                //Segment&verbundene Zellen:
                List<Cell> segCells = new List<Cell>();

                if(segment!=null)
                {
                    //iterate through all Synapses
                    foreach(Synapse syn in segment.synapses)
                    {
                        //Not yet sure whether applicable only for DistalSynapses
                        if(syn.GetType().Equals(typeof(DistalSynapse)))
                        {
                            DistalSynapse distSyn = syn as DistalSynapse;
                            segCells.Add(distSyn.inputSource);
                        }
                    }
                }
                //only allow connecting to Columns within locality radius
                //Deactivated: UK, 02.12
                Column cellColumn = cell.column;

                int minY, maxY, minX, maxX;

                //Now I want to allow connections over all the region!
                //if locality radius is 0, it means 'no restriction'
                //if(region.localityRadius>0)
                //{
                //    minY = Math.Max(0, cellColumn.cPos.Y-region.localityRadius);
                //    maxY = Math.Min(region.height-1, cellColumn.cPos.Y + region.localityRadius);
                //    minX = Math.Max(0, cellColumn.cPos.X - region.localityRadius);
                //    maxX = Math.Min(region.width-1, cellColumn.cPos.X + region.localityRadius);
                //}
                //else{
                //    minY = 0;
                //    maxY = region.height - 1;
                //    minX = 0;
                //    maxX = region.width - 1;
                //}

                minY = 0;
                maxY = region.height - 1;
                minX = 0;
                maxX = region.width - 1;

                //System.Diagnostics.Trace.WriteLine("=====================");
                //System.Diagnostics.Trace.WriteLine("New SegmentUpdateInfo");

                for(int i=minY;i<=maxY;i++)
                {
                    for(int j=minX;j<=maxX;j++)
                    {
                        Column col = region.columnGrid[j][i];
                        foreach(Cell c in col.cells)
                        {
                            if(c.wasLearning && !segCells.Contains(c))
                            {

                                //System.Diagnostics.Trace.WriteLine("Possible Learning Cells");
                                learningCells.Add(c);
                                //System.Diagnostics.Trace.WriteLine("StartCell:" + cell.column.cPos + " Index: " + cell.Index +
                                //    " , LearnCell: Col" + c.column.cPos + " Index: " + c.Index);
                            }
                        }
                    }
                }
            }

            //Basic allowed number of new Synapses
            int synCount = region.newSynapseCount;

            if(segment!=null)
            {
                //Todo: Achtung: Hier wieder nur distale Synapsen????!!!!!
                synCount = Math.Max(0, synCount-activeSynapses.Count);
            }

            int numberLCs = learningCells.Count;

            //clamp at -- of learn cells
            synCount = Math.Min(numberLCs, synCount);

            //Create field
            lcsChosen = new List<Cell>();

            if(numberLCs>0 && synCount>0)
            {
                //Take index sample from learning cells, put into lcsChosen
                //System.Diagnostics.Trace.WriteLine("Chosen LCs");
                List<int> uniqueCheck = new List<int>();

                for (int i = 0; i < synCount;i++ )
                {
                    int value = MathHelper.r.Next(0, numberLCs);
                    if (!uniqueCheck.Contains(value))
                    { 
                        Cell learnCell = learningCells[value];
                        lcsChosen.Add(learningCells[value]);
                        //System.Diagnostics.Trace.WriteLine("StartCell:" + cell.column.cPos + " Index: " + cell.Index +
                        //" , LearnCell: Col" + learnCell.column.cPos + " Index: " + learnCell.Index);

                        //Add to chech:
                        uniqueCheck.Add(value);
                    }
                }
            }

        }
    }
}
