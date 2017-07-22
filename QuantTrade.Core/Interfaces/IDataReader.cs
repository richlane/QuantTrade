using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;
using QuantTrade.Core.Indicators;

namespace QuantTrade.Core.Data
{
    public interface IDataReader
    {
        event OnDataHandler OnTradeBar;
       
        void ReadData(string symbol, 
                      Resolution resolution, 
                      DateTime? startDate,
                      DateTime? endDate);

        void ReadData(string symbol, Resolution resolution);
    }
}
