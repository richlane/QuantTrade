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
        private decimal _averageGain;
        private decimal _averageLoss;
        private int _period;
        int _samples;
        decimal _last;
        decimal _previousValue;
        

     
        public MovingAverageType MovingAverageType { get; set; }

        public bool IsReady { get; private set; }
        
        public decimal Value { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RelativeStrengthIndex(
            string symbol, 
            int period, 
            MovingAverageType movingAverageType = MovingAverageType.Wilders,
            Resolution resolution = Resolution.Daily)
        {
            MovingAverageType = movingAverageType;
            _period = period;
            Resolution = resolution;
            //Name = name;
        }

       
        /// <summary>
        /// Gets called from the CSVReader
        /// </summary>
        public void UpdateIndicator(TradeBar data)
        {
            _last = 0;
            decimal value = data.Close;

            if (_previousValue <= 0)
            {
                _previousValue = value;
                return;
            }

            if (_samples < _period - 1)
            {
                if (_previousValue < value)
                {
                    _averageGain += value - _previousValue;
                }
                else
                {
                    _averageLoss += _previousValue - value;
                }   

                _samples++;
                _previousValue = value;
                return;
            }

            if (_samples == _period - 1)
            {
                if (_previousValue < value)
                {
                    _averageGain += value - _previousValue;
                }
                else
                {
                    _averageLoss += _previousValue - value;
                }

                _samples++;
                _averageGain /= _period;
                _averageLoss /= _period;
                
            }
            else
            {
                _samples++;

                if (_previousValue < value)
                {
                    _averageGain = (_averageGain * (_period - 1.0m) + (value - _previousValue)) / _period;
                    _averageLoss = (_averageLoss * (_period - 1.0m)) / _period;
                }
                else
                {
                    _averageGain = (_averageGain * (_period - 1.0m)) / _period;
                    _averageLoss = (_averageLoss * (_period - 1.0m) + (_previousValue - value)) / _period;
                }
            }

            if (_averageLoss != 0)
            {
                IsReady = true;
                Value = Math.Round( 100.0m - 100.0m / (1.0m + _averageGain / _averageLoss), 2 );
            }
            else
            {
                IsReady = true;
                Value = 100m;
            }
               

            _previousValue = value;


        }
    }
}
