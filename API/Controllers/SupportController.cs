using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DomainModels;
using Npgsql;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        private readonly string _connectionString;

        public SupportController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> PostSupportRequest(SupportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO SupportRequests (Name, Email, Subject, Message, CreatedAt, Status) " +
                          "VALUES (@Name, @Email, @Subject, @Message, @CreatedAt, @Status)";
                    await connection.OpenAsync();
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("Name", request.Name);
                        cmd.Parameters.AddWithValue("Email", request.Email);
                        cmd.Parameters.AddWithValue("Subject", request.Subject);
                        cmd.Parameters.AddWithValue("Message", request.Message);
                        cmd.Parameters.AddWithValue("CreatedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("Status", "Pending");

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                //await AddSupportRequest(request);
                return Ok(new { message = "Support request submitted successfully" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
