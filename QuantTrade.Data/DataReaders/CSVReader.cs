using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using QuantTrade.Configuration;
using QuantTrade.Core;

namespace QuantTrade.Data.Providers 
{
    public class CSVReader : IDataReader
    {
        #region Event Handlers

        public event OnDataHandler OnData;
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
      
            using (TextReader fileReader = File.OpenText(inputFile))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                while (csv.Read())
                {
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

                    //Throw Events!!!!!
                    if(OnData != null)
                    {
                        OnData(bar, e);
                    }

                    //string x = "";
                }
            }

        }

    }
}
