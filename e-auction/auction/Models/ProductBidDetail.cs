using System.ComponentModel.DataAnnotations;
using System;
namespace auction.Models
{

    public class ProductBidDetail
    {

        [Required(ErrorMessage = "ProductId is required")]
        public string ProductId { get; set; }
        [Required(ErrorMessage = "BidAmount is required")]
        public decimal BidAmount { get; set; }
        [Required(ErrorMessage = "BuyerEmail is required")]
        public string BuyerEmail { get; set; }

    }

}
