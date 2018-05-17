using System;

namespace AutomataExistencias.Core.Extensions
{
    public static class DatetimeExtension
    {
        public static DateTime NullTo(this DateTime? value, DateTime @default)
        {
            return value ?? @default;
        }
    }
}