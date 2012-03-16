using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///
/// @created 20.3.2011
/// Taken from first approach by Mutakarnich in Python
/// Adapted by Uwe Kirschenmann
/// @author Barry Mutakarnich, Uwe Kirschenmann
///


namespace HTM.HTMLibrary
{
    /// <summary>
    /// A data structure representing a distal synapse. Contains a permanence value and the 
    ///source input to a lower input cell.  
    /// </summary>
     
    public class DistalSynapse : Synapse, ITemporal
    {

        public Cell inputSource { get; set; }

        /// <summary>
        /// Set inputSource and initial permanance value for the synapse.
        /// </summary>
        /// <param name="inputSrc">
        /// object providing source of the input to this synapse (either 
        /// a Column's Cell or a special InputCell.
        /// </param>
        public DistalSynapse(Cell inputSrc)
        {
            inputSource = inputSrc;
        }

        //Override properties from Synapse
        public  override bool isActive
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
                return inputSource.isActive ;
            }
            set
            {
                isActiveNotConnected = value;
            }
        }


        //Implement ITemporal Interface Methods
        public bool wasActive
        {
            get
            {
                return inputSource.wasActive && isConnected;
            }
            set
            {
                wasActive = value;
            }
        }

        public bool wasActiveNotConnected
        {
            get
            {
                return inputSource.wasActive;
            }
            set
            {
                wasActiveNotConnected = value;
            }
        }

        public bool wasActiveFromLearning
        {
            get
            {
                return inputSource.wasActive && inputSource.wasLearning;
            }
            set
            {
                wasActiveFromLearning = value;
            }
        }
    }
}
