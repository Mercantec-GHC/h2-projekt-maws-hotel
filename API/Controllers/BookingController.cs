using DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly string _connectionString;
        

        public BookingController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Method to delete booking from ID
        [HttpDelete("DeleteBooking/{id}")]
        public IActionResult DeleteBooking(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("DELETE FROM booking WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Booking deleted successfully." });
                        }
                        else
                        {
                            return NotFound(new { message = "Booking not found." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting booking.", error = ex.Message });
            }
        }

        // Method to add a new booking
        [HttpPost]
        public IActionResult AddBooking(Booking bookingRequest)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "INSERT INTO booking (date_start, date_end, profile_id, room_id) " +
                              "VALUES (@DateStart, @DateEnd, @ProfileId, @RoomId)";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@DateStart", bookingRequest.DateStart);
                        command.Parameters.AddWithValue("@DateEnd", bookingRequest.DateEnd);
                        command.Parameters.AddWithValue("@ProfileId", bookingRequest.ProfileId);
                        command.Parameters.AddWithValue("@RoomId", bookingRequest.RoomId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Booking added successfully." });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to add booking." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error adding booking.", error = ex.Message });
            }
        }

        // Get all bookings 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var allBookings = new List<Booking>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM booking", connection))
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

        // Get all bookings for today 
        [HttpGet("BookingsToday")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsForToday()
        {
            var allBookings = new List<Booking>();
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

        // Get bookings for a specific room that overlap with a given date range
        [HttpGet("BookingsForRoomForSpecificDate/{roomId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsForRoomAndDateRange(int roomId, DateTime dateStart, DateTime dateEnd)
        {
            var overlappingBookings = new List<Booking>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = $"SELECT * FROM booking WHERE room_id = {roomId} AND " +
                            $"('{dateStart:yyyy-MM-dd}' < date_end AND '{dateEnd:yyyy-MM-dd}' > date_start)";
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            overlappingBookings.Add(new Booking
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
            return overlappingBookings;
        }


        // Get bookings from UserID
        [HttpGet("userid/{UserID}")]
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

        // Get bookings from RoomID
        [HttpGet("roomid/{RoomId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsFromRoomId(int RoomID)
        {
            var allBookings = new List<Booking>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var query = $"SELECT * FROM booking WHERE room_id = '{RoomID}' AND date_end >= '{today}' ORDER BY date_start";
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
