using DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net.NetworkInformation;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly string _connectionString;
        public BookingController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            List<Booking> allBookings = new List<Booking>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("Select * FROM booking", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allBookings.Add(new Booking
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                DateStart = Convert.ToDateTime(reader["date_start"]),
                                DateEnd = Convert.ToDateTime(reader["date_end"]),
                                ProfileId = Convert.ToInt32(reader["profile_id"]),
                                RoomId = Convert.ToInt32(reader["room_id"])
                            });
                        }
                    }
                }
            }

            return allBookings;
        }

        [HttpGet("BookingsToday")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsForToday()
        {
            List<Booking> allBookings = new List<Booking>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var query = $"SELECT * FROM booking WHERE date_start <= '{today}' AND date_end >= '{today}'";
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allBookings.Add(new Booking
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                DateStart = Convert.ToDateTime(reader["date_start"]),
                                DateEnd = Convert.ToDateTime(reader["date_end"]),
                                ProfileId = Convert.ToInt32(reader["profile_id"]),
                                RoomId = Convert.ToInt32(reader["room_id"])
                            });
                        }
                    }
                }
            }

            return allBookings;
        }

        [HttpGet("Bookings/{UserID}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsFromUserID(int UserID)
        {
            List<Booking> allBookings = new List<Booking>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var query = $"SELECT * FROM booking WHERE profile_id = '{UserID}' AND date_end >= '{today}' ORDER BY date_start";
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allBookings.Add(new Booking
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                DateStart = Convert.ToDateTime(reader["date_start"]),
                                DateEnd = Convert.ToDateTime(reader["date_end"]),
                                ProfileId = Convert.ToInt32(reader["profile_id"]),
                                RoomId = Convert.ToInt32(reader["room_id"])
                            });
                        }
                    }
                }
            }

            return allBookings;
        }

    }

}
