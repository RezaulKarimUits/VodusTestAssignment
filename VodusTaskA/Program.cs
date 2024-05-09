
using Microsoft.EntityFrameworkCore;
using Vodus.Business;
using Vodus.Data;

class Program
{
    static void Main(string[] args)
    {

        var excelFilePath = @"C:\Users\Rezaul karim\Downloads\vodus-test-excel.xlsx";

        try
        {
            // Create DbContext instance
            var optionsBuilder = new DbContextOptionsBuilder<VodusDbContext>();
            optionsBuilder.UseNpgsql("User ID=postgres;Password=1234sadi;Host=localhost;Port=5432;Database=Vodus;");

            using (var dbContext = new VodusDbContext(optionsBuilder.Options))
            {
                var service = new ExcelService(dbContext);
                var products = service.ParseExcelToObjects(excelFilePath);

                string json = service.SerializeProductsToJson(products);

                service.SaveJsonToFile(json);

                service.SaveExcelDataToDatabase(products);

                Console.WriteLine(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    }
