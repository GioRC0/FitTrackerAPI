using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitTrackerAPI.DTOs.Authentication;
using FitTrackerAPI.DTOs.Tokens;
using FitTrackerAPI.Models.Authentication;
using FitTrackerAPI.Models.UserInfo;
using FitTrackerAPI.Repositories.Users;
using FitTrackerAPI.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FitTrackerAPI.Services.Auth;

public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService; // Mock de servicio de email

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _emailService = emailService;
        }

        // --- HASHING ---

        public string HashPassword(string password) => _passwordHasher.HashPassword(null, password);

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
        

        // --- REGISTRO (Requisito 3) ---

        public async Task<User> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                Username = dto.Email.ToLower(),
                PasswordHash = HashPassword(dto.Password),
                Role = "User",
                IsVerified = false, // Por defecto es false
                Profile = new Models.UserInfo.UserProfile
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Weight = dto.Weight,
                    Height = dto.Height
                }
            };

            // Uso del Repositorio para guardar User y UserProfile
            await _userRepository.AddUserAsync(user);

            return user;
        }

        // --- TOKENS (Requisito 1 & 5) ---
        
        public async Task<TokenDto> GenerateTokensAsync(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);
        
            // Access Token
            var accessExpiration = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["AccessTokenExpirationMinutes"]!));
            var accessClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("IsVerified", user.IsVerified.ToString())
            };
        
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(accessClaims),
                Expires = accessExpiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };
        
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
        
            // Refresh Token
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(int.Parse(jwtSettings["RefreshTokenExpirationDays"]!));
            var newRefreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString().Replace("-", ""),
                Expires = refreshTokenExpiration,
                Created = DateTime.UtcNow
            };
        
            // Reemplazar tokens antiguos con el nuevo y actualizar el usuario
            user.RefreshTokens.Clear();
            user.RefreshTokens.Add(newRefreshToken);
            await _userRepository.UpdateUserAsync(user);
        
            return new TokenDto
            {
                AccessToken = tokenHandler.WriteToken(accessToken),
                AccessTokenExpiration = accessExpiration,
                RefreshToken = newRefreshToken.Token,
                IsVerified = user.IsVerified
            };
        }

        public async Task<TokenDto> RefreshTokenAsync(string token)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(token);
            if (user == null)
            {
                throw new SecurityTokenException("Refresh Token inválido o expirado.");
            }

            var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
            if (refreshToken == null || !refreshToken.IsActive)
            {
                throw new SecurityTokenException("Refresh Token inválido o expirado.");
            }

            // Generar y devolver un nuevo par de tokens (esto también actualiza el usuario)
            return await GenerateTokensAsync(user);
        }

        // --- CÓDIGOS DE VERIFICACIÓN (Requisito 4) ---

        public async Task SendVerificationCodeAsync(User user)
        {
            if (user.IsVerified) return;

            // Generar código de 6 dígitos
            var code = new Random().Next(100000, 999999).ToString();
            var expiration = DateTime.UtcNow.AddMinutes(15);

            var validationCode = new ValidationCode
            {
                Code = code,
                ExpiresAt = expiration,
            };

            user.ValidationCodes.Add(validationCode);
            await _userRepository.UpdateUserAsync(user);

            // Llamada al Mock de servicio de email
            await _emailService.SendEmailAsync(user.Username, "Código de Verificación de Cuenta", $"Tu código es: {code}");
        }

        public async Task<bool> VerifyCodeAsync(VerifyCodeDto dto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(dto.Email.ToLower());
            if (user == null) return false;

            // 1. Obtener el código de validación válido desde el Repositorio
            var validCode = user.ValidationCodes
                .FirstOrDefault(vc => vc.Code == dto.Code && vc.ExpiresAt > DateTime.UtcNow && !vc.IsUsed);

            if (validCode == null) return false;
            
            validCode.IsUsed = true;
            user.IsVerified = true;
            await _userRepository.UpdateUserAsync(user);

            return true;
        }
    }