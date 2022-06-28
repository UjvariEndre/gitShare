using Binance.Net;
using CryptoAnalyzer.GlobalData;
using CryptoAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoAnalyzer
{
    class Globals
    {
        private const string apiKey = "a0f6JgemxnOVFbGeH7hEdE3snk8ZcW7K0XHFg4CuddVrwdr7EyzbSGH1tRHgfDhL";
        private const string apiSecret = "ZThtMOWzVl3Hx4t41tYdQrlj8kMTYJPhOYyD5L2q752e5obzBObDPdgDcq23Lqh5";
        public static Globals Instance { get; set; }
        public BinanceClient RestfulClient { get; private set; }
        public BinanceSocketClient SocketClient { get; private set; }
        public CryptoExchange.Net.Objects.WebCallResult<Binance.Net.Objects.Spot.SpotData.BinanceAccountInfo> SpotInfo { get; set; }
        public Dictionary<string, Coin> Top200MarketCap { get; set; }
        public string FileName { get; set; }
        public string IONXLCrack { get; set; }
        static Globals()
        {
            Instance = new Globals();
        }

        public async Task StartUp()
        {
            //GenerateCode.GetCode();
            //IronXL.License.LicenseKey = IONXLCrack;
            DateTime now = DateTime.Now;
            FileName = $"{now.Year}-{now.Month.ToString("D2")}-{now.Day.ToString("D2")}";
            Console.WriteLine(FileName);
            DoExcel.ReadExcel($"worksheets/{FileName}.xlsx");

            RestfulClient = new BinanceClient(new Binance.Net.Objects.BinanceClientOptions()
            {
                // Specify options for the client
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials(apiKey, apiSecret),
                AutoTimestamp = true,
                TradeRulesBehaviour = Binance.Net.Enums.TradeRulesBehaviour.AutoComply
            });

            SocketClient = new BinanceSocketClient(new Binance.Net.Objects.BinanceSocketClientOptions()
            {
                // Specify options for the client
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials(apiKey, apiSecret),
                AutoReconnect = true
            });
            try
            {
                SpotInfo = Instance.RestfulClient.General.GetAccountInfoAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Analysis.GetCandleSticks();
            DoExcel.WriteExcel($"worksheets/{FileName}.xlsx");
        }
    }
}
