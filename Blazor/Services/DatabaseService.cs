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
            return await _httpClient.GetFromJsonAsync<List<Booking>>(_baseURL + $"Booking/Bookings/{UserID}");
        }
        //Sender Support besked til Database 
        public async Task PostSupportRequest(SupportRequest request)
        {
             await _httpClient.PostAsJsonAsync(_baseURL + "Support", request);
        }
        public async Task PostBooking(Booking request)
        {
            await _httpClient.PostAsJsonAsync(_baseURL + "Booking", request);
        }
        public async Task DeleteBooking(int id)
        {
            await _httpClient.DeleteFromJsonAsync<Booking>(_baseURL + $"Booking/DeleteBooking/{id}");
        }
    }
}