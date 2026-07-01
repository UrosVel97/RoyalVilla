using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Data.DTOs;
using RoyalVilla_API.Models;

namespace RoyalVilla_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public AuthService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public Task<LoginResponseDTO> LoginAsync(LoginRequestDTO requestDTO)
        {
            throw new NotImplementedException();
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
    }
}
