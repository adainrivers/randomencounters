using System;
using ProjectM;
using GT.VRising.GameData;
using GT.VRising.GameData.Models;
using Wetstone.API;

namespace RandomEncounters.Models
{
    public class NpcDataModel
    {
        private NpcModel _npcModel;

        public NpcDataModel(string npcLine)
        {
            var fields = npcLine.Split("\t");
            if (fields.Length != 5)
            {
                return;
            }

            Id = int.Parse(fields[0]);
            Name = fields[1];
            PrefabName = fields[2];
            Level = int.Parse(fields[3]);
            BloodType = fields[4];
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string PrefabName { get; set; }
        public int Level { get; set; }
        public string BloodType { get; set; }

        public NpcModel NpcModel => _npcModel ??= new Lazy<NpcModel>(GetNpcModel).Value;

        private NpcModel GetNpcModel()
        {
            var world = VWorld.Server;
            var prefabCollectionSystem = world.GetExistingSystem<PrefabCollectionSystem>();
            try
            {
                var npcEntity = prefabCollectionSystem.PrefabLookupMap[new PrefabGUID(Id)];
                return GameData.Npcs.FromEntity(npcEntity);
            }
            catch
            {
                return null;
            }
        }
    }
}