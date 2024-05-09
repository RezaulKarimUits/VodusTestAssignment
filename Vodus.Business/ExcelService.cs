using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using System.Data;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Text.Json;
using Vodus.Data.Entity;
using Vodus.Data.Model;
using Vodus.Data;

namespace Vodus.Business
{
    public class ExcelService
    {
        private VodusDbContext _dbContext;

        public ExcelService(VodusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProductViewModel> ParseExcelToObjects(string filePath)
        {


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            {
                var products = new List<ProductViewModel>();

                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {

                                int id = 0;

                                if (int.TryParse(reader.GetValue(0)?.ToString(), out id))
                                {
                                    id = Convert.ToInt32(reader.GetValue(0));
                                    var image = reader.GetValue(1).ToString();
                                    var name = reader.GetValue(2).ToString();
                                    DateTime date = reader.GetDateTime(3);
                                    decimal price = Convert.ToDecimal(reader.GetValue(4));
                                    decimal discountedPrice = Convert.ToDecimal(reader.GetValue(5));
                                    products.Add(new ProductViewModel
                                    {
                                        Id = id,
                                        Name = name,
                                        Image = image,
                                        OrderDate = date,
                                        Price = price,
                                        DiscountedPrice = discountedPrice
                                    });
                                }

                            }
                        } while (reader.NextResult());
                    }
                }

                return products;
            }

        }
        public string SerializeProductsToJson(List<ProductViewModel> products)
        {
            return JsonConvert.SerializeObject(products);
        }
        public void SaveJsonToFile(string json)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, "Order.json");
                using var stream = File.Create(filePath);
                System.Text.Json.JsonSerializer.Serialize(stream, json, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                Console.WriteLine("JSON data saved to Order.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON file: {ex.Message}");
            }
        }
        public void SaveExcelDataToDatabase(List<ProductViewModel> products)
        {
            List<Order> orders = products.Select(p => new Order
            {
                Name = p.Name,
                Price = p.Price,
                Image = p.Image,
                DiscountedPrice = p.DiscountedPrice,
                OrderDate = DateTime.SpecifyKind(p.OrderDate, DateTimeKind.Utc)
            }).ToList();


            _dbContext.AddRange(orders);
            _dbContext.SaveChanges();

        }
    }
}
