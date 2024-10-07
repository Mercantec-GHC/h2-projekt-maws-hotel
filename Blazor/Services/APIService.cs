#region Usings
using Blazor.Components.Pages;
using DomainModels;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Blazor.Services.APIService;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace Blazor.Services
{
    public class APIService
    {
        private readonly string connectionString;
        private readonly HttpClient _httpClient;
        private readonly string _baseURL = "https://localhost:7207/api/";

        public APIService(string connectionString, HttpClient httpClient)
        {
            this.connectionString = connectionString;
            _httpClient = httpClient;
        }
        #region Booking
        //---CONNECTIONS TO BOOKING API---//

        // Create a new booking
        public async Task PostBooking(Booking request)
        {
            await _httpClient.PostAsJsonAsync(_baseURL + "Booking/CreateBooking", request);
        }

        //Get all bookings
        public async Task<List<Booking>> GetBookings()
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + "Booking");
        }

        //Get all bookings for today 
        public async Task<List<Booking>> GetBookingsForToday()
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + "Booking/BookingsToday");
        }

        //Get all bookings from a specific user by UserID
        public async Task<List<Booking>> GetBookingsFromUserID(int UserID)
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + $"Booking/userid/{UserID}");
        }

        //Get all bookings from a specific user by RoomID
        public async Task<List<Booking>> GetBookingsFromRoomId(int RoomId)
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + $"Booking/roomid/{RoomId}");
        }

        // Get booking by BookingId
        public async Task<Booking> GetBookingById(int id)
        {
            return await _httpClient.GetFromJsonAsync<Booking>(_baseURL + $"Booking/BookingID/{id}");
        }

        // Check existings bookings for a specific room by roomId & dates 
        public async Task<List<Booking>> CheckExistingBookings(int roomId, DateTime dateStart, DateTime dateEnd)
        {
            var url = $"{_baseURL}Booking/CheckExistingBookings/{roomId}?dateStart={dateStart:yyyy-MM-dd}&dateEnd={dateEnd:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<List<Booking>>(url);
        }

        // Edit an existing booking 
        public async Task<HttpResponseMessage> EditBooking(int id, Booking bookingRequest)
        {
            return await _httpClient.PutAsJsonAsync(_baseURL + $"Booking/EditBooking/{id}", bookingRequest);
        }

        //Delete a booking 
        public async Task DeleteBooking(int id)
        {
            await _httpClient.DeleteFromJsonAsync<Booking>(_baseURL + $"Booking/DeleteBooking/{id}");
        }
        #endregion

        #region Room
        //---CONNECTIONS TO ROOM API---//

        // Create room
        public async Task<HttpResponseMessage> CreateRoom(Room room)
        {
            return await _httpClient.PostAsJsonAsync<Room>(_baseURL + "Room/CreateRoom", room);
        }

        //Get all rooms
        public async Task<List<Room>> GetRooms()
        {
            return await _httpClient.GetFromJsonAsync<List<Room>>(_baseURL + "Room");
        }

        //Get room by RoomId
        public async Task<Room> GetRoomById(int id)
        {
            return await _httpClient.GetFromJsonAsync<Room>(_baseURL + $"Room/RoomId/{id}");
        }

        // Update room by roomId
        public async Task<HttpResponseMessage> UpdateRoom(int id, Room room)
        {
            return await _httpClient.PutAsJsonAsync(_baseURL + $"Room/EditRoom/{id}", room);
        }

        //Delete a room
        public async Task DeleteRoom(int id)
        {
            await _httpClient.DeleteFromJsonAsync<Room>(_baseURL + $"Room/DeleteRoom/{id}");
        }
        #endregion

        #region Support
        //---CONNECTIONS TO SUPPORT API---//

        //Create Support Request 
        public async Task PostSupportRequest(SupportRequest request)
        {
            await _httpClient.PostAsJsonAsync(_baseURL + "Support", request);
        }

        // Get all support requests
        public async Task<List<SupportRequest>> GetAllSupportRequests()
        {
            return await _httpClient.GetFromJsonAsync<List<SupportRequest>>(_baseURL + "Support");
        }

        // Get support request by ID
        public async Task<SupportRequest> GetSupportRequestById(int id)
        {
            return await _httpClient.GetFromJsonAsync<SupportRequest>(_baseURL + $"Support/{id}");
        }

        // Update support request
        public async Task<HttpResponseMessage> UpdateSupportRequest(int id, SupportRequest request)
        {
            return await _httpClient.PutAsJsonAsync(_baseURL + $"Support/{id}", request);
        }

        // Delete support request by ID
        public async Task<HttpResponseMessage> DeleteSupportRequest(int id)
        {
            return await _httpClient.DeleteAsync(_baseURL + $"Support/{id}");
        }
        #endregion

        #region Profile
        //---CONNECTIONS TO PROFILE API---//

        // Create a new profile 
        public async Task<string> PostProfile(RegisterDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseURL + "Profile/register", request);

            if (response.IsSuccessStatusCode)
            {
                return "Registration successful!";
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        // Get all profiles
        public async Task<List<Profile>> GetAllProfiles()
        {
            return await _httpClient.GetFromJsonAsync<List<Profile>>(_baseURL + "api/profile/all");

        }

        //Get profile by userID
        public async Task<Profile> GetProfileByUserID(int userID)
        {
            return await _httpClient.GetFromJsonAsync<Profile>(_baseURL + $"profile/{userID}");
        }

        // Get user-email by userId
        public async Task<string> GetUserEmailById(int userId)
        {
            try
            {
                var profile = await _httpClient.GetFromJsonAsync<Profile>(_baseURL + $"profile/{userId}");
                return profile?.Email ?? throw new Exception("User email not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user email: {ex.Message}");
                throw;
            }
        }

        //Edit profile by id
        public async Task<HttpResponseMessage> edit_profile(int id, Profile profile)
        {
            return await _httpClient.PutAsJsonAsync(_baseURL + $"profile/{id}", profile);
        }

        public async Task<HttpResponseMessage> DeleteProfile(int id){
            return await _httpClient.DeleteAsync($"api/profile/delete/{id}");

        }

        // Login to profile
        public async Task<HttpResponseMessage> LoginProfile(LoginDto loginDto)
        {
            return await _httpClient.PostAsJsonAsync(_baseURL + "profile/login", loginDto);
        }
    }
}
#endregion