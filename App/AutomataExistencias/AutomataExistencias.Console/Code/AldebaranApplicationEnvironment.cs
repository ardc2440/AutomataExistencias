using AutomataExistencias.Core.Configuration;

namespace AutomataExistencias.Console.Code
{
    public class AldebaranApplicationEnvironment : IAldebaranApplicationEnvironment
    {
        public string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["Aldebaran"].ConnectionString;
        }
    }
}
