# Random Encounters mod for V Rising

See [V Rising Database](https://gaming.tools/v-rising) for detailed information about V Rising items, NPCs and more.

This server side mod spawns a random NPC near a random online player at random intervals, and the player wins a random item reward if the NPC is killed within the given time limit.

It can be configured using the configuration files which are created on first run.

## Installation

Random Encounters mod requires Wetstone plugin installed. The latest version of Wetstone can be downloaded from https://v-rising.thunderstore.io/package/molenzwiebel/Wetstone/

If you are not planning to use the hot reload feature of Wetstone, simply place the plugin dll into `BepInEx\plugins` folder and run the server. Otherwise, please check [Wetstone documentation](https://molenzwiebel.github.io/Wetstone/features/reload.html) about how to place the mod to the correct folder.

## Configuration files and options:

### Main.cfg
- **Enabled:** 
Determines whether the random encounters are enabled or not.
- **EncounterTimerMin:** 
Minimum seconds before a new encounter is initiated. This value is divided by the online users count.
- **EncounterTimerMax:** 
Maximum seconds before a new encounter is initiated. This value is divided by the online users count.
- **EncounterLength:** 
Maximum seconds until the player can kill the NPC for a reward.
- **EncounterMinLevelDifference:** 
The lower value for the NPC level - Player level difference.
- **EncounterMaxLevelDifference:** 
The upper value for the NPC level - Player level difference.
- **EncounterMessageTemplate:** 
System message template for the encounter.
- **RewardMessageTemplate:** 
System message template for the reward.

### Items.cfg

The list of all the items from the game. Individual items can be enabled or disabled. Only enabled items are considered for the random reward.

### NPCs.cfg

The list of all the NPCs from the game. Individual NPCs can be enabled or disabled. Only enabled NPCs are considered for the random encounter.

