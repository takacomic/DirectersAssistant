using MelonLoader;
using System;
using System.Reflection;

namespace Directers_Assistant.src.Logging
{
    public enum SurvivorLogLevel
    {
        Trace = 0,
        Debug = 1,
        Verbose = 2,
        Msg = 3,
        Warning = 4,
        Error = 5,
        Critical = 6
    }

    internal class SurvivorLoggerAdapter
    {
        private static SurvivorLoggerAdapter? _instance;
        private static readonly object LockObj = new();

        private readonly MelonLogger.Instance _melonLogger;
        private object? _survivorLogInstance;
        private Type? _survivorLogType;
        private MethodInfo? _msgMethod;
        private MethodInfo? _msgScopedMethod;
        private MethodInfo? _errorMethod;
        private MethodInfo? _errorScopedMethod;
        private MethodInfo? _warningMethod;
        private MethodInfo? _warningScopedMethod;
        private MethodInfo? _debugMethod;
        private MethodInfo? _debugScopedMethod;
        private MethodInfo? _traceMethod;
        private MethodInfo? _traceScopedMethod;
        private MethodInfo? _verboseMethod;
        private MethodInfo? _verboseScopedMethod;
        private MethodInfo? _criticalMethod;
        private MethodInfo? _criticalScopedMethod;
        private MethodInfo? _exceptionMethod;
        private MethodInfo? _logThrottledMethod;
        private MethodInfo? _msgThrottledMethod;
        private MethodInfo? _warningThrottledMethod;
        private MethodInfo? _errorThrottledMethod;
        private MethodInfo? _isLevelEnabledMethod;
        private MethodInfo? _setLevelEnabledMethod;

        public bool IsSurvivorLoggerAvailable { get; private set; }

        private SurvivorLoggerAdapter()
        {
            _melonLogger = Melon<DirecterAssistantMod>.Logger;
        }

        public static SurvivorLoggerAdapter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        _instance ??= new SurvivorLoggerAdapter();
                    }
                }
                return _instance;
            }
        }

        public void Initialize()
        {
            DetectSurvivorLogger();
        }

        private void DetectSurvivorLogger()
        {
            try
            {
                foreach (MelonMod mod in MelonMod.RegisteredMelons)
                {
                    if (mod == null) continue;
                    if (mod.GetType().Name == "SurvivorLoggerMod" ||
                        mod.GetType().FullName?.Contains("SurvivorLogger") == true)
                    {
                        Type? survivorLogType = mod.GetType().Assembly.GetType("SurvivorLogger.SurvivorLog");
                        if (survivorLogType != null)
                        {
                            Type? genericLogType = survivorLogType.MakeGenericType(typeof(DirecterAssistantMod));
                            PropertyInfo? instanceProperty = genericLogType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                            if (instanceProperty != null)
                            {
                                _survivorLogInstance = instanceProperty.GetValue(null);
                                _survivorLogType = genericLogType;
                                CacheSurvivorLogMethods();
                                IsSurvivorLoggerAvailable = true;
                                _melonLogger.Msg("SurvivorLogger detected and integrated");
                                return;
                            }
                        }
                    }
                }

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type? survivorLogType = assembly.GetType("SurvivorLogger.SurvivorLog");
                    if (survivorLogType != null)
                    {
                        Type? genericLogType = survivorLogType.MakeGenericType(typeof(DirecterAssistantMod));
                        PropertyInfo? instanceProperty = genericLogType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                        if (instanceProperty != null)
                        {
                            _survivorLogInstance = instanceProperty.GetValue(null);
                            _survivorLogType = genericLogType;
                            CacheSurvivorLogMethods();
                            IsSurvivorLoggerAvailable = true;
                            _melonLogger.Msg("SurvivorLogger detected and integrated");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _melonLogger.Warning($"Error detecting SurvivorLogger: {ex.Message}");
            }

            IsSurvivorLoggerAvailable = false;
        }

        private void CacheSurvivorLogMethods()
        {
            if (_survivorLogType == null) return;

            _msgMethod = _survivorLogType.GetMethod("Msg", new[] { typeof(string) });
            _msgScopedMethod = _survivorLogType.GetMethod("Msg", new[] { typeof(string), typeof(string) });
            _errorMethod = _survivorLogType.GetMethod("Error", new[] { typeof(string) });
            _errorScopedMethod = _survivorLogType.GetMethod("Error", new[] { typeof(string), typeof(string) });
            _warningMethod = _survivorLogType.GetMethod("Warning", new[] { typeof(string) });
            _warningScopedMethod = _survivorLogType.GetMethod("Warning", new[] { typeof(string), typeof(string) });
            _debugMethod = _survivorLogType.GetMethod("Debug", new[] { typeof(string) });
            _debugScopedMethod = _survivorLogType.GetMethod("Debug", new[] { typeof(string), typeof(string) });
            _traceMethod = _survivorLogType.GetMethod("Trace", new[] { typeof(string) });
            _traceScopedMethod = _survivorLogType.GetMethod("Trace", new[] { typeof(string), typeof(string) });
            _verboseMethod = _survivorLogType.GetMethod("Verbose", new[] { typeof(string) });
            _verboseScopedMethod = _survivorLogType.GetMethod("Verbose", new[] { typeof(string), typeof(string) });
            _criticalMethod = _survivorLogType.GetMethod("Critical", new[] { typeof(string) });
            _criticalScopedMethod = _survivorLogType.GetMethod("Critical", new[] { typeof(string), typeof(string) });
            _exceptionMethod = _survivorLogType.GetMethod("Exception", new[] { typeof(Exception), typeof(string) });

            _logThrottledMethod = _survivorLogType.GetMethod("LogThrottled");
            _msgThrottledMethod = _survivorLogType.GetMethod("MsgThrottled");
            _warningThrottledMethod = _survivorLogType.GetMethod("WarningThrottled");
            _errorThrottledMethod = _survivorLogType.GetMethod("ErrorThrottled");

            _isLevelEnabledMethod = _survivorLogType.GetMethod("IsLevelEnabled");
            _setLevelEnabledMethod = _survivorLogType.GetMethod("SetLevelEnabled");
        }

        private void InvokeSurvivorLog(MethodInfo? method, params object?[] parameters)
        {
            if (_survivorLogInstance != null && method != null)
            {
                try
                {
                    method.Invoke(_survivorLogInstance, parameters);
                    return;
                }
                catch { }
            }
        }

        private bool InvokeSurvivorLogBool(MethodInfo? method, params object?[] parameters)
        {
            if (_survivorLogInstance != null && method != null)
            {
                try
                {
                    object? result = method.Invoke(_survivorLogInstance, parameters);
                    if (result is bool boolResult)
                    {
                        return boolResult;
                    }
                }
                catch { }
            }
            return false;
        }

        public void Msg(string message)
        {
            if (IsSurvivorLoggerAvailable)
            {
                InvokeSurvivorLog(_msgMethod, message);
            }
            else
            {
                _melonLogger.Msg(message);
            }
        }

        public void Msg(string scope, string message)
        {
            if (IsSurvivorLoggerAvailable && _msgScopedMethod != null)
            {
                InvokeSurvivorLog(_msgScopedMethod, scope, message);
            }
            else
            {
                _melonLogger.Msg($"[{scope}] {message}");
            }
        }

        public void Error(string message)
        {
            if (IsSurvivorLoggerAvailable)
            {
                InvokeSurvivorLog(_errorMethod, message);
            }
            else
            {
                _melonLogger.Error(message);
            }
        }

        public void Error(string scope, string message)
        {
            if (IsSurvivorLoggerAvailable && _errorScopedMethod != null)
            {
                InvokeSurvivorLog(_errorScopedMethod, scope, message);
            }
            else
            {
                _melonLogger.Error($"[{scope}] {message}");
            }
        }

        public void Warning(string message)
        {
            if (IsSurvivorLoggerAvailable)
            {
                InvokeSurvivorLog(_warningMethod, message);
            }
            else
            {
                _melonLogger.Warning(message);
            }
        }

        public void Warning(string scope, string message)
        {
            if (IsSurvivorLoggerAvailable && _warningScopedMethod != null)
            {
                InvokeSurvivorLog(_warningScopedMethod, scope, message);
            }
            else
            {
                _melonLogger.Warning($"[{scope}] {message}");
            }
        }

        public void Debug(string message)
        {
            if (IsSurvivorLoggerAvailable)
            {
                InvokeSurvivorLog(_debugMethod, message);
            }
            else
            {
                _melonLogger.Msg($"[DEBUG] {message}");
            }
        }

        public void Debug(string scope, string message)
        {
            if (IsSurvivorLoggerAvailable && _debugScopedMethod != null)
            {
                InvokeSurvivorLog(_debugScopedMethod, scope, message);
            }
            else
            {
                _melonLogger.Msg($"[DEBUG][{scope}] {message}");
            }
        }

        public void Trace(string message)
        {
            if (IsSurvivorLoggerAvailable)
            {
                InvokeSurvivorLog(_traceMethod, message);
            }
            else
            {
                _melonLogger.Msg($"[TRACE] {message}");
            }
        }

        public void Trace(string scope, string message)
        {
            if (IsSurvivorLoggerAvailable && _traceScopedMethod != null)
            {
                InvokeSurvivorLog(_traceScopedMethod, scope, message);
            }
            else
            {
                _melonLogger.Msg($"[TRACE][{scope}] {message}");
            }
        }

        public void Verbose(string message)
        {
            if (IsSurvivorLoggerAvailable)
            {
                InvokeSurvivorLog(_verboseMethod, message);
            }
            else
            {
                _melonLogger.Msg($"[VERBOSE] {message}");
            }
        }

        public void Verbose(string scope, string message)
        {
            if (IsSurvivorLoggerAvailable && _verboseScopedMethod != null)
            {
                InvokeSurvivorLog(_verboseScopedMethod, scope, message);
            }
            else
            {
                _melonLogger.Msg($"[VERBOSE][{scope}] {message}");
            }
        }

        public void Critical(string message)
        {
            if (IsSurvivorLoggerAvailable)
            {
                InvokeSurvivorLog(_criticalMethod, message);
            }
            else
            {
                _melonLogger.Error($"[CRITICAL] {message}");
            }
        }

        public void Critical(string scope, string message)
        {
            if (IsSurvivorLoggerAvailable && _criticalScopedMethod != null)
            {
                InvokeSurvivorLog(_criticalScopedMethod, scope, message);
            }
            else
            {
                _melonLogger.Error($"[CRITICAL][{scope}] {message}");
            }
        }

        public void Exception(Exception ex, string? message = null)
        {
            if (IsSurvivorLoggerAvailable && _exceptionMethod != null)
            {
                InvokeSurvivorLog(_exceptionMethod, ex, message);
            }
            else
            {
                if (message != null)
                {
                    _melonLogger.Error($"{message}: {ex.Message}");
                }
                else
                {
                    _melonLogger.Error(ex.Message);
                }
            }
        }

        public void LogThrottled(SurvivorLogLevel level, string message, string? scope = null, string? throttlingKey = null)
        {
            if (IsSurvivorLoggerAvailable && _logThrottledMethod != null)
            {
                InvokeSurvivorLog(_logThrottledMethod, level, message, scope, throttlingKey);
            }
            else
            {
                string prefix = scope != null ? $"[{scope}] " : "";
                switch (level)
                {
                    case SurvivorLogLevel.Error:
                        _melonLogger.Error($"[Throttled] {prefix}{message}");
                        break;
                    case SurvivorLogLevel.Warning:
                        _melonLogger.Warning($"[Throttled] {prefix}{message}");
                        break;
                    default:
                        _melonLogger.Msg($"[Throttled] {prefix}{message}");
                        break;
                }
            }
        }

        public void MsgThrottled(string message, string? scope = null, string? throttlingKey = null)
        {
            if (IsSurvivorLoggerAvailable && _msgThrottledMethod != null)
            {
                InvokeSurvivorLog(_msgThrottledMethod, message, scope, throttlingKey);
            }
            else
            {
                string prefix = scope != null ? $"[{scope}] " : "";
                _melonLogger.Msg($"[Throttled] {prefix}{message}");
            }
        }

        public void WarningThrottled(string message, string? scope = null, string? throttlingKey = null)
        {
            if (IsSurvivorLoggerAvailable && _warningThrottledMethod != null)
            {
                InvokeSurvivorLog(_warningThrottledMethod, message, scope, throttlingKey);
            }
            else
            {
                string prefix = scope != null ? $"[{scope}] " : "";
                _melonLogger.Warning($"[Throttled] {prefix}{message}");
            }
        }

        public void ErrorThrottled(string message, string? scope = null, string? throttlingKey = null)
        {
            if (IsSurvivorLoggerAvailable && _errorThrottledMethod != null)
            {
                InvokeSurvivorLog(_errorThrottledMethod, message, scope, throttlingKey);
            }
            else
            {
                string prefix = scope != null ? $"[{scope}] " : "";
                _melonLogger.Error($"[Throttled] {prefix}{message}");
            }
        }

        public bool IsLevelEnabled(SurvivorLogLevel level)
        {
            if (IsSurvivorLoggerAvailable && _isLevelEnabledMethod != null)
            {
                return InvokeSurvivorLogBool(_isLevelEnabledMethod, level);
            }
            return true;
        }

        public void SetLevelEnabled(SurvivorLogLevel level, bool enabled)
        {
            if (IsSurvivorLoggerAvailable && _setLevelEnabledMethod != null)
            {
                InvokeSurvivorLog(_setLevelEnabledMethod, level, enabled);
            }
        }
    }
}
