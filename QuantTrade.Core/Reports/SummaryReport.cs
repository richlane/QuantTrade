/*
* BSD 2-Clause License 
* Copyright (c) 2017, Rich Lane 
* All rights reserved. 
* 
* Redistribution and use in source and binary forms, with or without 
* modification, are permitted provided that the following conditions are met: 
* 
* Redistributions of source code must retain the above copyright notice, this 
* list of conditions and the following disclaimer. 
* 
* Redistributions in binary form must reproduce the above copyright notice, 
* this list of conditions and the following disclaimer in the documentation 
* and/or other materials provided with the distribution. 
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE 
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/

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
        [DisplayFormat(DataFormatString = "{0:P2}")]
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
