using BepInEx;
using BepInEx.Unity.IL2CPP;
using Bloody.Core;
using Bloody.Core.API.v1;
using BloodyEncounters.Data;
using BloodyEncounters.EventsHandler;
using HarmonyLib;
using Unity.Entities;
using VampireCommandFramework;

namespace BloodyEncounters
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("VRisingServer.exe")]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [BepInDependency("trodi.Bloody.Core")]
    public class Plugin : BasePlugin
    {
        Harmony _harmony;

        public static Bloody.Core.Helper.v1.Logger Logger;
        public static SystemsCore SystemsCore;

        public override void Load()
        {
            // Plugin startup logic
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

            Logger = new(Log);

            // Harmony patching
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            EventsHandlerSystem.OnInitialize += GameDataOnInitialize;
            EventsHandlerSystem.OnDestroy += GameDataOnDestroy;

            // Register all commands in the assembly with VCF
            CommandRegistry.RegisterAll();
        }

        public override bool Unload()
        {

            EventsHandlerSystem.OnInitialize -= GameDataOnInitialize;
            EventsHandlerSystem.OnDestroy -= GameDataOnDestroy;
            EventsHandlerSystem.OnDeath -= OnDeath.OnDeathEvent;

            CommandRegistry.UnregisterAssembly();
            _harmony?.UnpatchSelf();
            return true;
        }

        private static void GameDataOnInitialize(World world)
        {

            SystemsCore = Core.SystemsCore;

            Logger.LogInfo("Loading main data");
            Database.Initialize();
            Logger.LogInfo("Binding configuration");
            Data.Config.Initialize();

            EventsHandlerSystem.OnDeath += OnDeath.OnDeathEvent;

            EncounterSystem.Initialize();

        }

        private static void GameDataOnDestroy()
        {
            //Logger.LogInfo("GameDataOnDestroy");
        }
    }
}
