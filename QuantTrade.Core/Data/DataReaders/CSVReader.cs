/*
* BSD 2-Clause License 
* Copyright (c) 2017, Rich Lane 
* All rights reserved. 
* 
* Redistribution and use in source and binary forms, with or without 
* modification, are permitted provided that the following conditions are met: 
* 
* Redistributions of source code must retain the above copyright notice, this 
* list of conditions and the following disclaimer. 
* 
* Redistributions in binary form must reproduce the above copyright notice, 
* this list of conditions and the following disclaimer in the documentation 
* and/or other materials provided with the distribution. 
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE 
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QuantTrade.Core;
using QuantTrade.Core.Indicators;
using QuantTrade.Core.Securities;
using QuantTrade.Core.Utilities;
using System.Globalization;

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
        private bool _randomizeData = false;

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
                    Open = decimal.Parse(csv[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                    High = decimal.Parse(csv[2], NumberStyles.Any, CultureInfo.InvariantCulture),
                    Low = decimal.Parse(csv[3], NumberStyles.Any, CultureInfo.InvariantCulture),
                    Close = decimal.Parse(csv[4], NumberStyles.Any, CultureInfo.InvariantCulture),
                    Volume = decimal.Parse(csv[6], NumberStyles.Any, CultureInfo.InvariantCulture),
                    Dividend = decimal.Parse(csv[7], NumberStyles.Any, CultureInfo.InvariantCulture),
                    SplitCoefficient = decimal.Parse(csv[8], NumberStyles.Any, CultureInfo.InvariantCulture),
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
                if (splitDevisor > 1)
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
            DateTime? startDate,
            DateTime? endDate)
        {

            _symbol = symbol;
            _resolution = resolution;
            _randomizeData = Convert.ToBoolean(Config.GetToken("randomize-data"));

            //Download CSV & read the data
            _csvFile = _dataSource.GenerateData(symbol, resolution);
            readCSV();

            //Adjust bars
            adjustTradeBarsForSplits();

            //Loop the trade bars
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

                //Watch for a massive price drop due to sotck splits or some other strange event!!!.
                if (_previousTradebar != null)
                {
                    decimal variance = Math.Round((bar.Close - _previousTradebar.Close) / _previousTradebar.Close, 2);
                    if (Math.Abs(variance) > .25m)  ///25% price difference between todays and yesteradys close
                    {
                        Logger.Log($"Unusual price variance on {bar.Symbol} on {bar.Day.ToShortDateString()}: {_previousTradebar.Close} vs {bar.Close}", ConsoleColor.Red);
                    }
                }

                //Randominze Data
                if (_randomizeData) randomizeData(bar);

                //Throw Event to the alogos
                if (OnTradeBar != null)
                {
                    OnTradeBar(bar, e);
                }

                _previousTradebar = bar;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="newBar"></param>
        private void randomizeData(TradeBar tradeBar)
        {
            decimal volatility = .1m;

            Random randomizer = new Random();

            decimal rnd = Convert.ToDecimal(randomizer.NextDouble());

            decimal changePercent = 2 * volatility * rnd;

            if (changePercent > volatility)
            {
                changePercent -= (2 * volatility);
            }

            if (changePercent > 0)
            {
                changePercent = 1 + changePercent;
            }
            else
            {
                changePercent = 1 - changePercent;
            }

            //Update the prices
            tradeBar.Open = tradeBar.Open * changePercent;
            tradeBar.Close = tradeBar.Close * changePercent;
            tradeBar.High = tradeBar.High * changePercent;
            tradeBar.Low = tradeBar.Low * changePercent;

        }
    }
}
