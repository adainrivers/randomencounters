﻿using System;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using RandomEncounters.Components;
using RandomEncounters.Configuration;
using RandomEncounters.Patch;
using RandomEncounters.Utils;
using Wetstone.API;
using Wetstone.Hooks;

namespace RandomEncounters
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    [Wetstone.API.Reloadable]
    public class Plugin : BasePlugin
    {
        public const string PluginGuid = "gamingtools.RandomEncounters";
        public const string PluginName = "RandomEncounters";
        public const string PluginVersion = "0.5.3";

        internal static ManualLogSource Logger { get; private set; }


        private static Harmony _harmonyInstance;
        private static Timer _encounterTimer;

        public override void Load()
        {
            Logger = Log;

            Logger.LogDebug("Loading main data");
            DataFactory.Initialize();
            Logger.LogDebug("Binding configuration");
            PluginConfig.Initialize();

            TaskRunner.Initialize();
            Encounters.Initialize();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            _harmonyInstance = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmonyInstance.PatchAll(typeof(ServerEvents));
            _encounterTimer = new Timer();
            if (PluginConfig.Enabled.Value)
            {
                _encounterTimer.Start(
                    world =>
                    {
                        Logger.LogInfo("Starting an encounter.");
                        Encounters.StartEncounter(world);
                    }, 
                    input =>
                    {
                        if (input is not int onlineUsersCount)
                        {
                            Logger.LogError("Encounter timer delay function parameter is not a valid integer");
                            return TimeSpan.MaxValue;
                        }
                        if (onlineUsersCount < 1)
                        {
                            onlineUsersCount = 1;
                        }
                        var seconds =  new Random().Next(PluginConfig.EncounterTimerMin.Value, PluginConfig.EncounterTimerMax.Value);
                        Logger.LogInfo($"Next encounter will start in {seconds} seconds.");
                        return TimeSpan.FromSeconds(seconds) / onlineUsersCount;
                    });
            }

            Chat.OnChatMessage += Chat_OnChatMessage;
        }

        private void Chat_OnChatMessage(VChatEvent e)
        {
            if (e.Message.Equals("!randomencounter", StringComparison.OrdinalIgnoreCase) && e.User.IsAdmin)
            {
                Encounters.StartEncounter(VWorld.Server);
            }
        }

        public override bool Unload()
        {
            Chat.OnChatMessage-= Chat_OnChatMessage;
            Config.Clear();
            _encounterTimer.Stop();
            TaskRunner.Destroy();
            Encounters.Destroy();
            _harmonyInstance.UnpatchSelf();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is unloaded!");
            return true;
        }
    }
}
