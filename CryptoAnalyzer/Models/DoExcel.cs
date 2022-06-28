using CryptoAnalyzer.GlobalData;
using IronXL;
using System;
using System.Collections.Generic;
using System.IO;

namespace CryptoAnalyzer.Models
{
    class DoExcel
    {
        public static void ReadExcel(string fileName)
        {
            WorkBook workbook = WorkBook.Load(fileName);
            WorkSheet sheet = workbook.DefaultWorkSheet;
            Dictionary<string, Coin> top200 = new Dictionary<string, Coin>();
            foreach(var c in sheet.ToArray())
            {
                if (c.StringValue == "")
                    continue;
                Coin coin = new Coin()
                {
                    Ticker = c.StringValue,
                };
                top200.TryAdd(c.StringValue, coin);
            }
            Globals.Instance.Top200MarketCap = top200;
            Console.WriteLine("Reading Excel -> OK");
        }

        public static void WriteExcel(string fileName)
        {
            WorkBook workbook = WorkBook.Load(fileName);
            WorkSheet sheet = workbook.DefaultWorkSheet;
            int j = 0;
            foreach(var i in Globals.Instance.Top200MarketCap)
            {
                j++;
                sheet[$"B{j}"].DoubleValue = Convert.ToDouble(Math.Round(i.Value.HV30, 2));
            }
            //sheet["B1:B200"].SortDescending();
            //sheet["B2"].Style.BottomBorder.SetColor("#ff6600");
            //sheet["B2"].Style.BottomBorder.Type = IronXL.Styles.BorderType.Dashed;
            workbook.SaveAs($@"{Directory.GetCurrentDirectory()}\{fileName}");
            Console.WriteLine("Writing Excel -> OK");
            Console.WriteLine("Done");
        }
    }
}
