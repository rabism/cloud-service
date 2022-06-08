﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;


namespace auction.Entity
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string DetailDescription { get; set; }
        public string Category { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime BidEndDate { get; set; } = DateTime.Now;
        public UserDetail UserDetail { get; set; }


    }
}
