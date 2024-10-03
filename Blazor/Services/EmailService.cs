using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DomainModels;

public class EmailService
{
    private readonly IConfiguration _configuration;

    // Constructor to inject IConfiguration
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Method to send booking confirmation email
    public async Task SendBookingConfirmationEmail(Room room, Booking booking, string userEmail)
    {
        try
        {
            // Get email settings from configuration
            var emailSettings = _configuration.GetSection("EmailSettings");

            // Configure SMTP client
            var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["SmtpPort"]),
                Credentials = new NetworkCredential(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
            };

            // Read and convert logo image to base64
            string imagePath = "wwwroot/Maws-hoteller-photos/sunset resort.png";
            byte[] imageArray = File.ReadAllBytes(imagePath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            // Create email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SmtpUsername"]),
                Subject = "Booking Confirmation",
                Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #333;'>
                    <img src='cid:SunsetResortLogo' alt='Sunset Resort Logo' style='max-width: 200px; margin-bottom: 20px;' />
                    <h1 style='color: #4a4a4a;'>Thank you for your booking!</h1>
                    <div style='background-color: #f8f8f8; padding: 20px; border-radius: 5px;'>
                        <h2 style='color: #2c3e50;'>Booking Details</h2>
                        <p><strong>Room:</strong> {room.Name}</p>
                        <p><strong>Start Date:</strong> {booking.DateStart:d}</p>
                        <p><strong>End Date:</strong> {booking.DateEnd:d}</p>
                        <p><strong>Room Number:</strong> {booking.RoomId}</p>
                    </div>
                    <p style='margin-top: 20px;'>We hope you enjoy your stay at Sunset Resort!</p>
                </body>
                </html>",
                IsBodyHtml = true,
            };

            // Attach logo image as inline attachment
            Attachment inline = new Attachment(imagePath);
            inline.ContentId = "SunsetResortLogo";
            inline.ContentDisposition.Inline = true;
            inline.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
            mailMessage.Attachments.Add(inline);

            // Add recipient email address
            mailMessage.To.Add(userEmail);

            // Send email asynchronously
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            // Log error and rethrow
            Console.WriteLine($"Error sending confirmation email: {ex.Message}");
            throw;
        }
    }
}
