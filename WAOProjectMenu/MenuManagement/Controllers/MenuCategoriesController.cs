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
using System.Text;
//using RabbitMQ.Client;
//using System.Text.Json;


namespace MenuManagement.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuCategoriesController : ControllerBase
    {
        private readonly MenuDbContext _context;

        public MenuCategoriesController(MenuDbContext context)
        {
            _context = context;
        }

        // GET: api/MenuCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuCategory>>> GetMenuCategorys()
        {
            return await _context.MenuCategorys.ToListAsync();
        }

        // GET: api/MenuCategories/{MenuCategoryName}
        [HttpGet("{MenuCategoryName}")]
        public async Task<ActionResult<MenuCategory>> GetMenuCategorybyName(string MenuCategoryName)
        {
            var menuCategory = await _context.MenuCategorys.FirstOrDefaultAsync(mc => mc.MenuCategoryName == MenuCategoryName);

            if (menuCategory == null)
            {
                return NotFound();
            }

            return menuCategory;
        }


        // GET: api/MenuCategories/{categoryId}/MenuItems
        [HttpGet("{categoryId}/MenuItems")]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetMenuItemsForCategory(String categoryId)
        {
            var menuCategory = await _context.MenuCategorys.Include(mc => mc.MenuItems).FirstOrDefaultAsync(mc => mc.MenuCategoryName == categoryId);

            if (menuCategory == null)
            {
                return NotFound();
            }

            var menuItemsDto = menuCategory.MenuItems.Adapt<List<MenuItemDTO>>();
            return menuItemsDto;
        }

        // POST: api/MenuCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MenuCategoryDTO>> PostMenuCategory(MenuCategoryDTO menuCategoryDto)
        {
            // Check if a menu category with the same name already exists
            if (MenuCategoryExists(menuCategoryDto.MenuCategoryName))
            {
                return Conflict("A menu category with the same name already exists.");
            }

            var menuCategory = menuCategoryDto.Adapt<MenuCategory>();
            // Set the MenuCategoryName property
            menuCategory.MenuCategoryName = menuCategoryDto.MenuCategoryName;

            _context.MenuCategorys.Add(menuCategory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw; // You might want to handle this exception differently based on your requirements
            }

            var returnedMenuCategoryDto = menuCategory.Adapt<MenuCategoryDTO>();
            //PublishToRabbitMQ(menuCategoryDto);
            return Ok(returnedMenuCategoryDto);
        }



        // DELETE: api/MenuCategories/5
        [HttpDelete("{MenuCategoryName}")]
        public async Task<IActionResult> DeleteMenuCategory(string MenuCategoryName)
        {
            var menuCategory = await _context.MenuCategorys.FindAsync(MenuCategoryName);
            if (menuCategory == null)
            {
                return NotFound();
            }

            _context.MenuCategorys.Remove(menuCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuCategoryExists(string id)
        {
            return _context.MenuCategorys.Any(e => e.MenuCategoryName == id);
        }
    }
}
