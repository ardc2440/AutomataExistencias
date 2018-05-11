using System;
using System.Collections.Generic;

namespace AutomataExistencias.Console.Schedules
{
    public interface IJobSchedulerFactory
    {
        void Schedule(Dictionary<string, TimeSpan> scheduleConfig);
    }
}
