using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;

namespace QuantTrade.Core.Indicators
{
    public class RelativeStrengthIndex : IIndicator
    {
        public MovingAverageType MovingAverageType { get; set; }

        public int Period { get; set; }

        public Resolution Resolution { get; set; }

        public RelativeStrengthIndex(string symbol, int period, MovingAverageType movingAverageType = MovingAverageType.Wilders, Resolution resolution = Resolution.Daily)
        {
            MovingAverageType = movingAverageType;
            Period = period;
            Resolution = resolution;
        }

        public void UpdateIndicator(TradeBar data)
        {

        }
    }
}
