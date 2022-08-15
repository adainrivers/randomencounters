using System;
using System.Collections.Concurrent;
using System.Linq;
using ProjectM;
using RandomEncounters.Configuration;
using RandomEncounters.Models;
using RandomEncounters.Patch;
using RandomEncounters.Utils;
using Unity.Collections;
using Unity.Entities;
using GT.VRising.GameData;
using GT.VRising.GameData.Methods;
using GT.VRising.GameData.Models;

namespace RandomEncounters
{
    internal static class Encounters
    {
        private static readonly ConcurrentDictionary<ulong, ConcurrentDictionary<int, ItemDataModel>> RewardsMap = new();
        private static readonly ConcurrentDictionary<int, UserModel> NpcPlayerMap = new();

        private static readonly Entity StationEntity = new();
        private static float Lifetime => PluginConfig.EncounterLength.Value;
        private static string MessageTemplate => PluginConfig.EncounterMessageTemplate.Value;

        public static Random Random = new Random();

        internal static void Initialize()
        {
            ServerEvents.OnDeath += ServerEvents_OnDeath;
            ServerEvents.OnUnitSpawned += ServerEvents_OnUnitSpawned;
        }

        internal static void Destroy()
        {
            ServerEvents.OnDeath -= ServerEvents_OnDeath;
            ServerEvents.OnUnitSpawned -= ServerEvents_OnUnitSpawned;
        }

        internal static void StartEncounter(UserModel user = null)
        {
            var world = GameData.World;

            if (user == null)
            {
                var users = GameData.Users.Online;
                if (PluginConfig.SkipPlayersInCastle.Value)
                {
                    users = users.Where(u => !u.IsInCastle());
                }

                if (PluginConfig.SkipPlayersInCombat.Value)
                {
                    users = users.Where(u => !u.IsInCombat());
                }
                user = users.OrderBy(_ => Random.Next()).FirstOrDefault();
            }

            if (user == null)
            {
                Plugin.Logger.LogMessage("Could not find any eligible players for a random encounter...");
                return;
            }

            var npc = DataFactory.GetRandomNpc(user.Character.Equipment.Level);
            if (npc == null)
            {
                Plugin.Logger.LogWarning($"Could not find any NPCs within the given level range. (User Level: {user.Character.Equipment.Level})");
                return;
            }
            Plugin.Logger.LogMessage($"Attempting to start a new encounter for {user.CharacterName} with {npc.Name}");
            var minSpawnDistance = PluginConfig.MinSpawnDistance.Value;
            var maxSpawnDistance = PluginConfig.MaxSpawnDistance.Value;
            try
            {
                //var prefabCollectionSystem = world.GetExistingSystem<PrefabCollectionSystem>();
                //var npcPrefabEntity = prefabCollectionSystem.PrefabLookupMap[new PrefabGUID(npc.Id)];
                //test = world.EntityManager.Instantiate(npcPrefabEntity);
                //world.EntityManager.AddComponent(test, ComponentType.ReadOnly<Age>());
                //world.EntityManager.AddComponent(test, ComponentType.ReadWrite<LifeTime>());
                //world.EntityManager.AddComponent(test, ComponentType.ReadWrite<UnitSpawnHandler>());
                //world.EntityManager.SetComponentData(test, new LifeTime { Duration = Lifetime, EndAction = LifeTimeEndAction.Destroy });
                //world.EntityManager.SetComponentData(test, GameData.Users.GetUserByPlatformId(user.PlatformId).Internals.LocalToWorld.Value);
                //world.EntityManager.SetComponentData(test, new UnitSpawnHandler{StationEntity = StationEntity});

                NpcPlayerMap[npc.Id] = user;
                world.GetExistingSystem<UnitSpawnerUpdateSystem>()
                    .SpawnUnit(StationEntity, new PrefabGUID(npc.Id), user.Position, 1, minSpawnDistance, maxSpawnDistance, Lifetime);
            }
            catch(Exception ex)
            {
                Plugin.Logger.LogError(ex);
                // Suppress
            }
            //TaskRunner.Start(taskWorld => AfterSpawn(user.PlatformId, taskWorld, npc), TimeSpan.FromMilliseconds(1000));
        }

        private static void ServerEvents_OnUnitSpawned(World world, Entity entity)
        {
            var entityManager = world.EntityManager;
            if (!entityManager.HasComponent<PrefabGUID>(entity))
            {
                return;
            }

            var prefabGuid = entityManager.GetComponentData<PrefabGUID>(entity);
            if (!NpcPlayerMap.TryGetValue(prefabGuid.GuidHash, out var user))
            {
                return;
            }
            if (!entityManager.HasComponent<LifeTime>(entity))
            {
                return;
            }
            var lifeTime = entityManager.GetComponentData<LifeTime>(entity);
            if (Math.Abs(lifeTime.Duration - Lifetime) > 0.001)
            {
                return;
            }

            var npcData = DataFactory.GetAllNpcs().FirstOrDefault(n => n.Id == prefabGuid.GuidHash);
            if (npcData == null)
            {
                return;
            }

            NpcPlayerMap.TryRemove(prefabGuid.GuidHash, out _);

            if (!RewardsMap.ContainsKey(user.PlatformId))
            {
                RewardsMap[user.PlatformId] = new ConcurrentDictionary<int, ItemDataModel>();
            }
            var message =
                string.Format(
                    MessageTemplate,
                    npcData.Name, Lifetime);

            user.SendSystemMessage(message);
            Plugin.Logger.LogInfo($"Encounters started: {user.CharacterName} vs. {npcData.Name}");

            if (PluginConfig.NotifyAdminsAboutEncountersAndRewards.Value)
            {
                var onlineAdmins = DataFactory.GetOnlineAdmins(world);
                foreach (var onlineAdmin in onlineAdmins)
                {
                    onlineAdmin.SendSystemMessage($"Encounter started: {user.CharacterName} vs. {npcData.Name}");
                }
            }
            RewardsMap[user.PlatformId][entity.Index] = DataFactory.GetRandomItem();
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
                var userModel = GameData.Users.FromEntity(playerCharacter.UserEntity._Entity);


                if (RewardsMap.TryGetValue(userModel.PlatformId, out var bounties) &&
                    bounties.TryGetValue(deathEvent.Died.Index, out var itemModel))
                {
                    var itemGuid = new PrefabGUID(itemModel.Id);
                    var quantity = PluginConfig.Items[itemModel.Id];
                    if (!userModel.TryGiveItem(new PrefabGUID(itemModel.Id), quantity.Value, out _))
                    {
                        userModel.DropItemNearby(itemGuid, quantity.Value);
                    }
                    var message = string.Format(PluginConfig.RewardMessageTemplate.Value, itemModel.Color, itemModel.Name);
                    userModel.SendSystemMessage(message);
                    bounties.TryRemove(deathEvent.Died.Index, out _);
                    Plugin.Logger.LogInfo($"{userModel.CharacterName} earned reward: {itemModel.Name}");
                    var globalMessage = string.Format(PluginConfig.RewardAnnouncementMessageTemplate.Value,
                        userModel.CharacterName, itemModel.Color, itemModel.Name);
                    if (PluginConfig.NotifyAllPlayersAboutRewards.Value)
                    {
                        var onlineUsers = GameData.Users.Online;
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
    }
}