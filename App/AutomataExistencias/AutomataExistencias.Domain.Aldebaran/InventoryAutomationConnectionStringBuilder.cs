using System;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public static class InventoryAutomationConnectionStringBuilder
    {
        public static string Build(InventoryAutomationConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var serverPart = string.IsNullOrWhiteSpace(connection.PortNumber)
                ? connection.ServerName
                : $"{connection.ServerName},{connection.PortNumber}";

            return $"Server={serverPart};Database={connection.DatabaseName};User Id={connection.UserId};Password={connection.Password};MultipleActiveResultSets=True;";
        }
    }
}
