using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantTrade.Core;
using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core.Indicators;
using System.Diagnostics;

namespace QuantTrade.Tests
{
    [TestClass()]
    public class AlgoTests
    {

        [TestMethod()]
        public void ExponentialMovingAverage()
        {
            int period =6;

            //Read a string
            double[] vars = { 145.44, 139.8, 140.8, 137.8, 145.8, 146.3, 144.1, 143.1, 145, 147, 145.8, 147, 144.8, 144.4, 140.3, 141.9, 140.8, 140.3, 135.9, 139.6 };
            ExponentialMovingAverage ema = new Core.Indicators.ExponentialMovingAverage(period);
            
            for (int i = 0; i < vars.Length; i++)
            {
                ema.UpdateIndicator(Convert.ToDecimal(vars[i]));

                if (i+1 == period) Assert.IsTrue(ema.IsReady);

                if (i == 5) Assert.IsTrue(Math.Round(ema.Value,2) == 142.66m, "Assert 5");
                if (i == 6) Assert.IsTrue(Math.Round(ema.Value, 2) == 143.08m, "Assert 6");
                if (i == 7) Assert.IsTrue(Math.Round(ema.Value, 2) == 143.08m, "Assert 7");
                if (i == 8) Assert.IsTrue(Math.Round(ema.Value, 2) == 143.64m, "Assert 8");
                if (i == 9) Assert.IsTrue(Math.Round(ema.Value, 2) == 144.61m, "Assert 9");

            }

        }

        [TestMethod()]
        public void ReadDataTest()
        { 
            string symbol = "DONOTDELETE";

            IDataReader reader = new CSVReader();
            reader.OnData += Reader_OnData;
            reader.ReadData(symbol, Resolution.Daily);
            
        }

        private void Reader_OnData(TradeBar data, EventArgs e)
        {
            Assert.IsTrue(data.High > 0, "high");
            Assert.IsTrue(data.Low > 0, "low");
            Assert.IsTrue(data.Symbol == "DONOTDELETE", "symbol");
            Assert.IsTrue(data.TradeResolution == Resolution.Daily,  "traderes");
            Assert.IsTrue(data.Volume > 0,  "volume");
        }


        [TestMethod()]
        public void RunSimpleAlgo()
        {
            //IAlogorithm algo = new SimpleAlogrithm();
            //algo.Initialize();
        }
    }

}