﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using RandomEncounters.Utils;

namespace RandomEncounters.Configuration
{
    internal class PluginConfig
    {
        private static ConfigFile _mainConfig;
        private static ConfigFile _npcsConfig;
        private static ConfigFile _itemsConfig;

        public static ConfigEntry<bool> Enabled { get; private set; }
        public static ConfigEntry<bool> SkipPlayersInCastle { get; private set; }
        public static ConfigEntry<int> EncounterTimerMin { get; private set; }
        public static ConfigEntry<int> EncounterTimerMax { get; private set; }
        public static ConfigEntry<int> EncounterLength { get; private set; }
        public static ConfigEntry<int> EncounterMaxLevelDifferenceLower { get; private set; }
        public static ConfigEntry<int> EncounterMaxLevelDifferenceUpper { get; private set; }
        public static ConfigEntry<string> EncounterMessageTemplate { get; private set; }
        public static ConfigEntry<string> RewardMessageTemplate { get; private set; }
        public static ConfigEntry<string> RewardAnnouncementMessageTemplate { get; private set; }
        public static ConfigEntry<bool> NotifyAdminsAboutEncountersAndRewards { get; private set; }
        public static ConfigEntry<bool> NotifyAllPlayersAboutRewards { get; private set; }
        public static ConfigEntry<int> MinSpawnDistance { get; private set; }
        public static ConfigEntry<int> MaxSpawnDistance { get; private set; }
        public static ConfigEntry<bool> SkipPlayersInCombat { get; private set; }

        public static Dictionary<int, ConfigEntry<bool>> Npcs = new();
        public static Dictionary<int, ConfigEntry<bool>> Items = new();

        public static void Initialize()
        {
            var bepInExConfigFolder = BepInEx.Paths.ConfigPath ?? Path.Combine("BepInEx", "config");
            var configFolder = Path.Combine(bepInExConfigFolder, Plugin.PluginName);
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            var mainConfigFilePath = Path.Combine(configFolder, "Main.cfg");
            _mainConfig = File.Exists(mainConfigFilePath) ? new ConfigFile(mainConfigFilePath, false) : new ConfigFile(mainConfigFilePath, true);
            var npcsConfigFilePath = Path.Combine(configFolder, "NPCs.cfg");
            _npcsConfig = File.Exists(npcsConfigFilePath) ? new ConfigFile(npcsConfigFilePath, false) : new ConfigFile(npcsConfigFilePath, true);
            var itemsConfigFilePath = Path.Combine(configFolder, "Items.cfg");
            _itemsConfig = File.Exists(itemsConfigFilePath) ? new ConfigFile(itemsConfigFilePath, false) : new ConfigFile(itemsConfigFilePath, true);

            Enabled = _mainConfig.Bind("Main", "Enabled", true, "Determines whether the random encounter timer is enabled or not.");
            SkipPlayersInCastle = _mainConfig.Bind("Main", "SkipPlayersInCastle", true, "When enabled, players who are in a castle are excluded from encounters");
            SkipPlayersInCombat = _mainConfig.Bind("Main", "SkipPlayersInCombat", false, "When enabled, players who are in combat are excluded from the random encounters.");
            EncounterTimerMin = _mainConfig.Bind("Main", "EncounterTimerMin", 1200, "Minimum seconds before a new encounter is initiated. This value is divided by the online users count.");
            EncounterTimerMax = _mainConfig.Bind("Main", "EncounterTimerMax", 2400, "Maximum seconds before a new encounter is initiated. This value is divided by the online users count.");
            EncounterLength = _mainConfig.Bind("Main", "EncounterLength", 120, "Maximum seconds until the player can kill the NPC for a reward.");
            EncounterMaxLevelDifferenceLower = _mainConfig.Bind("Main", "EncounterMinLevelDifference", 99, "The lower value for the NPC level - Player level difference. For example, if player level is 50, and this setting is 20, then the lowest level of the spawned NPC would be 30.");
            EncounterMaxLevelDifferenceUpper = _mainConfig.Bind("Main", "EncounterMaxLevelDifference", 0, "The upper value for the NPC level - Player level difference.  For example, if player level is 50, and this setting is 20, then the highest level of the spawned NPC would be 70.");
            EncounterMessageTemplate = _mainConfig.Bind("Main", "EncounterMessageTemplate", "You have encountered a <color=#daa520>{0}</color>. You have <color=#daa520>{1}</color> seconds to kill it for a chance of a random reward.", "System message template for the encounter.");
            RewardMessageTemplate = _mainConfig.Bind("Main", "RewardMessageTemplate", "Congratulations. Your reward: <color={0}>{1}</color>.", "System message template for the reward.");
            RewardAnnouncementMessageTemplate = _mainConfig.Bind("Main", "RewardAnnouncementMessageTemplate", "{0} earned an encounter reward: <color={1}>{2}</color>.", "System message template for the reward announcement.");
            NotifyAdminsAboutEncountersAndRewards = _mainConfig.Bind("Main", "NotifyAdminsAboutEncountersAndRewards", true, "If enabled, all online admins are notified about encounters and rewards.");
            NotifyAllPlayersAboutRewards = _mainConfig.Bind("Main", "NotifyAllPlayersAboutRewards", false, "When enabled, all online players are notified about any player's rewards.");
            MinSpawnDistance = _mainConfig.Bind("Main", "MinSpawnDistance", 2, "Minimum spawn distance for the spawned unit.");
            MaxSpawnDistance = _mainConfig.Bind("Main", "MaxSpawnDistance", 4, "Maximum spawn distance for the spawned unit.");
            foreach (var npcModel in DataFactory.GetAllNpcs().OrderBy(i => i.Name))
            {
                Npcs[npcModel.Id] = _npcsConfig.Bind("NPCs", npcModel.PrefabName, true,
                    $"{npcModel.Name} - {(npcModel.BloodType == string.Empty ? "None" : npcModel.BloodType)} (Level: {npcModel.Level}) https://gaming.tools/v-rising/npcs/{npcModel.PrefabName.ToLowerInvariant()}");
            }

            foreach (var itemModel in DataFactory.GetAllItems().OrderBy(i => i.Name))
            {
                Items[itemModel.Id] = _itemsConfig.Bind("Items", itemModel.PrefabName, true,
                    $"{itemModel.Name} https://gaming.tools/v-rising/items/{itemModel.PrefabName.ToLowerInvariant()}");
            }
        }

        public static void Destroy()
        {
            _mainConfig.Clear();
            _npcsConfig.Clear();
            _itemsConfig.Clear();
        }

        private static string CleanupName(string name)
        {
            var rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(name, "");
        }

    }
}
