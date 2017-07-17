using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;
using QuantTrade.Data.Providers;
using System;
using QuantTrade.Core.Securities;

namespace QuantTrade.Algorithm
{
    public class BaseAlgorithm
    {
        public Portfolio Portfolio { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TransactionFee { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BaseAlgorithm()
        {
            Portfolio = new Portfolio();
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
        public virtual void Initialize(string symbol, Resolution resolution)
        {
            IDataReader reader = new CSVReader();
            reader.OnData += OnData;
            reader.ReadData(symbol, resolution, new AlphaAdvantage());
        }

        /// <summary>
        /// Event Handing new trade bar
        /// </summary>
        public virtual void OnData(TradeBar data, EventArgs e)
        {

        }

    }
}
