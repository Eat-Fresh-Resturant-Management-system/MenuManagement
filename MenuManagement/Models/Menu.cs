using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WAOProjectMenu.Models;
public class MenuItem
{
    [Key]
    public int  ItemId { get; set; }
    public string ItemName { get; set; }
    public int CategoryId { get; set; }
    public float Price { get; set; }
    public string Description { get; set; }

    public MenuCategory MenuCategory { get; set; } 

    public string MenuCategoryName { get; set; }

}

public class MenuCategory
{
    [Key]
    public string MenuCategoryName { get; set; }
    public List<MenuItem> MenuItems { get; set; }
}

public class MenuItemDTO 
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public string MenuCategoryName { get; set; }
    public float Price { get; set; }
    public string Description { get; set; }

}

public class MenuCategoryDTO
{

    public string MenuCategoryName { get; set; }
}

public class TableData
{
    public int Id { get; set; }
    public string IsAvailable { get; set; }

}