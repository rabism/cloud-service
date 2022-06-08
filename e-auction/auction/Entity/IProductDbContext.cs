using MongoDB.Driver;

namespace auction.Entity
{
    public interface IProductDbContext
    {
        IMongoCollection<Product> Product { get; }

        IMongoCollection<UserDetail> UserDetail {get;}

        IMongoCollection<ProductBid> ProductBid {get;}
    }
}