using System;
using System.Linq;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using RandomEncounters.Components;
using RandomEncounters.Configuration;
using RandomEncounters.Patch;
using RandomEncounters.Utils;
using Unity.Entities;
using GT.VRising.GameData;
using GT.VRising.GameData.Methods;
using Wetstone.Hooks;
using Logger = RandomEncounters.Utils.Logger;

namespace RandomEncounters
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    [BepInDependency("GT.VRising.GameData")]
    public class Plugin : BasePlugin
    {
        public const string PluginGuid = "gamingtools.RandomEncounters";
        public const string PluginName = "RandomEncounters";
        public const string PluginVersion = "0.8.3";

        internal static Logger Logger { get; private set; }
        internal static Plugin Instance { get; private set; }

        private static Harmony _harmonyInstance;
        private static Timer _encounterTimer;

        public override void Load()
        {
            if (GameData.IsClient)
            {
                Logger.LogError($"{PluginName} is only compatible with V Rising Server.");
                return;
            }
            Instance = this;
            Logger = new Logger(Log);

            // Plugin startup logic
            _harmonyInstance = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmonyInstance.PatchAll(typeof(ServerEvents));

            Chat.OnChatMessage += Chat_OnChatMessage;
            GameData.OnInitialize += GameData_OnInitialize;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private static void GameData_OnInitialize(World world)
        {
            Logger.LogInfo("Loading main data");
            DataFactory.Initialize();
            Logger.LogInfo("Binding configuration");
            PluginConfig.Initialize();

            Encounters.Initialize();

            _encounterTimer = new Timer();
            if (PluginConfig.Enabled.Value)
            {
                StartEncounterTimer();
            }
        }

        private static void StartEncounterTimer()
        {
            _encounterTimer.Start(
                _ =>
                {
                    Logger.LogInfo("Starting an encounter.");
                    Encounters.StartEncounter();
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
                    var seconds = new Random().Next(PluginConfig.EncounterTimerMin.Value, PluginConfig.EncounterTimerMax.Value);
                    Logger.LogInfo($"Next encounter will start in {seconds / onlineUsersCount} seconds.");
                    return TimeSpan.FromSeconds(seconds) / onlineUsersCount;
                });
        }

        internal static void Chat_OnChatMessage(VChatEvent e)
        {
            var message = e.Message.Trim().ToLowerInvariant();

            if (!e.User.IsAdmin)
            {
                return;
            }
            if (!message.StartsWith("!randomencounter") && !message.StartsWith("!re"))
            {
                return;
            }

            var senderModel = GameData.Users.FromEntity(e.SenderUserEntity);
            var command = message.Replace("!re", string.Empty).Replace("!randomencounter", string.Empty);
            switch (command)
            {
                case "":
                case " start":
                    Encounters.StartEncounter();
                    return;
                case " me":
                    Logger.LogWarning($"{senderModel.CharacterName} InCombat: {senderModel.IsInCombat()}");
                    Encounters.StartEncounter(senderModel);
                    break;
                case " disable":
                    if (!PluginConfig.Enabled.Value)
                    {
                        senderModel.SendSystemMessage("Already disabled.");
                        return;
                    }
                    PluginConfig.Enabled.Value = false;
                    _encounterTimer.Stop();
                    senderModel.SendSystemMessage("Disabled.");
                    break;
                case " enable":
                    if (PluginConfig.Enabled.Value)
                    {
                        senderModel.SendSystemMessage("Already enabled.");
                        return;
                    }
                    PluginConfig.Enabled.Value = true;
                    StartEncounterTimer();
                    senderModel.SendSystemMessage("Enabled.");
                    break;
                case " reload":
                    var currentStatus = PluginConfig.Enabled.Value;
                    PluginConfig.Initialize();
                    if (!currentStatus && PluginConfig.Enabled.Value)
                    {
                        StartEncounterTimer();
                    }

                    if (!PluginConfig.Enabled.Value)
                    {
                        _encounterTimer.Stop();
                    }
                    senderModel.SendSystemMessage("Reloaded configuration");
                    break;
                default:
                    if (!command.StartsWith(" "))
                    {
                        return;
                    }

                    command = command.Trim();
                    var onlineUsers = GameData.Users.Online;
                    var foundUser = onlineUsers.FirstOrDefault(u =>
                        u.CharacterName.Equals(command, StringComparison.OrdinalIgnoreCase));

                    if (foundUser != null)
                    {
                        Encounters.StartEncounter(foundUser);
                    }
                    else
                    {
                        senderModel.SendSystemMessage($"Could not find an online player with name {message}");
                    }
                    break;
            }
        }

        public override bool Unload()
        {
            GameData.OnInitialize -= GameData_OnInitialize;
            Chat.OnChatMessage-= Chat_OnChatMessage;
            Config.Clear();
            _encounterTimer?.Stop();
            Encounters.Destroy();
            _harmonyInstance?.UnpatchSelf();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is unloaded!");
            return true;
        }


    }
}