using BloodyEncounters.Data.Models;
using BloodyEncounters.Data;
using VampireCommandFramework;
using BloodyEncounters.Exceptions;
using System;

namespace BloodyEncounters.Commands
{

    [CommandGroup("be items")]
    internal class ItemsCommand
    {

        [Command("list", usage: "", description: "List items of NPC drop", adminOnly: true)]
        public void ListNPCItems(ChatCommandContext ctx, string NPCName)
        {

            try
            {
                if (Database.GetNPC(NPCName, out NpcEncounterModel npc))
                {
                    ctx.Reply($"{npc.name} Items List");
                    ctx.Reply($"----------------------------");
                    ctx.Reply($"--");
                    foreach (var item in npc.GetItems())
                    {
                        ctx.Reply($"Item {item.ItemID}");
                        ctx.Reply($"Stack {item.Stack}");
                        ctx.Reply($"--");
                    }
                    ctx.Reply($"----------------------------");
                }
                else
                {
                    throw new NPCDontExistException();
                }
            }
            catch (NPCDontExistException)
            {
                throw ctx.Error($"NPC with name '{NPCName}' does not exist.");
            }
            catch (ProductExistException)
            {
                throw ctx.Error($"This item configuration already exists at merchant '{NPCName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("add", usage: "<NameOfNPC> <ItemName> <ItemPrefabID> <Stack>", description: "Add a item to a NPC drop", adminOnly: true)]
        public void CreateItem(ChatCommandContext ctx, string NPCName, string ItemName, int ItemPrefabID, int Stack)
        {
            try
            {
                if(Database.GetNPC(NPCName, out NpcEncounterModel npc))
                {
                    npc.AddItem(ItemName,ItemPrefabID, Stack);
                    ctx.Reply($"Item successfully added to NPC '{NPCName}'");
                }
                else
                {
                    throw new NPCDontExistException();
                }
            }
            catch (NPCDontExistException)
            {
                throw ctx.Error($"NPC with name '{NPCName}' does not exist.");
            }
            catch (ProductExistException)
            {
                throw ctx.Error($"This item configuration already exists at merchant '{NPCName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        // .encounter product remove Test1 736318803
        [Command("remove", usage: "<NameOfNPC> <ItemName>", description: "Remove a item to a NPC", adminOnly: true)]
        public void RemoveProduct(ChatCommandContext ctx, string NPCName, string ItemName)
        {
            try
            {
                if (Database.GetNPC(NPCName, out NpcEncounterModel npc))
                {
                    npc.RemoveItem(ItemName);
                    ctx.Reply($"NPC '{NPCName}'\'s item has been successfully removed");
                }
                else
                {
                    throw new NPCDontExistException();
                }
            }
            catch (NPCDontExistException)
            {
                throw ctx.Error($"NPC with name '{NPCName}' does not exist.");
            }
            catch (ProductDontExistException)
            {
                throw ctx.Error($"This item does not exist at NPC '{NPCName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }
    }
}
