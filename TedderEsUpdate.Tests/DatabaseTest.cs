using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TedderEsUpdate.Models;
using Xunit;

namespace TedderEsUpdate.Tests
{
    public class DatabaseTest
    {
        [Fact]
        public async Task GetAllWebProducts()
        {
            using AghMageContext db = new AghMageContext();
            db.Database.OpenConnection();

            using DbCommand command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "call get_current_products();";

            using var result = await command.ExecuteReaderAsync();
            List<ProductModel> productModelList = new List<ProductModel>();

            try
            {
                productModelList = result.Cast<IDataRecord>()
                .Select(dr => new ProductModel
                {
                    Type = dr.GetString(0),
                    Sku = dr.GetString(1),
                    CreatedAt = dr.GetDateTime(2),
                    Image = dr.GetString(3),
                    Url = dr.GetString(4),
                    Price = dr.GetDecimal(5),
                    Name = dr.GetString(6),
                    Description = dr.GetString(7)
                }).AsParallel()
                .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.InnerException.ToString());
            }

            Assert.NotNull(productModelList);
            Assert.IsType<List<ProductModel>>(productModelList);
            Assert.NotEmpty(productModelList);
        }

        [Fact]
        public async Task GetWebProductCount()
        {
            using AghMageContext db = new AghMageContext();
            db.Database.OpenConnection();

            using DbCommand command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "call get_current_products_count();";

            using var result = await command.ExecuteReaderAsync();
            result.Read();
            int count = result.GetInt32(0);

            Debug.WriteLine($"Product count: {count}");

            Assert.IsType<int>(count);
            Assert.InRange(count, 1_000, 10_000);
        }

        [Fact]
        public async Task GetAllWebBlogPosts()
        {
            using AghMageContext db = new AghMageContext();
            var blogPosts = await db.AwBlog
                    .Where(b => b.Status == 1)
                    .Select(b => new ProductModel
                    {
                        Type = "blog",
                        Name = b.Title,
                        CreatedAt = b.CreatedTime,
                        Url = b.Identifier,
                        Price = 0,
                        Description = "",
                        Sku = "",
                        Image = ""
                    }).ToListAsync();

            Assert.NotNull(blogPosts);
            Assert.IsType<List<ProductModel>>(blogPosts);
            Assert.NotEmpty(blogPosts);
        }

        [Fact]
        public void GetWebBlogPostsCount()
        {
            using AghMageContext db = new AghMageContext();
            int count = db.AwBlog
                .Where(b => b.Status == 1).Count();

            Assert.IsType<int>(count);
            Assert.InRange(count, 500, 10_000);
        }
    }
}