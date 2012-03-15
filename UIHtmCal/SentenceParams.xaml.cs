using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HTM.HTMBitmapProcessor;
using HTM.HTMInterfaces;

namespace HTM.UIHtmCal
{
   
    public struct RectanglePos
    {
        public int X, Y, deltaX, deltaY, sideLength, factor;
    }

    /// <summary>
    /// Interaktionslogik für SentenceParams.xaml
    /// </summary>
    public partial class SentenceParams : Window
    {

        private RectanglePos rectPos;
        public Rectangle BasicRectangle { get; set; }
        public string SelectString { get; set; }
        public RectanglePos RectPos { get; set; }
        public SentenceProcessor HTMProcessor { get; set; }
        public IRegionProcessor RegionProcessor { get; set; }
        public string StringNoise { get; set; }
        public string StringLetter { get; set; }

        public SentenceParams()
        {
            //Initialize
            InitializeComponent();
            HTMProcessor = SentenceProcessor.Instance;
            StringNoise = Properties.Settings.Default.SliderNoise;
            StringLetter = Properties.Settings.Default.SliderLetters;
            //Define positions for shifting rectangles
            rectPos = new RectanglePos();
            rectPos.X = 0;
            rectPos.Y = 0;
            rectPos.deltaX = 60;
            rectPos.deltaY = 60;
            rectPos.sideLength = 16;
            rectPos.factor = 3;

            RectPos = rectPos;


            //Create the rectangle that shall be drawn
            createRectangle();

        }

        /// <summary>
        /// If a new sentence is chosen: Draw the sentence as bitmaps in the area below. 
        /// Therefore get the according letters from the SentenceProcessor that allows to 
        /// traverse the bitmap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sentences_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selString = e.AddedItems[0] as ListBoxItem;
            string listString = selString.Content as string;

            //Initialize region processor
            RegionProcessor = HTMProcessor.Initalize(listString);
            //get modified string
            SentenceProcessor sentProc = HTMProcessor as SentenceProcessor;
            SelectString = sentProc.StringSentence;
            
            //Clear and Draw
            ClearAndDraw();
        }

        /// <summary>
        /// Clears Canvas for next string and also draws!
        /// </summary>
        private void ClearAndDraw()
        {
            //Clear
            rectPos.X = 0;
            rectPos.Y = 0;

            //Clear Canvas
            AlphaBeta.Children.Clear();

            //Draw Bitmap
            DrawSentenceBitMap();
        }

        /// <summary>
        /// Creates basic rectangle
        /// </summary>
        private Rectangle createRectangle()
        {
            // Create a red Ellipse.
            Rectangle myRectangle = new Rectangle();

            // Set the width and height of the Ellipse.
            myRectangle.Width = rectPos.factor;
            myRectangle.Height = rectPos.factor;

            myRectangle.StrokeThickness = 0.5;
            myRectangle.Stroke = Brushes.LightGray;

            return myRectangle;
        }

        /// <summary>
        /// Creates number of Rectangles with side-length of 16x5 (80) according to number of letters
        /// per line 5 letters
        /// </summary>
        private void DrawSentenceBitMap()
        {
            foreach (int[,] item in HTMProcessor.BMSentence)
            {

                if (rectPos.X>= AlphaBeta.Width-60)
                {
                    rectPos.X = 0;
                    rectPos.Y += rectPos.deltaY;
                }

                DrawLetterBitMap();
                rectPos.X += rectPos.deltaX;
            }
        }

        /// <summary>
        /// Draws the letter from the bitmap according to bits
        /// </summary>
        private void DrawLetterBitMap()
        {
            //Step the Processor
            HTMProcessor.Step();

            int[,] letter = HTMProcessor.GetOutPut();

            if (letter != null)
            { 
                for (int i = 0; i < rectPos.sideLength; i++)
                {
                    for (int j = 0; j < rectPos.sideLength; j++)
                    {

                        Rectangle myRectangle = createRectangle();
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        myRectangle.Fill = mySolidColorBrush;
                        AlphaBeta.Children.Add(myRectangle);
                        Canvas.SetLeft(myRectangle, rectPos.X + j * rectPos.factor);
                        Canvas.SetTop(myRectangle, rectPos.Y + i * rectPos.factor);


                        if (letter[i, j] == 0)
                        {
                            //For zeros: White rectangles
                            mySolidColorBrush.Color = Color.FromRgb(255, 255, 255);
                        }
                        else
                        {
                            //For 1s: black rectangles
                            mySolidColorBrush.Color = Color.FromRgb(0, 0, 0);
                        }

                    }
                }
            }
 
        }

        #region slider eventhandler
        private void sliderDistort_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //Get slider
            Slider actualSlider = sender as Slider;
            string actualSliderString = actualSlider.Name;

            //Set Value in HTMProcessor
            if(HTMProcessor!=null && RegionProcessor!=null)
            {
                int value = (int)Math.Round(actualSlider.Value);
                if(actualSliderString.Equals(StringNoise))
                {
                    SentenceProcessor.NoiseChanger.SliderNoise = value;
                    HTMProcessor.addNoise();
                }
                else{
                    SentenceProcessor.SequenceChanger.SliderSequence = value;
                    HTMProcessor.addSequenceMember();
                }

                //Reset Counter to draw
                HTMProcessor.Initialize();

                //Clear and Draw
                ClearAndDraw();
            }
        }

        #endregion

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            //Cast to MainWindow
            MainWindow parent = Owner as MainWindow;

            //disable Data-Set-Steering Buttons
            parent.InitHTM.IsEnabled = false;

            parent.EnableSteeringButtons(false);
            this.Close();
        }

        private void Button_Click_ok(object sender, RoutedEventArgs e)
        {
            //Cast to MainWindow
            MainWindow parent = Owner as MainWindow;

            //disable Data-Set-Steering Buttons
            if (SelectString != null && SelectString.Length>0)
            {
                parent.chosenDataSource = InputSource.Sentences;
                parent.InitHTM.IsEnabled = true;
            }

            this.Close();
        }



    }
}
