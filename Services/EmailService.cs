using System.Net;
using System.Net.Mail;

namespace PharmacyInventoryAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config,
            ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendLowStockAlert(
            string supplierEmail,
            string medicineName,
            int quantity)
        {
            try
            {
                var smtpHost = _config["Email:SmtpHost"];
                var smtpPort = int.Parse(_config["Email:SmtpPort"]!);
                var fromEmail = _config["Email:FromEmail"];
                var password = _config["Email:Password"];

                var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(fromEmail!),
                    Subject = $"Low Stock Alert: {medicineName}",
                    Body = $"""
                        Dear Supplier,

                        This is an automated alert from Pharmacy Inventory System.

                        Medicine: {medicineName}
                        Current Stock: {quantity} units remaining

                        Please arrange immediate restocking.

                        Regards,
                        Pharmacy Management System
                        """,
                    IsBodyHtml = false
                };

                mail.To.Add(supplierEmail);
                await client.SendMailAsync(mail);

                _logger.LogInformation(
                    "Low stock email sent to supplier {Email} for {Medicine}",
                    supplierEmail, medicineName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Email failed to {Email}: {Error}",
                    supplierEmail, ex.Message);
            }
        }
    }
}