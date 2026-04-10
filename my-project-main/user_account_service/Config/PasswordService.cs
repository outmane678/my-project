    using Microsoft.AspNetCore.Identity;
    using user_account_service.Models.Entities;

    namespace user_account_service.Config;
    
        public class PasswordService
        {
            private readonly PasswordHasher<UserAccount> _passwordHasher;

            public PasswordService()
            {
                _passwordHasher = new PasswordHasher<UserAccount>();
            }

            // Equivalent de passwordEncoder.encode()
            public string HashPassword(string plainPassword)
            {
                return _passwordHasher.HashPassword(null!, plainPassword);
            }

            // Equivalent de passwordEncoder.matches()
            public bool VerifyPassword(string hashedPassword, string plainPassword)
            {
                var result = _passwordHasher.VerifyHashedPassword(
                    null!,
                    hashedPassword,
                    plainPassword
                );

                return result == PasswordVerificationResult.Success;
            }
        }
