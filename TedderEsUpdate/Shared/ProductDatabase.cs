using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TedderEsUpdate.Models;

namespace TedderEsUpdate.Shared
{
    public class ProductDatabase
    {
        public static async Task<List<ProductModel>> GetAllWebProducts()
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

            return productModelList;
        }
    }
}