using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Drawing;
using HTM.HTMInterfaces;
using HTM.HTMAnalytics;

///
/// @created 21.3.2011
/// Taken from first approach by Mutakarnich in Python
/// Adapted by Uwe Kirschenmann
/// @author Barry Matukarnich, Uwe Kirschenmann
///


namespace HTM.HTMLibrary
{
    /// <summary>
    /// Code to represent an entire Hierarchical Temporal Memory (HTM) Region of Columns that implement Numenta's new Cortical Learning Algorithms (CLA). 
    /// The Region is defined by a matrix of columns, each of which contains several cells.  
    /// The main idea is that given a matrix of input bits, the Region will first sparsify the input such that only a few Columns will become 'active'. 
    /// As the input matrix changes over time, different sets of Columns will become active in sequence.  
    /// The Cells inside the Columns will attempt to learn these temporal transitions and eventually the Region will be able to make predictions about what 
    /// may happen next given what has happened in the past. 
    /// For (much) more information, visit www.numenta.com.
    /// SpatialPooling snippet from the Numenta docs:
    /// The code computes activeColumns(t) = the list of columns that win due to 
    /// the bottom-up input at time t. This list is then sent as input to the 
    /// temporal pooler routine.
    /// Phase 1: compute the overlap with the current input for each column
    /// Phase 2: compute the winning columns after inhibition
    /// Phase 3: update synapse permanence and internal variables
    /// 
    /// 1) Start with an input consisting of a fixed number of bits. These bits might represent sensory data 
    ///    or they might come from another region lower in the hierarchy.
    /// 2) Assign a fixed number of columns to the region receiving this input. Each column has 
    ///    an associated dendrite segmentUpdateList. Each dendrite segmentUpdateList has a set of potential synapses 
    ///    representing a subset of the input bits. Each potential synapse has a permanence value.
    ///    Based on their permanence values, some of the potential synapses will be valid.
    /// 3) For any given input, determine how many valid synapses on each column are connected to active input bits.
    /// 4) The number of active synapses is multiplied by a 'boosting' factor which is dynamically determined by how often a column is 
    ///    active relative to its neighbors. 
    /// 5) The columns with the highest activations after boosting disable all but a fixed percentage of the columns within an inhibition radius. 
    ///    The inhibition radius is itself dynamically determined by the spread (or 'fan-out') of input bits. There is 
    ///    now a sparse set of active columns.
    /// 6) For each of the active columns, we adjust the permanence values of all the potential 
    ///    synapses. The permanence values of synapses aligned with active input bits are 
    ///    increased. The permanence values of synapses aligned with inactive input bits are 
    ///    decreased. The changes made to permanence values may change some synapses from being 
    ///    valid to not valid, and vice-versa.
    ///    
    ///Represent an entire region of HTM columns for the CLA.
    /// </summary>
    public class Region : IRegionProcessor, IStatistics, INotifyPropertyChanged
    {
        private int[,] inputData;
        
        private int[,] outputData;
        private Dictionary<Cell, List<SegmentUpdateInfo>> segmentUpdateMap;
        private Dictionary<Cell, List<SegmentUpdateInfo>> recentUpdateMap;


        //Properties
        //public List<Column> columns { get; set; }
        public ObservableCollection<Column> columns { get; set; }
        public List<List<Column>> columnGrid { get; set; }
        public int cellsPerCol { get; set; }
        public int segActiveThreshold { get; set; }
        //public int minOverlap { get; set; }
        public bool spatialLearning {get;set;}
        public bool temporalLearning { get; set; }
        public int desiredLocalActivity{ get; set; }
        public float inhibitionRadius { get; set; }
        public int localityRadius { get; set; }
        public int newSynapseCount { get; set; }
        public float pctInputPerCol { get; set; }
        public float pctMinOverlap { get; set; }
        public float pctLocalActivity { get; set; }
        public int inputWidth { get; set; }
        public int inputHeight { get; set; }
        public int[,] InputData 
        {
            get { return inputData;}
        }
        public int width { get; set; }
        public int height { get; set; }
        public float xSpace { get; set; }
        public float ySpace { get; set; }
        public int inputRadius { get; set; }


        /// <summary>
        /// Prior to receiving any inputs, the region is initialized by computing a list of initial 
        /// potential synapses for each column. This consists of a random set of inputs selected 
        /// from the input space. Each input is represented by a synapse and assigned a random 
        /// permanence value. The random permanence values are chosen with two criteria. 
        /// First, the values are chosen to be in a small range around connectedPerm (the minimum 
        /// permanence value at which a synapse is considered "connected"). This enables potential 
        /// synapses to become connected (or disconnected) after a small number of training 
        /// iterations. Second, each column has a natural center over the input region, and the 
        /// permanence values have a bias towards this center (they have higher values near 
        /// the center).
        /// 
        /// In addition to this I have added a concept of Locality Radius, which is an additional parameter to control how 
        /// far away synapse connections can be made instead of allowing connections anywhere.  The reason for this is that in the
        /// case of video images I wanted to experiment with forcing each Column to only
        /// learn on a small section of the total input to more effectively learn lines or corners in a small section without 
        /// being 'distracted' by learning larger patterns in the overall input space (which hopefully higher 
        /// hierarchical Regions would handle more successfully).  Passing in 0 for locality radius will mean no restriction
        /// which will more closely follow the Numenta doc if desired.
        /// </summary>
        /// <param name="cellsPerCol"></param>
        /// <param name="colGridSize"></param>
        /// <param name="inputSize"></param>
        /// <param name="localityRadius"></param>
        /// <param name="newSynapseCount"></param>
        /// <param name="pctInputPerCol"></param>
        /// <param name="pctLocalActivity"></param>
        /// <param name="pctMinOverlap"></param>
        public Region(Point inputSize, Point colGSize, float pctInputPCol, float pctMinOlap, int localityRad,
            float pctLocActivity, int cellsPCol, int segActThreshold, int newSynCount)
        {
            inputWidth  = inputSize.X;
            inputHeight = inputSize.Y;
            int[,] inputData = new int[inputWidth, inputHeight];

            localityRadius = localityRad;
            cellsPerCol = cellsPCol;
            segActiveThreshold = segActThreshold;
            newSynapseCount = newSynCount;
            pctLocalActivity = pctLocActivity;
            pctInputPerCol = pctInputPCol;
            pctMinOverlap = pctMinOlap;

            spatialLearning = true;
            temporalLearning = true;

            //Reduce the number of columns and map centers of input x,y correctly
            //For now: column grid will be half-lengths of input grid in both dimensions
            width = colGSize.X;
            height= colGSize.Y;
            xSpace = ((float)inputWidth-1)/((float)width-1);
            ySpace = ((float)inputHeight-1)/((float)height-1);
            columns = new ObservableCollection<Column>();
            //columnGrid operates with nested lists -> 2D-Array
            columnGrid = new List<List<Column>>();

            for (int i = 0; i < width; i++)
            {
                //Create vertical column blocks
                List<Column> yCols = new List<Column>();
                for(int j=0;j<height;j++)
                {
                    //Translate back in original space
                    int srcPosX = (int) Math.Round(i*xSpace);
                    int srcPosY = (int)Math.Round(j*ySpace);

                    Point srcPos = new Point(srcPosX, srcPosY);

                    //Create a column with sourceCoords and GridCoords
                    Column col = new Column(this, srcPos, new Point(i,j));
                    //Add column to vertical block & columns List
                    yCols.Add(col);
                    columns.Add(col);
                }
                //Add vertical block to columnGrid
                columnGrid.Add(yCols);
            }


            //Prepare OutputData-Field
            outputData = new int[columnGrid.Count, columnGrid[0].Count];

            //segmentUpdateList: A list of segmentUpdate structures. 
            //segmentUpdateList(c,i) is the list of changes for cell i in column c.
            segmentUpdateMap = new Dictionary<Cell,List<SegmentUpdateInfo>>();
            recentUpdateMap = new Dictionary<Cell, List<SegmentUpdateInfo>>();

            //calc inputRadius for Columns from localityRadius
            inputRadius = (int) Math.Round(localityRadius * xSpace);

            int synapsesPerSegment;

            //Now connect all potentialSynapses for the Columns (proximal segments)
            if (localityRadius == 0)
            {
                synapsesPerSegment = (int)(inputWidth * inputHeight * pctInputPerCol);
            }
   

            int longerSide = Math.Max(inputWidth, inputHeight);


            InputSpace maxInputSpace = new InputSpace();
            maxInputSpace.minY = 0;
            maxInputSpace.maxY = inputHeight - 1;
            maxInputSpace.minX = 0;
            maxInputSpace.maxX = inputWidth - 1;
            
            //Create Segments with potential synapses for columns
            foreach(Column col in columns)
            {

                col.createProximalSegments(inputRadius, localityRadius, maxInputSpace, longerSide);
            }

            inhibitionRadius = averageReceptiveFieldSize();

            if(localityRadius == 0){
                desiredLocalActivity = (int) Math.Round(inhibitionRadius * pctLocalActivity);
            }
            else{
                desiredLocalActivity = (int)(Math.Pow(localityRadius, 2) * pctLocalActivity);
            }

            desiredLocalActivity = (int) Math.Max(2, desiredLocalActivity); //wenigstens 2...

        }

        /// <summary>
        ///Perform SpatialPooling for the current input in this Region. The result will be a subset of Columns being set as active as well
        ///as (proximal) synapses in all Columns having updated permanences and boosts, and the Region will update inhibitionRadius.
        ///From the Numenta Docs:
        ///Phase 1: 
        ///     Compute the overlap with the current input for each column. Given an input vector, the first phase 
        ///     calculates the overlap of each column with that vector. The overlap for each column is simply the number 
        ///     of connected synapses with active inputs, multiplied by its boost. If this value is below minOverlap, 
        ///     we set the overlap score to zero.
        ///Phase 2: 
        ///     Compute the winning columns after inhibition. The second phase calculates which columns remain as winners after the 
        ///     inhibition step. desiredLocalActivity is a parameter that controls the number of columns that end up winning. 
        ///     For example, if desiredLocalActivity is 10, a column will be a winner if its overlap score is greater than the
        ///     score of the 10'th highest column within its inhibition radius.
        ///Phase 3: 
        ///     Update synapse permanence and internal variables.The third phase performs learning; it updates the permanence values of all 
        ///     synapses as necessary, as well as the boost and inhibition radius.
        ///     The main learning rule is implemented in lines 20-26. For winning columns, if a synapse is active, 
        ///     its permanence value is incremented, otherwise it is decremented. Permanence values are constrained to be between 0 and 1.
        ///     Lines 28-36 implement boosting. There are two separate boosting mechanisms in place to help a column learn connections. 
        ///     If a column does not win often enough (as measured by activeDutyCycle), its overall boost value is 
        ///     increased (line 30-32). Alternatively, if a column's connected synapses do not overlap well with any 
        ///     inputs often enough (as measured by overlapDutyCycle), its permanence values are boosted (line 34-36). 
        ///     
        ///Note: Once learning is turned off, boost(c) is frozen. 
        ///Finally at the end of Phase 3 the inhibition radius is recomputed (line 38).
        /// </summary>
        private void performSpatialPooling()
        {
            //Phase 1: Compute Overlap
            foreach(Column col in columns)
            {
                col.computeOverlap();
            }

            //INHIBITION
            //Phase 2: Compute active columns: 
            foreach (Column col in columns)
            {
                col.computeColumnInhibition(desiredLocalActivity);
            }

            //Phase 3:
            //Todo: hier ebenfalls: Columns werden sehr oft durchlaufen!
            if(spatialLearning==true){
                foreach (Column col in columns)
                { 
                    col.performLearning();
                }
            }

            inhibitionRadius = averageReceptiveFieldSize();
            //inhibitionRadius = 0;
            
        }

        /// <summary>
        /// The radius of the average connected receptive field size of all the columns. 
        /// The connected receptive field size of a column includes only the connected synapses 
        /// (those with permanence values >= connectedPerm). This is used to determine the extent of lateral inhibition between columns.
        /// </summary>
        /// <returns>The average connected receptive field size (in column grid space).</returns>
        private float averageReceptiveFieldSize()
        {
            List<double> dists = new List<double>();
            foreach(Column col in columns)
            {
                foreach(Synapse syn in col.getConnectedSynapses())
                {
                    //Cast to proximalSynapse:
                    ProximalSynapse proxSyn = syn as ProximalSynapse;
                    double d = Math.Sqrt(Math.Pow(col.iPos.X - proxSyn.inputSource.ix,2) +
                        Math.Pow(col.iPos.Y - proxSyn.inputSource.iy, 2));
                    dists.Add(d/ xSpace);
                }
            }

            return (float)dists.Sum() / dists.Count;
        }

        /// <summary>
        ///From the Numenta Docs:
        ///The input to this code is activeColumns(t), as computed by the spatial pooler. 
        ///The code computes the active and predictive state for each cell at the current timestep, t. 
        ///The boolean OR of the active and predictive states for each cell forms the output of the temporal pooler for the next level.
        ///Phase 1: 
        /// Compute the active state, activeState(t), for each cell.
        /// The first phase calculates the activeState for each cell that is in a winning column. 
        /// For those columns, the code further selects one cell per column as the learning cell (learnState). 
        /// The logic is as follows: if the bottom-up input was predicted by any cell (i.e. its predictiveState output was 1 due 
        /// to a sequence segmentUpdateList), then those cells become active (lines 23-27). 
        /// If that segmentUpdateList became active from cells chosen with learnState on, this cell is selected as the learning cell (lines 28-30). 
        /// If the bottom-up input was not predicted, then all cells in the col become active (lines 32-34). 
        /// In addition, the best matching cell is chosen as the learning cell (lines 36-41) and a new segmentUpdateList is added to that cell.
        ///Phase 2:
        /// Compute the predicted state, predictiveState(t), for each cell.
        /// The second phase calculates the predictive state for each cell. A cell will turn on its predictive state output 
        /// if one of its segments becomes active, i.e. if enough of its lateral inputs are currently active due to feed-forward input. 
        /// In this case, the cell queues up the following changes: a) reinforcement of the currently active segmentUpdateList (lines 47-48), 
        /// and b) reinforcement of a segmentUpdateList that could have predicted this activation, i.e. a segmentUpdateList that has a (potentially weak)
        /// match to activity during the previous time step (lines 50-53).
        ///Phase 3:
        ///Update synapses. The third and last phase actually carries out learning. In this phase segmentUpdateList 
        ///updates that have been queued up are actually implemented once we get feed-forward input and the cell is chosen as a learning cell 
        ///(lines 56-57). Otherwise, if the cell ever stops predicting for any reason, we negatively reinforce the segments (lines 58-60).
        /// </summary>
        private void performTemporalPooling()
        {
            //Phase1
            //18. for c in activeColumns(t)
            //19.
            //20.   buPredicted = false
            //21.   lcChosen = false
            //22.   for i = 0 to cellsPerColumn - 1
            //23.     if predictiveState(c, i, t-1) == true then
            //24.       s = getActiveSegment(c, i, t-1, activeState)
            //25.       if s.sequenceSegment == true then
            //26.         buPredicted = true
            //27.         activeState(c, i, t) = 1
            //28.         if segmentActive(s, t-1, learnState) then
            //29.           lcChosen = true
            //30.           learnState(c, i, t) = 1
            //31.
            //32.   if buPredicted == false then
            //33.     for i = 0 to cellsPerColumn - 1
            //34.       activeState(c, i, t) = 1
            //35.
            //36.   if lcChosen == false then
            //37.     i,s = getBestMatchingCell(c, t-1)
            //38.     learnState(c, i, t) = 1
            //39.     sUpdate = getSegmentActiveSynapses (c, i, s, t-1, true)
            //40.     sUpdate.sequenceSegment = true
            //41.     segmentUpdateList.add(sUpdate)
    
            //Phase 1: Compute cell active states and segment learning updates

            bool buPredicted;
            bool lcChosen;

            foreach(Column col in columns)
            {
                if (col.IsActive == true)
                {
                    buPredicted = false;
                    lcChosen = false;
                    foreach(Cell cell in col.cells)
                    {
                        if (cell.wasPredicted == true)
                        {
                            Segment segment = cell.getPreviousActiveSegment();
                            if (segment != null && segment.isSequence)
                            {
                                buPredicted = true;
                                cell.isActive = true;
                                if (temporalLearning && segment.wasActiveFromLearning)
                                {
                                    lcChosen = true;
                                    cell.isLearning = true;
                                }
                            }
                        }
                    }
                    if (!buPredicted)
                    {
                        foreach (Cell cell in col.cells)
                        {
                            cell.isActive = true;
                        }
                    }
                    if (temporalLearning && !lcChosen)
                    {
                        //isSequence=true, previous=true
                        Dictionary<Cell, Segment> tupelCellSeg = col.getBestMatchingCell(true, true);
                        Cell bestCell = tupelCellSeg.Keys.First();
                        Segment bestSegment = tupelCellSeg.Values.First();
                        bestCell.isLearning = true;
                        //previous=true, addNewSynapses=true
                        SegmentUpdateInfo segmentToUpdate = bestCell.getSegmentActiveSynapses(true, bestSegment, true);
                        segmentToUpdate.isSequence = true;

                        //Temporäry buffer
                        List<SegmentUpdateInfo> segList;

                        //Add cell to UpdateMap
                        if (!segmentUpdateMap.ContainsKey(bestCell))
                        {
                            segList = new List<SegmentUpdateInfo>();
                        }
                        else
                        {
                            segList = segmentUpdateMap[bestCell];
                        }

                        segList.Add(segmentToUpdate);
                        segmentUpdateMap[bestCell] = segList;

                        //bestSeg may be partial-sort-of match, but it could dec-perm
                        //other syns from different step if cell overlaps...
                        //try better minOverlap to prevent bad boosting?
                        //try to disable learning if predictions match heavily?
                    }
                }
            }

            //Phase2
            //42. for c, i in cells
            //43.   for s in segments(c, i)
            //44.     if segmentActive(s, t, activeState) then
            //45.       predictiveState(c, i, t) = 1
            //46.
            //47.       activeUpdate = getSegmentActiveSynapses (c, i, s, t, false)
            //48.       segmentUpdateList.add(activeUpdate)
            //49.
            //50.       predSegment = getBestMatchingSegment(c, i, t-1)
            //51.       predUpdate = getSegmentActiveSynapses(
            //52.                                   c, i, predSegment, t-1, true)
            //53.       segmentUpdateList.add(predUpdate)

            foreach (Column col in columns)
            {
                foreach (Cell cell in col.cells)
                {
                    List<Segment> activeSegs = new List<Segment>();

                    //a.) reinforcement of the currently active segments
                    //
                    foreach (Segment seg in cell.segments)
                    {
                        if (seg.isActive == true)
                        {
                            cell.isPredicting = true;

                            if (seg.isSequence)
                            {
                                cell.isSegmentPredicting = true;
                            }

                            activeSegs.Add(seg);

                            if(temporalLearning)
                            {
                                //get all relevant synapses no matter sequence or previously active
                                SegmentUpdateInfo activeSegUpdate = cell.getSegmentActiveSynapses(false, seg, false);
                                List<SegmentUpdateInfo> segList = segmentUpdateMap[cell];
                                if (segList == null)
                                {
                                    segList = new List<SegmentUpdateInfo>();
                                }
                                segList.Add(activeSegUpdate);
                                segmentUpdateMap[cell] = segList;
                            }
                            break;
                        }
                    }

                    //b) reinforcement of a segment that could have predicted 
                    //   this activation, i.e. a segment that has a (potentially weak)
                    //   match to activity during the previous time step (lines 50-53).
                    if (temporalLearning && cell.isPredicting)
                    {
                        //isSequence=false, previous=true
                        Segment predSegment = cell.getBestMatchingSegment(false, true);
                        //if predSegment:
                        // if not predSegment:
                        // print "New predSegment on col ",col.irow,col.icol
                        // elif predSegment not in activeSegs:
                        // print "predSegment update 2x on col",col.irow,col.icol
                        //TODO: if predSegment is None, do we still add new? ok if same as above seg?
                        //previous=true, newSynapses=true

                        SegmentUpdateInfo predSegUpdate = cell.getSegmentActiveSynapses(true, predSegment, true);
                        List<SegmentUpdateInfo> segList = segmentUpdateMap[cell];
                        if (segList == null)
                        {
                            segList = new List<SegmentUpdateInfo>();
                        }
                        segList.Add(predSegUpdate);
                        segmentUpdateMap[cell] = segList;
                        
                    }
                        
                    
                }
            }

            //Phase3
            //54. for c, i in cells
            //55.   if learnState(c, i, t) == 1 then
            //56.     adaptSegments (segmentUpdateList(c, i), true)
            //57.     segmentUpdateList(c, i).delete()
            //58.   else if predictiveState(c, i, t) == 0 and predictiveState(c, i, t-1)==1 then
            //59.     adaptSegments (segmentUpdateList(c,i), false)
            //60.     segmentUpdateList(c, i).delete()
            recentUpdateMap.Clear();
            if (temporalLearning)
            {
                foreach (Column col in columns)
                {
                    foreach(Cell cell in col.cells)
                    {
                        if (segmentUpdateMap.ContainsKey(cell))
                        {
                            if (cell.isLearning)
                            {
                                if(cell.isPredicting)
                                {
                                    System.Diagnostics.Trace.WriteLine("Problem: falsches Lernen");
                                }
                                //positive reinforcement = true
                                adaptSegments(segmentUpdateMap[cell], true);
                                recentUpdateMap[cell] = segmentUpdateMap[cell];
                                segmentUpdateMap[cell].Clear();
                            }
                            else
                            {
                                if (!cell.isPredicting && cell.wasPredicted)
                                {
                                    //Positive reinforcement=false
                                    adaptSegments(segmentUpdateMap[cell], false);
                                    recentUpdateMap[cell] = segmentUpdateMap[cell];
                                    segmentUpdateMap[cell].Clear();
                                }
                            }
                        }
                    }
                }
            }



        }

        /// <summary>
        /// Given the list of columns, return the k'th highest overlap values.
        /// </summary>
        public double kthScore(List<Column> cols, int k)
        {
            List<double> sortedOverlapColumns = new List<double>();
            foreach(Column c in cols)
            {
                sortedOverlapColumns.Add(c.Overlap);
            }
            sortedOverlapColumns.Sort();
         
            //sorted ascending: get k'th-highest overlap
            return sortedOverlapColumns[sortedOverlapColumns.Count - k - 1];
        }


        /// <summary>
        /// Return the list of all the columns that are within inhibitionRadius of the input column.
        /// Neighbors are defined as the columns being around the relevant column as center.
        /// </summary>
        /// <param name="column"></param>
        /// <returns>List of columns  within inhibitionRadius of input column.</returns>
        public List<Column> neighbors(Column column)
        {
            //List for neighbors
            List<Column> neighborList = new List<Column>();

            int irad = (int) Math.Round(inhibitionRadius);
            int x0  = Math.Max(0, Math.Min(column.cPos.X-1, column.cPos.X-irad));
            int y0  = Math.Max(0, Math.Min(column.cPos.Y-1, column.cPos.Y-irad));
            int x1  = Math.Min(width -1 , Math.Max(column.cPos.X+1, column.cPos.X+irad));
            int y1 =  Math.Min(height-1 , Math.Max(column.cPos.Y+1, column.cPos.Y + irad));

            //x1 = Math.Min(columnGrid.Count, x1 + 1);
            //Todo: Hier muss noch Implementation rein, da columnGrid noch nicht definiert
            //y1 = Math.Min(columnGrid[0].Count, y1 + 1);

            //Iterate columnGrid
            for (int i = x0; i<= x1;i++ )
            {
                for (int j = y0; j <= y1; j++)
                {
                    neighborList.Add(columnGrid[i][j]);
                }
            }


            return neighborList;

        }


        /// <summary>
        /// This function iterates through a list of segmentUpdateInfo's and reinforces each segment. For each segmentUpdate element, 
        /// the following changes are performed. If positiveReinforcement is true then synapses on the active 
        /// list get their permanence counts incremented by permanenceInc. All other synapses get their permanence counts 
        /// decremented by permanenceDec. If positiveReinforcement is false, then synapses on the active list get 
        /// their permanence counts decremented by permanenceDec. After this step, any synapses in segmentUpdate 
        /// that do yet exist get added with a permanence count of initialPerm. These new synapses are randomly 
        /// chosen from the set of all cells that have learnState output = 1 at time step t.
        /// </summary>
        /// <param name="segmentUpdateList"></param>
        /// <param name="positiveReinforcement"></param>
        public void adaptSegments(List<SegmentUpdateInfo> segmentUpdateList, bool positiveReinforcement)
        {
            foreach(SegmentUpdateInfo segInfo in segmentUpdateList)
            {
                if (segInfo.segment != null)
                {
                    if (positiveReinforcement)
                    {
                        foreach (Synapse syn in segInfo.segment.synapses)
                        {
                            int anzActSyn = segInfo.activeSynapses.Count;
                            if (segInfo.activeSynapses.Contains(syn))
                            {
                                syn.increasePermance();
                            }
                            else
                            {
                                syn.decreasePermance();
                            }
                        }
                    }
                    else
                    {
                        //Todo: Hier wiederum: alle Synapsen durchlaufen, obwohl schon oben erledigt werden könnte
                        foreach (Synapse syn in segInfo.segment.synapses)
                        {
                            syn.decreasePermance();
                        }
                    }
                }

                //Erzeugen neuer Synapse zu ausgewählten Lernzellen
                Segment segment = segInfo.segment;
                if (segInfo.addNewSynapses && positiveReinforcement)
                {
                    if (segInfo.segment == null)
                    {
                        if (segInfo.lcsChosen.Count > 0)
                        {
                            //Creates segment with according synapses. Input sources
                            //provided by lcsChosen (learning cell) list
                            segment = segInfo.cell.createSegment(segInfo.lcsChosen);
                            //Just add distal synapses
                            List<DistalSynapse> distalSynapses = new List<DistalSynapse>();
                            foreach (Synapse syn in segment.synapses)
                            {
                                if (syn.GetType().Equals(typeof(DistalSynapse)))
                                {
                                    distalSynapses.Add(syn as DistalSynapse);
                                }
                            }
                            //Synapses are added to seginfo -> only used for drawing i guess
                            segInfo.addedSynapses = distalSynapses;
                            //segment isSequence according to SegInfo
                            segment.isSequence = segInfo.isSequence;
                        }
                    }
                    else
                    {
                        if (segInfo.lcsChosen.Count > 0)
                        {
                            List<DistalSynapse> added = segInfo.segment.createSynpasesToLearningCells(segInfo.lcsChosen);
                            segInfo.addedSynapses = added;
                        }
                    }
                }
            }
        }
        #region IRegionProcessor

        /// <summary>
        /// Update the values of the inputData for this Region by copying row references from the specified newInput parameter.
        /// </summary>
        /// <param name="newInput">2d Array used for next Region time step. The newInput array must 
        /// have the same shape as the original inputData.</param>
        public void SetInput(int[,] inputArray)
        {
            int newWidth, newHeight, oldWidth, oldHeight;

            newWidth = inputArray.GetLength(0);
            newHeight = inputArray.GetLength(1);

            if (inputData != null)
            {
                oldWidth = inputData.GetLength(0);
                oldHeight = inputData.GetLength(1);

                //Copy only secured same shapes
                if (newWidth == oldWidth && newHeight == oldHeight)
                {
                    inputData = inputArray;
                }
            }
            else{
                //Start with copy
                inputData = inputArray;
            }

        }

        /// <summary>
        /// Determine the output bit-matrix of the most recently run time step for this Region.  
        /// The Region output is a 2d numpy array representing all columns present in the Region.  
        /// Bits are set to 1 if a Column is active or it contains at least 1 predicting cell, all other bits are 0. The output data
        /// will be a 2d numpy array of dimensions corresponding the column grid for this Region.  
        /// Note: the Numenta doc suggests the Region output should potentially include bits for each individual cell.  
        /// My first-pass implementation is Column only for now since in the case or 2 or 3 cells, the spatial positioning of the original 
        /// grid shape can become lost and I'm not sure yet how desirable this is or isn't for the case of video input).
        /// </summary>
        /// <returns>a 2d numpy array of same shape as the column grid containing the Region's collective output.</returns>
        public int[,] GetOutPut()
        {
            foreach (Column col in columns)
            {
                if (col.IsActive)
                {
                    outputData[col.cPos.X, col.cPos.Y] = 1;
                }
                else
                {
                    foreach (Cell cell in col.cells)
                    {
                        if (cell.isPredicting)
                        {
                            outputData[col.cPos.X, col.cPos.Y] = 1;
                            break;
                        }
                    }
                }
            }

            return outputData;
        }

        /// <summary>
        /// Run one time step iteration for this Region.  All cells will have their current (last run) state pushed back to be their 
        /// new previous state and their new current state reset to no activity.  
        /// Then SpatialPooling following by TemporalPooling is performed for one time step.
        /// </summary>
        public void Step()
        {
            List<double> dists = new List<double>();
            foreach (Column col in columns)
            {
                foreach (Cell cell in col.cells)
                {
                    cell.nextTimeStep();
                }

                //Compute Column statistics:
                col.ComputeBasicStatistics();

                //Reset Selection of DataGrid-Columns:
                col.IsDataGridSelected = false;
            }

            //Compute Region statistics
            ComputeBasicStatistics();

            //Start pooling
            performSpatialPooling();
            performTemporalPooling();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }


        #endregion


        #region PropertyChanged-Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region IStatistics

        private float stepCounter;
        public float StepCounter
        {
            get { return stepCounter; }
            set
            {
                stepCounter = value;
                OnPropertyChanged("StepCounter");
            }
        }

        private float activityCounter;
        public float ActivityCounter
        {
            get { return activityCounter; }
            set
            {
                activityCounter = value;
                OnPropertyChanged("ActivityCounter");
            }
        }

        private float activityRate;
        public float ActivityRate
        {
            get { return activityRate; }
            set
            {
                activityRate = value;
                OnPropertyChanged("ActivityRate");
            }
        }

        private float predictionCounter;
        public float PredictionCounter
        {
            get { return predictionCounter; }
            set
            {
                predictionCounter = value;
                OnPropertyChanged("PredictionCounter");
            }
        }

        private float correctPredictionCounter;
        public float CorrectPredictionCounter
        {
            get { return correctPredictionCounter; }
            set
            {
                correctPredictionCounter = value;
                OnPropertyChanged("CorrectPredictionCounter");
            }
        }

        private float segmentPredictionCounter;
        public float SegmentPredictionCounter
        {
            get { return segmentPredictionCounter; }
            set
            {
                segmentPredictionCounter = value;
                OnPropertyChanged("SegmentPredictionCounter");
            }
        }

        private float correctSegmentPredictionCounter;
        public float CorrectSegmentPredictionCounter
        {
            get { return correctSegmentPredictionCounter; }
            set
            {
                correctSegmentPredictionCounter = value;
                OnPropertyChanged("CorrectSegmentPredictionCounter");
            }
        }


        private float predictPrecision;
        public float PredictPrecision
        {
            get { return predictPrecision; }
            set
            {
                predictPrecision = value;
                OnPropertyChanged("PredictPrecision");
            }
        }

        public void InitializeStatisticParameters()
        {
            StepCounter = 0;
            PredictionCounter = 0;
            segmentPredictionCounter = 0;
            ActivityCounter = 0;
            CorrectPredictionCounter = 0;
            CorrectSegmentPredictionCounter = 0;
            ActivityRate = 0.0f;
            PredictPrecision = 0.0f;
        }

        public void ComputeBasicStatistics()
        {
            StepCounter++;

            //Calculates Column-Activity Rate
            ActivityRate = ActivityCounter / (StepCounter*columns.Count);

            //Regional Precision-Counter
            if (SegmentPredictionCounter > 0)
            {
                PredictPrecision = (CorrectSegmentPredictionCounter / SegmentPredictionCounter);
            }
 
        }

        #endregion

        #region INotifyPropertyChanged-Handler

        /// <summary>
        /// Hanlder to raise PC-Event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

    } // of class


    /// <summary>
    /// Represent a single input bit from an external source.
    /// </summary>
    public class InputCell
    {
        public int ix {get; set;}
        public int iy { get; set; }
        public Region CRegion { get; set; }

        //Property isActive
        public bool isActive 
        { 
            get
            {
                int value = CRegion.InputData[ix, iy];
                return (value==1)?true:false;
            }
        }

        public InputCell(int x, int y, Region reg)
        {
            ix = x;
            iy = y;
            CRegion = reg;
        }
    }
}
