﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTestApi.Data;
using WebTestApi.Models;

namespace WebTestApi.Controllers {
    [ApiController]
    [Route("v1/products")]

    public class ProductController : ControllerBase {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context) {
            var products = await context.Products
                .Include(x => x.Category)
                .ToListAsync();

            return products;

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Product>> GetById([FromServices] DataContext context, int id) {
            var product = await context.Products
                .Include(x => x.Category)
                .AsNoTracking() //não "hidratação", não cria proxies do obj (evita criar versões "inuteis")
                .FirstOrDefaultAsync(x => x.Id == id);

            return product;

        }

        [HttpGet]
        [Route("categories/{id:int}")]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id) {
            var products = await context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync();

            return products;

        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post([FromServices] DataContext context, [FromBody] Product model) { //[FromServices] serve para inicialização do repo
            if (ModelState.IsValid) {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return model;

            } else {
                return BadRequest(ModelState);

            }

        }

    }
}
