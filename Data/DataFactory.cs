using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Bloody.Core.Models;
using Bloody.Core;
using BloodyEncounters.Data.Models;

namespace BloodyEncounters.Data
{
    internal static class DataFactory
    {
        private static readonly Random Random = new();
        private static List<NpcEncounterModel> _npcs;
        private static List<ItemEncounterModel> _items;

        internal static NpcEncounterModel GetRandomNpc()
        {
            _npcs = Database.NPCS;
            return _npcs
                .GetRandomItem();
        }

        internal static ItemEncounterModel GetRandomItem(NpcEncounterModel npc)
        {
            _items = npc.items;
            return _items
                .GetRandomItem();
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
