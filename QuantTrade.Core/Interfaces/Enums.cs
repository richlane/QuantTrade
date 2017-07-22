using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core
{
    public enum Resolution
    {
        Daily
    }
 
    public enum MovingAverageType
    {
        Wilders,
        Exponential
    }


    public enum OrderStatus
    {
        Complete,
        New,
        Pending,
        Cancelled
    }

    public enum OrderType
    {
        MOC,
        MOO,
        Market
    }

    public enum Action
    {
        Buy,
        Sell,
        Hold
    }
}
