using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;
using QuantTrade.Data.Providers;
using System;

namespace QuantTrade.Algorithm 
{
    public class BaseAlgorithm 
    {
        public virtual void Initialize(string symbol, Resolution resolution)
        {
            
            IDataReader reader = new CSVReader();
            reader.OnData += OnData;
            reader.ReadData(symbol, resolution, new AlphaAdvantage());
        }

        public virtual void OnData(TradeBar data, EventArgs e)
        {

        }

    }
}
