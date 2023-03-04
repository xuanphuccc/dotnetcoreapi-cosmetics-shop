﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class ShoppingCartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public int Qty { get; set; }


        public int? CartId { get; set; }
        [ForeignKey("CartId")]
        public ShoppingCart? ShoppingCart { get; set; }

        public int? ProductItemId { get; set; }
        [ForeignKey("ProductItemId")]
        public ProductItem? ProductItem { get; set; }
    }
}
