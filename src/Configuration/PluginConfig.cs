﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using RandomEncounters.Models;
using RandomEncounters.Utils;

namespace RandomEncounters.Configuration
{
    internal class PluginConfig
    {
        private static ConfigFile _mainConfig;
        private static ConfigFile _npcsConfig;
        private static ConfigFile _itemsConfig;

        public static ConfigEntry<bool> Enabled { get; private set; }
        public static ConfigEntry<int> EncounterTimerMin { get; private set; }
        public static ConfigEntry<int> EncounterTimerMax { get; private set; }
        public static ConfigEntry<int> EncounterLength { get; private set; }
        public static ConfigEntry<int> EncounterMaxLevelDifferenceLower { get; private set; }
        public static ConfigEntry<int> EncounterMaxLevelDifferenceUpper { get; private set; }
        public static ConfigEntry<string> EncounterMessageTemplate { get; private set; }
        public static ConfigEntry<string> RewardMessageTemplate { get; private set; }

        public static Dictionary<int, ConfigEntry<bool>> Npcs = new();
        public static Dictionary<int, ConfigEntry<bool>> Items = new();

        public static void Initialize()
        {
            var configFolder = Path.Combine("BepInEx", "config", Plugin.PluginName);
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

            Enabled = _mainConfig.Bind("Main", "Enabled", true, "Determines whether the random encounters are enabled or not.");
            EncounterTimerMin = _mainConfig.Bind("Main", "EncounterTimerMin", 120, "Minimum seconds before a new encounter is initiated.");
            EncounterTimerMax = _mainConfig.Bind("Main", "EncounterTimerMax", 240, "Maximum seconds before a new encounter is initiated.");
            EncounterLength = _mainConfig.Bind("Main", "EncounterLength", 120, "Maximum seconds until the player can kill the NPC for a reward.");
            EncounterMaxLevelDifferenceLower = _mainConfig.Bind("Main", "EncounterMinLevelDifference", 99, "The lower value for the NPC level - Player level difference.");
            EncounterMaxLevelDifferenceUpper = _mainConfig.Bind("Main", "EncounterMaxLevelDifference", 0, "The upper value for the NPC level - Player level difference.");
            EncounterMessageTemplate = _mainConfig.Bind("Main", "EncounterMessageTemplate", "You have encountered a <color=#daa520>{0}</color>. You have <color=#daa520>{1}</color> seconds to kill it for a chance of a random reward.", "System message template for the encounter.");
            RewardMessageTemplate = _mainConfig.Bind("Main", "RewardMessageTemplate", "Congratulations. Your reward: <color={0}>{1}</color>.", "System message template for the reward.");
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