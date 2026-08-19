using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Methods;
using Bloody.Core.Models.v1;
using Bloody.Core.Patch.Server;
using BloodyEncounters.Data;
using BloodyEncounters.Data.Models;
using ProjectM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace BloodyEncounters
{
    internal class EncounterSystem
    {
        public static bool EncounterStarted = false;

        private static readonly ConcurrentDictionary<ulong, ConcurrentDictionary<int, ItemEncounterModel>> RewardsMap = new();

        private static readonly ConcurrentDictionary<int, UserModel> NpcPlayerMap = new();

        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;

        internal static Dictionary<long, (float actualDuration, Action<Entity> Actions)> PostActions = new();

        public static System.Random Random = new System.Random();

        internal static void Initialize()
        {
            if (Config.Enabled.Value)
            {
                GenerateStats();
                StartEncounterTimer();
            }
        }

        internal static void GenerateStats()
        {
            var npcNeedStats = Database.NPCS.Where(x => x.unitStats == null).ToList();

            foreach (var npcModel in npcNeedStats)
            {
                npcModel.GenerateStats();
            }

            Database.saveDatabase();
        }

        public static Action encounterAction;
        public static void StartEncounterTimer()
        {

            Plugin.Logger.LogInfo($"Start Timner for encounters");

            encounterAction = () =>
            {
                if (Config.Enabled.Value)
                {
                    var startAction = () =>
                    {
                        StartEncounter();
                    };

                    ActionScheduler.RunActionOnMainThread(startAction);

                }
            };

            CoroutineHandler.StartRandomIntervalCoroutine(encounterAction, Config.EncounterTimerMin.Value, Config.EncounterTimerMax.Value);
        }

        internal static void StartEncounter(UserModel user = null)
        {
            if (!EncounterStarted)
            {
                var world = Core.World;

                if (user == null)
                {
                    var users = GameData.Users.Online.Where(u => GameData.Users.FromEntity(u.Entity).Character.Equipment.Level >= Config.EncounterMinLevel.Value);

                    if (Config.SkipPlayersInCastle.Value)
                    {
                        users = users.Where(u => !u.IsInCastle());
                    }

                    if (Config.SkipPlayersInCombat.Value)
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

                var npc = DataFactory.GetRandomNpc();

                if (npc == null)
                {
                    Plugin.Logger.LogWarning($"Could not find any NPCs");
                    return;
                }

                Plugin.Logger.LogMessage($"Attempting to start a new encounter for {user.CharacterName}");

                try
                {
                    var message = string.Empty;
                    NpcPlayerMap[npc.PrefabGUID] = user;
                    if (npc.Group != "")
                    {

                        var npcsTeam = Database.NPCS.Where(x => x.Group == npc.Group).ToList();
                        foreach (var npcTeam in npcsTeam)
                        {
                            npcTeam.Spawn(user.Entity, user.Position, npc.Lifetime);


                        }

                        message = string.Format(Config.EncounterMessageGroupTemplate.Value, npc.Group, npc.Lifetime);

                    }
                    else
                    {
                        npc.Spawn(user.Entity, user.Position);
                        message = string.Format(Config.EncounterMessageTemplate.Value, npc.name, npc.Lifetime);
                    }


                    user.SendSystemMessage(message);
                    EncounterStarted = true;

                    var actionDisableEncounter = () =>
                    {
                        if (EncounterStarted)
                        {
                            if (npc.Group != "")
                            {

                                var npcsTeam = Database.NPCS.Where(x => x.Group == npc.Group).ToList();
                                foreach (var npcTeam in npcsTeam)
                                {
                                    npcTeam.KillNpc(user.Entity);

                                }
                                EncounterStarted = false;
                            }
                            else
                            {
                                npc.KillNpc(user.Entity);
                                EncounterStarted = false;
                            }

                        }
                    };

                    CoroutineHandler.StartRepeatingCoroutine(actionDisableEncounter, npc.Lifetime, 1);

                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogError(ex);
                    EncounterStarted = false;
                }
            }

        }
    }
}
