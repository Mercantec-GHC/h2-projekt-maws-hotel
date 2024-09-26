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
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Mvc;


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
        //Henter alle rum
        public async Task<List<Room>> GetRooms()
        {
            return await _httpClient.GetFromJsonAsync<List<Room>>(_baseURL + "Room");
        }
        //Henter alle bookings
        public async Task<List<Booking>> GetBookings()
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + "Booking");
        }
        //Henter alle bookings for i dag
        public async Task<List<Booking>> GetBookingsForToday()
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + "Booking/BookingsToday");
        }
        //Henter bookings for en specifik bruger ved brug af UserID 
        public async Task<List<Booking>> GetBookingsFromUserID(int UserID)
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + $"Booking/userid/{UserID}");
        }

        //Henter bookings for en specifik bruger ved brug af UserID 
        public async Task<List<Booking>> GetBookingsFromRoomId(int RoomId)
        {
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + $"Booking/roomid/{RoomId}");
        }

        public async Task<Room> GetRoomById(int id)
        {
            return await _httpClient.GetFromJsonAsync<Room>(_baseURL + $"Room/RoomId/{id}");
        }

        // Tjekker for eksisterende bookinger for et specifikt værelsesnummer
        public async Task<List<Booking>> CheckExistingBookings(int roomId, DateTime dateStart, DateTime dateEnd)
        {
            var url = $"{_baseURL}Booking/CheckExistingBookings/{roomId}?dateStart={dateStart:yyyy-MM-dd}&dateEnd={dateEnd:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<List<Booking>>(url);
        }

        // Opretter en ny booking
        public async Task PostBooking(Booking request)
        {
            await _httpClient.PostAsJsonAsync(_baseURL + "Booking/CreateBooking", request);
        }

        // Henter booking via ID 
        public async Task<Booking> GetBookingById(int id)
        {
           return await _httpClient.GetFromJsonAsync<Booking>(_baseURL + $"Booking/BookingID/{id}");
        }

         // Ændre en eksisterende booking
    public async Task<HttpResponseMessage> EditBooking(int id, Booking bookingRequest)
    {
        return await _httpClient.PutAsJsonAsync(_baseURL + $"Booking/EditBooking/{id}", bookingRequest);
    }

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
        
        //Sletter en booking 
        public async Task DeleteBooking(int id)
        {
            await _httpClient.DeleteFromJsonAsync<Booking>(_baseURL + $"Booking/DeleteBooking/{id}");
        }

        // Opretter en ny profile
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

        // Login to profile
        public async Task<HttpResponseMessage> LoginProfile(LoginDto loginDto)
        {
             return await _httpClient.PostAsJsonAsync(_baseURL + "profile/login", loginDto);
        }

        public async Task<Profile> GetProfileByUserID(int userID)
        {
            return await _httpClient.GetFromJsonAsync<Profile>(_baseURL + $"profile/{userID}");
        }
    }
}