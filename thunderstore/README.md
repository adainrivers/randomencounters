# Random Encounters mod for V Rising

See [V Rising Database](https://gaming.tools/v-rising) for detailed information about V Rising items, NPCs and more.

This server side mod spawns a random NPC near a random online player at random intervals, and the player wins a random item reward if the NPC is killed within the given time limit.

It can be configured using the configuration files which are created on first run.

## Change Log

### 0.8.3

- Fixed an issue with InCombat detection.
- Mod now depends on https://v-rising.thunderstore.io/package/adainrivers/GT_VRising_GameData/

### 0.8.2

- Logging improvements.

### 0.8.1

- Fixed an issue which was causing spawned NPC not being detected.

### 0.8.0

- Fixed an issue which was causing the server crash.
- You can now specify the quantities for each item in `Items.cfg`. Unobtainable items are disabled by default.
- Increased performance for spawn detection.

### 0.7.1

- Fixed an issue which was causing the same player being picked every time.

## Source Code

Source code of this mod can be found at:
https://github.com/adainrivers/randomencounters

## Installation

Random Encounters mod requires Wetstone plugin installed. The latest version of Wetstone can be downloaded from https://v-rising.thunderstore.io/package/molenzwiebel/Wetstone/

**Since version 0.7.0, the hot reloading feature of Wetstone does NOT work for RandomEncounters. Instead, please use the `!re reload` feature after you make an configuration change.**

## Chat Commands

All chat commands use the default prefix of `!randomencounter` or `!re`. They are only accessible by server admins.

### Commands

`!re start` or just `!re`: Starts an encounter for a random online user.

`!re me`: Starts an encounter for the admin who sends the command.

`!re reload`: Reloads the configuration without restarting the server.

`!re {playerName}`: Starts an encounter for the given player, for example `!re Adain`.

`!re disable`: Disables the random encounter timer.

`!re enabled`: Enables the random encounter timer.

## Configuration files and options:

### Main.cfg
```
[Main]

## Determines whether the random encounter timer is enabled or not.
# Setting type: Boolean
# Default value: true
Enabled = true

## When enabled, players who are in a castle are excluded from encounters
# Setting type: Boolean
# Default value: true
SkipPlayersInCastle = true

## When enabled, players who are in combat are excluded from the random encounters.
# Setting type: Boolean
# Default value: false
SkipPlayersInCombat = false

## Minimum seconds before a new encounter is initiated. This value is divided by the online users count.
# Setting type: Int32
# Default value: 1200
EncounterTimerMin = 1200

## Maximum seconds before a new encounter is initiated. This value is divided by the online users count.
# Setting type: Int32
# Default value: 2400
EncounterTimerMax = 2400

## Maximum seconds until the player can kill the NPC for a reward.
# Setting type: Int32
# Default value: 120
EncounterLength = 120

## The lower value for the NPC level - Player level difference. For example, if player level is 50, and this setting is 20, then the lowest level of the spawned NPC would be 30.
# Setting type: Int32
# Default value: 99
EncounterMinLevelDifference = 99

## The upper value for the NPC level - Player level difference.  For example, if player level is 50, and this setting is 20, then the highest level of the spawned NPC would be 70.
# Setting type: Int32
# Default value: 0
EncounterMaxLevelDifference = 0

## System message template for the encounter.
# Setting type: String
# Default value: You have encountered a <color=#daa520>{0}</color>. You have <color=#daa520>{1}</color> seconds to kill it for a chance of a random reward.
EncounterMessageTemplate = You have encountered a <color=#daa520>{0}</color>. You have <color=#daa520>{1}</color> seconds to kill it for a chance of a random reward.

## System message template for the reward.
# Setting type: String
# Default value: Congratulations. Your reward: <color={0}>{1}</color>.
RewardMessageTemplate = Congratulations. Your reward: <color={0}>{1}</color>.

## System message template for the reward announcement.
# Setting type: String
# Default value: {0} earned an encounter reward: <color={1}>{2}</color>.
RewardAnnouncementMessageTemplate = {0} earned an encounter reward: <color={1}>{2}</color>.

## If enabled, all online admins are notified about encounters and rewards.
# Setting type: Boolean
# Default value: true
NotifyAdminsAboutEncountersAndRewards = true

## When enabled, all online players are notified about any player's rewards.
# Setting type: Boolean
# Default value: false
NotifyAllPlayersAboutRewards = false

## Minimum spawn distance for the spawned unit.
# Setting type: Int32
# Default value: 2
MinSpawnDistance = 2

## Maximum spawn distance for the spawned unit.
# Setting type: Int32
# Default value: 4
MaxSpawnDistance = 4


```

### Items.cfg

The list of all the items from the game. Individual items can be enabled or disabled. Only enabled items are considered for the random reward. 

*NEW in 0.8.0:* You can now specify the quantities for each item. Unobtainable items are disabled by default.

### NPCs.cfg

The list of all the NPCs from the game. Individual NPCs can be enabled or disabled. Only enabled NPCs are considered for the random encounter.

