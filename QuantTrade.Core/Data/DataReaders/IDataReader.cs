﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;


namespace QuantTrade.Core.Data
{
    public interface IDataReader
    {
        event OnDataIndicatorHandler OnDataIndicator;
        event OnDataHandler OnData;
       
        void ReadData(string symbol, Resolution resolution, IGenerator dataGenerator);
    }
}