using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using auction.Entity;

namespace auction.Entity
{
    public class ProductBid
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get;set;}
        public decimal BidAmount {get;set;}
        public UserDetail UserDetail { get; set; }
        public Product Product{get;set;}

    }
}
