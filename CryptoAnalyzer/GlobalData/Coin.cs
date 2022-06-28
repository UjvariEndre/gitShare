using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoAnalyzer.GlobalData
{
    class Coin
    {
        public string Ticker { get; set; }
        public decimal LastPrice { get; set; }
        public decimal HV30 { get; set; }
        public List<Ohlc> Candlesticks { get; set; }
    }
}
