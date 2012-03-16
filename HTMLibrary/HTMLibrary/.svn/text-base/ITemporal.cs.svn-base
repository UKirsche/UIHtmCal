using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTM.HTMLibrary
{
    public interface ITemporal
    {
        /// <summary>
        /// Return true if this Synapse was active and connected due to the previous input at t-1. 
        /// </summary>
        bool wasActive { get; set; }

        /// <summary>
        /// Return true if this Synapse was active but not connected due to the previous input at t-1. 
        /// </summary>
        bool wasActiveNotConnected { get; set; }

        /// <summary>
        /// Return true if this Synapse was active due to the input previously being
        /// in a learning state. 
        /// </summary>
        bool wasActiveFromLearning { get; set; }
    }
}
