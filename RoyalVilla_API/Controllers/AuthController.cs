using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVIlla.DTO;
using RoyalVilla_API.Services;

namespace RoyalVilla_API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<ApiResponse<UserDTO>>> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
    {
        try
        {
            if (registrationRequestDTO == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Registration data is required"));
            }

            if (await _authService.IsEmailExistsAsync(registrationRequestDTO.Email))
            {
                return Conflict(ApiResponse<object>.Conflict($"User with email {registrationRequestDTO.Email} already exists"));
            }

            var user = await _authService.RegisterAsync(registrationRequestDTO);

            if (user == null)
            {
                return BadRequest(ApiResponse<UserDTO>.BadRequest("User registration failed"));
            }

            var response = ApiResponse<UserDTO>.CreatedAt(user, "User registered successfully");

            return CreatedAtAction(nameof(Register), new { id = user.Id }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Error(500,"An error occurred while processing your request", ex.Message));
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO loginRequestDTO)
    {
        try
        {
            if (loginRequestDTO == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Login data is required"));
            }

            var loginResponse = await _authService.LoginAsync(loginRequestDTO);

            if (loginResponse == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Invalid email or password"));
            }

            var response = ApiResponse<LoginResponseDTO>.Ok(loginResponse, "User logged in successfully");

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Error(500, "An error occurred while processing your request", ex.Message));
        }
    }
}
