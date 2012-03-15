using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HTM.HTMInterfaces;

namespace HTM.HTMAnalytics
{

	public class ContextStatisticEntry : IStatistics
	{

		

		#region Properties

		public float PredictPrecisionOld { get; set; }
		public float PredictionCounterOld { get; set; }
		public string ContextSentence { get; set; }
		public float PrecisionDelta { get; set; }
		#endregion

		public ContextStatisticEntry(string sentence, IStatistics statistics)
		{
			ContextSentence = sentence;
			StepCounter = statistics.StepCounter;
			ActivityCounter = statistics.ActivityCounter;
			PredictionCounter = statistics.PredictionCounter;
			CorrectPredictionCounter = statistics.CorrectPredictionCounter;
			PredictPrecision = statistics.PredictPrecision;
			ActivityRate = statistics.ActivityRate;
		}

		public float ActivityCounter { get; set; }

		public float PredictionCounter { get; set; }

		public float CorrectPredictionCounter { get; set; }

		public float ActivityRate { get; set; }

		public float PredictPrecision { get; set; }

		public float StepCounter { get; set; }

		public void InitializeStatisticParameters() 
		{
			throw new NotImplementedException();
		}

		public void ComputeBasicStatistics()
		{
			float deltaPrediction = PredictionCounter - PredictionCounterOld;
			float deltaPrecision = PredictPrecision - PredictPrecisionOld;
			if (deltaPrediction > 0)
			{
				PrecisionDelta = deltaPrecision;
			}
			else
			{
				PrecisionDelta = 0;
			}
		}


		public float SegmentPredictionCounter
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public float CorrectSegmentPredictionCounter
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}

	public class ListContextStatisticLog : ObservableCollection<ContextStatisticEntry>
	{
		public ListContextStatisticLog()
		{

		}

		public void AddContextStatisticEntry(ContextStatisticEntry ce)
		{
			if (ce != null)
			{
				if (this.Count > 0)
				{
					ContextStatisticEntry lastEntry = this.Last();
					//Needed for statistics
					ce.PredictionCounterOld = lastEntry.PredictionCounter;
					ce.PredictPrecisionOld = lastEntry.PredictPrecision;

					//ComputeBasic statistics for element:
					ce.ComputeBasicStatistics();
				}
				this.Add(ce);
			}
		}
	}
}
