using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.Methods;
using BloodyEncounters.Data;
using ProjectM;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;

namespace BloodyEncounters.EventsHandler
{
    internal class OnDeath
    {
        public static void OnDeathEvent(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;

            foreach (var deathEvent in deathEvents)
            {
                if (!deathEvent.Killer.Has<PlayerCharacter>() || !deathEvent.Died.Has<NameableInteractable>())
                {
                    continue;
                }

                var playerCharacter = sender.EntityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer);
                var userModel = GameData.Users.FromEntity(playerCharacter.UserEntity);
                var npcGUID = deathEvent.Died.Read<PrefabGUID>();
                var npc = _prefabCollectionSystem._PrefabDataLookup[npcGUID].AssetName;

                var modelNpc = Database.NPCS.Where(x => x.AssetName == npc.ToString()).FirstOrDefault();

                if (modelNpc == null)
                {
                    EncounterSystem.EncounterStarted = false;
                    continue;
                }

                if (!modelNpc.GetNPCEntity())
                {
                    EncounterSystem.EncounterStarted = false;
                    continue;
                }

                try
                {
                    var modelItem = DataFactory.GetRandomItem(modelNpc);
                    if (modelItem == null)
                    {
                        Plugin.Logger.LogWarning($"Encounter NPC '{modelNpc.name}' has no configured rewards; skipping reward.");
                        continue;
                    }

                    var itemGuid = new PrefabGUID(modelItem.ItemID);
                    var quantity = modelItem.Stack;
                    if (!userModel.TryGiveItem(itemGuid, quantity, out _))
                    {
                        Plugin.Logger.LogError($"{userModel.CharacterName} Error Drop {modelItem.name}");
                    }

                    var message = string.Format(Config.RewardMessageTemplate.Value, modelItem.Color, modelItem.name);
                    userModel.SendSystemMessage(message);

                    Plugin.Logger.LogDebug($"{userModel.CharacterName} earned reward: {modelItem.name}");

                    var globalMessage = string.Format(Config.RewardAnnouncementMessageTemplate.Value, userModel.CharacterName, modelItem.Color, modelItem.name);

                    if (Config.NotifyAllPlayersAboutRewards.Value)
                    {
                        var onlineUsers = GameData.Users.Online;
                        foreach (var model in onlineUsers.Where(u => u.PlatformId != userModel.PlatformId))
                        {
                            model.SendSystemMessage(globalMessage);
                        }

                    }
                    else if (Config.NotifyAdminsAboutEncountersAndRewards.Value)
                    {
                        var onlineAdmins = GameData.Users.Online.Where(x => x.IsAdmin == true);
                        foreach (var onlineAdmin in onlineAdmins)
                        {
                            onlineAdmin.SendSystemMessage($"{userModel.CharacterName} earned an encounter reward: <color={modelItem.Color}>{modelItem.name}</color>");
                        }
                    }
                }
                catch (Exception error)
                {
                    Plugin.Logger.LogError($"Error processing encounter reward: {error}");
                }
                finally
                {
                    EncounterSystem.EncounterStarted = false;
                }

            }
        }
    }
}
