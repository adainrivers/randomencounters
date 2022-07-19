using System;
using System.Collections.Concurrent;
using System.Linq;
using ProjectM;
using RandomEncounters.Components;
using RandomEncounters.Configuration;
using RandomEncounters.Models;
using RandomEncounters.Patch;
using RandomEncounters.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using VRising.GameData;
using VRising.GameData.Methods;
using VRising.GameData.Models;
using VRising.GameData.Utils;

namespace RandomEncounters
{
    internal static class Encounters
    {
        public static ConcurrentDictionary<ulong, ConcurrentDictionary<int, ItemDataModel>> RewardsMap = new();
        private static readonly Entity StationEntity = new();
        private static float Lifetime => PluginConfig.EncounterLength.Value;
        private static string MessageTemplate => PluginConfig.EncounterMessageTemplate.Value;

        internal static void Initialize()
        {
            ServerEvents.OnDeath += ServerEvents_OnDeath;
        }

        internal static void Destroy()
        {
            ServerEvents.OnDeath -= ServerEvents_OnDeath;
        }

        private static void ServerEvents_OnDeath(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            foreach (var deathEvent in deathEvents)
            {
                if (!sender.EntityManager.HasComponent<PlayerCharacter>(deathEvent.Killer))
                {
                    continue;
                }

                var playerCharacter = sender.EntityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer);
                var userModel = GameData.Users.GetUserFromEntity(playerCharacter.UserEntity._Entity);


                if (RewardsMap.TryGetValue(userModel.PlatformId, out var bounties) &&
                    bounties.TryGetValue(deathEvent.Died.Index, out var itemModel))
                {
                    var itemGuid = new PrefabGUID(itemModel.Id);
                    if (!userModel.TryGiveItem(new PrefabGUID(itemModel.Id), 1, out _))
                    {
                        userModel.DropItemNearby(itemGuid, 1);
                    }
                    var message = string.Format(PluginConfig.RewardMessageTemplate.Value, itemModel.Color, itemModel.Name);
                    userModel.SendSystemMessage(message);
                    bounties.TryRemove(deathEvent.Died.Index, out _);
                    Logger.LogInfo($"{userModel.CharacterName} earned reward: {itemModel.Name}");
                    var globalMessage = string.Format(PluginConfig.RewardAnnouncementMessageTemplate.Value,
                        userModel.CharacterName, itemModel.Color, itemModel.Name);
                    if (PluginConfig.NotifyAllPlayersAboutRewards.Value)
                    {
                        var onlineUsers = GameData.Users.GetOnlineUsers();
                        foreach (var model in onlineUsers.Where(u => u.PlatformId != userModel.PlatformId))
                        {
                            model.SendSystemMessage(globalMessage);
                        }

                    }
                    else if (PluginConfig.NotifyAdminsAboutEncountersAndRewards.Value)
                    {
                        var onlineAdmins = DataFactory.GetOnlineAdmins(sender.World);
                        foreach (var onlineAdmin in onlineAdmins)
                        {
                            onlineAdmin.SendSystemMessage($"{userModel.CharacterName} earned an encounter reward: <color={itemModel.Color}>{itemModel.Name}</color>");
                        }
                    }
                }
            }
        }

        internal static void StartEncounter(UserModel user = null)
        {
            var world = GameData.World;

            if (user == null)
            {
                var users = GameData.Users.GetOnlineUsers();
                if (PluginConfig.SkipPlayersInCastle.Value)
                {
                    users = users.Where(u => !u.IsInCastle());
                }

                if (PluginConfig.SkipPlayersInCombat.Value)
                {
                    users = users.Where(u => !u.IsInCombat());
                }
                user = users.FirstOrDefault();
            }

            if (user == null)
            {
                Logger.LogMessage("Could not find any eligible players for a random encounter...");
                return;
            }

            var npc = DataFactory.GetRandomNpc(user.Character.Equipment.Level);
            if (npc == null)
            {
                Logger.LogWarning($"Could not find any NPCs within the given level range. (User Level: {user.Character.Equipment.Level})");
                return;
            }
            Logger.LogMessage($"Attempting to start a new encounter for {user.CharacterName} with {npc.Name}");
            var minSpawnDistance = PluginConfig.MinSpawnDistance.Value;
            var maxSpawnDistance = PluginConfig.MaxSpawnDistance.Value;
            world.GetExistingSystem<UnitSpawnerUpdateSystem>().SpawnUnit(StationEntity, new PrefabGUID(npc.Id), user.Position, 1, minSpawnDistance, maxSpawnDistance, Lifetime);
            TaskRunner.Start(taskWorld => AfterSpawn(user.PlatformId, taskWorld, npc), TimeSpan.FromMilliseconds(1000));
        }

        private static object AfterSpawn(ulong userPlatformId, World world, NpcDataModel npcData)
        {

            var user = GameData.Users.GetUserByPlatformId(userPlatformId);
            if (user == null)
            {
                return null;
            }
            Logger.LogDebug($"User is at {user.Position.x} {user.Position.z}");
            var possibleEntitiesQuery =
                world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabGUID>(),
                    ComponentType.ReadOnly<LocalToWorld>(),
                    ComponentType.ReadOnly<AggroConsumer>(),
                    ComponentType.ReadOnly<LifeTime>());

            var foundEntity = possibleEntitiesQuery.AsNpcs()
                .FirstOrDefault(e =>
                    e.PrefabGUID.GuidHash == npcData.Id &&
                    Math.Abs(e.LifeTime - Lifetime) < 0.001 &&
                    !e.IsDead &&
                    e.Position.Distance(user.Position) < 50 + PluginConfig.MaxSpawnDistance.Value);

            if (foundEntity != null)
            {
                if (!RewardsMap.ContainsKey(user.PlatformId))
                {
                    RewardsMap[user.PlatformId] = new ConcurrentDictionary<int, ItemDataModel>();
                }
                var message =
                    string.Format(
                        MessageTemplate,
                        npcData.Name, Lifetime);

                user.SendSystemMessage(message);
                Logger.LogInfo($"Encounters started: {user.CharacterName} vs. {npcData.Name}");

                if (PluginConfig.NotifyAdminsAboutEncountersAndRewards.Value)
                {
                    var onlineAdmins = DataFactory.GetOnlineAdmins(world);
                    foreach (var onlineAdmin in onlineAdmins)
                    {
                        onlineAdmin.SendSystemMessage($"Encounter started: {user.CharacterName} vs. {npcData.Name}");
                    }
                }
                RewardsMap[user.PlatformId][foundEntity.Entity.Index] = DataFactory.GetRandomItem();
            }
            else
            {
                Logger.LogWarning("Could not find the spawned entity.");
            }


            return new object();
        }
    }
}