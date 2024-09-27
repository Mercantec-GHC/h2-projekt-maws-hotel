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

        //GetRooms
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
    }
}
