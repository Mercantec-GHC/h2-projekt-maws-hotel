using DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly string _connectionString;
        public RoomController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        // Create a new room
        [HttpPost("CreateRoom")]
        public IActionResult CreateRoom(Room roomRequest)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "INSERT INTO room (price, digital_key, type, photos, description, name) " +
                              "VALUES (@Price, @DigitalKey, @Type, @Photos, @Description, @Name) RETURNING id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Price", roomRequest.Price);
                        command.Parameters.AddWithValue("@DigitalKey", roomRequest.DigitalKey);
                        command.Parameters.AddWithValue("@Type", roomRequest.Type);
                        command.Parameters.AddWithValue("@Photos", roomRequest.Photos);
                        command.Parameters.AddWithValue("@Description", roomRequest.Description);
                        command.Parameters.AddWithValue("@Name", roomRequest.Name);

                        int newRoomId = Convert.ToInt32(command.ExecuteScalar());

                        return CreatedAtAction(nameof(GetRoomById), new { RoomId = newRoomId }, new { message = "Room created successfully.", id = newRoomId });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating room.", error = ex.Message });
            }
        }

        //Get all rooms 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            List<Room> allRooms = new List<Room>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM room", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allRooms.Add(new Room
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Price = Convert.ToSingle(reader["price"]),
                                DigitalKey = Convert.ToInt32(reader["digital_key"]),
                                Type = Convert.ToInt32(reader["type"]),
                                Photos = reader["photos"].ToString(),
                                Description = reader["description"].ToString(),
                                Name = Convert.ToString(reader["name"])
                            });
                        }
                    }
                }
            }
            return allRooms;
        }
        //Get room by ID 
        [HttpGet("RoomId/{RoomId}")]
        public async Task<ActionResult<Room>> GetRoomById(int RoomId)
        {
            Room room = new Room();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand($"SELECT * FROM room WHERE id = '{RoomId}'", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            room = new Room
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Price = Convert.ToSingle(reader["price"]),
                                DigitalKey = Convert.ToInt32(reader["digital_key"]),
                                Type = Convert.ToInt32(reader["type"]),
                                Photos = reader["photos"].ToString(),
                                Description = reader["description"].ToString(),
                                Name = Convert.ToString(reader["name"])
                            };
                        }
                    }
                }
            }
            return room;
        }

        // Edit an existing room by id
        [HttpPut("EditRoom/{id}")]
        public IActionResult EditRoom(int id, Room roomRequest)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "UPDATE room SET price = @Price, digital_key = @DigitalKey, " +
                              "type = @Type, photos = @Photos, description = @Description, " +
                              "name = @Name WHERE id = @Id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Price", roomRequest.Price);
                        command.Parameters.AddWithValue("@DigitalKey", roomRequest.DigitalKey);
                        command.Parameters.AddWithValue("@Type", roomRequest.Type);
                        command.Parameters.AddWithValue("@Photos", roomRequest.Photos);
                        command.Parameters.AddWithValue("@Description", roomRequest.Description);
                        command.Parameters.AddWithValue("@Name", roomRequest.Name);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Room updated successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = "Room not found." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating room.", error = ex.Message });
            }
        }

        // Delete Room by id
        [HttpDelete("DeleteRoom/{id}")]
        public IActionResult DeleteRoom(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("DELETE FROM room WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Room deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = "Room not found." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting room.", error = ex.Message });
            }
        }
    }
}