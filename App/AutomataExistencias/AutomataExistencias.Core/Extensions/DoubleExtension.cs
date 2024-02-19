namespace AutomataExistencias.Core.Extensions
{
    public static class DoubleExtension
    {
        public static double NullTo(this double? value, double @default = 0)
        {
            return value ?? @default;
        }
    }
}