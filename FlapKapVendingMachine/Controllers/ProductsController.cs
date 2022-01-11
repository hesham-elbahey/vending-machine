using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlapKapVendingMachine.Domain.Models;
using FlapKapVendingMachine.Persistence.Contexts;
using FlapKapVendingMachine.DTOs;
using Microsoft.AspNetCore.Authorization;
using FlapKapVendingMachine.Constants;
using FlapKapVendingMachine.Attributes;
using FlapKapVendingMachine.Extensions;

namespace FlapKapVendingMachine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>A list of product objects</returns>
        // GET: api/Products
        [HttpGet]
        public ActionResult<IEnumerable<ProductDTO>> GetProducts()
        {
            return _context.Stocks.GetProducts();
        }

        /// <summary>
        /// Get all products of a specific seller
        /// </summary>
        /// <param name="id">The seller ID</param>
        /// <returns>A list of product objects</returns>
        // GET: api/sellers/{id}/Products
        [HttpGet("~/api/sellers/{id}/products")]
        [Authorize(Policy = PolicyNames.SameSeller)]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsBySeller(string id)
        {
            return _context.Stocks.GetProducts(id);
        }

        /// <summary>
        /// Get a specific product
        /// </summary>
        /// <param name="id">The product ID</param>
        /// <returns>A specific product</returns>
        // GET: api/Products/5
        [HttpGet("{id}")]
        public ActionResult<ProductDTO> GetProduct(int id)
        {
            var productDTO = _context.Stocks.GetProduct(id);

            if (productDTO == null)
            {
                return NotFound();
            }

            return productDTO;
        }

        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <param name="productDTO">A DTO containing the product object</param>
        /// <param name="id">The seller ID</param>
        /// <returns>No content</returns>
        // PUT: api/sellers/{id}/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("~/api/sellers/{id}/products/{productId}")]
        [Authorize(Policy = PolicyNames.SameSeller)]
        [ModelValidation]
        #pragma warning disable IDE0060 // Remove unused parameter
        public async Task<IActionResult> PutProduct(int productId, ProductDTO productDTO, string id)
        #pragma warning restore IDE0060 // Remove unused parameter
        {
            if (productId != productDTO.Id)
            {
                return BadRequest();
            }

            Product product = new()
            {
                ProductName = productDTO.ProductName,
                Cost = productDTO.Cost,
                Id = productDTO.Id,
                SellerId = productDTO.SellerId,
            };
            _context.Entry(product).State = EntityState.Modified;
            var stock = _context.Stocks.FirstOrDefault(s => s.ProductId == productId);
            ProductDTO outputProduct = _context.Stocks.GetProduct(stock.ProductId);
            stock.AmountAvailable = productDTO.AmountAvailable + stock.AmountAvailable - outputProduct.AmountAvailable;
            _context.Entry(stock).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(productId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="productDTO">A DTO containing the product object</param>
        /// <returns>The newly created product</returns>
        // POST: api/sellers/{id}/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("~/api/sellers/{id}/products")]
        [Authorize(Policy = PolicyNames.SameSeller)]
        [ModelValidation]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductDTO productDTO)
        {
            var machine = _context.VendingMachines.FirstOrDefault();
            if (machine == null)
                return Forbid();
            Product product = new()
            {
                ProductName = productDTO.ProductName,
                Cost = productDTO.Cost,
                SellerId = productDTO.SellerId,
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            Stock stock = new()
            {
                AmountAvailable = productDTO.AmountAvailable,
                ProductId = product.Id,
                VendingMachineId = machine.Id
            };
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            productDTO.Id = product.Id;
            return CreatedAtAction("GetProduct", new { id = product.Id }, productDTO);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <returns>No content</returns>
        // DELETE: api/sellers/{id}/Products/5
        [HttpDelete("~/api/sellers/{id}/products/{productId}")]
        [Authorize(Policy = PolicyNames.SameSeller)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return UnprocessableEntity(new { Message = "Orders are attached to this product so it cannot be deleted." });
            }

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
