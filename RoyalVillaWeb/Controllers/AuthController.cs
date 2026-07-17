using Microsoft.AspNetCore.Mvc;
using RoyalVIlla.DTO;
using RoyalVillaWeb.Services.IServices;

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
                var response = await _authService.LoginAsync<LoginResponseDTO>(loginRequestDTO);

                if (response != null)
                {
                    // Handle successful login, e.g., store token in session or cookie
                    TempData["success"] = "Login successful!";
                    return RedirectToAction("Index", "Villa");
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
