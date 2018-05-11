using AutomataExistencias.Core.Configuration;

namespace AutomataExistencias.Console.Code
{
    public class CatapromApplicationEnvironment : ICatapromApplicationEnvironment
    {
        public string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["Cataprom"].ConnectionString;
        }
    }
}
