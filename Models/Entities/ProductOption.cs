﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class ProductOption
    {
        [Key]
        public int ProductOptionId { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        [Required]
        public string Value { get; set; } = string.Empty;


        public int? OptionTypeId { get; set; }
        [ForeignKey("OptionTypeId")]
        public ProductOptionType? ProductOptionType { get; set; }

        public List<ProductConfiguration>? ProductConfigurations { get; set; }
    }
}
