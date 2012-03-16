using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using HTM.HTMInterfaces;

///
/// @created 21.3.2011
/// Taken from first approach by Mutakarnich in Python
/// Adapted by Uwe Kirschenmann
/// @author Barry Mutakarnich, Uwe Kirschenmann
///

namespace HTM.HTMLibrary
{
    /// <summary>
    /// A data structure representing a synapse. Contains a permanence value and the 
    /// source input index.  Also contains a 'location' in the input space that this synapse
    /// roughly represents.
    /// </summary>
    public class Cell : IStatisticsCells, INotifyPropertyChanged, IDataGridSelected
    {
        private const int MIN_SYNAPSES_PER_SEGMENT_THRESHOLD = 1;


        #region Properties

        /// <summary>
        /// Position in Column
        /// </summary>
        public int Index { get; set; }

        private bool isCellActive;
        public bool isActive
        {
            get { return isCellActive; }
            set
            {
                isCellActive = value;
                OnPropertyChanged("isActive");
            }
        }

        private bool wasCellActive;
        public bool wasActive
        {
            get { return wasCellActive; }
            set
            {
                wasCellActive = value;
                OnPropertyChanged("wasActive");
            }
        }
        
        private bool isCellLearning;
        public bool isLearning
        {
            get { return isCellLearning; }
            set
            {
                isCellLearning = value;
                OnPropertyChanged("isLearning");
            }
        }

        private bool wasCellLearning;
        public bool wasLearning
        {
            get { return wasCellLearning; }
            set
            {
                wasCellLearning = value;
                OnPropertyChanged("wasLearning");
            }
        }


        private bool isCellPredicting;
        public bool isPredicting
        {
            get { return isCellPredicting; }
            set
            {
                isCellPredicting = value;
                OnPropertyChanged("isPredicting");
            }
        }

        private bool isCellSegmentPredicting;
        public bool isSegmentPredicting
        {
            get { return isCellSegmentPredicting; }
            set
            {
                isCellSegmentPredicting = value;
                OnPropertyChanged("isSegmentPredicting");
            }
        }

        private bool wasCellSegmentPredicted;
        public bool wasSegmentPredicted
        {
            get { return wasCellSegmentPredicted; }
            set
            {
                wasCellSegmentPredicted = value;
                OnPropertyChanged("wasSegmentPredicted");
            }
        }

        private bool wasCellPredicted;
        public bool wasPredicted
        {
            get { return wasCellPredicted; }
            set
            {
                wasCellPredicted = value;
                OnPropertyChanged("wasPredicted");
            }
        }


        public List<Segment> segments { get; set; }
        public Column column { get; set; }


        public bool IsDataGridSelected { get; set; }

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

        #endregion


        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Create a new Cell belonging to the specified Column. The index is an 
        /// integer id to distinguish this Cell from others in the Column.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="intdx"></param>
        public Cell(Column col, int intdx)
        {
            column = col;
            Index = intdx;
            isActive = false;
            wasActive = false;
            isPredicting = false;
            wasPredicted = false;
            isLearning = false;
            wasLearning = false;
            segments = new List<Segment>();

            //Initialize Paramters for Statistics
            InitializeStatisticParameters();
            
        }


        /// <summary>
        ///  Advance this cell to the next time step. The current state of this cell 
        /// (active, learning, predicting) will be set as the previous state and the current
        /// state will be reset to no cell activity by default until it can be determined. 
        /// </summary>
        public void nextTimeStep()
        {

            //Compute basic statistcs -> IStatistics
            ComputeBasicStatistics();

            wasPredicted = isPredicting;
            wasSegmentPredicted = isSegmentPredicting;
            wasActive = isActive;
            wasLearning = isLearning;
            isPredicting = false;
            isSegmentPredicting = false;
            isActive = false;
            isLearning = false;

        }

      
        /// <summary>
        /// Create a new segmentUpdateList for this Cell. The new segmentUpdateList will initially connect to
        /// at most newSynapseCount synapses randomly selected from the set of cells that
        /// were in the learning state at t-1 (specified by the learningCells parameter).
        /// </summary>
        /// <param name="List">learningCells: the set of available learning cells to add to the segmentUpdateList.</param>
        /// <returns> Created segmentUpdateList</returns>
        public Segment createSegment(List<Cell> learningCells)
        {
            Segment newSegment = new Segment(column.region.segActiveThreshold);
            newSegment.createSynpasesToLearningCells(learningCells);
            segments.Add(newSegment);
            return newSegment;
        }

        /// <summary>
        /// For this cell, return a segmentUpdateList that was active in the previous time
        /// step. If multiple segments were active, sequence segments are given preference. 
        /// Otherwise, segments with most activity are given preference.
        /// </summary>
        public Segment getPreviousActiveSegment()
        {
            List<Segment> activeSegs = new List<Segment>();
            foreach(Segment seg in segments)
            {
                if (seg.wasActive == true)
                {
                    activeSegs.Add(seg);
                }
            }

            if (activeSegs.Count == 1)
            {
                return activeSegs[0];
            }
            else if (activeSegs.Count > 1)
            {
                List<Segment> segSequence = new List<Segment>();
                foreach (Segment segAc in activeSegs)
                {
                    if (segAc.isSequence == true)
                    {
                        segSequence.Add(segAc);
                    }
                }

                if (segSequence.Count == 1)
                {
                    return segSequence[0];
                }
                else if (segSequence.Count > 1)
                {
                    activeSegs = segSequence;
                }

                //if multiple possible segments, return segment with most activity
                Dictionary<int, Segment> maxActives = new Dictionary<int,Segment>();
                foreach (Segment segMax in activeSegs)
                {
                    int lastSynapses = segMax.getPrevActiveSynapses(true).Count;
                    if (!maxActives.ContainsKey(lastSynapses))
                    {
                        maxActives.Add(segMax.getPrevActiveSynapses(true).Count, segMax);
                    }
                }

                int maxVal = maxActives.Keys.Max();

                return maxActives[maxVal];
            }

            return null;


        }

        /// <summary>
        /// Return a SegmentUpdateInfo object containing proposed changes to the specified segmentUpdateList.  
        /// If the segmentUpdateList is None, then a new segmentUpdateList is to be added, otherwise
        /// the specified segmentUpdateList is updated.  If the segmentUpdateList exists, find all active
        /// synapses for the segmentUpdateList (either at t or t-1 based on the 'previous' parameter)
        /// and mark them as needing to be updated.  If newSynapses is true, then
        /// Region.newSynapseCount - len(activeSynapses) new synapses are added to the
        /// segmentUpdateList to be updated.  The (new) synapses are randomly chosen from the set
        /// of current learning cells (within Region.localityRadius if set).
        /// </summary>
        /// <param name="previous">Defaults to false</param>
        /// <param name="segmentUpdateList">Defaults to null</param>
        /// <param name="newSynapses">Defaults to false</param>
        public SegmentUpdateInfo getSegmentActiveSynapses(bool previous, Segment segment, bool newSynapses)
        {
            //Return a list of proposed changes (synapses to update or add)
            //to this segment.
            //activeSynapses are those that are active (at time t) (to inc or dec later).
            //If newSynapses, then add new synapses to the segment randomly
            //  sampled from set of cells with learn state True (at time t)
            //  Either:
            //    a) new segment and new synapses for it
            //    b) existing segment, only update perm of existing syns
            //    c) existing segment, update perm of existing syns and add new syns

            List<DistalSynapse> activeSynsDist = new List<DistalSynapse>();
            List<Synapse> activeSynsAll = new List<Synapse>();

            if(segment!=null)
            {
                if(previous)
                {
                    activeSynsDist = segment.getPrevActiveSynapses(false);
                }
                else{
                    activeSynsAll = segment.getActiveSynapses(false);
                }
            }

            return new SegmentUpdateInfo(this, segment, activeSynsDist, newSynapses);

        }


        ///<summary>
        ///For this cell (at t-1 if previous=True else at t), find the segmentUpdateList (only
        ///consider sequence segments if isSequence is True, otherwise only consider
        ///non-sequence segments) with the largest number of active synapses. 
        ///This routine is aggressive in finding the best match. The permanence 
        ///value of synapses is allowed to be below connectedPerm. 
        ///The number of active synapses is allowed to be below activationThreshold, 
        ///but must be above minThreshold. The routine returns that segmentUpdateList. 
        ///If no segments are found, then None is returned.
        /// </summary>
        /// 
        /// <param name="previous">Defaults to false</param>
        public Segment getBestMatchingSegment(bool isSequence, bool previous)
        {
            List<Segment> segSequence = new List<Segment>();
            Segment bestSegment = null;
            int bestSynapseCount = MIN_SYNAPSES_PER_SEGMENT_THRESHOLD;
            int synCount = 0;

            //Todo: Code hier wirklich ok??? -> zweimal foreach, kommt öfters bei Barry vor.
            foreach(Segment seg in segments)
            {
                if (seg.isSequence == isSequence)
                {
                    segSequence.Add(seg);
                }
            }

            foreach(Segment seg in segSequence)
            {
                if (previous)
                {
                    //Get all the active previously synapses, no matter connection value
                    synCount = seg.getPrevActiveSynapses(false).Count;
                }
                else
                {
                    //Get all the momentarily active previously synapses, no matter connection value
                    synCount = seg.getActiveSynapses(false).Count;
                }

                if (synCount > bestSynapseCount)
                {
                    bestSynapseCount = synCount;
                    bestSegment = seg;
                }
            }

            return bestSegment;
        }

        #region IStatistics-Implementation

        /// <summary>
        /// Basic statistics on cell-level
        /// A: Absolute paramters:
        ///  StepCounter: aboluste number of steps
        ///  Activity Counter: absolute number of cell activations
        ///  Prediction Counter: absolute number of cell predictions
        ///  Correct Prediction Counter: absolute number of CORRECT cell predictions
        ///  Learning Counter: absolute number of learning activations
        ///  Segment Counter: absolute number of segments per cell
        ///  MaxSynCounter: max. Number of synapses per cell
        /// B: Averages:
        ///  Activity Rate: ActivityCounter/StepCounter
        ///  Precision: CorrectPredictions/Predictions
        ///  
        /// </summary>
        public void ComputeBasicStatistics()
        {
            //Increase Step
            StepCounter++;

            if (isActive)
            {
                //Update cell
                ActivityCounter++;
                //Update column
                column.ActivityCounter++;
                if (wasPredicted)
                {
                    CorrectPredictionCounter++;
                    column.CorrectPredictionCounter++;
                    column.region.CorrectPredictionCounter++;
                    if (wasSegmentPredicted)
                    {
                        CorrectSegmentPredictionCounter++;
                        column.CorrectSegmentPredictionCounter++;
                        column.region.CorrectSegmentPredictionCounter++;
                    }
                }
            }

            if (isPredicting)
            {
                PredictionCounter++;
                column.PredictionCounter++;
                column.region.PredictionCounter++;

                if (isSegmentPredicting)
                {
                    SegmentPredictionCounter++;
                    column.SegmentPredictionCounter++;
                    column.region.SegmentPredictionCounter++;
                    PredictPrecision = CorrectSegmentPredictionCounter / SegmentPredictionCounter;
                }
            }


            if (isLearning)
            {
                LearningCounter++;
                column.LearningCounter++;

            }
            //Calculate Activity Rate:
            ActivityRate = ActivityCounter / StepCounter;

            //Display Number of Segments per Cell:
            if (segments != null)
            {
                NumberSegments = segments.Count;
                int maxSyn = 0;
                foreach (Segment item in segments)
                {
                    int newSyn = item.synapses.Count;
                    if (newSyn > maxSyn)
                    {
                        maxSyn = newSyn;
                    }
                }

                MaxSynapseCount = maxSyn;
            }
        }

        /// <summary>
        /// Initialize the basic statistcs parameters
        /// </summary>
        public void InitializeStatisticParameters()
        {
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

