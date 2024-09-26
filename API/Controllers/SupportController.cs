using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DomainModels;
using Npgsql;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Collections.Generic;

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
        //Create Ticket (Support Request)
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
                return Ok(new { message = "Support request submitted successfully" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSupportRequests()
        {
            try
            {
                var supportRequests = new List<SupportRequest>();
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT * FROM SupportRequests ORDER BY CreatedAt DESC";
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var supportRequest = new SupportRequest
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                    Message = reader.GetString(reader.GetOrdinal("Message")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    Status = reader.GetString(reader.GetOrdinal("Status"))
                                };
                                supportRequests.Add(supportRequest);
                            }
                        }
                    }
                }
                return Ok(supportRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }

            //Get Support Request by Id
            [HttpGet("{id}")]
        public async Task<IActionResult> GetSupportRequest(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT * FROM SupportRequests WHERE Id = @Id";
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("Id", id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var supportRequest = new SupportRequest
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                    Message = reader.GetString(reader.GetOrdinal("Message")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    Status = reader.GetString(reader.GetOrdinal("Status"))
                                };
                                return Ok(supportRequest);
                            }
                            else
                            {
                                return NotFound($"Support request with ID {id} not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        //Edit Support Request By Id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupportRequest(int id, SupportRequest request)
        {
            if (request == null || id != request.Id)
            {
                return BadRequest("Invalid request");
            }

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = @"UPDATE SupportRequests 
                                SET Name = @Name, Email = @Email, Subject = @Subject, 
                                    Message = @Message, Status = @Status
                                WHERE Id = @Id";
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("Id", id);
                        cmd.Parameters.AddWithValue("Name", request.Name);
                        cmd.Parameters.AddWithValue("Email", request.Email);
                        cmd.Parameters.AddWithValue("Subject", request.Subject);
                        cmd.Parameters.AddWithValue("Message", request.Message);
                        cmd.Parameters.AddWithValue("Status", request.Status);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Support request updated successfully" });
                        }
                        else
                        {
                            return NotFound($"Support request with ID {id} not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        //Delete Support Request By Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupportRequest(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "DELETE FROM SupportRequests WHERE Id = @Id";
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("Id", id);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = $"Support request with ID {id} deleted successfully" });
                        }
                        else
                        {
                            return NotFound($"Support request with ID {id} not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}

