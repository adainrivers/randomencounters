using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ProjectM;
using RandomEncounters.Models;
using Unity.Entities;

namespace RandomEncounters.Utils
{
    public static class UnityUtils
    {
        public static List<T> ToList<T>(this BlobArray<T> blobArray) where T : new()
        {
            if (blobArray == null)
            {
                return null;
            }

            var result = new List<T>();
            for (var i = 0; i < blobArray.Length; i++)
            {
                result.Add(blobArray[i]);
            }

            return result;
        }

        public static void AddItemToInventory(World world, UserModel userModel, int itemId, int amount)
        {
            unsafe
            {
                var gameData = world.GetExistingSystem<GameDataSystem>();
                var bytes = stackalloc byte[Marshal.SizeOf<FakeNull>()];
                var bytePtr = new IntPtr(bytes);
                Marshal.StructureToPtr<FakeNull>(new()
                {
                    value = 0,
                    has_value = true
                }, bytePtr, false);
                var boxedBytePtr = IntPtr.Subtract(bytePtr, 0x10);
                var hack = new Il2CppSystem.Nullable<int>(boxedBytePtr);
                var hasAdded = InventoryUtilitiesServer.TryAddItem(world.EntityManager, gameData.ItemHashLookupMap, userModel.LocalCharacter._Entity, new PrefabGUID(itemId), amount, out _, out Entity e, default, hack);
                if (!hasAdded)
                {
                    InventoryUtilitiesServer.CreateDropItem(world.EntityManager, userModel.Entity, new PrefabGUID(itemId), 1, new Entity());

                }
            }
        }
    }

    struct FakeNull
    {
        public int value;
        public bool has_value;
    }
}
