using System;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using Newtonsoft.Json;

namespace AutomataExistencias.Application
{
    public static class SerializationThrottler
    {
        private static readonly ConcurrentDictionary<string, DateTime> LastSerialized = new ConcurrentDictionary<string, DateTime>();
        private static readonly TimeSpan DefaultInterval = TimeSpan.FromMinutes(5);

        private static TimeSpan GetInterval()
        {
            try
            {
                var cfg = ConfigurationManager.AppSettings["Logging.FatalSerializationIntervalMinutes"];
                if (int.TryParse(cfg, out var minutes) && minutes > 0)
                    return TimeSpan.FromMinutes(minutes);
            }
            catch { }
            return DefaultInterval;
        }

        public static string SerializeIfAllowed(object obj, string key)
        {
            if (obj == null) return null;
            var now = DateTime.UtcNow;
            var interval = GetInterval();
            var mapKey = key ?? string.Empty;
            var last = LastSerialized.GetOrAdd(mapKey, DateTime.MinValue);
            if (now - last < interval) return null;
            var updated = LastSerialized.TryUpdate(mapKey, now, last);
            if (!updated)
            {
                last = LastSerialized.GetOrAdd(mapKey, DateTime.MinValue);
                if (now - last < interval) return null;
                LastSerialized[mapKey] = now;
            }
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return null;
            }
        }
    }
}
