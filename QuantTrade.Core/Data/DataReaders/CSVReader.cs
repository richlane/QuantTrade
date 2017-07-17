using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QuantTrade.Core.Configuration;
using QuantTrade.Core;

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
        /// <param name="symbol"></param>
        /// <param name="resolution"></param>
        /// <param name="dataGenerator"></param>
        public void ReadData(string symbol, Resolution resolution, IGenerator dataGenerator)
        {
            //Make sure we have data to read
            string inputFile= dataGenerator.GenerateData(symbol, resolution);
      
            using (StreamReader fileReader = new StreamReader(inputFile))
            {
                while (!fileReader.EndOfStream) //best way to do it
                {
                    var csv = fileReader.ReadLine().Split(',');

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
                        Volume = decimal.Parse(csv[5])
                    };


                    //1. Throw event to the indicators
                    if (OnDataIndicator != null)
                    {
                        OnDataIndicator(bar, e);
                    }

                    //2. Throw event to the alogos
                    if (OnData != null)
                    {
                        OnData(bar, e);
                    }


                }

            
            }

        }

    }
}
