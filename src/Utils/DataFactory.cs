using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomEncounters.Configuration;
using RandomEncounters.Models;
using Unity.Entities;
using VRising.GameData;
using VRising.GameData.Models;

namespace RandomEncounters.Utils
{
    internal static class DataFactory
    {
        private static readonly Random Random = new();
        private static List<NpcDataModel> _npcs;
        private static List<ItemDataModel> _items;

        internal static void Initialize()
        {
            var tsv = Encoding.UTF8.GetString(PluginResources.npcs);
            _npcs = tsv.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(l => new NpcDataModel(l)).Where(n => n.NpcModel != null && n.NpcModel.HasDropTable).ToList();
            tsv = Encoding.UTF8.GetString(PluginResources.items);
            _items = tsv.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(l => new ItemDataModel(l)).ToList();
        }

        internal static NpcDataModel GetRandomNpc(float playerLevel)
        {
            var lowestLevel = playerLevel - PluginConfig.EncounterMaxLevelDifferenceLower.Value;
            var highestLevel = playerLevel + PluginConfig.EncounterMaxLevelDifferenceUpper.Value;
            Plugin.Logger.LogInfo($"Searching an NPC between levels {lowestLevel} and {highestLevel}");
            return _npcs
                .Where(n => PluginConfig.Npcs.TryGetValue(n.Id, out var npcSetting) && npcSetting.Value && n.Level >= lowestLevel && n.Level <= highestLevel).ToList()
                .GetRandomItem();
        }

        internal static ItemDataModel GetRandomItem()
        {
            return _items
                .Where(n => PluginConfig.Items.TryGetValue(n.Id, out var itemSetting) && itemSetting.Value > 0).ToList()
                .GetRandomItem();
        }

        internal static int GetOnlineUsersCount()
        {
            return GameData.Users.GetOnlineUsers().Count();
        }

        internal static List<ItemDataModel> GetAllItems()
        {
            return _items;
        }

        internal static List<NpcDataModel> GetAllNpcs()
        {
            return _npcs;
        }

        internal static List<UserModel> GetOnlineAdmins(World world)
        {
            return GameData.Users.GetOnlineUsers().Where(u => u.IsAdmin).ToList();
        }

        private static T GetRandomItem<T>(this List<T> items)
        {
            if (items == null || items.Count == 0)
            {
                return default;
            }

            return items[Random.Next(items.Count)];
        }
    }
}