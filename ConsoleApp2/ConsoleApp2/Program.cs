using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Out> max = new List<Out>();
            using (var reader1 = new StreamReader(@"C:\orders.csv"))
            using (var reader2 = new StreamReader(@"C:\order_items.csv"))
            using (var reader3 = new StreamReader(@"C:\products.csv"))
            using (var csv1 = new CsvReader(reader1, CultureInfo.InvariantCulture))
            using (var csv2 = new CsvReader(reader2, CultureInfo.InvariantCulture))
            using (var csv3 = new CsvReader(reader3, CultureInfo.InvariantCulture))
            {
                var records1 = csv1.GetRecords<Orders>();
                var records2 = csv2.GetRecords<Order_Items>();
                var records3 = csv3.GetRecords<Products>();


                var result = records1.Join(
                    records2,
                    rec1 => rec1.ID,
                    rec2 => rec2.ORDER_ID,
                    (t, pid) => new
                    {
                        t = t.DATE_TIME,
                        pid = pid.PRODUCT_ID,
                        pid.QUANTITY
                    }).Join(
                    records3,
                    recp => recp.pid,
                    rec3 => rec3.ID,
                    (time, price) => new
                    {
                        time = time.t,
                        name = price.NAME,
                        quantity = time.QUANTITY,
                        price = price.PRICE_PER_UNIT
                    }).GroupBy(day => day.time.Day);
                foreach (var item in result)
                {
                    max.Add(new Out()
                    {
                        Date = item.Where(max => (max.price * max.quantity) == item.Select(x => x.price * x.quantity).Max()).Select(day => day.time).FirstOrDefault(),
                        Name = item.Where(max => (max.price * max.quantity) == item.Select(x => x.price * x.quantity).Max()).Select(day => day.name).FirstOrDefault(),
                        Max = item.Select(x => x.price * x.quantity).Max()
                    });
                }
                foreach (var item in max)
                {
                    Console.WriteLine($"{item.Date.Date.Month}/{item.Date.Date.Day}/{item.Date.Date.Year} {item.Name} {item.Max}");
                }
            }

        }
    }
}
