using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenuManagement.Data;
using WAOProjectMenu.Models;

namespace OrderProcessing.Controllers
{
    [Route("grp-13/webmenuapi/api/[controller]")]
    [ApiController]
    public class TableDatasController : ControllerBase
    {
        private readonly MenuDbContext _context;

        public TableDatasController(MenuDbContext context)
        {
            _context = context;
        }

        // GET: api/TableDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableData>>> GetTableDatas()
        {
            return await _context.TableDatas.ToListAsync();
        }

        // GET: api/TableDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TableData>> GetTableData(int id)
        {
            var tableData = await _context.TableDatas.FindAsync(id);

            if (tableData == null)
            {
                return NotFound();
            }

            return tableData;
        }

        // PUT: api/TableDatas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTableData(int id, TableData tableData)
        {
            if (id != tableData.Id)
            {
                return BadRequest();
            }

            _context.Entry(tableData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TableDataExists(id))
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

        // POST: api/TableDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TableData>> PostTableData(TableData tableData)
        {
            _context.TableDatas.Add(tableData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTableData", new { id = tableData.Id }, tableData);
        }

        // DELETE: api/TableDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTableData(int id)
        {
            var tableData = await _context.TableDatas.FindAsync(id);
            if (tableData == null)
            {
                return NotFound();
            }

            _context.TableDatas.Remove(tableData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TableDataExists(int id)
        {
            return _context.TableDatas.Any(e => e.Id == id);
        }
    }
}
