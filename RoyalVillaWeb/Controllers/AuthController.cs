using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RoyalVIlla.DTO;
using RoyalVillaWeb.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RoyalVillaWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _authService.LoginAsync<ApiResponse<LoginResponseDTO>>(loginRequestDTO);

                if (response != null && response.Success && response.Data != null)
                {
                    LoginResponseDTO loginResponse = response.Data;

                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(loginResponse.Token);

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(c => c.Type == "email").Value));
                    identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(c => c.Type == "role").Value));

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "Invalid login attempt.";
                }
            }
            return View(loginRequestDTO);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registerRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _authService.RegisterAsync<object>(registerRequestDTO);

                if (response != null)
                {
                    TempData["success"] = "Registration successful!";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["error"] = "Registration failed.";
                }
            }
            return View(registerRequestDTO);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
