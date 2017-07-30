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
    public class CSVReader : IDataReader
    {
        #region Event Handlers

        public event OnDataHandler OnTradeBar;  //Used by base algorithm
        public EventArgs e = null;

        #endregion

        private TradeBar _previousTradebar;

        /// <summary>
        /// 
        /// </summary>
        public void ReadData(string symbol, 
            Resolution resolution, 
            DateTime ? startDate, 
            DateTime ? endDate)
        {
            //Make sure we have data to read
            IGenerator dataGenerator = new AlphaAdvantage();
            string inputFile= dataGenerator.GenerateData(symbol, resolution);
            int index = 0;

            string[] data = File.ReadAllLines(inputFile);

            for (int i = 0; i < data.Length; i++)
            {
                index++;
                var csv = data[i].Split(',');

                bool skipLine = false;
                DateTime transactionDate = DateTime.Parse(csv[0]);

                //Filter dates if applicable
                if (startDate != null && endDate != null)
                {
                    if (transactionDate < startDate || transactionDate > endDate)
                    {
                        skipLine = true;
                    }
                }

                if (skipLine) continue;

                //CSV Data: Time, Open, High, Low, Close, Volume
                TradeBar bar = new TradeBar()
                {
                    TradeResolution = resolution,
                    Symbol = symbol,
                    Day = DateTime.Parse(csv[0]),
                    Open = decimal.Parse(csv[1]),
                    High = decimal.Parse(csv[2]),
                    Low = decimal.Parse(csv[3]),
                    Close = decimal.Parse(csv[4]),
                    Volume = decimal.Parse(csv[5]),
                    SampleNumber = index
                };

                
                //Watch for a massive  price drop due to sotck splits or some other strange event!!!.
                if(_previousTradebar != null)
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
