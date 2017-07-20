using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;


namespace QuantTrade.Core.Indicators
{
    public class RelativeStrengthIndex : BaseIndicator, IIndicator
    {
        
        private ExponentialMovingAverage _avgGains;
        private ExponentialMovingAverage _avgLoss;
        private decimal _previousValue;


        /// <summary>
        /// Constructor
        /// </summary>
        public RelativeStrengthIndex(int period)
        {
            this.Buffer = new List<decimal>();
            Period = period;
            _avgGains = new ExponentialMovingAverage(Period, 1m / period);
            _avgLoss = new ExponentialMovingAverage(Period, 1m / period);
        }

        /// <summary>
        /// Gets called from other indicators
        /// </summary>
        /// <param name="data"></param>
        public void UpdateIndicator (decimal data)
        {

        }


        /// <summary>
        /// Gets called from the CSVReader
        /// </summary>
        public void UpdateIndicator(TradeBar data)
        {
            var input = Math.Round(data.Close,2);
            decimal ? previousInput = null;

            //make sure we have enough info in the buffer
            if(Buffer.Count >= 2)
            {
                previousInput = Buffer[Buffer.Count - 1];
            }

            if (previousInput != null && input >= previousInput)
            {
                _avgGains.UpdateIndicator(input - previousInput.Value);
                _avgLoss.UpdateIndicator(0m);
            }
            else if (previousInput != null && input < previousInput)
            {
                _avgGains.UpdateIndicator(0m);
                _avgLoss.UpdateIndicator(previousInput.Value - input);
            }

            //Only need 2 items in the buffer
            Buffer.Add(input);

            if (Buffer.Count > 2)
            Buffer.RemoveAt(0);

            //Make sure we are really warmed up
            if(_avgLoss.IsReady && _avgLoss.IsReady)            if (data.SampleNumber >= 5)
                this.IsReady = true; 

            if (_avgLoss.Value == 0m)
            {
                // all up days is 100
                Value =  100m;
            }
            else
            {
                var rs = _avgGains.Value / _avgLoss.Value;
                Value= 100m - (100m / (1 + rs));
            }

            
        }
    }
}
