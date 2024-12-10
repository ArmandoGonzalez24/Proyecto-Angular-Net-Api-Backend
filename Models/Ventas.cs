using System;
using Microsoft.AspNetCore.Mvc;
using ProductCategoryCrud.Models;
using ProductCategoryCrud.Data;

namespace ProductCategoryCrud.Models
{
    //sales
    public class Ventas {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Comprador { get; set; }
        public string Vendedor { get; set; }
    }
}