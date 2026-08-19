using BloodyEncounters.Data;
using BloodyEncounters.Exceptions;
using System;
using VampireCommandFramework;

namespace BloodyEncounters.Commands
{
    [CommandGroup("be npc")]
    internal class NpcCommand
    {

        [Command("list", usage: "", description: "List of npc", adminOnly: true)]
        public void ListMerchant(ChatCommandContext ctx)
        {

            var npcs = Database.NPCS;

            if (npcs.Count == 0)
            {
                throw ctx.Error($"There are no npcs created");
            }
            ctx.Reply($"NPCS List");
            ctx.Reply($"----------------------------");
            ctx.Reply($"--");
            foreach (var npc in npcs)
            {
                ctx.Reply($"NPC {npc.name}");
                ctx.Reply($"Group {npc.Group}");
                ctx.Reply($"--");
            }
            ctx.Reply($"----------------------------");
        }

        [Command("create", usage: "<NameOfNPC> <PrefabGUIDOfNPC> <LevelsAbovePlayer> <LifeTime> <Group>", description: "Create a NPC", adminOnly: true)]
        public void CreateNPC(ChatCommandContext ctx, string npcName, int prefabGUID, int levelAbove, int lifeTime, string group = "")
        {
            try
            {
                if (Database.AddNPC(npcName, prefabGUID, levelAbove, lifeTime, group))
                {
                    ctx.Reply($"NPC '{npcName}' created successfully");
                }
            }
            catch (NPCExistException)
            {
                throw ctx.Error($"NPC with name '{npcName}' exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("remove", usage: "<NameOfNPC>", description: "Remove a NPC", adminOnly: true)]
        public void RemoveMerchant(ChatCommandContext ctx, string npcName)
        {

            try
            {
                if (Database.RemoveNPC(npcName))
                {
                    ctx.Reply($"NPC '{npcName}' remove successfully");
                }
            }
            catch (NPCDontExistException)
            {
                throw ctx.Error($"NPC with name '{npcName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }
    }
}
