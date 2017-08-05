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
using QuantTrade.Core.Securities;
using QuantTrade.Core.Utilities;
using QuantTrade.Core.Reports;

namespace QuantTrade.Tests
{
    [TestClass()]
    public class DataTests
    {

    
        [TestMethod()]
        public void ReadDataTest()
        { 
            string symbol = "DONOTDELETE";

            IDataReader reader = new CSVReader(new AlphaAdvantage());
            reader.OnTradeBar += Reader_OnData;
            reader.ReadData(symbol, Resolution.Daily, null, null);
        }

        [TestMethod()]
        private void Reader_OnData(TradeBar data, EventArgs e)
        {
            Assert.IsTrue(data.High > 0, "high");
            Assert.IsTrue(data.Low > 0, "low");
            Assert.IsTrue(data.Symbol == "DONOTDELETE", "symbol");
            Assert.IsTrue(data.TradeResolution == Resolution.Daily,  "traderes");
            Assert.IsTrue(data.Volume > 0,  "volume");
        }

    }

}