using MenuManagement.Data;

namespace Server.Data;

public class SeedData
{
    public static void Seed(MenuDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();
    }
}