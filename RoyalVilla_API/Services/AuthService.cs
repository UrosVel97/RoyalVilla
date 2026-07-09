using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RoyalVIlla.DTO;

namespace RoyalVilla_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(ApplicationDbContext db, IConfiguration configuration, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _mapper = mapper;
        }   

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower());

                if (user == null || user.Password != loginRequestDTO.Password)
                {
                    return null;
                }

                var token = GenerateJwtToken(user);


                return new LoginResponseDTO
                {
                    UserDTO = _mapper.Map<UserDTO>(user),
                    Token = token
                };


            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while logging in the user: {ex.Message}");
            }
        }

        public async Task<UserDTO> RegisterAsync(RegistrationRequestDTO requestDTO)
        {
            try
            {

                if (await IsEmailExistsAsync(requestDTO.Email))
                {
                    throw new InvalidOperationException($"User with email {requestDTO.Email} already exists.");
                }

                User user = new()
                {
                    Email = requestDTO.Email,
                    Name = requestDTO.Name,
                    Password = requestDTO.Password,
                    Role = string.IsNullOrEmpty(requestDTO.Role) ? "Customer" : requestDTO.Role,
                    CreatedDate = DateTime.Now,
                };

                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while registering the user: {ex.Message}");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
