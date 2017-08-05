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
    public class LoggerTests
    {
        [TestMethod()]
        public void LoggerTest()
        {
            //Just making sure nothing exlodes (i.e. null values, etc)
            Logger.LogSummaryReport(new SummaryReport());

            Logger.Log("test");
        }


    }

}