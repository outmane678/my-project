using Microsoft.EntityFrameworkCore;
using user_account_service.Data;
using user_account_service.Models.Entities;
using user_account_service.Config;
using user_account_service.Client;
using user_account_service.DTOs.Requests;

namespace user_account_service.Services.Implementations;

    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly JwtTokenService _jwtService;
        private readonly IEmployeeApi _employeeApi; // Changé de EmployeeClient à IEmployeeApi

        public AuthService(
            AppDbContext context,
            PasswordService passwordService,
            JwtTokenService jwtService,
            IEmployeeApi employeeApi) // Changé ici aussi
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _employeeApi = employeeApi; // Changé ici aussi
        }

        // =========================
        // SIGNUP (avec token employé)
        // =========================
        public async Task Signup(SignupRequestDto request, string token)
        {
            var employee = await _employeeApi.GetEmployeeByToken(token); // Changé
            if (employee == null)
                throw new Exception("Token invalide ou expiré");

            if (await _context.UserAccounts.AnyAsync(u => u.EmployeeId == employee.Id))
                throw new Exception("Compte déjà existant pour cet employé");

            if (await _context.UserAccounts.AnyAsync(u => u.Username == request.Username))
                throw new Exception("Username déjà utilisé");

            var user = new UserAccount
            {
                Username = request.Username,
                Password = _passwordService.HashPassword(request.Password),
                EmployeeId = employee.Id,
                Role = "USER"
            };
            await _employeeApi.VerifyEmployee(employee.Id); // Changé
            _context.UserAccounts.Add(user);
            await _context.SaveChangesAsync();

           
        }

        // =========================
        // LOGIN + JWT
        // =========================
        public async Task<string> Login(LoginRequestDto request)
        {
            var user = await _context.UserAccounts
                .SingleOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
                throw new Exception("Utilisateur non trouvé");

            var isValid = _passwordService.VerifyPassword(
                user.Password,
                request.Password
            );

            if (!isValid)
                throw new Exception("Identifiants invalides");

            return _jwtService.GenerateToken(user.Username, user.Role);
        }

        // =========================
        // GET EMPLOYEE ID FROM TOKEN
        // =========================
        public Guid GetEmployeeIdFromToken(string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                throw new Exception("Token manquant");

            var token = authHeader.Substring(7);
            var principal = _jwtService.ValidateToken(token);

            var username = principal.Identity?.Name;
            if (username == null)
                throw new Exception("Token invalide");

            var user = _context.UserAccounts
        .SingleOrDefault(u => u.Username == username);
            if (user == null)
                throw new Exception("Utilisateur non trouvé");

            return user.EmployeeId;
        }
    }

