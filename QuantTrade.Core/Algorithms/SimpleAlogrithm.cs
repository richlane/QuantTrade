using QuantTrade.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Algorithm
{
    public class SimpleAlogrithm : BaseAlgorithm, IAlogorithm
    {

        private string _symbol = "SPY";
       
        private int _startYear = 2013;
        private int _endYear = 2013;

        //Account Settings
        private decimal _transactionFee = 7M;
        private decimal _availableCash = 10000M;
        private Resolution _resolution = Resolution.Daily;


        /// <summary>
        /// Launch Algo
        /// </summary>
        public void Initialize()
        {
            SetStartDate(_startYear, 1, 1);
            SetEndDate(_endYear, 12, 31);
            Portfolio.Cash = _availableCash;
            TransactionFee = _transactionFee;
            Resolution = _resolution;
            Symbol = _symbol;

           // Indicators.Add(new RelativeStrengthIndex(_symbol, 2));

            RunTest();

        }

        /// <summary>
        /// Event Handing new trade bar
        /// </summary>
        public override void OnData(TradeBar data, EventArgs e)
        {
            string x = "";
        }

    }
}
