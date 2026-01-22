using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IAutomataConnectivityPatternService
    {
        IEnumerable<AutomataConnectivityErrorPattern> GetActivePatternsForTarget(char target); // 'D' or 'O' or 'B'
    }
}
