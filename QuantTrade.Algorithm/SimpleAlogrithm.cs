using QuantTrade.Core;
using QuantTrade.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Algorithm
{
    public class SimpleAlogrithm : BaseAlgorithm, IAlogorithm
    {
        private string _symbol = "SPY";
        private Resolution _resolution = Resolution.Daily;

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            base.Initialize(_symbol, _resolution);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="e"></param>
        public override void OnData(TradeBar data, EventArgs e)
        {
            string x = "";
        }

    }
}
