using iroxitBlazorApp.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iroxitBlazorApp.Shared;

namespace iroxitBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public VentaController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Venta>>> GetVentas()
        {
            var ventas = await _context.Ventas
                                     .Include(v => v.Producto) // Include Producto here
                                     .ToListAsync();
            return Ok(ventas);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Venta>> GetSingleVenta(int id)
        {
            var venta = await _context.Ventas
                                      .Include(v => v.Producto) // Include Producto here
                                      .FirstOrDefaultAsync(v => v.IDVentas == id);
            if (venta == null)
            {
                return NotFound();
            }
            return Ok(venta);
        }

        [HttpPost]
        public async Task<ActionResult<Venta>> CreateVenta(Venta objeto)
        {

            _context.Ventas.Add(objeto);
            await _context.SaveChangesAsync();
            return Ok(await GetDbVenta());
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<List<Venta>>> DeleteVenta(int id)
        {
            var DbObjeto = await _context.Ventas.FirstOrDefaultAsync(Ob => Ob.IDVentas == id);
            if (DbObjeto == null)
            {
                return NotFound("no existe :/");
            }

            _context.Ventas.Remove(DbObjeto);
            await _context.SaveChangesAsync();

            return Ok(await GetDbVenta());
        }


        private async Task<List<Venta>> GetDbVenta()
        {
            return await _context.Ventas.ToListAsync();
        }

        [HttpGet]
        [Route("GetVentaPerProduct/{productoId}")]
        public async Task<ActionResult<List<Venta>>> GetVentaPerProduct(int productoId)
        {
            // Aqui Checamos que el Id del Producto sea Valido
            if (productoId <= 0)
            {
                return BadRequest("Invalid product ID. Please provide a positive integer.");
            }

            // Aqui hacemos el fetch para poder linkear el id del producto dentro de la venta
            var ventas = await _context.Ventas
                .Include(v => v.Producto)
                .Where(v => v.IDProductos == productoId)
                .ToListAsync();

            // Aqui nos alerta en caso de que no se haya encontrado alguna venta
            if (ventas.Count == 0)
            {
                return NotFound("No sales found for the specified product.");
            }
            return Ok(ventas);
        }

        [HttpGet]
        [Route("GetProductoMasVendido")]
        public async Task<ActionResult<List<Producto>>> GetProductoMasVendido(int numProducts = 1)
        {
            try
            {
                // Validate input
                if (numProducts <= 0)
                {
                    return BadRequest("Invalid number of products. Please provide a positive integer.");
                }

                // Calculate销量 for each product using a subquery
                var topSellingProducts = await (from v in _context.Ventas
                                                group v by v.IDProductos into g
                                                select new
                                                {
                                                    ProductoID = g.Key,
                                                    TotalVentas = g.Sum(v => v.CantidadVendida)
                                                } into t
                                                orderby t.TotalVentas descending
                                                select t.ProductoID).Take(numProducts).ToListAsync();

                // Fetch product details for the top selling products
                var productos = await _context.Productos
                    .Where(p => topSellingProducts.Contains(p.IDProductos))
                    .ToListAsync();

                // Handle no products found gracefully
                if (productos.Count == 0)
                {
                    return NotFound("No top selling products found.");
                }

                return Ok(productos);
            }
            catch (Exception ex)
            {
                // Handle potential exceptions gracefully
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving top selling products.", error = ex.Message });
            }
        }

    }
}
