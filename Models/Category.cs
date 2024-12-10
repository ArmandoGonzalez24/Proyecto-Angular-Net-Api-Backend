using Microsoft.AspNetCore.Mvc;
using ProductCategoryCrud.Models;
using ProductCategoryCrud.Data;

namespace ProductCategoryCrud.Models
{

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }        
    }
}