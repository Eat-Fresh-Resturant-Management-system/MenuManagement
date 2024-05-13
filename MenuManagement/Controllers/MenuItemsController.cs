using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenuManagement.Data;
using WAOProjectMenu.Models;
using Microsoft.AspNetCore.Authorization;
using Mapster;

namespace MenuManagement.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly MenuDbContext _context;

        public MenuItemsController(MenuDbContext context)
        {
            _context = context;
        }

        // GET: api/MenuItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItems()
        {
            return await _context.MenuItems.ToListAsync();
        }

        // GET: api/MenuItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItem>> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            return menuItem;
        }

        
        // POST: api/MenuItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MenuItemDTO>> PostMenuItem(MenuItemDTO menuItemDto)
        {
            var menuCategory = await _context.MenuCategorys.FindAsync(menuItemDto.MenuCategoryName);
            if (menuCategory == null)
            {
                return BadRequest("MenuCategory does not exist");
            }

            var menuItem = menuItemDto.Adapt<MenuItem>();
            _context.MenuItems.Add(menuItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MenuItemExists(menuItem.ItemId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var returnedMenuItemDto = menuItem.Adapt<MenuItemDTO>();
            return CreatedAtAction("GetMenuItem", new { id = returnedMenuItemDto.ItemId }, returnedMenuItemDto);
        }

        // DELETE: api/MenuItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.ItemId == id);
        }
    }
}
