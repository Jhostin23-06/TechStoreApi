using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;

namespace TechStoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleRepository _repository;

        public SalesController(ISaleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            var sale = await _repository.GetByIdAsync(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult<Sale>> PostSale(Sale sale)
        {            
            decimal totalCalculado = 0;
            foreach (var detalle in sale.Detalles)
            {
                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                totalCalculado += detalle.Subtotal;
            }
            sale.Total = totalCalculado;
            sale.Fecha = DateTime.Now; // Fecha del servidor

            await _repository.AddAsync(sale);

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }
    }
}