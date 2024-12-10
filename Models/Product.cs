using Microsoft.AspNetCore.Mvc;
using ProductCategoryCrud.Models;
using ProductCategoryCrud.Data;

namespace ProductCategoryCrud.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }        
        public decimal Score { get; set; }

        public string ImageUrl { get; set; }
    }
}