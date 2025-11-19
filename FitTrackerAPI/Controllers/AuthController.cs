using FitTrackerAPI.DTOs.Authentication;
using FitTrackerAPI.DTOs.Tokens;
using FitTrackerAPI.Repositories.Users;
using FitTrackerAPI.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FitTrackerAPI.Controllers;

[ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository; // Necesario para buscar usuarios en login

        // Se inyectan los servicios requeridos
        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        // 1. POST api/auth/register (Requisito 3)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Validar si el usuario ya existe
            var existingUser = await _userRepository.GetUserByUsernameAsync(dto.Email.ToLower());
            if (existingUser != null)
            {
                return Conflict(new { Message = "User already exists with this email." });
            }
            
            // 1. Crear User y UserProfile
            var user = await _authService.RegisterAsync(dto);
            
            // 2. Generar tokens iniciales (Access y Refresh)
            var tokenDto = await _authService.GenerateTokensAsync(user);
            
            // 3. Enviar el código de verificación (Requisito 4)
            await _authService.SendVerificationCodeAsync(user); 

            // Devuelve el token inicial (aunque IsVerified será false)
            return Ok(tokenDto);
        }

        // 2. POST api/auth/login (Requisito 1)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(dto.Email.ToLower());

            if (user == null || !_authService.VerifyPassword(user.PasswordHash, dto.Password))
            {
                // Mensaje genérico por seguridad
                return Unauthorized(new { Message = "Invalid credentials." });
            }
            
            // Generar y devolver Access Token (corto) y Refresh Token (largo)
            var tokenDto = await _authService.GenerateTokensAsync(user);
            ; 
            
            // eviar codigo si el usuario no esta verificado
            if (!user.IsVerified) 
                await _authService.SendVerificationCodeAsync(user);
            
            // La respuesta incluye el RefreshToken para persistencia de sesión offline/larga duración
            return Ok(tokenDto); 
        }

        // 3. POST api/auth/send-verification-code (Requisito 4)
        // Usado por el frontend cuando detecta que IsVerified = false
        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] string email)
        {
            var user = await _userRepository.GetUserByUsernameAsync(email.ToLower());
            if (user == null)
            {
                // Responder genéricamente para no revelar si el email existe
                return Ok(new { Message = "Verification code requested." });
            }
            
            if (user.IsVerified)
            {
                return BadRequest(new { Message = "Account is already verified." });
            }
            
            await _authService.SendVerificationCodeAsync(user);
            return Ok(new { Message = "Verification code sent to email." });
        }

        // 4. POST api/auth/verify-code (Requisito 4)
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto dto)
        {
            var success = await _authService.VerifyCodeAsync(dto);
            
            if (!success)
            {
                return BadRequest(new { Message = "Invalid or expired code." });
            }
            
            var user = await _userRepository.GetUserByUsernameAsync(dto.Email.ToLower());
            
            // Tras la verificación exitosa, generamos un nuevo set de tokens para actualizar 
            // el claim 'IsVerified' en el AccessToken (aunque el token anterior aún no haya expirado)
            var tokenDto = await _authService.GenerateTokensAsync(user!);

            return Ok(new { Message = "Account successfully verified.", Tokens = tokenDto });
        }
        
        // 5. POST api/auth/refresh (Requisito 5 - Lógica para la persistencia de sesión)
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                // Llama al servicio para validar el token de refresco antiguo y emitir uno nuevo
                var tokenDto = await _authService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(tokenDto);
            }
            catch (SecurityTokenException ex)
            {
                // Error de seguridad: Token inválido, expirado, o revocado.
                // Esto fuerza al cliente Flutter a eliminar sus tokens y hacer un re-login completo.
                return Unauthorized(new { Message = "Session expired or invalid token. Please log in again." });
            }
        }
    }