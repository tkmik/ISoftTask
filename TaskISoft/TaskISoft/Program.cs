using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Out> max = new List<Out>();
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader1 = new StreamReader(exePath + "..\\..\\..\\Files\\orders.csv"))
            using (var reader2 = new StreamReader(exePath + "..\\..\\..\\Files\\order_items.csv"))
            using (var reader3 = new StreamReader(exePath + "..\\..\\..\\Files\\products.csv"))
            using (var csv1 = new CsvReader(reader1, CultureInfo.InvariantCulture))
            using (var csv2 = new CsvReader(reader2, CultureInfo.InvariantCulture))
            using (var csv3 = new CsvReader(reader3, CultureInfo.InvariantCulture))
            {
                var records1 = csv1.GetRecords<Orders>();
                var records2 = csv2.GetRecords<Order_Items>();
                var records3 = csv3.GetRecords<Products>();
                var result = records1.Join(
                    records2,
                    table1 => table1.ID,
                    table2 => table2.ORDER_ID,
                    (order, order_items) => new
                    {
                        datetime = order.DATE_TIME,
                        product_id = order_items.PRODUCT_ID,
                        quantity = order_items.QUANTITY
                    }).Join(
                    records3,
                    table2 => table2.product_id,
                    table3 => table3.ID,
                    (order_times, products) => new
                    {
                        datetime = order_times.datetime,
                        name = products.NAME,
                        quantity = order_times.quantity,
                        price = products.PRICE_PER_UNIT
                    }).GroupBy(day => day.datetime.Day);
                foreach (var item in result)
                {
                    max.Add(new Out()
                    {
                        Date = item.Where(max => (max.price * max.quantity) == item.Select(x => x.price * x.quantity).Max()).Select(day => day.datetime).FirstOrDefault(),
                        Name = item.Where(max => (max.price * max.quantity) == item.Select(x => x.price * x.quantity).Max()).Select(day => day.name).FirstOrDefault(),
                        Max = item.Select(max => max.price * max.quantity).Max()
                    });
                }
                foreach (var item in max)
                {
                    Console.WriteLine($"" +
                        $"{item.Date.Date.Day}/" +
                        $"{item.Date.Date.Month}/" +
                        $"{item.Date.Date.Year} " +
                        $"{item.Name} " +
                        $"{item.Max}");
                }
            }
        }
    }
}
