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
        public Portfolio Portfolio { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TransactionFee { get; set; }

        public Resolution Resolution { get; set; }

        public String Symbol { get; set; }

        public List<IIndicator> Indicators { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BaseAlgorithm()
        {
            Portfolio = new Portfolio();
            Indicators = new List<IIndicator>();
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

            reader.ReadData(Symbol, Resolution, new AlphaAdvantage());
        }

        /// <summary>
        /// Event Handing new trade bar
        /// </summary>
        public virtual void OnData(TradeBar data, EventArgs e)
        {

        }

    }
}
