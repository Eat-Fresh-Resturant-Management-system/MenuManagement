using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Collections.Generic;
using WAOProjectMenu.Models;

namespace MenuManagement.Data
{
    public class MenuDbContext : DbContext
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options)
        { }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuCategory> MenuCategorys { get; set; }

        public DbSet<TableData> TableDatas { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MenuItem>()
            .HasOne(w => w.MenuCategory)
            .WithMany(wp => wp.MenuItems)
            .HasForeignKey(w => w.MenuCategoryName)
            .OnDelete(DeleteBehavior.Restrict); // Specify OnDelete behavior
        }
    }
}
