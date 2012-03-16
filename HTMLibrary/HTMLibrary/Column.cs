using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using HTM.HTMInterfaces;
using HTM.HTMAnalytics;

///
/// @created 21.3.2011
/// Taken from first approach by Mutakarnich in Python
/// Adapted by Uwe Kirschenmann
/// @author Barry Mutakarnich, Uwe Kirschenmann
///


namespace HTM.HTMLibrary
{


    public struct InputSpace 
    {
        public int minX, maxX;
        public int minY, maxY;

        public int area;

        /// <summary>
        /// Compute area from InputSpace
        /// </summary>
        public void computeInputArea()
        {
            area =  (maxX - minX + 1) * (maxY - minY + 1);
        }

    }

    /// <summary>
    /// Represents a single column of cells within an HTM Region. 
    /// </summary>
    public class Column : IStatisticsColumns, INotifyPropertyChanged, IDataGridSelected
    {
        private const float EMA_ALPHA = 0.005f;

        //Refactored from Region
        const int RAD_BIAS_PEAK = 2;
        const float RAD_BIAS_STD_DEV = 0.25f;

        private float overlapDutyCycle;

        public Point iPos { get; set; }
        public Point cPos { get; set; }

        public Region region { get; set; }
        public float activeDutyCycle { get; set; }
        public Segment proximalSegment { get; set; }
        //public List<Cell> cells{get;set;}
        public ObservableCollection<Cell> cells{get; set;}

        #region PropertyChanged-Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion



        #region DataGrid-ColumnSelection

        public bool IsDataGridSelected { get; set; }

        #endregion


        #region RefactorProximalSynpasesinColumn

        /// <summary>
        /// Calculate the input space for each column as a square below the column center.
        /// Can differ from column to column
        /// </summary>
        private InputSpace colInputSpace;
        public InputSpace ColInputSpace
        {
            get
            {
                return colInputSpace;
            }
            set
            {
                colInputSpace = value;
            }
        }

        /// <summary>
        /// Copies Area form InputSpace for DataGrid
        /// </summary>
        private int inputArea;
        public int InputArea
        {
           get
           {
                return inputArea;
           }
           set
           {
                inputArea = value;              
                OnPropertyChanged("InputArea");
           }
        }

        /// <summary>
        /// Toggle whether or not this Column is currently active.
        /// </summary>
        private bool isActive;
        public bool IsActive 
        { 
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        /// <summary>
        /// Boost-Factor
        /// </summary>
        private float boost;
        public float Boost
        {
            get
            {
                return boost;
            }
            set
            {
                boost = value;
                OnPropertyChanged("Boost");
            }
        }

        /// <summary>
        /// The spatial pooler overlap with a particular input pattern. Just connected proximal synapses 
        /// count to overlap.
        /// </summary>
        private double overlap;
        public double Overlap
        {
            get
            {
                return overlap;
            }
            set
            {
                overlap = value;
                OnPropertyChanged("Overlap");
            }
        }

        /// <summary>
        /// MinOverlap-Value established by input parameters and receptive-field size. 
        /// Changes for each column
        /// </summary>
        private double minOverlap;
        public double MinOverlap
        {
            get
            {
                return minOverlap;
            }
            set
            {
                minOverlap = value;
                OnPropertyChanged("MinOverlap");
            }
        }


        /// <summary>
        /// MinLocal Activity for column to become active neighborhood-check
        /// </summary>
        private double minLocalActivity;
        public double MinLocalActivity {
            get
            {
                return minLocalActivity;
            }
            set
            {
                minLocalActivity = value;
                OnPropertyChanged("MinLocalActivity");
            }
        }

        /// <summary>
        /// Results from computeColumnInhibition. 
        /// Only possible if Overlap>MinOverlap but Overlap LT MinLocalActivity.
        /// </summary>
        private bool isInhibited;
        public bool IsInhibited
        {
            get
            {
                return isInhibited;
            }
            set
            {
                isInhibited = value;
                OnPropertyChanged("IsInhibited");
            }
        }

        #endregion





        #region IStatisticsColumn

        /* These are the same implementations as for Cell*/
        private float cellStepCounter;
        public float StepCounter
        {
            get { return cellStepCounter; }
            set
            {
                cellStepCounter = value;
                OnPropertyChanged("StepCounter");
            }
        }

        private float cellActivityCounter;
        public float ActivityCounter
        {
            get { return cellActivityCounter; }
            set
            {
                cellActivityCounter = value;
                OnPropertyChanged("ActivityCounter");
            }
        }

        private float cellActivityRate;
        public float ActivityRate
        {
            get { return cellActivityRate; }
            set
            {
                cellActivityRate = value;
                OnPropertyChanged("ActivityRate");
            }
        }

        private float cellPredictionCounter;
        public float PredictionCounter
        {
            get { return cellPredictionCounter; }
            set
            {
                cellPredictionCounter = value;
                OnPropertyChanged("PredictionCounter");
            }
        }

        private float cellCorrectPredictionCounter;
        public float CorrectPredictionCounter
        {
            get { return cellCorrectPredictionCounter; }
            set
            {
                cellCorrectPredictionCounter = value;
                OnPropertyChanged("CorrectPredictionCounter");
            }
        }

        private float cellSegmentPredictionCounter;
        public float SegmentPredictionCounter
        {
            get { return cellSegmentPredictionCounter; }
            set
            {
                cellSegmentPredictionCounter = value;
                OnPropertyChanged("SegmentPredictionCounter");
            }
        }

        private float cellCorrectSegmentPredictionCounter;
        public float CorrectSegmentPredictionCounter
        {
            get { return cellCorrectSegmentPredictionCounter; }
            set
            {
                cellCorrectSegmentPredictionCounter = value;
                OnPropertyChanged("CorrectSegmentPredictionCounter");
            }
        }

        private float cellPredictPrecision;
        public float PredictPrecision
        {
            get { return cellPredictPrecision; }
            set
            {
                cellPredictPrecision = value;
                OnPropertyChanged("PredictPrecision");
            }
        }

        private float cellLearningCounter;
        public float LearningCounter
        {
            get { return cellLearningCounter; }
            set
            {
                cellLearningCounter = value;
                OnPropertyChanged("LearningCounter");
            }
        }


        private float cellNumberSegments;
        public float NumberSegments
        {
            get { return cellNumberSegments; }
            set
            {
                cellNumberSegments = value;
                OnPropertyChanged("NumberSegments");
            }
        }


        private float cellMaxSynapseCount;
        public float MaxSynapseCount
        {
            get { return cellMaxSynapseCount; }
            set
            {
                cellMaxSynapseCount = value;
                OnPropertyChanged("MaxSynapseCount");
            }
        }

        /* Max Values and Column-Specific*/
        private float columnActivityCounter;
        public float ColumnActivityCounter
        {
            get { return columnActivityCounter; }
            set
            {
                columnActivityCounter = value;
                OnPropertyChanged("ColumnActivityCounter");
            }
        }

        private float columnMaxSynapseCount;
        public float ColumnMaxSynapseCount
        {
            get { return columnMaxSynapseCount; }
            set
            {
                columnMaxSynapseCount = value;
                OnPropertyChanged("ColumnMaxSynapseCount");
            }
        }

        private float maxNumberSegments;
        public float MaxNumberSegments
        {
            get { return maxNumberSegments; }
            set
            {
                maxNumberSegments = value;
                OnPropertyChanged("MaxNumberSegments");
            }
        }

        private float maxLearningCounter;
        public float MaxLearningCounter
        {
            get { return maxLearningCounter; }
            set
            {
                maxLearningCounter = value;
                OnPropertyChanged("MaxLearningCounter");
            }
        }

        private float maxCellActivityCounter;
        public float MaxCellActivityCounter
        {
            get { return maxLearningCounter; }
            set
            {
                maxCellActivityCounter = value;
                OnPropertyChanged("MaxCellActivityCounter");
            }
        }

        private float maxPredictionCounter;
        public float MaxPredictionCounter
        {
            get { return maxPredictionCounter; }
            set
            {
                maxPredictionCounter = value;
                OnPropertyChanged("MaxPredictionCounter");
            }
        }

        private float maxCorrectPredictionCounter;
        public float MaxCorrectPredictionCounter
        {
            get { return maxCorrectPredictionCounter; }
            set
            {
                maxCorrectPredictionCounter = value;
                OnPropertyChanged("MaxCorrectPredictionCounter");
            }
        }

        #endregion

        /// <summary>
        /// Construct a new Column for the given parent region at source row/column 
        /// position srcPos and column grid position pos.
        /// </summary>
        /// <param name="region">The parent Region this Column belongs to.</param>
        /// <param name="srcPos">A Point (srcX,srcY) of this Column's 'center' position in terms of the proximal-synapse input space.</param>
        /// <param name="pos">A Point(x,y) of this Column's position within the Region's column grid.</param>
        public Column(Region region, Point srcPos, Point pos)
        {
            this.region = region;

            IsActive = false;
            boost = 1.0f;
            activeDutyCycle = 1;
            overlapDutyCycle = 1.0f;
            overlap = 0;

            //Fill Column with contextual cells
            cells = new ObservableCollection<Cell>();

            for (int i = 0; i < region.cellsPerCol; i++)
            {
                cells.Add(new Cell(this, i));
            }

            //Create Segment and add:
            proximalSegment = new Segment(region.segActiveThreshold);

            //Add Position:
            iPos = srcPos;
            cPos = pos;
        }

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
        /// <param name="inputRadius"></param>
        /// <param name="localityRadius"></param>
        /// <param name="maxInputSpace"></param>
        /// <param name="longerSide"></param>
        public void createProximalSegments(int inputRadius, int localityRadius, InputSpace maxInputSpace, int longerSide)
        {

            colInputSpace = maxInputSpace;

            
            if (localityRadius > 0)
            {
                //Compute values of inputSquare
                //Cut radius on frontiers
                colInputSpace.minY = Math.Max(0, iPos.Y - region.inputRadius);
                colInputSpace.maxY = Math.Min(region.inputHeight - 1, iPos.Y + region.inputRadius);
                colInputSpace.minX = Math.Max(0, iPos.X - region.inputRadius);
                colInputSpace.maxX = Math.Min(region.inputWidth - 1, iPos.X + region.inputRadius);

            }

            //Compute area
            colInputSpace.computeInputArea();
            InputArea = ColInputSpace.area;
            //System.Diagnostics.Trace.WriteLine("Area: " + colInputSpace.area);


            //Proximal synapses pro Column (input segment)
            int synapsesPerSegment = (int)(colInputSpace.area * region.pctInputPerCol);

            //The minimum number of inputs that must be active for a column to be 
            //considered during the inhibition step.
            //Being the quotient per 
            minOverlap = Math.Round(synapsesPerSegment * region.pctMinOverlap/InputArea,2);
            //Overlap must be at least 1
            if (minOverlap == 0)
            {
                minOverlap = 0.1f;
            }


            //Be sure to sample unique input positions to connect to synapses

            //Get all Positions
            List<Point> allPos = new List<Point>();
            for (int i = colInputSpace.minY; i <= colInputSpace.maxY; i++)
            {
                for (int j = colInputSpace.minX; j <= colInputSpace.maxX; j++)
                {
                    Point srcPos = new Point(j, i);
                    allPos.Add(srcPos);
                }
            }

            //Draw random sample from allPos-Indices:
            List<Point> samplePoints = new List<Point>();
            int lower = 0;
            int upper = allPos.Count; //Achtung: Upper-Bound in Math.Rand exclusive!

            //Some extra code needed as we should draw unique values
            List<int> uniqueCheck = new List<int>();


            //Put unique values to samplePoints
            for (int i = 0; i < synapsesPerSegment; i++)
            {
                int prevIndex = MathHelper.r.Next(lower, upper);
                while (uniqueCheck.Contains(prevIndex))
                {
                    prevIndex = MathHelper.r.Next(lower, upper);
                }


                //Now add
                uniqueCheck.Add(prevIndex);
                samplePoints.Add(allPos[prevIndex]);
            }

            foreach (Point pos in samplePoints)
            {
                InputCell inputCell = new InputCell(pos.X, pos.Y, region);
                double permanence = Math.Max(0, MathHelper.Normal(Synapse.CONNECTED_PERM, Synapse.PERMANENCE_INC));
                double distance = Math.Sqrt(Math.Pow(iPos.X - pos.X, 2) + Math.Pow(iPos.Y - pos.Y, 2));
                //double localityBias = RAD_BIAS_PEAK/0.4 * Math.Exp(Math.Pow(distance/(longerSide*RAD_BIAS_STD_DEV),2)/-2);
                double localityBias = RAD_BIAS_PEAK / 2.0f * Math.Exp(Math.Pow(distance / (longerSide * RAD_BIAS_STD_DEV), 2) / -2);
                double permanenceBiased = Math.Min(1.0f, permanence * localityBias);
                ProximalSynapse synapse = new ProximalSynapse(inputCell, permanenceBiased);
                //System.Diagnostics.Trace.WriteLine("Distanz: " + distance);
                //System.Diagnostics.Trace.WriteLine("Bias: " + localityBias);
                //System.Diagnostics.Trace.WriteLine("Permanenz: " + permanence);
                //System.Diagnostics.Trace.WriteLine("Proxi-Permanenz: " + synapse.permanence);
                proximalSegment.addSynapse(synapse);
            }
        }

        

        /// <summary>
        /// Return the (last computed) input overlap for this Column in terms of the 
        /// percentage of active synapses out of total existing synapses
        /// </summary>
        /// <returns>Last active synapses</returns>
        public float getOverlapPercentage()
        {
            return (float)overlap / proximalSegment.synapses.Count;
        }


        /// <summary>
        /// Return the list of all currently connected proximal synapses for  this Column.
        /// </summary>
        /// <returns>List of connected proximal synapses</returns>
        public List<Synapse> getConnectedSynapses()
        {
            return proximalSegment.getConnectedSynapses();
        }

        /// <summary>
        ///  For this column, return the cell with the best matching segmentUpdateList (at time t-1 if prevous=True else at time t). 
        ///  Only consider sequence segments if isSequence is True, otherwise only consider non-sequence segments. If no cell has a 
        ///  matching segmentUpdateList, then return the cell with the fewest number of segments.@return 
        ///  </summary>
        /// <returns>List containing the best cell and its best segmentUpdateList (may be None).</returns>
        public Dictionary<Cell, Segment> getBestMatchingCell(bool isSequence, bool previous)
        {
            Cell bestCell = null;
            Segment bestSegment = null;
            int bestCount = 0;
            int synCount;
            int fewestCount=Int16.MaxValue;
            Dictionary<Cell, Segment> dictCelSeg = new Dictionary<Cell, Segment>();

            //Todo: Hier muss nachgesehen werden: es wird wiederum das Segment durchlaufen, obwohl schon vorher passiert!
            foreach(Cell cell in cells)
            {
                Segment seg = cell.getBestMatchingSegment(isSequence, previous);
                if(seg!=null)
                {
                    if(previous)
                    {
                        synCount = seg.getPrevActiveSynapses(false).Count;
                    }
                    else
                    {
                        synCount = seg.getActiveSynapses(false).Count;
                    }

                    if(synCount > bestCount)
                    {
                        bestCell = cell;
                        bestSegment = seg;
                        bestCount = synCount;
                    }
                }
            }

            //if there are no active sequences, run for segment count, no matter active or not
            if(bestCell==null)
            {
                foreach (Cell cell in cells)
                {
                    int actCount = cell.segments.Count;
                    if (actCount < fewestCount)
                    {
                        fewestCount = actCount;
                        bestCell = cell;
                    }
                }
            }

            dictCelSeg.Add(bestCell, bestSegment);
            return dictCelSeg;

        }

        /// <summary>
        /// The spatial pooler overlap of this column with a particular input pattern.
        /// The overlap for each column is simply the number of connected synapses with active 
        /// inputs, multiplied by its boost. If this value is below minOverlap, we set the 
        /// overlap score to zero.
        /// Attention: refactored regarding minOverlap from column: overlap is now computed as the former overlap per area
        /// as this will make areas with inequal size comparable
        /// </summary>
        public void computeOverlap()
        {
            //Just connnected synapses allowed
            double activeProxSynapses = proximalSegment.getActiveSynapses(true).Count;
            double overlapTemp = Math.Round(activeProxSynapses/InputArea,2);

            if (overlapTemp < minOverlap)
            {
                overlapTemp = 0;
            }
            else
            {
                overlapTemp *= boost;

            }

            Overlap = Math.Round(overlapTemp,2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void computeColumnInhibition(int desiredLocalActivity)
        {
            IsActive = false;
            IsInhibited = false;

            if (Overlap > 0)
            {
                MinLocalActivity = region.kthScore(region.neighbors(this), desiredLocalActivity);
                //if (Overlap >= MinLocalActivity)
                if (Overlap >= 0.0)
                {
                    IsActive = true;
                }
                else
                {
                    IsInhibited = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void performLearning()
        {
            if (IsActive == true)
            {
                updatePermanences();
            }

            performBoosting();
        }

        /// <summary>
        /// Update the permanence value of every synapse in this column based on whether active.
        /// This is the main learning rule (for the column's proximal dentrite). 
        /// For winning columns, if a synapse is active, its permanence value is incremented, 
        /// otherwise it is decremented. Permanence values are constrained to be between 0 and 1.
        /// </summary>
        public void updatePermanences()
        {
            foreach(Synapse syn in proximalSegment.synapses)
            {
                if (syn.isActiveNotConnected || syn.isActive)
                {
                    syn.increasePermance();
                }
                else
                {
                    syn.decreasePermance();
                }
            }
        }


        /// <summary>
        /// There are two separate boosting mechanisms in place to help a column learn connections. If a column does not win often 
        /// enough (as measured by activeDutyCycle), its overall boost value is increased (line 30-32). 
        /// Alternatively, if a column's connected synapses do not overlap well with any inputs often enough (as measured by 
        /// overlapDutyCycle), its permanence values are boosted (line 34-36). 
        /// Note: once learning is turned off, boost(c) is frozen.
        /// </summary>
        public void performBoosting()
        {
            //minDutyCycle(c) A variable representing the minimum desired firing rate for a cell. 
            //If a cell's firing rate falls below this value, it will be boosted. This value is 
            //calculated as 1% of the maximum firing rate of its neighbors.
            float minDutyCycle = 0.01f * maxDutyCycle(region.neighbors(this));
            updateActiveDutyCycle();
            Boost = boostFunction(minDutyCycle);
            updateOverlapDutyCycle();

            if (overlapDutyCycle < minDutyCycle)
            {
                increasePermanences(0.1f * Synapse.CONNECTED_PERM);
            }
        }

        /// <summary>
        /// Returns the maximum active duty cycle of the columns in the given list of columns.
        /// </summary>
        /// <param name="cols"></param>
        /// <returns>Maximum active duty cycle</returns>
        public float maxDutyCycle(List<Column> cols)
        {
            List<float> maxDutyList = new List<float>();
            foreach(Column col in cols)
            {
                maxDutyList.Add(col.activeDutyCycle);
            }

            return maxDutyList.Max();
        }

        /// <summary>
        /// Computes a moving average of how often this column has been active 
        /// after inhibition.
        /// </summary>
        public void updateActiveDutyCycle()
        {
            float newCycle = (1.0f - EMA_ALPHA) * activeDutyCycle;
            if (IsActive)
            {
                newCycle += EMA_ALPHA;
            }

            activeDutyCycle = newCycle;
        }

        /// <summary>
        /// Computes a moving average of how often this column has overlap greater than minOverlap.
        /// Exponential moving average (EMA):
        /// St = a * Yt + (1-a)*St-1
        /// </summary>
        public void updateOverlapDutyCycle()
        {
            float newCycle = (1.0f - EMA_ALPHA) * overlapDutyCycle;
            if (overlap > minOverlap)
            {
                newCycle += EMA_ALPHA;
            }
            overlapDutyCycle = newCycle;
        }


        /// <summary>
        /// Returns the boost value of this column. The boost value is a scalar >= 1. 
        /// If activeDutyCyle(c) is above minDutyCycle(c), the boost value is 1. 
        /// The boost increases linearly once the column's activeDutyCycle starts falling below its minDutyCycle.
        /// </summary>
        /// <param name="minDutyCycle"></param>
        /// <returns>boostValue</returns>
        public float boostFunction(float minDutyCycle)
        {
            if (activeDutyCycle > minDutyCycle)
            {
                return 1.0f;
            }
            else if (activeDutyCycle  == minDutyCycle)
            {
                return boost * 1.05f; //fix at +5%
            }

            return minDutyCycle / activeDutyCycle;
        }

        /// <summary>
        /// Increase the permanence value of every synapse in this column by a scale factor
        /// </summary>
        /// <param name="scale"></param>
        public void increasePermanences(float scale)
        {
            foreach(Synapse syn in proximalSegment.synapses)
            {
                syn.increasePermance(scale);
            }
        }
  

        #region IStatistics-Implementation


        /// <summary>
        /// Computes statistics on column level from acumulated cells. 
        /// The parameters are updated by the cells stepping.
        /// </summary>
        public void ComputeBasicStatistics()
        {
            StepCounter++;

            //ColumnActivity
            if (IsActive)
            {
                ColumnActivityCounter++;
                region.ActivityCounter++;
            }
            ActivityRate = ColumnActivityCounter / StepCounter;

            //Basics
            if (SegmentPredictionCounter > 0)
            {
                PredictPrecision = (CorrectSegmentPredictionCounter / SegmentPredictionCounter);
            }
 

            //temporary lists
            List<float> listMaxCellActivityCounter = new List<float>();
            List<float> listMaxCorrectPredictionCounter = new List<float>();
            List<float> listMaxLearningCounter = new List<float>();
            List<float> listMaxNumberSegments = new List<float>();
            List<float> listMaxPredictionCounter = new List<float>();
            List<float> listColumnMaxSynapseCount = new List<float>();

            //Get Max-Values from Cells
            foreach (Cell cell in cells)
            {
                listMaxCellActivityCounter.Add(cell.ActivityCounter);
                listMaxCorrectPredictionCounter.Add(cell.CorrectPredictionCounter);
                listMaxLearningCounter.Add(cell.LearningCounter);
                listMaxNumberSegments.Add(cell.NumberSegments);
                listMaxPredictionCounter.Add(cell.PredictionCounter);
                listColumnMaxSynapseCount.Add(cell.MaxSynapseCount);
            }

            //Sort the lists
            listMaxCellActivityCounter.Sort();
            listMaxCorrectPredictionCounter.Sort();
            listMaxLearningCounter.Sort();
            listMaxNumberSegments.Sort();
            listMaxPredictionCounter.Sort();
            listColumnMaxSynapseCount.Sort();

            //Get Max values:
            MaxCellActivityCounter = listMaxCellActivityCounter.Last();
            MaxCorrectPredictionCounter = listMaxCorrectPredictionCounter.Last();
            MaxLearningCounter = listMaxLearningCounter.Last();
            MaxNumberSegments = listMaxNumberSegments.Last();
            MaxPredictionCounter = listMaxPredictionCounter.Last();
            MaxSynapseCount = listColumnMaxSynapseCount.Last();

        }

        public void InitializeStatisticParameters()
        {

            ActivityCounter = 0;
            StepCounter = 0;
            PredictionCounter = 0;
            SegmentPredictionCounter = 0;
            ActivityCounter = 0;
            LearningCounter = 0;
            CorrectPredictionCounter = 0;
            CorrectSegmentPredictionCounter = 0;
            ActivityRate = 0.0f;
            PredictPrecision = 0.0f;
            NumberSegments = 0.0f;
            ColumnActivityCounter = 0.0f;
            MaxCellActivityCounter = 0.0f;
            MaxCorrectPredictionCounter = 0.0f;
            MaxLearningCounter = 0.0f;
            MaxNumberSegments = 0.0f;
            MaxPredictionCounter = 0.0f;
            ColumnMaxSynapseCount = 0.0f;


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
    }
}
