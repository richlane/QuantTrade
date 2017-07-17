using QuantTrade.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Data
{
    public delegate void OnDataIndicatorHandler(TradeBar data, EventArgs e);

    public delegate void OnDataHandler(TradeBar data, EventArgs e);
}
