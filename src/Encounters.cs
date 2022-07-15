using System;
using System.Collections.Concurrent;
using ProjectM;
using ProjectM.Network;
using RandomEncounters.Components;
using RandomEncounters.Configuration;
using RandomEncounters.Models;
using RandomEncounters.Patch;
using RandomEncounters.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace RandomEncounters
{
    internal static class Encounters
    {
        public static ConcurrentDictionary<ulong, ConcurrentDictionary<int, ItemModel>> RewardsMap = new();
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
                var user = sender.EntityManager.GetComponentData<User>(playerCharacter.UserEntity._Entity);


                if (RewardsMap.TryGetValue(user.PlatformId, out var bounties) &&
                    bounties.TryGetValue(deathEvent.Died.Index, out var itemModel))
                {
                    var userModel = UserModel.FromUser(sender.World, user, deathEvent.Killer);
                    UnityUtils.AddItemToInventory(sender.World, userModel, itemModel.Id, 1);
                    var message = string.Format(PluginConfig.RewardMessageTemplate.Value, itemModel.Color, itemModel.Name);
                    ServerChatUtils.SendSystemMessageToClient(sender.EntityManager, user, message);
                    bounties.TryRemove(deathEvent.Died.Index, out _);
                }
            }
        }

        internal static void StartEncounter(World world)
        {
            var user = DataFactory.GetRandomUser(world);
            if (user == null)
            {
                Logger.LogMessage("No one is online...");
                return;
            }

            var npc = DataFactory.GetRandomNpc(user.Level);
            if (npc == null)
            {
                Logger.LogWarning($"Could not find any NPCs within the given level range. (User Level: {user.Level})");
                return;
            }
            Logger.LogMessage($"Attempting to start a new encounter for {user.CharacterName} with {npc.Name}");
            world.GetExistingSystem<UnitSpawnerUpdateSystem>().SpawnUnit(StationEntity, new PrefabGUID(npc.Id), user.LocalToWorld.Position, 1, 2, 4, Lifetime);
            TaskRunner.Start(taskWorld => AfterSpawn(user.PlatformId, taskWorld, npc), TimeSpan.FromMilliseconds(500));
        }

        private static object AfterSpawn(ulong userPlatformId, World world, NpcModel npc)
        {
            var user = DataFactory.GetUserByPlatformId(world, userPlatformId);
            if (user == null)
            {
                return null;
            }
            Logger.LogDebug($"User is at {user.LocalToWorld.Position.x} {user.LocalToWorld.Position.z}");
            var possibleEntitiesQuery =
                world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PrefabGUID>(),
                    ComponentType.ReadOnly<LocalToWorld>(),
                    ComponentType.ReadOnly<AggroConsumer>(),
                    ComponentType.ReadOnly<LifeTime>());
            var possibleEntities = possibleEntitiesQuery.ToEntityArray(Allocator.Temp);
            var foundEntity = Entity.Null;
            foreach (var possibleEntity in possibleEntities)
            {
                var prefabGuid = world.EntityManager.GetComponentData<PrefabGUID>(possibleEntity);
                if (prefabGuid.GuidHash != npc.Id)
                {
                    continue;
                }

                var lifeTime = world.EntityManager.GetComponentData<LifeTime>(possibleEntity);
                if (Math.Abs(lifeTime.Duration - Lifetime) > 0.001)
                {
                    continue;
                }

                if (world.EntityManager.HasComponent<Dead>(possibleEntity))
                {
                    continue;
                }

                var localToWorld = world.EntityManager.GetComponentData<LocalToWorld>(possibleEntity);

                var userPos = user.LocalToWorld.Position;
                var npcPos = localToWorld.Position;

                if (Math.Abs(userPos.x - npcPos.x) < 20 && Math.Abs(userPos.z - npcPos.z) < 20)
                {
                    Logger.LogInfo(
                        $"Found the entity {possibleEntity.Index},{possibleEntity.Version} at {localToWorld.Position.x} {localToWorld.Position.z}");

                    foundEntity = possibleEntity;
                    break;
                }
            }

            if (foundEntity != Entity.Null)
            {
                if (!RewardsMap.ContainsKey(user.PlatformId))
                {
                    RewardsMap[user.PlatformId] = new ConcurrentDictionary<int, ItemModel>();
                }
                var message =
                    string.Format(
                        MessageTemplate,
                        npc.Name, Lifetime);

                ServerChatUtils.SendSystemMessageToClient(world.EntityManager, user.User, message);
                RewardsMap[user.PlatformId][foundEntity.Index] = DataFactory.GetRandomItem();
            }
            else
            {
                Logger.LogWarning("Could not find the spawned entity.");
            }


            return new object();
        }
    }
}
