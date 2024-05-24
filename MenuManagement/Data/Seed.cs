namespace MenuManagement.Data;

public class SeedData
{
    public static void Seed(MenuDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();
    }
}