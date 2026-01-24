using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Core.Extensions;
using NLog;

namespace AutomataExistencias.Application
{
    public class NotificationService : INotificationService
    {
        private readonly IAutomataNotificationRecipientService _recipientService;
        private readonly Logger _logger;

        public NotificationService(IAutomataNotificationRecipientService recipientService)
        {
            _recipientService = recipientService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void NotifyConnectivityDown(IEnumerable<InventoryAutomationConnection> connections, DateTime since)
        {
            try
            {
                var recipients = _recipientService.GetActiveByType("CONNECTIVITY_DOWN").Select(s => s.Email).ToList();
                if (!recipients.Any())
                {
                    _logger.Warn("No recipients configured for CONNECTIVITY_DOWN");
                    return;
                }

                var subject = "AutomataExistencias - Connectivity DOWN";
                var body = $"Detected connectivity failure since {since:u}. Affected connections:\n" +
                           string.Join("\n", connections.Select(c => $"ConnId={c.InventoryAutomationConnectionId}, Server={c.ServerName}, Database={c.DatabaseName}"));

                SendEmail(recipients, subject, body);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error sending connectivity down notification: {ex.ToJson()}");
            }
        }

        public void NotifyConnectivityRecovered(int itemsRecovered, DateTime since, DateTime until)
        {
            try
            {
                var recipients = _recipientService.GetActiveByType("CONNECTIVITY_RECOVERED").Select(s => s.Email).ToList();
                if (!recipients.Any())
                {
                    _logger.Warn("No recipients configured for CONNECTIVITY_RECOVERED");
                    return;
                }

                var subject = "AutomataExistencias - Connectivity RECOVERED";
                var body = $"Connectivity recovered. Period: {since:u} - {until:u}. Items recovered: {itemsRecovered}.";

                SendEmail(recipients, subject, body);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error sending connectivity recovered notification: {ex.ToJson()}");
            }
        }

        private void SendEmail(IEnumerable<string> to, string subject, string body)
        {
            try
            {
                var smtpHost = System.Configuration.ConfigurationManager.AppSettings["Smtp.Host"];
                var smtpPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Smtp.Port"] ?? "25");
                var smtpUser = System.Configuration.ConfigurationManager.AppSettings["Smtp.User"];
                var smtpPass = System.Configuration.ConfigurationManager.AppSettings["Smtp.Password"];
                var from = System.Configuration.ConfigurationManager.AppSettings["Smtp.From"] ?? "no-reply@automata.example.com";

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    if (!string.IsNullOrEmpty(smtpUser))
                    {
                        client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    }
                    client.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Smtp.EnableSsl"] ?? "false");

                    var mail = new MailMessage();
                    mail.From = new MailAddress(from);
                    foreach (var address in to.Distinct())
                        mail.To.Add(address);
                    mail.Subject = subject;
                    mail.Body = body;

                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error sending email: {ex.ToJson()}");
            }
        }
    }
}
