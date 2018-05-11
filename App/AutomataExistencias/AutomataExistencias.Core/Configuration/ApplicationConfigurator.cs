using System.Configuration;

namespace AutomataExistencias.Core.Configuration
{
    public class ApplicationConfigurator : IConfigurator
    {
        public string GetKey(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}