using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderManagementSystem.Infrastructure
{
    public static class OrderManagementContextSeed
    {
        public async static Task SeedAsync(OrderManagementDbContext _dbContext)
        {
            //start with brands and category 

            if (_dbContext.Products.Count() == 0) //if ProductBrands not contain any elements
            {
                var productsData = File.ReadAllText("../OrderManagementSystem.Infrastructure/Data/DataSeeding/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products?.Count() > 0)
                {
                    //Projection to set ID to zero 
                    //brands = brands.Select(b => new ProductBrand()
                    //{ 
                    //    Name = b.Name
                    //}).ToList();

                    foreach (var product in products)
                    {
                        _dbContext.Set<Product>().Add(product);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }

        }
    }
}
