using Newtonsoft.Json;
using System;

namespace AutomataExistencias.Core.Extensions
{
   public static class ExceptionExtension
    {
        public static string ToJson(this Exception ex)
        {
            var msg =
                new
                {
                    ex.Message,
                    InnerException = ex.InnerException == null ? null : ex.InnerException.Message,
                    ex.StackTrace
                };
            return JsonConvert.SerializeObject(msg);
        }
    }
}
