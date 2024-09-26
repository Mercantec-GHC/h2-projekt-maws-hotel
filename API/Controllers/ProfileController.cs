using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ProfileController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _context.Profiles.AnyAsync(p => p.Email == registerDto.Email))
            {
                return BadRequest("Email already exists");
            }

            var profile = new Profile
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Birthday = registerDto.Birthday?.ToUniversalTime(),
                Address = registerDto.Address,
                PhoneNumber = registerDto.PhoneNumber
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == loginDto.Email);

            if (profile == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, profile.Password))
            {
                return Unauthorized("Invalid email or password");
            }

            var token = GenerateJwtToken(profile);

            return Ok(new { Token = token, UserId = profile.Id, IsAdmin = profile.Administrator });
        }

        [HttpDelete("deactivate/{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
            {
                return NotFound();
            }

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();


            return Ok();
        }

        private string GenerateJwtToken(Profile profile)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString()),
                new Claim(ClaimTypes.Email, profile.Email),
                new Claim(ClaimTypes.Role, profile.Administrator ? "Administrator" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] Profile updatedProfile)
        {
            if (id != updatedProfile.Id)
            {
                return BadRequest();
            }

            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            profile.Name = updatedProfile.Name;
            profile.Email = updatedProfile.Email;
            profile.Birthday = updatedProfile.Birthday;
            profile.Address = updatedProfile.Address;
            profile.PhoneNumber = updatedProfile.PhoneNumber;
            profile.Administrator = updatedProfile.Administrator;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ProfileExists(int id)
        {
            // G� igennem alle profiler i databasen og tjek, om der findes en med det givne id
            foreach (var profile in _context.Profiles)
            {
                if (profile.Id == id)
                {
                    return true; // Profilen findes
                }
            }
            return false; // Profilen findes ikke
        }

        [HttpPut("admin/{id}")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminUpdateProfile(int id, [FromBody] Profile updatedProfile)
        {
            try
            {
                if (id != updatedProfile.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var profile = await _context.Profiles.FindAsync(id);
                if (profile == null)
                {
                    return NotFound($"Profile with ID {id} not found");
                }

                profile.Name = updatedProfile.Name;
                profile.Email = updatedProfile.Email;
                profile.Birthday = updatedProfile.Birthday;
                profile.Address = updatedProfile.Address;
                profile.PhoneNumber = updatedProfile.PhoneNumber;
                profile.Administrator = updatedProfile.Administrator;

                await _context.SaveChangesAsync();
                return Ok("Profile updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                {
                    return NotFound($"Profile with ID {id} no longer exists");
                }
                else
                {
                    return StatusCode(500, "A concurrency error occurred while updating the profile");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the profile: {ex.Message}");
            }
        }

        [HttpGet("all")]
        // [Authorize(Roles = "Administrator")] // Temporarily comment this out for testing
        public async Task<ActionResult<IEnumerable<Profile>>> GetAllProfiles()
        {
            try
            {
                return await _context.Profiles.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while fetching profiles");
            }
        }



    }
}