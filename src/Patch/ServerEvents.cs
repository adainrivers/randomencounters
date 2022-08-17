using System;
using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Unity.Entities;

namespace RandomEncounters.Patch
{
    public delegate void OnUnitSpawnedEventHandler(World world, Entity entity);
    public delegate void DeathEventHandler(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents);

    public static class ServerEvents
    {
        public static event DeathEventHandler OnDeath;
        public static event OnUnitSpawnedEventHandler OnUnitSpawned;

        [HarmonyPatch(typeof(DeathEventListenerSystem), nameof(DeathEventListenerSystem.OnUpdate))]
        [HarmonyPostfix]
        private static void DeathEventListenerSystemPatch_Postfix(DeathEventListenerSystem __instance)
        {
            try
            {
                var deathEvents =
                    __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
                if (deathEvents.Length > 0)
                {
                    OnDeath?.Invoke(__instance, deathEvents);
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }

        [HarmonyPatch(typeof(UnitSpawnerReactSystem), nameof(UnitSpawnerReactSystem.OnUpdate))]
        [HarmonyPostfix]
        public static void Prefix(UnitSpawnerReactSystem __instance)
        {
            var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                try
                {
                    OnUnitSpawned?.Invoke(__instance.World, entity);
                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError(e);
                }
            }
        }
    }
}