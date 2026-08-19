# BloodyEncounters

**BloodyEncounters** represents an enhanced iteration of the RandonmEncounters mod by [@adainrivers](https://github.com/adainrivers), functioning as a server-side mod that introduces a thrilling dynamic to gameplay. It randomly spawns NPCs near online players at unpredictable intervals, challenging players to defeat these NPCs within a set time limit to earn random item rewards.

## IMPORTANT NOTE

Version 3.1.0 targets the current V Rising 1.1 server line. It requires VampireCommandFramework 0.11.0 and Bloody.Core 2.0.2. Bloodstone is no longer required.

## NEW IN 3.0.0

- Complete refactoring of the mod.
- Added the functionality that if an NPC has the group value filled, it spawns all the NPCs in that group.
- Added the ability to modify npc statistics.


```json
[
 {
    "name": "Rifle Man",
    "PrefabGUID": 1148936156,
    "AssetName": "CHAR_ChurchOfLight_Rifleman",
    "levelAbove": 10,
    "items": [
      {
        "name": "Blood Rose Potion",
        "ItemID": 429052660,
        "Stack": 25,
        "Chance": 1,
        "Color": "#daa520"
      },
      {
        "name": "Blood Token",
        "ItemID": -77477508,
        "Stack": 10,
        "Chance": 1,
        "Color": "#daa520"
      }
    ],
    "Lifetime": 300,
    "Group": "Uno",
    "unitStats": {
      "PhysicalCriticalStrikeChance": 0,
      "PhysicalCriticalStrikeDamage": 2,
      "SpellCriticalStrikeChance": 0,
      "SpellCriticalStrikeDamage": 2,
      "PhysicalPower": 45.550457,
      "SpellPower": 45.550457,
      "ResourcePower": 28.449999,
      "SiegePower": 17,
      "ResourceYieldModifier": 1,
      "ReducedResourceDurabilityLoss": 1,
      "PhysicalResistance": 0,
      "SpellResistance": 0,
      "SunResistance": 0,
      "FireResistance": 0,
      "HolyResistance": 0,
      "SilverResistance": 0,
      "SilverCoinResistance": 0,
      "GarlicResistance": 0,
      "PassiveHealthRegen": 1,
      "CCReduction": 0,
      "HealthRecovery": 1,
      "DamageReduction": 0,
      "HealingReceived": 0,
      "ShieldAbsorbModifier": 1,
      "BloodEfficiency": 1
    }
  }
]
```

<details>
<summary>Changelog</summary>

`3.0.0`
- Complete refactoring of the mod
- Added an optional extar parameter called "group" to the NPC add command
- Added option in the mod configuration for the default message of the groups
- Added the functionality that if an NPC has the group value filled, it spawns all the NPCs in that group.
- Added the ability to modify npc statistics.
- Fixed bug that caused all npcs like those configured in the mod to drop the configured items when they were not part of the encounter

`2.0.8`
- Fixed reload command

`2.0.5`
- Updated the timer system through Coroutine that brings the new version of Bloody.Core
- Removed the original Drop Table from every NPC you set up.
- Removed the ability to unlock Trophies by killing a VBlood that you set up for encounters.
- Eliminate the possibility of tracking VBlood in case they are configured for meetings

`2.0.4`
- Update Timer Systems

`2.0.4`
- Bloody.Core dependency removed as dll and added as frameworkrk

`2.0.3`
- Fixed the bug that the encounter spawn had. A new spawn system has been generated to avoid incompatibilities with other mods
- Fixed bug that caused the BloodyEncounter reward system and death message to also affect the game's default NPC if the NPC Prefab was configured as BloodyEncounter.

`2.0.0`
- World boss functionality has been removed to create a standalone mod called [BloodyBoss](https://github.com/oscarpedrero/BloodyBoss)
- Updated to a VRising 1.0

`1.5.0`
- Added World Boss

`1.0.0`
- Initial public release of the mod
</details>

## Mod Features
BloodyEncounters elevates gameplay by injecting an element of unpredictability and formidable challenges. As players venture outside their castles, there's a chance a random NPC will spawn nearby. You can customize the NPC level difference, ensuring balanced encounters for all players. This mod is highly configurable, offering an array of options to tailor the experience to your preferences.

## Requirements:

For the correct functioning of this mod you must have the following dependencies installed on your server:

1. [BepInExPack for V Rising 1.733.2](https://thunderstore.io/c/v-rising/p/BepInEx/BepInExPack_V_Rising/)
2. [VampireCommandFramework 0.11.0](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/)
3. [Bloody.Core 2.0.2](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/)

Bloodstone is not a dependency of version 3.1.0.

## Installation
1. Install the required dependencies on the dedicated server.
2. Copy `BloodyEncounters.dll` to the server's `BepInEx/plugins` directory.
3. Launch the server once to generate `BepInEx/config/BloodyEncounters/BloodyEncounters.cfg` and `NPCS.json`.
4. `NPCS.json` starts empty. Add at least one encounter NPC with `.be npc create` before using `.be me` or automatic encounters.

## Commands
It's crucial to note that for any command containing a name argument such as `<NameOfNPC>`, `<ItemName>` or `<GroupName>`, if your name consists of more than one word, include it inside `""` to ensure proper functionality (e.g., "Rifle Man" or "Blood Rose Potion").

```ansi
.be npc create <NameOfNPC> <PrefabGUIDOfNPC> <LevelsAbovePlayer> <LifeTime> <GroupName>
```
- Create your desired NPCs to include in the encounter randomized pool.
  - **NameOfNPC**: The NPC name that will appear in the chat when the player triggers an encounter event.
  - **PrefabGUIDOfNPC**: The GUID of the NPC you prefer to use.
  - **LevelsAbovePlayer**: Specify how many levels you want the NPC to be above the player level. For example, if a player is level 10 and the value is 10, the NPC will spawn at level 20.
  - **LifeTime**: The duration the player has to kill the NPC encounters in seconds.
  - **Group** ( Optional ): If you give it a group value, when this NPC touches it it spawns all the NPCS in that group..
  - Example: `.be npc create "Rifle Man" 1148936156 10 300 "Group One"`
```ansi
.be npc remove <NameOfNPC>
```
- Removes an NPC from the encounter randomized pool.
  -  **NameOfNPC**: The NPC name you want to remove.
  - Example: `.be npc remove "Rifle Man"`
```ansi
.be items add <NameOfNPC> <ItemName> <ItemPrefabID> <Stack>
```
- Adds items/rewards to the randomized pool that the player will receive from killing a particular NPC encounter.
  - **NameOfNPC**: The NPC name to which you want to add items.
  - **ItemName**: The name of the item/reward appearing in the chat once the player wins the encounter.
  - **ItemPrefabID**: The GUID for the item you want to add.
  - **Stack**: The quantity of items the player will gain upon winning the encounter (e.g., x25 Blood Potions).
  - Example: `.be items add "Rifle Man" "Blood Rose Potion" 429052660 25`
```ansi
.be items list (NPCName)
```
- Displays the list of items included within a particular NPC.
  - Example: `.be items list "Rifle Man"`
```ansi
.be enable
```
- Enables the BloodyEncounter mod.
  - Example: `.be enable`
```ansi
.be disable
```
- Disables the BloodyEncounter mod.
  - Example: `.be disable`
```ansi
.be reload
```
- Reloads the mod configuration in real-time.
  - Example: `.be reload`
```ansi
.be start
```
- Triggers an encounter for a random online player.
  - Example: `.be start`
```ansi
.be me
```
- Triggers an encounter for yourself.
  - Example: `.be me`
```ansi
.be player <PlayerName>
```
- Triggers an encounter for a specific player.
  - Example: `.be player Vex`

# Resources

[Complete items list of prefabs/GUID](https://discord.com/channels/978094827830915092/1117273637024714862/1117273642817044571)

# Credits

This mod was originally developed by [@adainrivers](https://github.com/adainrivers/randomencounters) and was ported/updated to be compatible with the Gloomrot update.

[V Rising Mod Community](https://discord.gg/vrisingmods) is the premier community of mods for V Rising.

[@Deca](https://github.com/decaprime), thank you for the exceptional frameworks [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework) and [BloodStone](https://github.com/decaprime/Bloodstone), based on [WetStone](https://github.com/molenzwiebel/Wetstone) by [@Molenzwiebel](https://github.com/molenzwiebel).

[@Backxtar](https://github.com/Backxtar), owner & founder of [Bloody Mary](https://discord.gg/sE2hqbxUU4) server, and [@LecherousCthulhu](https://github.com/HasturDev) & [@Willis](https://github.com/emelonakos) for being amazing community modders and part of the *BloodyTeam*.

**Special thanks to the testers and supporters of the project:**

- @Vex, owner & founder of [Vexor RPG](https://discord.gg/JpVsKVvKNR) server, a tester and great supporter who provided his server as a test platform and took care of all the graphics and documentation.
