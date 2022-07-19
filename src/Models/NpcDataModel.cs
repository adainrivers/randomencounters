namespace RandomEncounters.Models
{
    public class NpcDataModel
    {
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
    }
}