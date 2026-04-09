using user_account_service.Data;
using user_account_service.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnet_app.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Equivalent de loadUserByUsername()
        public async Task<UserAccount?> LoadUserByUsername(string username)
        {
            return await _context.UserAccounts
                .SingleOrDefaultAsync(u => u.Username == username);
        }
    }
}
