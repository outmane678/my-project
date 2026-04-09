using user_account_service.Data;
using Microsoft.EntityFrameworkCore;
using user_account_service.Models.Entities;
namespace user_account_service.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    public DbSet<UserAccount> UserAccounts {get; set;}
}