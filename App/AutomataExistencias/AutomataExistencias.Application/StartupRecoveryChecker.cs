using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Core.Extensions;
using NLog;

namespace AutomataExistencias.Application
{
    public class StartupRecoveryChecker : IStartupRecoveryChecker
    {
        private readonly IItemService _itemService;
        private readonly IStockService _stockService;
        private readonly IPackagingService _packagingService;
        private readonly ITransitOrderService _transitOrderService;
        private readonly IItemByColorService _itemByColorService;
        private readonly IMoneyService _moneyService;
        private readonly IUnitMeasuredService _unitMeasuredService;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;
        private readonly Core.IAutomataState _automataState;
        private readonly INotificationService _notificationService;
        private readonly Core.Configuration.IConfigurator _configurator;
        private readonly Logger _logger;
        private readonly IRecoveryService _recoveryService;

        private readonly IInventoryAutomationConnectionService _inventoryConnectionService;

        public StartupRecoveryChecker(IMoneyService moneyService, IUnitMeasuredService unitMeasuredService, IItemService itemService, IStockService stockService, IPackagingService packagingService,
            ITransitOrderService transitOrderService, IItemByColorService itemByColorService,
            IConnectivityErrorClassifier connectivityErrorClassifier, Core.IAutomataState automataState,
            INotificationService notificationService, Core.Configuration.IConfigurator configurator,
            IInventoryAutomationConnectionService inventoryConnectionService,
            IRecoveryService recoveryService)
        {
            _moneyService = moneyService;
            _unitMeasuredService = unitMeasuredService;
            _itemService = itemService;
            _stockService = stockService;
            _packagingService = packagingService;
            _transitOrderService = transitOrderService;
            _itemByColorService = itemByColorService;
            _connectivityErrorClassifier = connectivityErrorClassifier;
            _automataState = automataState;
            _notificationService = notificationService;
            _configurator = configurator;
            _inventoryConnectionService = inventoryConnectionService;
            _recoveryService = recoveryService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public bool ShouldRunRecoveryOnStartup()
        {
            try
            { 
                var minAttempts = ParseIntOrDefault("ConnectivityError.MinAttempts", 5);
                var percentThreshold = ParseDoubleOrDefault("ConnectivityError.PercentThreshold", 70);

                // REFAC: Strict fallback behavior as requested by operator:
                //  - worldEvents: records from the 7 sync tables that are pending and within the validation window
                //  - worldErrors: subset of worldEvents where Attempts >= maxAttempts and Exception matches connectivity patterns
                //  - return true if worldErrors / worldEvents >= percentThreshold

                var windowMinutes = ParseIntOrDefault("ConnectivityError.WindowMinutes", 15);
                var cutoff = DateTime.UtcNow.AddMinutes(-windowMinutes);
                var maxAttempts = ParseIntOrDefault("SyncAttempts", 2);

                // Helper to check date and attempts using reflection.
                // IMPORTANT: do NOT require Exception here — worldEvents must include all pending records in the window,
                // regardless of whether they currently have an Exception value.
                Func<object, bool> IsWithinWindowAndPending = (obj) =>
                {
                    try
                    {
                        var type = obj.GetType();

                        // fecha property - check a set of likely names. If present and parseable, exclude when outside the window.
                        var dateProp = type.GetProperty("IntegrationDate") ?? type.GetProperty("FechaIntegra") ?? type.GetProperty("FECHA_INTEGRA") ?? type.GetProperty("Fecha_Integra")
                                       ?? type.GetProperty("Date") ?? type.GetProperty("FECHA") ?? type.GetProperty("Fecha")
                                       ?? type.GetProperty("DeliveredDate") ?? type.GetProperty("FECHAESTRECIBO");

                        if (dateProp != null)
                        {
                            var dateVal = dateProp.GetValue(obj);
                            DateTime? parsedUtc = null;

                            if (dateVal is DateTime dt)
                            {
                                // normalize unspecified as Local
                                if (dt.Kind == DateTimeKind.Unspecified)
                                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
                                parsedUtc = dt.ToUniversalTime();
                            }
                            else if (dateVal is string s && !string.IsNullOrWhiteSpace(s))
                            {
                                if (DateTime.TryParse(s, out var parsed))
                                {
                                    if (parsed.Kind == DateTimeKind.Unspecified)
                                        parsed = DateTime.SpecifyKind(parsed, DateTimeKind.Local);
                                    parsedUtc = parsed.ToUniversalTime();
                                }
                                else if (DateTime.TryParse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out parsed))
                                {
                                    parsedUtc = parsed.ToUniversalTime();
                                }
                            }

                            if (parsedUtc.HasValue)
                            {
                                // if the event is older than cutoff, exclude it
                                if (parsedUtc.Value < cutoff)
                                {
                                    try
                                    {
                                        var idProp = type.GetProperty("Id") ?? type.GetProperty("ID");
                                        var idVal = idProp?.GetValue(obj)?.ToString() ?? "<no-id>";
                                        _logger.Debug($"StartupRecoveryChecker: excluding event outside window. Type={type.Name} Id={idVal} DateUtc={parsedUtc.Value:o} Cutoff={cutoff:o}");
                                    }
                                    catch { }
                                    return false;
                                }
                                else
                                {
                                    try
                                    {
                                        var idProp = type.GetProperty("Id") ?? type.GetProperty("ID");
                                        var idVal = idProp?.GetValue(obj)?.ToString() ?? "<no-id>";
                                        _logger.Debug($"StartupRecoveryChecker: including event inside window. Type={type.Name} Id={idVal} DateUtc={parsedUtc.Value:o} Cutoff={cutoff:o}");
                                    }
                                    catch { }
                                }
                            }
                            else
                            {
                                // couldn't parse date string/unknown type -> keep conservative include
                                _logger.Debug($"StartupRecoveryChecker: could not parse date property for type {type.Name}; including conservatively");
                            }
                        }

                        // Attempts/Intentos: include records with attempts <= maxAttempts as pending
                        var attemptsProp = type.GetProperty("Attempts") ?? type.GetProperty("Intentos") ?? type.GetProperty("ATTEMPTS");
                        if (attemptsProp != null)
                        {
                            var attemptsVal = attemptsProp.GetValue(obj);
                            var attempts = 0;
                            if (attemptsVal != null) attempts = Convert.ToInt32(attemptsVal);
                            return attempts <= maxAttempts;
                        }

                        // No attempts property: include conservatively
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"StartupRecoveryChecker.IsWithinWindowAndPending error: {ex}");
                        return true;
                    }
                };

                // gather worldEvents from all tables
                var worldEvents = new System.Collections.Generic.List<string>();
                var worldEventObjects = new System.Collections.Generic.List<object>();

                Action<System.Collections.IEnumerable> collect = (enumerable) =>
                {
                    foreach (var o in enumerable)
                    {
                        if (IsWithinWindowAndPending(o))
                        {
                            worldEventObjects.Add(o);
                        }
                    }
                };

                collect(_itemService.Get());
                collect(_itemByColorService.Get());
                collect(_stockService.Get());
                collect(_transitOrderService.Get());
                collect(_packagingService.Get());
                collect(_unitMeasuredService.Get());
                collect(_moneyService.Get());

                var totalEvents = worldEventObjects.Count;
                if (totalEvents < minAttempts)
                {
                    _logger.Info($"StartupRecoveryChecker: worldEvents {totalEvents} < minAttempts {minAttempts}. No recovery needed.");
                    return false;
                }

                // worldErrors: Attempts >= maxAttempts, Exception present and matches connectivity patterns
                int worldErrors = 0;
                foreach (var ev in worldEventObjects)
                {
                    try
                    {
                        var type = ev.GetType();
                        var exProp = type.GetProperty("Exception");
                        var exText = exProp?.GetValue(ev) as string;
                        if (string.IsNullOrEmpty(exText)) continue;

                        // attempts
                        var attemptsProp = type.GetProperty("Attempts") ?? type.GetProperty("Intentos") ?? type.GetProperty("ATTEMPTS");
                        var attempts = 0;
                        if (attemptsProp != null && attemptsProp.GetValue(ev) != null)
                            attempts = Convert.ToInt32(attemptsProp.GetValue(ev));

                        // worldErrors are those that reached exactly maxAttempts
                        if (attempts != maxAttempts) continue;

                        var parsed = ExtractInnerException(exText);
                        if (_connectivityErrorClassifier.IsDestinationConnectivityError(parsed))
                        {
                            worldErrors++;
                        }
                    }
                    catch { }
                }

                var pct = (worldErrors * 100.0) / totalEvents;
                _logger.Info($"StartupRecoveryChecker: worldEvents={totalEvents}, worldErrors={worldErrors}, pct={pct:0.##}% (threshold={percentThreshold}%)");
                return pct >= percentThreshold;
            }
            catch (Exception ex)
            {
                _logger.Error($"StartupRecoveryChecker error: {ex.ToJson()}");
                // In doubt, do not block startup
                return false;
            }
        }

        public bool TryRunRecoveryOnStartup()
        {
            // Minimal behavior: notify recipients and return false (meaning recovery not completed)
            try
            {
                _logger.Info("StartupRecoveryChecker: triggering notification for startup recovery (background)");

                // Ensure recovery service will run: mark connectivity DOWN so RecoveryService does not skip
                try
                {
                    if (!_automataState.IsDestinationConnectivityDown)
                    {
                        _logger.Info("StartupRecoveryChecker: marking DestinationConnectivityDown=true to allow startup recovery run");
                        _automataState.IsDestinationConnectivityDown = true;
                        if (_automataState.DestinationConnectivityDownSince == null)
                            _automataState.DestinationConnectivityDownSince = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"StartupRecoveryChecker: failed to mark connectivity down before recovery: {ex}");
                }

                // Start background recovery loop so startup is not blocked
                try
                {
                    _logger.Info("StartupRecoveryChecker: starting background recovery task. Service will continue starting in recovery mode.");

                    Task.Factory.StartNew(async () =>
                    {
                        try
                        {
                            var retryInterval = ParseIntOrDefault("Recovery.StartupRetryIntervalSeconds", 60);
                            var maxAttempts = ParseIntOrDefault("Recovery.StartupMaxAttempts", 10);
                            if (retryInterval <= 0) retryInterval = 60;
                            if (maxAttempts < 0) maxAttempts = 10;

                            var attempt = 0;
                            while (maxAttempts == 0 || attempt < maxAttempts)
                            {
                                attempt++;
                                _logger.Info($"StartupRecoveryChecker: background recovery attempt {attempt}");
                                var ok = _recovery_service_try();
                                if (ok)
                                {
                                    _logger.Info($"StartupRecoveryChecker: background recovery succeeded on attempt {attempt}");
                                    break;
                                }
                                else
                                {
                                    _logger.Warn($"StartupRecoveryChecker: background recovery attempt {attempt} failed. Will retry in {retryInterval} seconds");
                                }
                                await Task.Delay(retryInterval * 1000).ConfigureAwait(false);
                            }

                            if (maxAttempts > 0 && attempt >= maxAttempts)
                            {
                                _logger.Warn($"StartupRecoveryChecker: reached max background recovery attempts ({maxAttempts}). Giving up.");
                            }
                        }
                        catch (Exception bgEx)
                        {
                            _logger.Error($"StartupRecoveryChecker: background recovery task threw: {bgEx}");
                        }
                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
                }
                catch (Exception exBg)
                {
                    _logger.Error($"StartupRecoveryChecker: could not start background recovery task: {exBg}");
                    // fallback to synchronous attempt (best-effort)
                    return _recovery_service_try();
                }

                // Do not block startup; return true so Program.Main continues
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"StartupRecoveryChecker TryRunRecoveryOnStartup error: {ex.ToJson()}");
                return false;
            }
        }

        // wrapper to call recovery and surface exceptions cleanly
        private bool _recovery_service_try()
        {
            try
            {
                return _recoveryService.TryRecoverOnce();
            }
            catch (Exception ex)
            {
                _logger.Error($"RecoveryService.TryRecoverOnce threw: {ex}");
                return false;
            }
        }

        // Try to extract InnerException message from serialized JSON exception text
        private string ExtractInnerException(string exText)
        {
            try
            {
                if (string.IsNullOrEmpty(exText)) return exText;
                // crude check for JSON-like content
                if (exText.TrimStart().StartsWith("{") && exText.Contains("InnerException"))
                {
                    // attempt to find "InnerException":"..." pattern
                    var marker = "\"InnerException\":";
                    var idx = exText.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        var start = idx + marker.Length;
                        // skip optional whitespace and opening quote
                        while (start < exText.Length && (exText[start] == ' ' || exText[start] == '\\' || exText[start] == '"')) start++;
                        var end = exText.IndexOf('"', start);
                        if (end > start)
                        {
                            var inner = exText.Substring(start, end - start);
                            return inner;
                        }
                    }
                }
            }
            catch { }
            return exText;
        }

        private int ParseIntOrDefault(string key, int def)
        {
            if (int.TryParse(_configurator.GetKey(key), out var v)) return v;
            return def;
        }

        private double ParseDoubleOrDefault(string key, double def)
        {
            if (double.TryParse(_configurator.GetKey(key), out var v)) return v;
            return def;
        }
    }
}
