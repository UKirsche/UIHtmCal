using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace HTM.UIHtmCal
{
    class UserInputSynapse : INotifyPropertyChanged
    {

        /// <summary>
        /// Input Parameter: Synapses with permanences above this value are connected.
        /// </summary>
        int connectedPerm;
        public int ConnectedPerm
        {
            get { return connectedPerm; }
            set
            {
                this.connectedPerm = value;
            }
        }

        /// <summary>
        /// Input Parameter: Amount permanences of synapses are incremented in learning.
        /// </summary>
        int incPerm;
        public int IncPerm
        {
            get { return incPerm; }
            set
            {
                this.incPerm = value;
            }
        }


        /// <summary>
        /// Input Parameter: Amount permanences of synapses are incremented in learning.
        /// </summary>
        int decPerm;
        public int DecPerm
        {
            get { return decPerm; }
            set
            {
                this.decPerm = value;
            }
        }


        /// <summary>
        /// Input Parameter: Amount permanences of synapses are incremented in learning.
        /// </summary>
        int initialPerm;
        public int InitialPerm
        {
            get { return initialPerm; }
            set
            {
                this.initialPerm = value;
            }
        }

        #region INotifyPropertyChanged Members


        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region PropertyChanged
        /// <summary> 
        /// Fire the property changed event 
        /// </summary> 
        /// <param name="propertyName">Name of the property.</param> 
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public UserInputSynapse()
        {
            //Load XML-Config-File
            SynapseConfLoad.LoadSynapseConfigXML();

            IncPerm = SynapseConfLoad.SynParams.incPerm;
            DecPerm = SynapseConfLoad.SynParams.decPerm;
            InitialPerm = SynapseConfLoad.SynParams.intPerm;
            ConnectedPerm = SynapseConfLoad.SynParams.connPerm;


        }

        public void saveUserInput()
        {
            SynapseParams synParams = new SynapseParams();

            synParams.incPerm = IncPerm;
            synParams.decPerm = DecPerm;
            synParams.intPerm = InitialPerm;
            synParams.connPerm = ConnectedPerm;

            SynapseConfLoad.SynParams= synParams;
            SynapseConfLoad.SaveSynapseConfiXML();
        }

    }
}
