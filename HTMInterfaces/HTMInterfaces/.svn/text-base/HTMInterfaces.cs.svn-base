using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace HTM.HTMInterfaces
{
    public interface IRegionProcessor
    {
        /// <summary>
        /// Sets input for HTM Region
        /// </summary>
        void SetInput(int[,] inputArray);

        /// <summary>
        /// Region output
        /// </summary>
        int[,] GetOutPut();

        /// <summary>
        /// Initializes next processing step
        /// </summary>
        void Step();

        /// <summary>
        /// Initializes region
        /// </summary>
        void Initialize();
    }

    public interface IDBLogHandler
    {
        /// <summary>
        /// Put new elements in AppContextElementsDB
        /// </summary>
        /// <param name="listInput">list for input</param>
        void SetAppVocabularyElement(List<string> listInput);

        /// <summary>
        /// Returns the Raw Events: Elements..Ancestor4 for UILogDB
        /// </summary>
        string[] GetAppEventFields();

        /// <summary>
        /// Get app context elements from AppContextElementsDB
        /// </summary>
        List<string> GetAppVocabulary();
    }




    public interface IDBBasic
    {
        /// <summary>
        /// Basic Provider.String Property from NET
        /// </summary>
        string LogProvider { get; set; }
        /// <summary>
        /// Basic Connection.String Property from NET
        /// </summary>
        string LogConnectionString { get; set; }
        /// <summary>
        /// Basic DBFactor from NET
        /// </summary>
        DbProviderFactory DBFactory { get; set; }
        /// <summary>
        /// Basic DBConnection from NET
        /// </summary>
        DbConnection LogConnection { get; set; }
        /// <summary>
        /// Basic DBCommand from NET
        /// </summary>
        DbCommand LogCommand { get; set; }
        /// <summary>
        /// Basic Datareader from NET
        /// </summary>
        DbDataReader LogReader { get; set; }
    }

    public interface IEventFormatter
    {

        /// <summary>
        /// Returns list of app context elements from db fields UILog
        /// </summary>
        List<string> GetContextFromFields(string[] inputFields);

        /// <summary>
        /// Returs 2-dim-array as bitmap
        /// </summary>
        /// <param name="inputVektor">List of IDs to build BitMapArray</param>
        int[,] BuildBitMap(List<int> inputVektor);
    }

    public interface IGamer
    {
        /// <summary>
        /// Imitates a game loop for repeated run of HTM-Code
        /// </summary>
        void PlayLoop();

        /// <summary>
        /// Step through HTM phases as single step (Games: round based)
        /// </summary>
        void PlayStep();
    }

    /// <summary>
    /// Checks for selection of UI-DataGrid-Row (Column, Cell)
    /// </summary>
    public interface IDataGridSelected
    {
        bool IsDataGridSelected { get; set; }
    }

    #region BasicStatistics
    /// <summary>
    /// Basic Statistics for Cells, Columns, Regions
    /// </summary>
    public interface IStatistics
    {
        float ActivityCounter { get; set; }
        /// <summary>
        /// Records Sequence-Segment cell predictions, not overall cell predictions
        /// </summary>
        float SegmentPredictionCounter { get; set; }
        /// <summary>
        /// Records overall cell predictions
        /// </summary>
        float PredictionCounter { get; set; }
        /// <summary>
        /// Quotient of correct cell Sequence-Segment-Predictions to all cell Sequence-Segment-Predictions
        /// </summary>
        float CorrectSegmentPredictionCounter { get; set; }
        /// <summary>
        /// Quotient of correct cell predictions to all cell predictions
        /// </summary>
        float CorrectPredictionCounter { get; set; }
        float ActivityRate { get; set; }
        float PredictPrecision { get; set; }
        float StepCounter { get; set; }

        /// <summary>
        /// Paramters are set to 0.
        /// </summary>
        void InitializeStatisticParameters();

        /// <summary>
        /// Compute Basic statistcs
        /// </summary>
        void ComputeBasicStatistics();

    }



    /// <summary>
    /// Statistics for Cells
    /// </summary>
    public interface IStatisticsCells : IStatistics
    {
        float LearningCounter { get; set; }
        float NumberSegments { get; set; }
        float MaxSynapseCount { get; set; }

    }

    /// <summary>
    /// Statistics for Columns.
    /// Mainly extends IStatisticsCells to retrieve max-vals from cells
    /// </summary>
    public interface IStatisticsColumns : IStatisticsCells
    {
        float ColumnActivityCounter { get; set; }
        float ColumnMaxSynapseCount { get; set; }
        float MaxNumberSegments { get; set; }
        float MaxLearningCounter { get; set; }
        float MaxCellActivityCounter { get; set; }
        float MaxPredictionCounter { get; set; }
        float MaxCorrectPredictionCounter { get; set; }

    }
    #endregion


    /// <summary>
    /// Change bitmaps and sequences randomly
    /// </summary>
    public interface IDistortion
    {
        void addNoise();
        void addSequenceMember();
    }

    /// <summary>
    /// Creates the following Sentences as Bitmap:
    /// Please Be A Fine Girl Kiss Me
    /// Inside Crystal Mountain Evil takes its form
    /// Our so called leaders speak with words they try to jail you
    /// The power of now is the absence of longing
    /// </summary>
    public interface ISentence
    {
       string StringSentence
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides a property for holding some kind of text to log.
    /// </summary>
    public interface IStatisticsLogSentence
    {
        string LogSentence
        {
            get;
            set;
        }
    }


    
}
