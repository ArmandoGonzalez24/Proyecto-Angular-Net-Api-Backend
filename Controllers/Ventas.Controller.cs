using Microsoft.AspNetCore.Mvc;
using ProductCategoryCrud.Data;
using ProductCategoryCrud.Models;
using System.Linq;

namespace ProductCategoryCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult RegistrarVenta([FromBody] VentaRequest request)
        {
            if (request == null || request.Venta == null || !request.Items.Any())
                return BadRequest("Datos inválidos.");

            var venta = request.Venta;
            _context.Ventas.Add(venta);
            _context.SaveChanges();

            foreach (var item in request.Items)
            {
                item.VentaId = venta.Id;
                _context.VentasItems.Add(item);
            }
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObtenerVentaPorId), new { id = venta.Id }, new { venta, items = request.Items });
        }

        [HttpGet]
        public IActionResult ObtenerVentas()
        {
            var ventas = _context.Ventas.ToList();
            return Ok(ventas);
        }

        [HttpGet("{id:int}")]
        public IActionResult ObtenerVentaPorId(int id)
        {
            var venta = _context.Ventas.Find(id);
            if (venta == null) return NotFound();

            var items = _context.VentasItems.Where(i => i.VentaId == id).ToList();
            return Ok(new { venta, items });
        }
    }

    public class VentaRequest
    {
        public Ventas Venta { get; set; }
        public List<VentasItem> Items { get; set; }
    }
}
