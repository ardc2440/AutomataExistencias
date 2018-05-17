namespace AutomataExistencias.Core.Extensions
{
    public static class DecimalExtension
    {
        public static decimal NullTo(this decimal? value, decimal @default = 0)
        {
            return value ?? @default;
        }
    }
}