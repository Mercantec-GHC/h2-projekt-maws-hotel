using Blazor.Components.Pages;
using DomainModels;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Blazor.Services.DatabaseService;
using Microsoft.AspNetCore.Http.HttpResults;



namespace Blazor.Services
{
    public class DatabaseService
    {
        private readonly string connectionString;
        private readonly HttpClient _httpClient;
        private readonly string _baseURL = "https://localhost:7207/api/";

        public DatabaseService(string connectionString, HttpClient httpClient)
        {
            this.connectionString = connectionString;
            _httpClient = httpClient;
        }

        public async Task<List<Room>> GetRooms()
        {
            return await _httpClient.GetFromJsonAsync<List<Room>>(_baseURL + "Room");
        }

        public async Task<List<Booking>> GetBookings()
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + "Booking");
        }

        public async Task<List<Booking>> GetBookingsForToday()
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + "Booking/BookingsToday");
        }

        public async Task<List<Booking>> GetBookingsFromUserID(int UserID)
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + $"Booking/Bookings/{UserID}");
        }



        public bool UpdateProfile(Profile profile)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"
                            UPDATE profile 
                            SET name = @Name, email = @Email, birthday = @Birthday, 
                                address = @Address, phone_number = @PhoneNumber, administrator = @Administrator
                            WHERE id = @Id";

                        command.Parameters.AddWithValue("@Id", profile.Id);
                        command.Parameters.AddWithValue("@Name", profile.Name);
                        command.Parameters.AddWithValue("@Email", profile.Email);
                        command.Parameters.AddWithValue("@Birthday", profile.Birthday ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Address", profile.Address ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PhoneNumber", profile.PhoneNumber ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Administrator", profile.Administrator);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProfile: {ex.Message}");
                return false;
            }
        }

        public int ExecuteSql(string sql)
        {

            var rowsaffected = -1;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        rowsaffected = Convert.ToInt32(result);
                    }
                }
            }
            return rowsaffected;
        }

        public void AddSupportRequest(SupportRequest request)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = "INSERT INTO SupportRequests (Name, Email, Subject, Message, CreatedAt, Status) " +
                      "VALUES (@Name, @Email, @Subject, @Message, @CreatedAt, @Status)";
                connection.Open();
                using (var cmd = new NpgsqlCommand(sql, connection))

                {

                    cmd.Parameters.AddWithValue("Name", request.Name);
                    cmd.Parameters.AddWithValue("Email", request.Email);
                    cmd.Parameters.AddWithValue("Subject", request.Subject);
                    cmd.Parameters.AddWithValue("Message", request.Message);
                    cmd.Parameters.AddWithValue("CreatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("Status", "Pending");

                    cmd.ExecuteNonQuery();

                }
            }

        }
    }

}


    





