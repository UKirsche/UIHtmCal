using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTM.HTMInterfaces;
using System.Text.RegularExpressions;
using HTM.HTMAnalytics;
namespace HTM.HTMBitmapProcessor
{
    /// <summary>
    /// Class designed as singleton as called from several parts of program. The Sentence Processor basically takes a written sentence
    /// as its input and converts it to a bitmap-list. In addition it implements the interface for the <see cref="IRegionProcessor"/> and
    /// <see cref="IDistortion"/> as well as <see cref="ISentence"/>.
    /// </summary>
    public class SentenceProcessor: IRegionProcessor, ISentence, IDistortion, IStatisticsLogSentence
    {

        #region Inner Class Noiser/Sequencer
        //Nested Class: Inner

        /// <summary>
        /// Noise changer mainly changes bits of an input bitmap at random.
        /// </summary>
        public class NoiseChanger
        {
            /// <summary>
            /// Fields
            /// </summary>
            private const int DIMLENGTH = 16;
            private  static int[,] sampleArray;

            /// <summary>
            /// Random factor for nois pixels in bitmaps
            /// </summary>
            public static int SliderNoise { get; set; }

            public NoiseChanger()
            {
                sampleArray = new int[DIMLENGTH, DIMLENGTH];
            }

            /// <summary>
            /// Reset the 2-Dim-Array to check whether combinationes
            /// of indizes have already been drawn
            /// </summary>
            public void InitializeSampleArray()
            {
                for (int i = 0; i < DIMLENGTH; i++)
                {
                    for (int j = 0; j < DIMLENGTH; j++)
                    {
                        sampleArray[i, j] = 0;
                    }
                }
            }


            /// <summary>
            /// Changes Bits of letter bitmaps according to slider position
            /// </summary>
            /// <param name="dimLength"></param>
            /// <param name="drawnCombinations"></param>
            public void ChangeBits(int[,] item)
            {
                //in each bitmap change number-factor bits
                int col, row;

                for (int i = 0; i < SliderNoise; i++)
                {
                    do
                    {
                        col = MathHelper.r.Next(0, DIMLENGTH);
                        row = MathHelper.r.Next(0, DIMLENGTH);

                    } while (sampleArray[col, row] == 1);

                    //Set as occupied
                    sampleArray[col, row] = 1;

                    //change the item value at relevant position:
                    if (item[col, row] == 0)
                    {
                        item[col, row] = 1;
                    }
                    else
                    {
                        item[col, row] = 0;
                    }
                }
                
            }
        }
        /// <summary>
        /// Sequence changer is hierarchically the first change operation on a bitmap sequence. It insersts letters at random into a bitmap sentence.
        /// </summary>
        public class SequenceChanger
        {
            /// <summary>
            /// Random factor insertion for sequence
            /// </summary>
            public static int SliderSequence { get; set; }


            /// <summary>
            /// Inserts letters at random positions in sentence
            /// </summary>
            /// <param name="bmSequence"></param>
            /// <param name="bmProcessor"></param>
            public void ChangeSequence(List<int[,]> bmSequence, BitMapAlphaNumericProcessor bmProcessor, SentenceProcessor sentProcess)
            {

                string sentence = sentProcess.StringSentence;

                //Get length -> -1 because sequence has count-1 inner positions
                int innerLength = bmSequence.Count - 1;
                List<int> positionInserts = new List<int>();
                int position;

                for (int i = 0; i < SliderSequence; i++)
                {
                    do
                    {
                        position = MathHelper.r.Next(1, innerLength);
                    } while (positionInserts.Contains(position));

                    //insert:
                    positionInserts.Add(position);
                }

                //Draw randomly an alphabet letter:
                char[] alphaBeta = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
                                       'Q', 'R', 'S', 'T', 'U', 'V', 'W','X','Y','Z'};

                for (int i = positionInserts.Count-1; i >=0 ; i--)
                {
                    int letterPosition = MathHelper.r.Next(0, alphaBeta.Length-1);
                    char letterChar = alphaBeta[letterPosition];
                    //Insert Letter!
                    bmSequence.Insert(positionInserts[i], bmProcessor.getLetter(letterChar));
                    sentence = sentence.Insert(positionInserts[i], letterChar.ToString());
                }

                //Reset sentence
                sentProcess.StringSentence = sentence;
            }
        }
        /// <summary>
        /// BackUpManager keeps information of former states of bitmap sequence in order to handle interface operations.
        /// </summary>
        public class BackUpManager
        {


            /// <summary>
            /// Original Sequence for restoring after random change
            /// </summary>
            private List<int[,]> backUpGlobalSentence;

            private List<int[,]> backUpLetterSentence;

            public BackUpManager()
            {
            }


            /// <summary>
            /// Restores BMSequence (Sentence) from Backup only if called from same source
            /// </summary>
            public List<int[,]> RestoreGlobalBMSentence()
            {

                return RestoreBMSentence(backUpGlobalSentence);
            }

            /// <summary>
            /// Restores BMSequence (Sentence) from Backup only if called from same source
            /// </summary>
            public List<int[,]> RestoreLetterBMSentence()
            {

                return RestoreBMSentence(backUpLetterSentence);
            }



            /// <summary>
            /// Encapsulated method to restore a bitmap-Sentence
            /// </summary>
            /// <param name="backUpSentence"></param>
            /// <returns></returns>
            private List<int[,]> RestoreBMSentence(List<int[,]> backUpSentence)
            {
                List<int[,]> bmSentence = new List<int[,]>();

                //Restore original sequence
                foreach (int[,] item in backUpSentence)
                {
                    bmSentence.Add(CopyBMSentence(item));
                }
                return bmSentence;
            }

            /// <summary>
            /// Backs up the bitmap in backUpLetterSentence
            /// </summary>
            /// <param name="toBackuUpMap"></param>
            public void BackUpLetterBMSentence(List<int[,]> toBackuUpMap)
            {
                BackUpBMSentence(ref backUpLetterSentence, toBackuUpMap);
            }


            /// <summary>
            /// Backs up the bitmap in backUpLetterSentence
            /// </summary>
            /// <param name="toBackuUpMap"></param>
            public void BackUpGlobalBMSentence(List<int[,]> toBackuUpMap)
            {
                BackUpBMSentence(ref backUpGlobalSentence, toBackuUpMap);
            }

            /// <summary>
            /// Backups-Up a Map to a targetMap
            /// </summary>
            /// <param name="toBackuUpMap"></param>
            private void BackUpBMSentence(ref List<int[,]> targetBackUp, List<int[,]> sourceBackUp)
            {
                targetBackUp = new List<int[,]>();

                if (sourceBackUp != null && sourceBackUp.Count > 0)
                {
                    //Jetzt liegt wechsel vor, also mache neues backup!
                    foreach (int[,] item in sourceBackUp)
                    {
                        targetBackUp.Add(CopyBMSentence(item));
                    }
                }
            }
           
            /// <summary>
            /// Copies 2-dim-Array
            /// </summary>
            /// <param name="array"></param>
            /// <returns></returns>
            private int[,] CopyBMSentence(int[,] array)
            {
                int dim1 = array.GetLength(0);
                int dim2 = array.GetLength(1);

                int[,] copyArray = new int[dim1, dim2];
                for (int i = 0; i < dim1; i++)
                {
                    for (int j = 0; j < dim2; j++)
                    {
                        copyArray[i, j] = array[i, j];
                    }
                }

                return copyArray;
            }
        }
    
        #endregion


        private static int counter;

        /// <summary>
        /// Class for creating random noise
        /// </summary>
        public static NoiseChanger RandomNoiser { get; set; }

        /// <summary>
        /// Class for creating random sequence insertions
        /// </summary>
        public static SequenceChanger RandomSequencer { get; set; }

        /// <summary>
        /// Handles backup of sequence
        /// </summary>
        public static BackUpManager BackupManager { get; set; }


        /// <summary>
        /// Property for singleton as needed for Sentencprocessor
        /// </summary>
        private static SentenceProcessor instance=null;
        public static SentenceProcessor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SentenceProcessor();
                }
                return instance;
            }
        }

        /// <summary>
        /// BM of sentence from ListBox
        /// </summary>
        public List<int[,]> BMSentence { get; set; }
        

        /// <summary>
        /// Link to BMAlphaNumericProcessor that gets letters as Bitmaps
        /// </summary>
        public BitMapAlphaNumericProcessor BMAlphaProcessor { get; set; }

        /// <summary>
        /// Output for IRegionProcessor
        /// </summary>
        private int[,] BitMapOutput { set; get; }

        #region ISentence
        /// <summary>
        /// Chosen Sentence from ListBox
        /// </summary>
        public string StringSentence
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// private constructor for Singleton
        /// </summary>
        private SentenceProcessor()
        {
            //Create the NoiseChanger once
            RandomNoiser = new NoiseChanger();

            //Create the SequenceChanger once
            RandomSequencer = new SequenceChanger();

            //Create the BackupManager
            BackupManager = new BackUpManager();
        }


        /// <summary>
        /// Returns the instance as IRegion Processor to traverse chosen sentence
        /// Therefore connect to BitMapAlphanumericProcessor to circle through letters and create bitmap accordingly
        /// </summary>
        /// <param name="sentenceChose">Chosen sentence from dialog window</param>
        /// <returns></returns>
        public IRegionProcessor Initalize(string sentenceChosen)
        {
            counter = 0;

            //Get the BitMapAlphaProcessor-Instance:
            BMAlphaProcessor = BitMapAlphaNumericProcessor.Instance;
            BMAlphaProcessor.Initialize();

            //prepare sentence
            StringSentence = prepareSentence(sentenceChosen);

            //Traverse StringSentenceCharacter by character and put the result on the Queue
            BMSentence = new List<int[,]>();

            //Create Bitmaplist
            foreach (char item in StringSentence)
            {
                int[,] bitMap = BMAlphaProcessor.getLetter(item);
                if (bitMap == null)
                {
                    bitMap = BMAlphaProcessor.getNumber(item);
                }
                BMSentence.Add(bitMap);
            }

            //Create Backup of Sequence for sliders
            BackupManager.BackUpLetterBMSentence(BMSentence);
            BackupManager.BackUpGlobalBMSentence(BMSentence);

            return this;
        }


        private string prepareSentence(string sentenceChosen)
        {
            //Remove the whitespaces:
            string trim = Regex.Replace(sentenceChosen, @"\s", "");
            string trimUpper = trim.ToUpper();
            return trimUpper;
        }

        #region IRegionProcessor

        /// <summary>
        /// Return Bitmap of letter, number as output
        /// </summary>
        /// <returns></returns>
        public int[,] GetOutPut()
        {
            return BitMapOutput;
        }


        public void SetInput(int[,] inputArray)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Step through Queue of Bitmaps and set output for this
        /// </summary>
        public void Step()
        {
            if (BMSentence.Count > 0 && counter < BMSentence.Count)
            {
                LogSentence = StringSentence.ElementAt(counter).ToString();
                BitMapOutput = BMSentence[counter++];
            }
            else
            {
                Initialize();
            }
        }

        /// <summary>
        /// Resets counter for bitmap sequence to 0.
        /// </summary>
        public void Initialize()
        {
            counter = 0;
        }
        #endregion

        #region Distortion

        /// <summary>
        /// Adds pixels to the WHOLE current sequence of Bitmaps. 
        /// NoiseChanger class takes care and dds and removes pixels randomly (1s)
        /// </summary>
        public void addNoise()
        {
            if (BMSentence != null && BMSentence.Count > 0)
            {
                //Restore BMLetterSentence
                BMSentence = BackupManager.RestoreLetterBMSentence();

                //Change bits for each bitmap
                foreach (int[,] item in BMSentence)
                {
                    //Initialize sample array to avoid double draws
                    RandomNoiser.InitializeSampleArray();
                    //Delegate
                    RandomNoiser.ChangeBits(item);
                }
            }
        }

        /// <summary>
        /// Add letters (BitMaps) to the existing sentence as randomly drawn from Alphabet
        /// </summary>
        public void addSequenceMember()
        {
            //Restore Backup
            BMSentence = BackupManager.RestoreGlobalBMSentence();

            if (BMSentence != null && BMSentence.Count > 0)
            {
                RandomSequencer.ChangeSequence(BMSentence, BMAlphaProcessor, this);

                //Create letter backup
                BackupManager.BackUpLetterBMSentence(BMSentence);

                //Now add noise!
                addNoise();

            }
        }

        #endregion

        #region IStatisticsLogSentence
        /// <summary>
        /// Sentence create for Log-DataGrid an describing the actual input bitmap. In this case its just the actual letter of the sentence.
        /// </summary>
        public string LogSentence { get; set; }
        #endregion

    }
}
