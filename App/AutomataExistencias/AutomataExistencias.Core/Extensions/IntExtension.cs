namespace AutomataExistencias.Core.Extensions
{
    public static class IntExtension
    {
        public static int NullTo(this int? value, int @default = 0)
        {
            return value ?? @default;
        }
        public static int ToInt(this string value)
        {
            return int.Parse(value);
        }
    }
}