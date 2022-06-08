using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;


namespace auction.Entity
{
    public class ProductDbContext : IProductDbContext
    {
        private readonly IMongoDatabase _database = null;
        public ProductDbContext()
        {
            string connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            string database = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");
            if (connectionString == null || database == null)
            {
                connectionString = "mongodb://localhost:27017";
                database = "AuctionDB";
            }

            var client = new MongoClient(connectionString);
            if (client != null)
                _database = client.GetDatabase(database);
        }


        public IMongoCollection<Product> Product
        {
            get
            {
                return _database.GetCollection<Product>("Product");
            }
        }

        public IMongoCollection<UserDetail> UserDetail
        {
            get
            {
                return _database.GetCollection<UserDetail>("UserDetail");
            }
        }

        public IMongoCollection<ProductBid> ProductBid
        {
            get
            {
                return _database.GetCollection<ProductBid>("ProductBid");
            }
        }
    }
}
