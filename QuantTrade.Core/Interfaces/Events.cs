using QuantTrade.Core;
using QuantTrade.Core.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core
{
    public delegate void OnDataHandler(TradeBar data, EventArgs e);
    public delegate void OnOrderHandler(Order data, EventArgs e);
}
