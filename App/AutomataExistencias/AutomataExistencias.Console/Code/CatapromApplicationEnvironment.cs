using System.Configuration;
using AutomataExistencias.Core.Configuration;

namespace AutomataExistencias.Console.Code
{
    public class CatapromApplicationEnvironment : ICatapromApplicationEnvironment
    {
        public string GetConnectionString()
        {
            // In multidestino mode the Cataprom connection is provided per-destination
            // from the Inventory_Automation_Connections table. This method should not
            // be used by production code anymore.
            throw new System.NotSupportedException("Cataprom connection is multi-destination. Do not use CatapromApplicationEnvironment.GetConnectionString() in production.");
        }
    }
}
