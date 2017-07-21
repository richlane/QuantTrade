﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QuantTrade.Core.Configuration;
using QuantTrade.Core;
using QuantTrade.Core.Indicators;

namespace QuantTrade.Core.Data
{
    public class CSVReader : IDataReader
    {
        #region Event Handlers

        public event OnDataIndicatorHandler OnDataIndicator;  //used by indicators
        public event OnDataHandler OnData;  //Used by algorithms
        public EventArgs e = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void ReadData(string symbol, Resolution resolution)
        {
            ReadData(symbol, resolution, new List<IIndicator>(), null, null);
        }

 
        /// <summary>
        /// 
        /// </summary>
        public void ReadData(string symbol, 
            Resolution resolution, 
            List<IIndicator> indicators, 
            DateTime ? startDate, 
            DateTime ? endDate)
        {
            //Make sure we have data to read
            IGenerator dataGenerator = new AlphaAdvantage();
            string inputFile= dataGenerator.GenerateData(symbol, resolution);
            int index = 0;


            //Read the file
            using (StreamReader fileReader = new StreamReader(inputFile))
            {
                while (!fileReader.EndOfStream) 
                {
                    index++;
                    var csv = fileReader.ReadLine().Split(',');

                    bool skipLine = false;
                    DateTime transactionDate = DateTime.Parse(csv[0]);

                    //Filter dates if applicable
                    if (startDate != null && endDate != null)
                    {
                        if(transactionDate < startDate || transactionDate > endDate)
                        {
                            skipLine = true;
                        }
                    }

                    if (skipLine) continue;

                    //Time, Open, High, Low, Close, Volume
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

                    
                    //Update Indicators
                    foreach (IIndicator ind in indicators)
                    {
                        ind.UpdateIndicator(bar);
                    }
                  
                    //Throw Event to the alogos
                    if (OnData != null)
                    {
                        OnData(bar, e);
                    }


                }

            
            }

        }

    }
}
