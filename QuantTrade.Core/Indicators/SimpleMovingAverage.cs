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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;
using System.ComponentModel;
using System.Diagnostics;
using QuantTrade.Core.Securities;

namespace QuantTrade.Core.Indicators
{
    /// <summary>
    /// SimpleMovingAverage
    /// </summary>
    public class SimpleMovingAverage : BaseIndicator, IIndicator
    {
        #region Properties

        private List<decimal> _rollingSum;

        public override bool IsReady
        {
            get
            {
                return _rollingSum.Count >= Period;
            }
        }

        #endregion


        /// <summary>
        /// Constructor - Creating standard SMA
        /// </summary>
        public SimpleMovingAverage(int period)
        {
            Period = period;
            _rollingSum = new List<decimal>();
        }


        // <summary>
        /// Gets called from other indicators
        /// </summary>
        public void UpdateIndicator(decimal price)
        {
            calculateValue(price);
        }

        /// <summary>
        /// Gets called from the CSVReader
        /// </summary>
        public void UpdateIndicator(TradeBar data)
        {
            calculateValue(data.Close);
        }

        /// <summary>
        /// 
        /// </summary>
        private void calculateValue(decimal price)
        {
            Samples++;

            //Make sure the last closing price is at the top
            _rollingSum.Insert(0, price);

            //Trim the buffer by remove the oldest price from the running numbers
            if (_rollingSum.Count > Period)
            {
                _rollingSum.RemoveAt(_rollingSum.Count - 1);
            }

            Value = _rollingSum.Average();
        }
    }
}
