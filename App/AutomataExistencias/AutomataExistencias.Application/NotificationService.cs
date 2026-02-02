using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.Domain.Aldebaran;
using NLog;

namespace AutomataExistencias.Application
{
    public class NotificationService : INotificationService
    {
        private readonly IAutomataNotificationRecipientService _recipientService;
        private readonly Logger _logger;
        private readonly Core.IAutomataState _automataState;
        // Track whether we've already sent a connectivity-down notification to avoid repeats
        private readonly object _connectivityNotifLock = new object();
        private bool _connectivityDownNotified;

        public NotificationService(IAutomataNotificationRecipientService recipientService, Core.IAutomataState automataState)
        {
            _recipientService = recipientService;
            _automataState = automataState;
            _logger = LogManager.GetCurrentClassLogger();
        }

        // Origin is treated as part of aggregated connectivity checks; no separate origin notifications implemented here.

        public void NotifyConnectivityDown(IEnumerable<InventoryAutomationConnection> connections, DateTime since, IEnumerable<Item> failedItems = null, int consecutiveFailures = 0)
        {
            try
            {
                // Send only once per DOWN transition. If we've already sent a DOWN notification and
                // the state hasn't been recovered yet, skip duplicate notifications.
                lock (_connectivityNotifLock)
                {
                    if (_connectivityDownNotified)
                    {
                        _logger.Debug("NotifyConnectivityDown: duplicate notification suppressed (already sent for current DOWN state)");
                        return;
                    }
                    // mark as sent now (will be cleared on recovered)
                    _connectivityDownNotified = true;
                }
                var recipients = _recipientService.GetActiveByType("CONNECTIVITY").Select(r => r.Email).Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().ToList();
                if (!recipients.Any())
                {
                    _logger.Warn("NotifyConnectivityDown: no recipients configured for CONNECTIVITY");
                    return;
                }

                var subject = "[Automata] Connectivity DOWN" + (consecutiveFailures > 0 ? $" - Attempts={consecutiveFailures}" : string.Empty);

                // Build body including origin vs destination error counts when available
                var body = $"Connectivity to destinations marked DOWN since {since:u}.\r\n\r\nDestinations:\r\n" +
                           string.Join("\r\n", connections.Select(c => $"- {c.ServerName} - {c.DatabaseName} (Id={c.InventoryAutomationConnectionId})"));

                try
                {
                    var totalErrors = 0;
                    var originErrors = 0;
                    var destErrors = 0;
                    try
                    {
                        if (_automataState != null)
                        {
                            totalErrors = _automataState.GetTotalConnectivityErrorCount();
                            // Use configured window minutes for origin count so origin behaves like any other destination
                            var app = System.Configuration.ConfigurationManager.AppSettings;
                            int windowMinutes;
                            if (!int.TryParse(app["ConnectivityError.WindowMinutes"], out windowMinutes) || windowMinutes <= 0)
                                windowMinutes = 15;
                            originErrors = _automataState.GetConnectivityErrorCountForConnection(0, windowMinutes);
                            destErrors = totalErrors - originErrors;
                        }
                    }
                    catch { }

                    if (totalErrors > 0)
                    {
                        body += $"\r\n\r\nError counts: Total={totalErrors}, Origin={originErrors}, Destinations={destErrors}\r\n";
                    }
                }
                catch { }

                if (failedItems != null && failedItems.Any())
                {
                    body += "\r\n\r\nItems with connectivity failures:\r\n" +
                            string.Join("\r\n", failedItems.Select(i => $"- {i.Name} (Ref={i.Reference})"));
                }

                // fire-and-forget async send; errors are logged inside SendEmailAsync
                _ = SendEmailAsync(recipients, subject, body);
            }
            catch (Exception ex)
            {
                _logger.Warn($"NotifyConnectivityDown failed: {ex}");
            }
        }

        public void NotifyConnectivityRecovered(int itemsRecovered, DateTime since, DateTime until)
        {
            try
            {
                // Clear the marker so future DOWN transitions will notify again
                lock (_connectivityNotifLock)
                {
                    _connectivityDownNotified = false;
                }

                var recipients = _recipientService.GetActiveByType("CONNECTIVITY").Select(r => r.Email).Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().ToList();
                if (!recipients.Any())
                {
                    _logger.Warn("NotifyConnectivityRecovered: no recipients configured for CONNECTIVITY");
                    return;
                }

                var subject = "[Automata] Connectivity RECOVERED";
                var body = $"Connectivity recovered. Items recovered: {itemsRecovered}.\r\nSince: {since:u}\r\nUntil: {until:u}";
                _ = SendEmailAsync(recipients, subject, body);
            }
            catch (Exception ex)
            {
                _logger.Warn($"NotifyConnectivityRecovered failed: {ex}");
            }
        }
                       
        // New: notify both non-connectivity (business) errors and pending connectivity errors
        public void NotifyPendingAndNonConnectivityErrors(IEnumerable<string> nonConnectivityDescriptions, IEnumerable<string> pendingConnectivityDescriptions, DateTime since)
        {
            try
            {
                var recipients = _recipientService.GetActiveByType("GENERAL").Select(r => r.Email).Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().ToList();
                if (!recipients.Any())
                {
                    _logger.Warn("NotifyPendingAndNonConnectivityErrors: no recipients configured for GENERAL");
                    return;
                }

                var subject = "[Automata] Pending sync errors (business and connectivity)";

                var body = $"Pending sync errors detected since {since:u}.\r\n\r\n";

                if (nonConnectivityDescriptions != null && nonConnectivityDescriptions.Any())
                {
                    body += "Non-connectivity (business) errors:\r\n";
                    body += string.Join("\r\n", nonConnectivityDescriptions);
                    body += "\r\n\r\n";
                }

                if (pendingConnectivityDescriptions != null && pendingConnectivityDescriptions.Any())
                {
                    body += "Pending connectivity errors (require manual attention or recovery):\r\n";
                    body += string.Join("\r\n", pendingConnectivityDescriptions);
                    body += "\r\n\r\n";
                }

                body += "Note: items listed above may still be retried automatically depending on SyncAttempts and Recovery configuration.";

                _ = SendEmailAsync(recipients, subject, body);
            }
            catch (Exception ex)
            {
                _logger.Warn($"NotifyPendingAndNonConnectivityErrors failed: {ex}");
            }
        }

        private async System.Threading.Tasks.Task SendEmailAsync(IEnumerable<string> to, string subject, string body)
        {
            try
            {
                // Prefer explicit appSettings keys; fallback to legacy JSON if not present
                var app = System.Configuration.ConfigurationManager.AppSettings;
                var server = app["Mail.Server"];
                var portStr = app["Mail.Port"];
                var fromEmail = app["Mail.SenderEmail"];
                var fromName = app["Mail.SenderName"];
                var password = app["Mail.Password"];
                var secure = app["Mail.SecureSocketOption"];

                // Require explicit appSettings keys
                if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(fromEmail))
                {
                    _logger.Error("Mail configuration missing: ensure Mail.Server and Mail.SenderEmail are set in App.config");
                    return;
                }

                int port = 25;
                int.TryParse(portStr, out port);

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress((string)fromEmail, (string)fromName);
                    foreach (var addr in to.Where(t => !string.IsNullOrWhiteSpace(t)))
                        message.To.Add(addr);
                    message.Subject = subject;
                    message.Body = body;

                    using (var client = new SmtpClient((string)server, port))
                    {
                        client.UseDefaultCredentials = false;
                        if (!string.IsNullOrWhiteSpace((string)password))
                        {
                            client.Credentials = new NetworkCredential((string)fromEmail, (string)password);
                        }
                        var ssl = (secure ?? string.Empty).ToUpperInvariant();
                        client.EnableSsl = ssl == "SSL" || ssl == "STARTTLS";

                        try
                        {
                            await client.SendMailAsync(message).ConfigureAwait(false);
                            _logger.Info($"Email sent: Subject='{subject}' To={string.Join(", ", to)}");
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"SendEmailAsync failed sending Subject='{subject}' To={string.Join(", ", to)} | Exception: {ex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"SendEmailAsync fatal error preparing message Subject='{subject}': {ex}");
            }
        }
    }
}
