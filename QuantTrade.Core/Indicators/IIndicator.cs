using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;

namespace QuantTrade.Core.Indicators
{
    public interface IIndicator
    {
        void UpdateIndicator(TradeBar data);
    }
}
