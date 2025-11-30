using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using project1.Application.DTOs.Auth;
using project1.Application.Interfaces;
using project1.Domain.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace project1.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResponse> AuthenticateAsync(LoginRequest request, string ipAddress)
        {
            var users = await _unitOfWork.Repository<User>().FindAsync(u => u.Email == request.Email);
            var user = users.Count > 0 ? users[0] : null;

            if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);

            // Save refresh token
            refreshToken.UserId = user.Id;
            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync();

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string token, string ipAddress)
        {
            var tokens = await _unitOfWork.Repository<RefreshToken>().FindAsync(t => t.Token == token);
            var refreshToken = tokens.Count > 0 ? tokens[0] : null;

            if (refreshToken == null || !refreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            var user = await _unitOfWork.Repository<User>().GetByIdAsync(refreshToken.UserId);
            if (user == null) throw new UnauthorizedAccessException("User not found");

            // Rotate token
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            newRefreshToken.UserId = user.Id;
            
            // Revoke old
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            
            _unitOfWork.Repository<RefreshToken>().Update(refreshToken);
            await _unitOfWork.Repository<RefreshToken>().AddAsync(newRefreshToken);
            await _unitOfWork.CompleteAsync();

            var jwtToken = GenerateJwtToken(user);

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = jwtToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task RevokeRefreshTokenAsync(string token, string ipAddress)
        {
            var tokens = await _unitOfWork.Repository<RefreshToken>().FindAsync(t => t.Token == token);
            var refreshToken = tokens.Count > 0 ? tokens[0] : null;

            if (refreshToken == null || !refreshToken.IsActive) return;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            
            _unitOfWork.Repository<RefreshToken>().Update(refreshToken);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RevokeAllForUserAsync(Guid userId)
        {
            var tokens = await _unitOfWork.Repository<RefreshToken>().FindAsync(t => t.UserId == userId && t.Revoked == null);
            foreach (var t in tokens)
            {
                t.Revoked = DateTime.UtcNow;
                t.RevokedByIp = "Admin/System";
                _unitOfWork.Repository<RefreshToken>().Update(t);
            }
            await _unitOfWork.CompleteAsync();
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string ipAddress, bool isAdminRequest)
        {
            if (await _unitOfWork.Repository<User>().ExistsAsync(u => u.Email == request.Email))
            {
                throw new InvalidOperationException("Email already registered");
            }

            // Role validation
            if (!Enum.TryParse<project1.Domain.Enums.UserRole>(request.Role, true, out var role))
            {
                throw new InvalidOperationException("Invalid role");
            }

            // Security check: Only Admin can create Admin/Teacher
            if (!isAdminRequest && (role == project1.Domain.Enums.UserRole.Admin || role == project1.Domain.Enums.UserRole.Teacher))
            {
                throw new UnauthorizedAccessException("You cannot register as Admin or Teacher");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Role = role,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Auto-login after register
            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.UserId = user.Id;

            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync();

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        // Helpers
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("Key")!);
            var issuer = jwtSection.GetValue<string>("Issuer");
            var audience = jwtSection.GetValue<string>("Audience");
            var expMinutes = jwtSection.GetValue<int>("AccessTokenExpirationMinutes", 15);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("name", user.Name)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var expDays = jwtSection.GetValue<int>("RefreshTokenExpirationDays", 7);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(expDays),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}
