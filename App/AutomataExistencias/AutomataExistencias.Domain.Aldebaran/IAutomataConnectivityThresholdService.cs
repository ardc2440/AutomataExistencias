using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IAutomataConnectivityThresholdService
    {
        AutomataConnectivityThreshold GetThresholdForEntity(string entityName);
        IEnumerable<AutomataConnectivityThreshold> GetAllActive();
    }
}
