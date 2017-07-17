using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;


namespace QuantTrade.Data.Providers
{
    public delegate void OnDataHandler(TradeBar data, EventArgs e);

    public interface IDataReader
    {
        event OnDataHandler OnData;
       
        void ReadData(string symbol, Resolution resolution, IGenerator dataGenerator);
    }
}
