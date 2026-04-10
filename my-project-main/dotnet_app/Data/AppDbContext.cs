using dotnet_app.Data;
using dotnet_app.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace dotnet_app.Data;
public class AppDbContext : DbContext
{
     public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
}