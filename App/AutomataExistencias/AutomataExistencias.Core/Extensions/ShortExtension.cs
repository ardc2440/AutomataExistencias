namespace AutomataExistencias.Core.Extensions
{
    public static class ShortExtension
    {
        public static short NullTo(this short? value, short @default = 0)
        {
            return value ?? @default;
        }
    }
}