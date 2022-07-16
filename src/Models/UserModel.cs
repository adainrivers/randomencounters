using System;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace RandomEncounters.Models;

public class UserModel
{
    public UserModel() { }

    public static UserModel FromUser(World world, User user, Entity userEntity)
    {
        var model = new UserModel
        {
            Index = user.Index,
            PlatformId = user.PlatformId,
            CharacterName = user.CharacterName.ToString(),
            IsAdmin = user.IsAdmin,
            IsConnected = user.IsConnected,
            TimeLastConnected = user.TimeLastConnected,
            Entity = userEntity,
            User = user,
            LocalCharacter = user.LocalCharacter,
            LocalToWorld = world.EntityManager.GetComponentData<LocalToWorld>(userEntity)
        };
        model.FromCharacter = new FromCharacter { User = model.Entity, Character = model.LocalCharacter._Entity };

        var charEntity = model.FromCharacter.Character;
        try
        {
            var equipment = world.EntityManager.GetComponentData<Equipment>(charEntity);
            model.Level = equipment.ArmorLevel.Value + equipment.WeaponLevel.Value + equipment.SpellLevel.Value;
        }
        catch 
        {
            return null;
        }
        return model;
    }

    public bool IsInCastle(World world)
    {
        var query = world.EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PrefabGUID>(),
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadOnly<UserOwner>(),
            ComponentType.ReadOnly<CastleFloor>());
        var entities = query.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            var localToWorld = world.EntityManager.GetComponentData<LocalToWorld>(entity);
            var position = localToWorld.Position;
            var userPosition = LocalToWorld.Position;
            if (Math.Abs(userPosition.x - position.x) < 3 && Math.Abs(userPosition.z - position.z) < 3)
            {
                return true;
            }
        }
        return false;
    }

    public float Level { get; set; }

    public FromCharacter FromCharacter { get; set; }

    public User User { get; set; }

    public LocalToWorld LocalToWorld { get; set; }
    
    public Entity Entity { get; set; }

    public NetworkedEntity LocalCharacter { get; set; }

    public long TimeLastConnected { get; set; }

    public bool IsConnected { get; set; }

    public bool IsAdmin { get; set; }

    public string CharacterName { get; set; }

    public ulong PlatformId { get; set; }

    public int Index { get; set; }

    public void SendSystemMessage(World world, string message)
    {
        ServerChatUtils.SendSystemMessageToClient(world.EntityManager, User, message);
    }
}