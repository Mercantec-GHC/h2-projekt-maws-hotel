using DomainModels;

namespace Blazor.Services
{
    public class Calendar
    {

    private List<Booking> exBookingsOnRoomId = new List<Booking>();
    private List<DateTime> specialDates = new List<DateTime>();

    private async Task GetBookingsFromRoomId(int RoomId)
    {
        var existingBookings = await dbService.CheckExistingBookings(RoomId, (DateTime)date_start, (DateTime)date_end);

    }


        private string CheckDate(DateTime date)
        {
            specialDates.Clear();
            foreach (var booking in exBookingsOnRoomId)
            {
                specialDates.AddRange(Enumerable.Range(0, (booking.DateEnd - booking.DateStart).Days + 1).Select(DateTimeOffset => booking.DateStart.AddDays(DateTimeOffset)));
            }
            // compare only the date portion to find a match
            return this.specialDates.Contains(date.Date) ? "special-day" : string.Empty;
        }

    }
}
