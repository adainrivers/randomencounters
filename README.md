# Random Encounters mod for V Rising

See [V Rising Database](https://gaming.tools/v-rising) for detailed information about V Rising items, NPCs and more.

This server side mod spawns a random NPC near a random online player at random intervals, and the player wins a random item reward if the NPC is killed within the given time limit.

It can be configured using the configuration files which are created on first run.

## Source Code

Source code of this mod can be found at:
https://github.com/adainrivers/randomencounters

## Installation

Random Encounters mod requires Wetstone plugin installed. The latest version of Wetstone can be downloaded from https://v-rising.thunderstore.io/package/molenzwiebel/Wetstone/

If you are not planning to use the hot reload feature of Wetstone, simply place the plugin dll into `BepInEx\plugins` folder and run the server. Otherwise, please check [Wetstone documentation](https://molenzwiebel.github.io/Wetstone/features/reload.html) about how to place the mod to the correct folder.

## Chat Commands

All chat commands use the default prefix of `!randomencounter` or `!re`. They are only accessible by server admins.

### Commands

`!re start` or just `!re`: Starts an encounter for a random online user.
`!re me`: Starts an encounter for the admin who sends the command.
`!re reload`: Reloads the configuration without restarting the mod.
`!re {playerName}`: Starts an encounter for the given player, for example `!re Adain`

## Configuration files and options:

### Main.cfg
```
[Main]

## Determines whether the random encounters are enabled or not.
# Setting type: Boolean
# Default value: true
Enabled = true

## When enabled, players who are in a castle are excluded from encounters
# Setting type: Boolean
# Default value: true
SkipPlayersInCastle = true

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
EncounterMaxLevelDifferenceLower = 99

## The upper value for the NPC level - Player level difference.  For example, if player level is 50, and this setting is 20, then the highest level of the spawned NPC would be 70.
# Setting type: Int32
# Default value: 0
EncounterMaxLevelDifferenceUpper = 0

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

## If enabled, all online players are notified about any player's rewards.
# Setting type: Boolean
# Default value: false
NotifyAllPlayersAboutRewards = false
```

### Items.cfg

The list of all the items from the game. Individual items can be enabled or disabled. Only enabled items are considered for the random reward.

### NPCs.cfg

The list of all the NPCs from the game. Individual NPCs can be enabled or disabled. Only enabled NPCs are considered for the random encounter.

