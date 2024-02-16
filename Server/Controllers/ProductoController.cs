using iroxitBlazorApp.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iroxitBlazorApp.Shared;

namespace iroxitBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ProductoController(ApplicationDbContext context)
        {

            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Producto>>> GetProducto()
        {
            var lista = await _context.Productos.ToListAsync();
            return Ok(lista); 
        }

        [HttpGet]
        [Route("ProductosExistentesTitulos")]
        public async Task<ActionResult<List<string>>> GetProductoTitulos()
        {
            var titulos = await _context.Productos
                .Select(producto => producto.Titulo)
                .ToListAsync();

            return Ok(titulos);
        }



        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<List<Producto>>> GetSingleProducto(int id)
        {
            var miobjeto = await _context.Productos.FirstOrDefaultAsync(ob => ob.IDProductos == id);
            if (miobjeto == null)
            {
                return NotFound(" :/");
            }

            return Ok(miobjeto);
        }
        [HttpPost]



        private async Task<List<Producto>> GetDbProducto()
        {
            return await _context.Productos.ToListAsync();
        }


        [HttpGet]
        [Route("GetProductoStockBajo")]
        public async Task<ActionResult<List<Producto>>> GetProductoStockBajo()
        {
            try
            {
                // Efficiently filter products with less than 100 units using Where()
                var lowStockProducts = await _context.Productos
                    .Where(p => p.Existencias < 100)
                    .ToListAsync();

                // Handle no products found gracefully
                if (lowStockProducts.Count == 0)
                {
                    return NotFound("No products with low stock found.");
                }

                return Ok(lowStockProducts);
            }
            catch (Exception ex)
            {
                // Handle potential exceptions gracefully
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving low stock products.", error = ex.Message });
            }
        }


    }
}
