using BepInEx.Logging;

namespace RandomEncounters.Utils
{
    internal static class Logger
    {
        internal static void LogInfo(object data) => Plugin.Logger.LogInfo(data);
        internal static void LogWarning(object data) => Plugin.Logger.LogWarning(data);
        internal static void Log(LogLevel logLevel, object data) => Plugin.Logger.Log(logLevel, data);
        internal static void LogDebug(object data) => Plugin.Logger.LogDebug(data);
        internal static void LogFatal(object data) => Plugin.Logger.LogFatal(data);
        internal static void LogError(object data) => Plugin.Logger.LogError(data);
        internal static void LogMessage(object data) => Plugin.Logger.LogMessage(data);
    }
}