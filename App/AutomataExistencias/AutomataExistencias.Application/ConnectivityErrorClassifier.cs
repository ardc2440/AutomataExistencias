using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public class ConnectivityErrorClassifier : IConnectivityErrorClassifier
    {
        private readonly IAutomataConnectivityPatternService _patternService;
        private List<string> _destinationPatterns;
        private List<string> _originPatterns;

        public ConnectivityErrorClassifier(IAutomataConnectivityPatternService patternService)
        {
            _patternService = patternService;
            Refresh();
        }

        public bool IsDestinationConnectivityError(string exceptionText)
        {
            if (string.IsNullOrWhiteSpace(exceptionText)) return false;
            var text = exceptionText.ToUpperInvariant();
            return _destinationPatterns.Any(p => text.Contains(p));
        }

        public bool IsOriginConnectivityError(string exceptionText)
        {
            if (string.IsNullOrWhiteSpace(exceptionText)) return false;
            var text = exceptionText.ToUpperInvariant();
            return _originPatterns.Any(p => text.Contains(p));
        }

        public void Refresh()
        {
            _destinationPatterns = _patternService.GetActivePatternsForTarget('D').Select(s => s.Pattern.ToUpperInvariant()).ToList();
            _originPatterns = _patternService.GetActivePatternsForTarget('O').Select(s => s.Pattern.ToUpperInvariant()).ToList();

            // Include 'B' (both) patterns in both lists
            var both = _patternService.GetActivePatternsForTarget('B').Select(s => s.Pattern.ToUpperInvariant()).ToList();
            _destinationPatterns.AddRange(both);
            _originPatterns.AddRange(both);
        }
    }
}
