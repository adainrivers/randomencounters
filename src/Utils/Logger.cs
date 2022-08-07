using System;
using BepInEx.Logging;

namespace RandomEncounters.Utils
{
    internal class Logger
    {
        private readonly ManualLogSource _logger;

        internal Logger(ManualLogSource logger)
        {
            _logger = logger;
        }

        internal void Log(LogLevel logLevel, string message)
        {
            _logger.Log(logLevel, $"[{Plugin.PluginVersion}] {message}");
        }

        internal void LogInfo(string message) => Log(LogLevel.Info, message);
        internal void LogWarning(string message) =>  Log(LogLevel.Warning, message);
        internal void LogDebug(string message) =>  Log(LogLevel.Debug, message);
        internal void LogFatal(string message) =>  Log(LogLevel.Fatal, message);
        internal void LogError(string message) =>  Log(LogLevel.Error, message);
        internal void LogError(Exception exception) =>  Log(LogLevel.Error, exception.ToString());
        internal void LogMessage(string message) =>  Log(LogLevel.Message, message);
    }
}