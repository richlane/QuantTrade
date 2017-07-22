using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;
using System.Diagnostics;
using QuantTrade.Core.Securities;

namespace QuantTrade.Core.Indicators
{
    public class RelativeStrengthIndex : BaseIndicator, IIndicator
    {
        
        private ExponentialMovingAverage _avgGain;
        private ExponentialMovingAverage _avgLoss;
        private decimal ? _previousValue;
        
        public override bool IsReady
        {
            get
            {
                return _avgLoss.IsReady && _avgLoss.IsReady && (Samples >= Period * 5);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RelativeStrengthIndex(int period, MovingAverageType movingAverageType)
        {
            Period = period;
            
            //Creating a Wilders EMA
            if(movingAverageType == MovingAverageType.Wilders)
            { 
                 _avgGain = new ExponentialMovingAverage(Period, 1m / Period);
                 _avgLoss = new ExponentialMovingAverage(Period, 1m / Period);
            }
            //Creating a Standard EMA
            else
            {
                _avgGain = new ExponentialMovingAverage(Period, (decimal)period + 1.0m);
                _avgLoss = new ExponentialMovingAverage(Period, (decimal)period + 1.0m);

            }
        }

        /// <summary>
        /// Gets called from other indicators
        /// </summary>
        /// <param name="data"></param>
        public void UpdateIndicator (decimal data)
        {
            calculate(data);
        }


        /// <summary>
        /// Gets called from the CSVReader
        /// </summary>
        public void UpdateIndicator(TradeBar data)
        {
            calculate(data.Close);
            
        }

        private void calculate(decimal input)
        {
            Samples++;

            if (_previousValue != null && input >= _previousValue.Value)
            {
                _avgGain.UpdateIndicator(input - _previousValue.Value);
                _avgLoss.UpdateIndicator(0m);
            }
            else if (_previousValue != null && input < _previousValue.Value)
            {
                _avgGain.UpdateIndicator(0m);
                _avgLoss.UpdateIndicator(_previousValue.Value - input);
            }

            _previousValue = input;

            if (_avgLoss.Value == 0m)
            {
                // all up days is 100
                Value = 100m;
            }
            else
            {
                var rs = _avgGain.Value / _avgLoss.Value;
                Value = 100m - (100m / (1 + rs));
            }

           

        }
    }
}
