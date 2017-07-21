using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Data;

namespace QuantTrade.Core.Algorithm
{
    public class BaseAlgorithm
    {
        #region Properties 

        public Portfolio Portfolio { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TransactionFee { get; set; }

        public Resolution Resolution { get; set; }

        public String Symbol { get; set; }

        public List<IIndicator> Indicators { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public BaseAlgorithm()
        {
            Portfolio = new Portfolio();
            Indicators = new List<IIndicator>() ; 
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetStartDate(int year, int month, int day)
        {
            var start = new DateTime(year, month, day);
            start = start.Date;
            StartDate = start;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetEndDate(int year, int month, int day)
        {
            var end = new DateTime(year, month, day);
            end = end.Date;
            EndDate = end;
        }

        /// <summary>
        /// Read Data from data sources
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="resolution"></param>
        public void RunTest()
        {
            IDataReader reader = new CSVReader();
            reader.OnData += OnData;

            reader.ReadData(Symbol, Resolution, Indicators, StartDate, EndDate);
        }

        /// <summary>
        /// Event Handing new trade bar
        /// </summary>
        public virtual void OnData(TradeBar data, EventArgs e)
        {

        }

        #region Indicator Factories

        public RelativeStrengthIndex GenerateRelativeStrengthIndexIndicator(int period, MovingAverageType movingAverageType)
        {
            RelativeStrengthIndex indicator = new RelativeStrengthIndex(period, movingAverageType);
            Indicators.Add(indicator);
            return indicator;
        }
        
        public SimpleMovingAverage GenerateSimpleMovingAverageIndicator(int period)
        {
            SimpleMovingAverage indicator = new SimpleMovingAverage(period);
            Indicators.Add(indicator);
            return indicator;
        }
        #endregion

    }
}
