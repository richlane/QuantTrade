using System;

namespace QuantTrade.Core.Securities
{
    /// <summary>
    /// Stores tradebar information.
    /// </summary>
    public class TradeBar
    {
        public decimal SplitCoefficient { get; set; }

        public decimal Dividend { get; set; }

        public string Symbol { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Volume { get; set; }

        public decimal Open { get; set; }

        public decimal Close { get; set; }

        public DateTime Day { get; set; }

        public Resolution TradeResolution { get; set; }

        public int SampleNumber { get; set; }
    }
}
