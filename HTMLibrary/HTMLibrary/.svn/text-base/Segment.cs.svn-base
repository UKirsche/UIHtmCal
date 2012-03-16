using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///
/// @created 21.3.2011
/// Taken from first approach by Mutakarnich in Python
/// Adapted by Uwe Kirschenmann
/// @author Barry Mutakarnich, Uwe Kirschenmann
///


namespace HTM.HTMLibrary
{
    /// <summary>
    /// Represent a single dendrite segmentUpdateList that forms synapses (connections) to other cells.
    /// Each segmentUpdateList also maintains a boolean flag, sequenceSegment, indicating 
    /// whether the segmentUpdateList predicts feed-forward input on the next time step.
    /// Segments can be either proximal or distal (for spatial pooling or temporal pooling 
    /// respectively) however the class object itself does not need to know which
    /// it ultimately is as they behave identically.  Segments are considered 'active' 
    /// if enough of its existing synapses are connected and individually active.
    /// </summary>
    public class Segment
    {
        
        public List<Synapse> synapses {get; set;}
        public bool isSequence { get; set; }
        public float segActiveThreshold { get; set; }

        public Segment(float segActiveThreshold)
        {
            synapses = new List<Synapse>();
            isSequence = false;
            this.segActiveThreshold = segActiveThreshold;
        }


        /// <summary>
        /// his routine returns true if the number of connected synapses on this segmentUpdateList 
        /// that are active due to active states at time t is greater than activationThreshold.
        /// </summary>
        public bool isActive{
            get 
            {
                int returnVal = getActiveSynapses().Count;
                return returnVal >= this.segActiveThreshold;
            } 
        }


        /// <summary>
        /// This routine returns true if the number of connected synapses on this segmentUpdateList
        /// that were active due to active states at time t-1 is greater than activationThreshold.
        /// </summary>
        public bool wasActive{
            get 
            {
                return getPrevActiveSynapses().Count >= this.segActiveThreshold;
            } 
        }

        /// <summary>
        /// This routine returns true if the number of connected synapses on this segmentUpdateList 
        /// that were active due to learning states at time t-1 is greater than activationThreshold. 
        /// </summary>  
        public bool wasActiveFromLearning{ 
            get
            {
                List<DistalSynapse> previousActiveList = getPrevActiveSynapses();
                List<DistalSynapse> listLearning = new List<DistalSynapse>();

                foreach (DistalSynapse distSyn in listLearning)
                {
                    if (distSyn.wasActiveFromLearning) 
                    {
                        listLearning.Add(distSyn);
                    }
                }
                return listLearning.Count >= segActiveThreshold;
            }
        }


        /// <summary>
        /// Add the specified synapse object to this segmentUpdateList.
        /// </summary>
        /// <param name="synapse"></param>
        public void addSynapse(Synapse synapse)
        {
            synapses.Add(synapse);
        }

        /// <summary>
        /// Create a new synapse for this segmentUpdateList attached to the specified input source.
        /// </summary>
        /// <param name="inputSource">the input source of the synapse to create.</param>
        /// <returns>the newly created synapse.</returns>
        public DistalSynapse createSynapse(Cell inputSource)
        {
            DistalSynapse newSyn = new DistalSynapse(inputSource);
            newSyn.permanence = Synapse.INITIAL_PERMANENCE;
            synapses.Add(newSyn);
            return newSyn;
        }

        /// <summary>
        /// Create numSynapses new synapses for this segmentUpdateList attached to the specified
        /// learning cells.
        /// </summary>
        /// <param name="synapseCells">List of available learning cells to form synapses to</param>
        /// <returns>The list of synapses that were successfully added.</returns>
        public List<DistalSynapse> createSynpasesToLearningCells(List<Cell> synapseCells)
        {
            List<DistalSynapse> newSyns = new List<DistalSynapse>();
            foreach (Cell cell in synapseCells)
            {
                newSyns.Add(createSynapse(cell));
            }

            return newSyns;
        }

        /// <summary>
        /// Return a list of all the synapses that are currently connected (those with a
        /// permanence value above the threshold).
        /// </summary>
        /// <returns></returns>
        public List<Synapse> getConnectedSynapses()
        {
            List<Synapse> synapseList = new List<Synapse>();
            foreach (Synapse syn in synapses)
            {
                if(syn.isConnected)
                {
                    synapseList.Add(syn);
                }
            }

            return synapseList;
        }

        /// <summary>
        /// Return a list of all the currently active (firing) synapses on this segmentUpdateList.
        /// </summary>
        public List<Synapse> getActiveSynapses(bool connectedOnly=true)
        {
            List<Synapse> actSynapses = new List<Synapse>();
            if (!connectedOnly)
            {
                foreach (Synapse s in synapses)
                {
                    if (s.isActiveNotConnected)
                    {
                        actSynapses.Add(s);
                    }
                }
            }
            else
            {
                foreach (Synapse s in synapses)
                {
                    if (s.isActive)
                    {   
                        actSynapses.Add(s);
                    }
                }
            }

            return actSynapses;
        }


        /// <summary>
        /// Return a list of all the previously active (firing) synapses on this segmentUpdateList.
        /// </summary>
        /// <returns></returns>
        public List<DistalSynapse> getPrevActiveSynapses(bool connectedOnly=true)
        {
            List<DistalSynapse> previousActive = new List<DistalSynapse>();

            if (!connectedOnly)
            {
                foreach (DistalSynapse syn in synapses)
                {
                    if (syn.wasActiveNotConnected)
                    {
                        previousActive.Add(syn);
                    }

                }
            }
            else
            {
                foreach (DistalSynapse syn in synapses)
                {
                    if (syn.wasActive)
                    {
                        previousActive.Add(syn);
                    }
                }
            }
            
            return previousActive;
        }
    }
}
