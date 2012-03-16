using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTM.HTMLibrary
{
    public class ProximalSynapse : Synapse
    {
        private InputCell inputCell;
        private double p;

        public InputCell inputSource { get; set; }

        /// <summary>
        /// Set inputSource and initial permanance value for the synapse.
        /// </summary>
        /// <param name="inputSrc">
        /// object providing source of the input to this synapse (either 
        /// a Column's Cell or a special InputCell.
        /// </param>
        public ProximalSynapse(InputCell inputSrc, double permanence=Synapse.INITIAL_PERMANENCE)
        {
            inputSource = inputSrc;
            base.permanence = (float) permanence;
        }

        public override bool isActive
        {
            get
            {
                return inputSource.isActive && isConnected;
            }
            set
            {
                isActive = value;
            }
        }

        public override bool isActiveNotConnected
        {
            get
            {
                return inputSource.isActive;
            }
            set
            {
                isActiveNotConnected = value;
            }
        }

    }
}
