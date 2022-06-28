using Binance.Net.Enums;
using CryptoAnalyzer.GlobalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoAnalyzer.Models
{
    class Analysis
    {
        public async static Task GetCandleSticks()
        {
            DateTime now = DateTime.Now;
            DateTime end = Globals.Instance.RestfulClient.Spot.System.GetServerTimeAsync().GetAwaiter().GetResult().Data;
            DateTime start = end.AddDays(-30);
            AskTime askTime = new AskTime()
            {
                StartYear = start.Year,
                StartMonth = start.Month,
                StartDay = start.Day,
                StartHour = 0,
                StartMinute = 0,
                EndYear = end.Year,
                EndMonth = end.Month,
                EndDay = end.Day,
                EndHour = 0,
                EndMinute = 0
            };

            foreach (var c in Globals.Instance.Top200MarketCap.Keys)
            {
                try
                {
                    await Task.Run(() => DoSubscribeToKlines(c, askTime));
                    await Task.Run(() => CalculateHistoricalVolatility(c));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        public static decimal GetHistoricalVolatility()
        {
            return 0;
        }

        public async static Task DoSubscribeToKlines(string coin, AskTime t)
        {
            Coin tempCoin;
            Globals.Instance.Top200MarketCap.TryGetValue(coin, out tempCoin);
            var candles = await Task.Run(() => Globals.Instance.RestfulClient.Spot.Market.GetKlinesAsync($"{coin}USDT", KlineInterval.OneDay,
                    new DateTime(t.StartYear, t.StartMonth, t.StartDay, t.StartHour, t.StartMinute, 00),
                    new DateTime(t.EndYear, t.EndMonth, t.EndDay, t.EndHour, t.EndMinute, 00), 31).Result.Data.SkipLast(1).ToList());

            List<Ohlc> tempList = new List<Ohlc>();
            foreach (var i in candles)
            {
                Ohlc ohlc = new Ohlc()
                {
                    Open = i.Open,
                    High = i.High,
                    Low = i.Low,
                    Close = i.Close
                };
                tempList.Add(ohlc);
            }
            tempCoin.Candlesticks = tempList;
            Console.WriteLine($"Historical Data of {coin} -> OK");
        }

        public async static Task CalculateHistoricalVolatility(string coin)
        {
            Coin tempCoin;
            Globals.Instance.Top200MarketCap.TryGetValue(coin, out tempCoin);
            if(tempCoin.Candlesticks.Count == 30)
            {
                double n = 30;
                double sumR = 0;
                foreach (var i in tempCoin.Candlesticks)
                {
                    double R = Convert.ToDouble(i.Close / i.Open);
                    R = Math.Log(R);
                    sumR += R;
                }
                double Rvar = sumR / n;
                double sumR2 = 0;
                foreach (var i in tempCoin.Candlesticks)
                {
                    double R = Convert.ToDouble(i.Close / i.Open);
                    R = Math.Log(R);
                    sumR2 += Math.Pow((R - Rvar), 2);
                }
                double sigmaSqr = sumR2 / (n - 1);
                double sigma = Math.Sqrt(sigmaSqr);
                sigma = Math.Sqrt(365) * sigma * 100;
                tempCoin.HV30 = Convert.ToDecimal(sigma);
            }
            else
            {
                tempCoin.HV30 = -1;
            }
            Console.WriteLine($"Calculate HV30 of {coin} -> OK");
        }
    }
}
