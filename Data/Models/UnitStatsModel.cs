using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyEncounters.Data.Models
{
    internal class UnitStatsModel
    {
        public float PhysicalCriticalStrikeChance { get; set; }
        public float PhysicalCriticalStrikeDamage { get; set; }
        public float SpellCriticalStrikeChance { get; set; }
        public float SpellCriticalStrikeDamage { get; set; }
        public float PhysicalPower { get; set; }
        public float SpellPower { get; set; }
        public float ResourcePower { get; set; }
        public float SiegePower { get; set; }
        public float ResourceYieldModifier { get; set; }
        public float ReducedResourceDurabilityLoss { get; set; }
        public float PhysicalResistance { get; set; }
        public float SpellResistance { get; set; }
        public int SunResistance { get; set; }
        public int FireResistance { get; set; }
        public int HolyResistance { get; set; }
        public int SilverResistance { get; set; }
        public int SilverCoinResistance { get; set; }
        public int GarlicResistance { get; set; }
        public float PassiveHealthRegen { get; set; }
        public int CCReduction { get; set; }
        public float HealthRecovery { get; set; }
        public float DamageReduction { get; set; }
        public float HealingReceived { get; set; }
        public float ShieldAbsorbModifier { get; set; }
        public float BloodEfficiency { get; set; }

        // V Rising 1.1 removed 13 fields from UnitStats. Crit chance/damage, the
        // elemental resistances (sun/holy/silver/silver coin/garlic), resource yield,
        // durability loss, shield absorb and blood efficiency are no longer flat fields
        // on the struct - the game now drives them through UnitStatType + stat buffs.
        //
        // Those 13 are left out below rather than faked, so the mod builds and the
        // remaining 12 stats keep working exactly as before. The properties are still
        // on this model, so existing config JSON keeps loading without errors - the
        // unsupported values are simply ignored.
        public void SetStats(UnitStats stats)
        {
            PhysicalPower = stats.PhysicalPower.Value;
            SpellPower = stats.SpellPower.Value;
            ResourcePower = stats.ResourcePower.Value;
            SiegePower = stats.SiegePower.Value;
            PhysicalResistance = stats.PhysicalResistance.Value;
            SpellResistance = stats.SpellResistance.Value;
            FireResistance = stats.FireResistance.Value;
            PassiveHealthRegen = stats.PassiveHealthRegen.Value;
            CCReduction = stats.CCReduction.Value;
            HealthRecovery = stats.HealthRecovery.Value;
            DamageReduction = stats.DamageReduction.Value;
            HealingReceived = stats.HealingReceived.Value;
        }

        // Same 13 omissions as SetStats above - see the note there.
        public UnitStats FillStats(UnitStats stats)
        {
            stats.PhysicalPower._Value = PhysicalPower;
            stats.SpellPower._Value = SpellPower;
            stats.ResourcePower._Value = ResourcePower;
            stats.SiegePower._Value = SiegePower;
            stats.PhysicalResistance._Value = PhysicalResistance;
            stats.SpellResistance._Value = SpellResistance;
            stats.FireResistance._Value = FireResistance;
            stats.PassiveHealthRegen._Value = PassiveHealthRegen;
            stats.CCReduction._Value = CCReduction;
            stats.HealthRecovery._Value = HealthRecovery;
            stats.DamageReduction._Value = DamageReduction;
            stats.HealingReceived._Value = HealingReceived;

            return stats;
        }
    }
}
