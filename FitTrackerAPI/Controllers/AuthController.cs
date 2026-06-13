using FitTrackerAPI.DTOs.Authentication;
using FitTrackerAPI.DTOs.Tokens;
using FitTrackerAPI.Repositories.Users;
using FitTrackerAPI.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

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
            catch (SecurityTokenException)
            {
                // Error de seguridad: Token inválido, expirado, o revocado.
                // Esto fuerza al cliente Flutter a eliminar sus tokens y hacer un re-login completo.
                return Unauthorized(new { Message = "Session expired or invalid token. Please log in again." });
            }
        }

        // 6. GET api/auth/reset-password?token=xxx  — formulario web (fallback para Flutter / testing)
        [HttpGet("reset-password")]
        public IActionResult ResetPasswordPage([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token requerido.");

            // El token se lee desde window.location.search en JS para evitar conflictos
            // entre las llaves de CSS/JS y la interpolación de C#.
            const string html = """
                <!DOCTYPE html>
                <html lang="es">
                <head>
                    <meta charset="UTF-8" />
                    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                    <title>Restablecer contraseña — FitTracker</title>
                    <style>
                        * { box-sizing: border-box; margin: 0; padding: 0; }
                        body {
                            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
                            background: #f0f2f5;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            min-height: 100vh;
                            padding: 1rem;
                        }
                        .card {
                            background: #fff;
                            border-radius: 12px;
                            padding: 2.5rem;
                            width: 100%;
                            max-width: 420px;
                            box-shadow: 0 4px 24px rgba(0,0,0,.1);
                        }
                        h1 { font-size: 1.5rem; margin-bottom: .5rem; color: #1a1a2e; }
                        p.subtitle { color: #666; font-size: .9rem; margin-bottom: 1.5rem; }
                        label { display: block; font-size: .85rem; font-weight: 600; color: #333; margin-bottom: .4rem; }
                        input[type=password] {
                            width: 100%;
                            padding: .75rem 1rem;
                            border: 1.5px solid #ddd;
                            border-radius: 8px;
                            font-size: 1rem;
                            margin-bottom: 1.2rem;
                            transition: border-color .2s;
                        }
                        input[type=password]:focus { outline: none; border-color: #4f46e5; }
                        button {
                            width: 100%;
                            padding: .85rem;
                            background: #4f46e5;
                            color: #fff;
                            border: none;
                            border-radius: 8px;
                            font-size: 1rem;
                            font-weight: 600;
                            cursor: pointer;
                            transition: background .2s;
                        }
                        button:hover { background: #4338ca; }
                        button:disabled { background: #a5b4fc; cursor: not-allowed; }
                        .message {
                            margin-top: 1.2rem;
                            padding: .85rem 1rem;
                            border-radius: 8px;
                            font-size: .9rem;
                            display: none;
                        }
                        .message.success { background: #d1fae5; color: #065f46; }
                        .message.error   { background: #fee2e2; color: #991b1b; }
                    </style>
                </head>
                <body>
                    <div class="card">
                        <h1>Restablecer contraseña</h1>
                        <p class="subtitle">Ingresa tu nueva contraseña para FitTracker.</p>
                        <form id="form">
                            <label for="pwd">Nueva contraseña</label>
                            <input type="password" id="pwd" placeholder="Mínimo 6 caracteres" required minlength="6" />
                            <label for="pwd2">Confirmar contraseña</label>
                            <input type="password" id="pwd2" placeholder="Repite la contraseña" required minlength="6" />
                            <button type="submit" id="btn">Restablecer contraseña</button>
                        </form>
                        <div class="message" id="msg"></div>
                    </div>
                    <script>
                        const token = new URLSearchParams(window.location.search).get('token');

                        document.getElementById('form').addEventListener('submit', async (e) => {
                            e.preventDefault();
                            const pwd  = document.getElementById('pwd').value;
                            const pwd2 = document.getElementById('pwd2').value;
                            const btn  = document.getElementById('btn');

                            if (pwd !== pwd2) {
                                showMessage('Las contraseñas no coinciden.', false);
                                return;
                            }

                            btn.disabled = true;
                            btn.textContent = 'Procesando...';

                            try {
                                const res = await fetch('/api/auth/reset-password', {
                                    method: 'POST',
                                    headers: { 'Content-Type': 'application/json' },
                                    body: JSON.stringify({ token, newPassword: pwd })
                                });
                                const data = await res.json();
                                if (res.ok) {
                                    showMessage('Contraseña restablecida. Ya puedes iniciar sesión en la app.', true);
                                    document.getElementById('form').style.display = 'none';
                                } else {
                                    showMessage(data.message || 'El enlace no es válido o ya expiró.', false);
                                    btn.disabled = false;
                                    btn.textContent = 'Restablecer contraseña';
                                }
                            } catch {
                                showMessage('Error de conexión. Intenta de nuevo.', false);
                                btn.disabled = false;
                                btn.textContent = 'Restablecer contraseña';
                            }
                        });

                        function showMessage(text, success) {
                            const msg = document.getElementById('msg');
                            msg.textContent = text;
                            msg.className = 'message ' + (success ? 'success' : 'error');
                            msg.style.display = 'block';
                        }

                        function openInApp() {
                            const token = new URLSearchParams(window.location.search).get('token');
                            window.location.href = 'fittracker://reset-password?token=' + encodeURIComponent(token);
                        }
                    </script>
                    <div style="text-align:center; margin-top:1.5rem; padding-top:1rem; border-top:1px solid #eee;">
                        <p style="color:#888; font-size:.8rem; margin-bottom:.6rem;">¿Tienes la app instalada?</p>
                        <button onclick="openInApp()" style="background:transparent; border:1.5px solid #4f46e5; color:#4f46e5; padding:.6rem 1.2rem; border-radius:8px; font-size:.9rem; cursor:pointer;">
                            Abrir en FitTracker App
                        </button>
                    </div>
                </body>
                </html>
                """;

            return Content(html, "text/html");
        }

        // 7. POST api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.SendPasswordResetLinkAsync(dto.Email);
            // Siempre responde OK para no revelar si el email existe
            return Ok(new { Message = "If that email is registered, a password reset link has been sent." });
        }

        // 7. POST api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var success = await _authService.ResetPasswordAsync(dto);
            if (!success)
            {
                return BadRequest(new { Message = "Invalid or expired reset token." });
            }

            return Ok(new { Message = "Password successfully reset. You can now log in with your new password." });
        }
    }