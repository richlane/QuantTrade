using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantTrade.Core;
using QuantTrade.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuantTrade.Tests
{
    [TestClass()]
    public class CSVProviderTests
    {
        [TestMethod()]
        public void ReadDataTest()
        { 
            string symbol = "SPY";

            IDataReader reader = new CSVReader();
            reader.OnData += Reader_OnData;
            reader.ReadData(symbol, Resolution.Daily, new AlphaAdvantage());
        }
        private void Reader_OnData(TradeBar data, EventArgs e)
        {
            Assert.IsTrue(data.High > 0);
        }
    }
}