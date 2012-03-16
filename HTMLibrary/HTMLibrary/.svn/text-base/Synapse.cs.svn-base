using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTM.HTMLibrary
{
    public abstract class Synapse
    {
        /// <summary>
        /// Static parameters that apply to all Region instances
        /// </summary>
        public const float CONNECTED_PERM = 0.2f; //Synapses with permanences above this value are connected.
        public const float PERMANENCE_INC = 0.05f; //Amount permanences of synapses are incremented in learning.
        public const float PERMANENCE_DEC = 0.04f; //Amount permanences of synapses are decremented in learning.
        public const float INITIAL_PERMANENCE = CONNECTED_PERM; //initial permanence for distal synapses


        public float permanence { get; set; }
        public bool isConnected 
        {
            get
            {
                return permanence >= Synapse.CONNECTED_PERM;
            }
            set
            {
                isConnected = value;
            }
        }

        /// <summary>
        /// Return true if this Synapse is active and connected due to the current input. 
        /// </summary>
        public abstract bool isActive { get; set; }

        /// <summary>
        /// Return true if this Synapse is active but not connected due to the current input. 
        /// </summary>
        public abstract bool isActiveNotConnected { get; set; }

   
        /// <summary>
        /// Increase the permanence value of the synapse
        /// </summary>
        public void increasePermance()
        {
            permanence = Math.Min(1.0f, permanence + PERMANENCE_INC);
        }

        /// <summary>
        /// Increase the permanence value of the synapse
        /// </summary>
        /// <param name="amount">Amount to increase</param>
        public void increasePermance(float amount)
        {
            permanence = Math.Min(1.0f, permanence + amount);
        }

        /// <summary>
        /// Decrease the permance value of the synapse
        /// </summary>
        public void decreasePermance()
        {
            permanence = Math.Max(0.0f, permanence - PERMANENCE_DEC);
        }

        /// <summary>
        /// Decrease the permance value of the synapse
        /// </summary>
        /// <param name="amount">Amount to decrease</param>
        public void decreasePermance(float amount)
        {
            permanence = Math.Max(0.0f, permanence - amount);
        }
    }
 }
