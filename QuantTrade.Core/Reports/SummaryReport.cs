using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuantTrade.Core.Reports
{
    /// <summary>
    /// Report data for the algo output
    /// </summary>
    public class SummaryReport
    {
        [DisplayName("Algo Name")]
        public string AlgorithmName { get; set; }

        [DisplayName("Run Dates")]
        public string RunDates { get; set; }

        [DisplayName("Symbol")]
        public string Symbol { get; set; }

        [DisplayName("Buy & Hold")]
        public bool BuyAndHold { get; set; }

        [DisplayName("Starting Account")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal StartingAccount { get; set; }

        [DisplayName("Ending Account")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal EndingAccount { get; set; }

        [DisplayName("Net Profit")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal NetProfit { get; set; }

        [DisplayName("Annual Return")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public double AnnualReturn { get; set; }

        [DisplayName("Win Rate")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal WinRate { get; set; }

        [DisplayName("Loss Rate")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal LossRate { get; set; }

        [DisplayName("Max Drawdown")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal MaxDrawDown { get; set; }

        [DisplayName("Total Fees")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalFees { get; set; }

        [DisplayName("Total Trades")]
        public int TotalTrades { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }


}
}
