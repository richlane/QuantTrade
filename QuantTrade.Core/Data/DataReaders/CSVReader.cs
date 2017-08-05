using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QuantTrade.Core.Configuration;
using QuantTrade.Core;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Utilities;

namespace QuantTrade.Core.Data
{
    /// <summary>
    /// Reads historical stock quotes and kicks off and event for each tradebar created. 
    /// </summary>
    public class CSVReader : IDataReader
    {
        #region Event Handlers

        public event OnDataHandler OnTradeBar;  //Used by base algorithm
        public EventArgs e = null;

        #endregion

        #region Properties

        private TradeBar _previousTradebar;

        private List<TradeBar> _tradeBars;

        private string _symbol;

        private Resolution _resolution;

        private string _csvFile;

        private IDataSource _dataSource;

        #endregion

        public CSVReader(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        /// <summary>
        /// Reads CSV into a tradebar collection
        /// </summary>
        private void readCSV()
        {
            int index = 0;

            //Read CSV file into RAM & create new tradbar collection
            _tradeBars = new List<TradeBar>();
            string[] data = File.ReadAllLines(_csvFile);

            //Loop the csv
            for (int i = 0; i < data.Length; i++)
            {
                index++;
                var csv = data[i].Split(',');

                //Daily CSV Data: date, open, high, low, close, split/dividend-adjusted close, volume, dividend amount, split coefficient
                TradeBar bar = new TradeBar()
                {
                    TradeResolution = _resolution,
                    Symbol = _symbol,
                    Day = DateTime.Parse(csv[0]),
                    Open = decimal.Parse(csv[1]),
                    High = decimal.Parse(csv[2]),
                    Low = decimal.Parse(csv[3]),
                    Close = decimal.Parse(csv[4]),
                    Volume = decimal.Parse(csv[6]),
                    Dividend = decimal.Parse(csv[7]),
                    SplitCoefficient = decimal.Parse(csv[8]),
                    SampleNumber = index
                };

                _tradeBars.Add(bar);
            }
        }


        /// <summary>
        /// Adjust tradebars for splits 
        /// </summary>
        private void adjustTradeBarsForSplits()
        {
            //Reverse tradebar order - Newest to Oldest
            _tradeBars.Reverse();

            decimal splitDevisor = 1m;

            foreach (var item in _tradeBars)
            {
                //Step 1: Adjust for splits if needed
                if(splitDevisor > 1)
                {
                    item.Close = item.Close / splitDevisor;
                    item.Open = item.Open / splitDevisor;
                    item.High = item.High / splitDevisor;
                    item.Low = item.Low / splitDevisor;
                }

                //Step 2: Update the splitDevisor if needed
                if (item.SplitCoefficient > 1)
                    splitDevisor = splitDevisor * item.SplitCoefficient;
            }

            //Reverse tradebar order - Oldest to Newest
            _tradeBars.Reverse();
           
        }

        /// <summary>
        /// Gets and reads historical stock quotes using AlphaAdvantage site.
        /// </summary>
        public void ReadData(string symbol, 
            Resolution resolution, 
            DateTime ? startDate, 
            DateTime ? endDate)
        {

            _symbol = symbol;
            _resolution = resolution;

            //Download CSV
           // IDataSource dataGenerator = new AlphaAdvantage();
            _csvFile = _dataSource.GenerateData(symbol, resolution);

            //Create Tradebars
            readCSV();
            adjustTradeBarsForSplits();

            foreach (TradeBar bar in _tradeBars)
            {
                
                bool skipLine = false;
          
                //Filter dates if applicable
                if (startDate != null && endDate != null)
                {
                    if (bar.Day < startDate || bar.Day > endDate)
                    {
                        skipLine = true;
                    }
                }

                if (skipLine) continue;
                
                //Watch for a massive  price drop due to sotck splits or some other strange event!!!.
                if (_previousTradebar != null)
                {
                   decimal variance = Math.Round((bar.Close - _previousTradebar.Close)/ _previousTradebar.Close,2);  
                   if( Math.Abs(variance) > .25m)  ///25% price difference between todays and yesteradys close
                    {
                        Logger.Log($"Unusual price variance on {bar.Symbol} on {bar.Day.ToShortDateString()}: {_previousTradebar.Close} vs {bar.Close}", ConsoleColor.Red);
                    }
                }

                //Throw Event to the alogos
                if (OnTradeBar != null)
                {
                    OnTradeBar(bar, e);
                }

                _previousTradebar = bar;
            }

        }

    }
}
