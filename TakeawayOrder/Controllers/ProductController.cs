﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TakeawayOrder.Data;
using TakeawayOrder.Models;

namespace TakeawayOrder.Controllers
{
    public class ProductController : Controller
    {
        private DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Staff,Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return Redirect("/");
            }
            return Redirect("/");
        }
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Edit(long id)
        {
            Product product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return Redirect("/");
            }
            return Redirect("/");
        }
        public async Task<IActionResult> Delete(long id)
        {
            Product product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return Redirect("/");

        }
    }
}
