using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectM.Network;
using RandomEncounters.Configuration;
using RandomEncounters.Models;
using Unity.Collections;
using Unity.Entities;

namespace RandomEncounters.Utils
{
    internal static class DataFactory
    {
        private static readonly Random Random = new();
        private static List<NpcModel> _npcs;
        private static List<ItemModel> _items;

        internal static void Initialize()
        {
            var tsv = Encoding.UTF8.GetString(PluginResources.npcs);
            _npcs = tsv.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(l => new NpcModel(l)).ToList();
            tsv = Encoding.UTF8.GetString(PluginResources.items);
            _items = tsv.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(l => new ItemModel(l)).ToList();
        }

        internal static NpcModel GetRandomNpc(float playerLevel)
        {
            var lowestLevel = playerLevel - PluginConfig.EncounterMaxLevelDifferenceLower.Value;
            var highestLevel = playerLevel + PluginConfig.EncounterMaxLevelDifferenceUpper.Value;
            Logger.LogInfo($"Searching an NPC between levels {lowestLevel} and {highestLevel}");
            return _npcs
                .Where(n => PluginConfig.Npcs.TryGetValue(n.Id, out var npcSetting) && npcSetting.Value && n.Level >= lowestLevel && n.Level <= highestLevel).ToList()
                .GetRandomItem();
        }

        internal static ItemModel GetRandomItem()
        {
            return _items
                .Where(n => PluginConfig.Items.TryGetValue(n.Id, out var itemSetting) && itemSetting.Value).ToList()
                .GetRandomItem();
        }

        internal static UserModel GetRandomUser(World world, bool skipPlayersInCastle = false)
        {
            var users = GetOnlineUsers(world);
            if (skipPlayersInCastle)
            {
                users = users.Where(u => !u.IsInCastle(world)).ToList();
            }
            return users.GetRandomItem();
        }

        internal static int GetOnlineUsersCount(World world)
        {
            return GetOnlineUsers(world).Count;
        }

        internal static List<ItemModel> GetAllItems()
        {
            return _items;
        }

        internal static List<NpcModel> GetAllNpcs()
        {
            return _npcs;
        }

        internal static List<UserModel> GetOnlineadmins(World world)
        {
            return GetOnlineUsers(world).Where(u => u.IsAdmin).ToList();
        }


        internal static UserModel GetUserByPlatformId(World world, ulong platformId)
        {
            var query = world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>());
            var users = query.ToEntityArray(Allocator.Temp);

            foreach (var userEntity in users)
            {
                var user = world.EntityManager.GetComponentData<User>(userEntity);
                if (user.PlatformId == platformId)
                {
                    return UserModel.FromUser(world, user, userEntity);
                }
            }

            return null;
        }


        public static List<UserModel> GetOnlineUsers(World world)
        {
            var query = world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>());
            var users = query.ToEntityArray(Allocator.Temp);

            var result = new List<UserModel>();

            foreach (var userEntity in users)
            {
                var user = world.EntityManager.GetComponentData<User>(userEntity);
                var userModel = UserModel.FromUser(world, user, userEntity);
                if (userModel is { IsConnected: true })
                {
                    result.Add(userModel);
                }
            }

            return result;
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