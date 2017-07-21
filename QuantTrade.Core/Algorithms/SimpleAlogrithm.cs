using QuantTrade.Core;
using QuantTrade.Core.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Algorithm
{
    public enum Action
    {
        Buy, Sell, Hold
    }

    public class SimpleAlogrithm : BaseAlgorithm, IAlogorithm
    {
        private string _symbol = "SPY";
        bool _buyAndHold = true;

        int _smaLookBackPeriod = 30;
        int _rsiLookBackPeriod = 2;
        int _rsiBuyLevel = 30;
        int _rsiSellLevel = 70;

        private int _startYear = 2013;
        private int _endYear = 2013;

        //Account Settings
        private decimal _transactionFee = 7M;
        private decimal _availableCash = 10000M;

        //Indicators
        private Resolution _resolution = Resolution.Daily;
        private RelativeStrengthIndex _rsi;
        private SimpleMovingAverage _sma;

        #region Misc

        bool _isLastTradingDay;
        DateTime _previousDate;
        bool _holdingStock;
        bool _firstRun = true;
        decimal _sellStopPrice;
        decimal _pctToInvest;


        #endregion
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

            //Setup Indictors
            _rsi = GenerateRelativeStrengthIndexIndicator(_rsiLookBackPeriod, MovingAverageType.Wilders);
            _sma = GenerateSimpleMovingAverageIndicator(_smaLookBackPeriod);

        
            //Execute Tests
            RunTest();

            //All Done
            Console.WriteLine("Samples Read: " + _rsi.Samples);
        }

        /// <summary>
        /// Event Handing new trade bar
        /// </summary>
        public override void OnData(TradeBar data, EventArgs e)
        {
            //Make sure indicators are ready
            if (!_rsi.IsReady || !_sma.IsReady)
                return;

            //Watch for last trading day so we can liquidate portfolio
            if (data.Day >= EndDate.AddDays(-5).Date)
                _isLastTradingDay = true;

            Action action = getBuySellHoldDecision(data);


            switch (action)
            {
                case Action.Buy:
                    break;

                case Action.Sell:
                    break;
            }

            Console.WriteLine(action);

        }

        private Action getBuySellHoldDecision(TradeBar data)
        {
            if (_buyAndHold)
                return getLongTermBuyAndHold(data);

            Action action = Action.Hold;
            bool buying = false;


            /////////////////////////////////////////
            //Buy Logic
            /////////////////////////////////////////
            if (
                _rsi.Value  < _rsiBuyLevel &&
                _isLastTradingDay == false &&
                _pctToInvest < 1M
                )
            {
                action = Action.Buy;
                _holdingStock = true;
                buying = true;
      
                //_pctToInvest = 1M;

                if (_pctToInvest == 0)
                {
                    _pctToInvest = .2M;
                }
                else if (_pctToInvest == .2M)
                {
                    _pctToInvest = .38M;
                }
                else if (_pctToInvest == .38M)
                {
                    _pctToInvest = 1M;
                }

                //if (_pctToInvest == 0)
                //{
                //    _pctToInvest = .5M;
                //}
                //else if (_pctToInvest == .5M)
                //{
                //    _pctToInvest = 1M;
                //}
            }

            /////////////////////////////////////////
            //Sell and Hold Logic
            /////////////////////////////////////////
            if (_holdingStock && buying == false)
            {
                //Sell
                if (
                   _rsi.Value > _rsiSellLevel
                   && data.Close > _sma.Value
                    )
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    _holdingStock = false;
                    _sellStopPrice = 0;
                }
                //Sell
                else if (_isLastTradingDay)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    _holdingStock = false;
                    _sellStopPrice = 0;
                }
                //Sell
                else if (_sellStopPrice > 0 && data.Close < _sellStopPrice)
                {
                    action = Action.Sell;
                    _pctToInvest = 0;
                    _holdingStock = false;
                    _sellStopPrice = 0;
                }
                //Hold
                else
                {
                    action = Action.Hold;
                }

            }

            return action;

        }

        private Action getLongTermBuyAndHold(TradeBar data)
        {
            Action action = Action.Hold;
       
            if (_holdingStock == false &&
                _isLastTradingDay == false)
            {
                action = Action.Buy;
                _holdingStock = true;
                _pctToInvest = 1M;
            }
            else if (_holdingStock == true &&
                 _isLastTradingDay)
            {
                action = Action.Sell;
                _holdingStock = false;
                _pctToInvest = 0M;
            }


            return action;
        }
    }
}
