using Microsoft.EntityFrameworkCore;

namespace cancellation_token_demo;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Define your DbSets (tables) here
    public DbSet<Product> Products { get; set; }
}

// Define your model (entity) class
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
