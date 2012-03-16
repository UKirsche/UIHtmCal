using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTM;
using HTM.HTMInterfaces;
using log4net;
using log4net.Config;

namespace HTM.HTMBitmapProcessor
{
    public class BitMapAlphaNumericProcessor
    {
        /// <summary>
        /// Link to Alphabet
        /// </summary>
        public CreateBitMapNumbericAlphabet AlphaBetaNumeric{ get; set; }

        /// <summary>
        /// Property for singleton as needed for Sentencprocessor
        /// </summary>
        private static BitMapAlphaNumericProcessor instance = null;
        public static BitMapAlphaNumericProcessor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BitMapAlphaNumericProcessor();
                }
                return instance;
            }
        }

        /// <summary>
        /// Private Constructor for singleton instance
        /// </summary>
        private BitMapAlphaNumericProcessor() { }


        /// <summary>
        /// Main method of class as it gets the chosen LETTER from the Bitmaps class
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public int[,] getLetter(char letter)
        {
            int[,] letterBitmap = null;

            switch (letter)
            {
                case 'A' :
                    letterBitmap = AlphaBetaNumeric.LetterA;
                    break;
                case 'B':
                    letterBitmap = AlphaBetaNumeric.LetterB;
                    break;
                case 'C':
                    letterBitmap = AlphaBetaNumeric.LetterC;
                    break;
                case 'D':
                    letterBitmap = AlphaBetaNumeric.LetterD;
                    break;
                case 'E':
                    letterBitmap = AlphaBetaNumeric.LetterE;
                    break;
                case 'F':
                    letterBitmap = AlphaBetaNumeric.LetterF;
                    break;
                case 'G':
                    letterBitmap = AlphaBetaNumeric.LetterG;
                    break;
                case 'H':
                    letterBitmap = AlphaBetaNumeric.LetterH;
                    break;
                case 'I':
                    letterBitmap = AlphaBetaNumeric.LetterI;
                    break;
                case 'J':
                    letterBitmap = AlphaBetaNumeric.LetterJ;
                    break;
                case 'K':
                    letterBitmap = AlphaBetaNumeric.LetterK;
                    break;
                case 'L':
                    letterBitmap = AlphaBetaNumeric.LetterL;
                    break;
                case 'M':
                    letterBitmap = AlphaBetaNumeric.LetterM;
                    break;
                case 'N':
                    letterBitmap = AlphaBetaNumeric.LetterN;
                    break;
                case 'O':
                    letterBitmap = AlphaBetaNumeric.LetterO;
                    break;
                case 'P':
                    letterBitmap = AlphaBetaNumeric.LetterP;
                    break;
                case 'Q':
                    letterBitmap = AlphaBetaNumeric.LetterQ;
                    break;
                case 'R':
                    letterBitmap = AlphaBetaNumeric.LetterR;
                    break;
                case 'S':
                    letterBitmap = AlphaBetaNumeric.LetterS;
                    break;
                case 'T':
                    letterBitmap = AlphaBetaNumeric.LetterT;
                    break;
                case 'U':
                    letterBitmap = AlphaBetaNumeric.LetterU;
                    break;
                case 'V':
                    letterBitmap = AlphaBetaNumeric.LetterV;
                    break;
                case 'W':
                    letterBitmap = AlphaBetaNumeric.LetterW;
                    break;
                case 'X':
                    letterBitmap = AlphaBetaNumeric.LetterX;
                    break;
                case 'Y':
                    letterBitmap = AlphaBetaNumeric.LetterY;
                    break;
                case 'Z':
                    letterBitmap = AlphaBetaNumeric.LetterZ;
                    break;
                default:
                    break;
            }

            return letterBitmap;
        }


        /// <summary>
        /// Main method of class as it gets the chosen NUMBER from the Bitmaps class
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public int[,] getNumber(char number)
        {
            int[,] numberBitmap = null;

            switch (number)
            {
                case '0':
                    numberBitmap = AlphaBetaNumeric.Number0;
                    break;
                case '1':
                    numberBitmap = AlphaBetaNumeric.Number1;
                    break;
                case '2':
                    numberBitmap = AlphaBetaNumeric.Number2;
                    break;
                case '3':
                    numberBitmap = AlphaBetaNumeric.Number3;
                    break;
                case '4':
                    numberBitmap = AlphaBetaNumeric.Number4;
                    break;
                case '5':
                    numberBitmap = AlphaBetaNumeric.Number4;
                    break;
                case '6':
                    numberBitmap = AlphaBetaNumeric.Number6;
                    break;
                case '7':
                    numberBitmap = AlphaBetaNumeric.Number7;
                    break;
                case '8':
                    numberBitmap = AlphaBetaNumeric.Number8;
                    break;
                case '9':
                    numberBitmap = AlphaBetaNumeric.Number9;
                    break;
                default:
                    break;
            }

            return numberBitmap;
        }



        #region Initialization and Unitialization
        /// <summary>
        /// Initializer needs to things: connect to DB and Prepare Formatter to retrieve Context String
        /// </summary>
        public void Initialize()
        {

            //Create AlphaBeta as Bitmap and Numbers as well
            AlphaBetaNumeric = new CreateBitMapNumbericAlphabet();
        }
        #endregion


    }
}

