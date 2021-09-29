# Survival Balance Development Build

This is a development build of Survival Rebalance. That means that some features may be unoptimized, may be removed, or just may not work at all.
This build should only be used for testing and not for actual gameplay. If you do use this build for gameplay, remember that the next version could be vastly different, and some things might break (eg. corruption of Rebalance Data).
Note: Rebalance saves data in the "rebalance.dat" file in your world's folder. The format of this data could change in the future, which may cause the data to fail to load.

If you have any suggestions or have any problems with the mod, please let me know. You can message me on Discord if you're in the Total Miner Discord Server: **Dave The Monitor#6212**

This mod comes with an XML config file that you can modify to change how the mod works. Below is each option and what they do:

## Game

#### EnemyActors (ActorType\[\])

Actors that should be considered enemies. These actors are affected by the tier spawn system, and cannot spawn unless the current tier allows them to. You should only modify this if you want to add other actors to spawn tiers.
Default: { Dryad, Djinn, Goblin, Orc, TrollChief, TrollBoy, Werewolf, Diablo, Zombie }

#### LegendaryWeapons (Item\[\])

Legendary weapons that should be locked behind a Diablo kill (Aestus Flask use) if LockLegendaryWeapons is true.
Default: { BattleAxe, ElvenBow }

#### LegendaryItems (Item\[\])

Legendary items that should be locked behind a Diablo kill (Aestus Flask use) if LockAllLegendaryItems is true.
This requirement may change in the future to differentiate it from LegendaryWeapons.
This array could also be replaced entirely by LegendaryWeapons in the future.
Default: { BattleAxe, ElvenBow, SledgeHammer, GreenstoneGoldSledgeHammer, WaterTalisman, TenLeagueBoots }

#### EnableTierSpawns (bool)

Whether or not enemy spawns are locked behind tiers.
Default: true

#### LockLegendaryWeapons (bool)

Whether or not legendary weapons, as defined above, are locked behind a Diablo kill (Aestus Flask use).
Default: true

#### LockAllLegendaryItems (bool)

Whether or not all legendary items, as defined above, are locked behind a Diablo kill. (Aestus Flask use).
Default: false

#### LockAmuletOfFlight (bool)

Whether or not the Amulet of Flight is locked behind an unlock requirement.
The unlock requirement for the Amulet Of Flight has not yet been decided. My original idea was to lock it behind obtaining all of the listed legendary items, which may also be given a crafting recipes involving the Aestus Flask dropped by Diablos.
Default: true

#### DebugMode (bool)

Enables debug mode, which re-enables the NPCSpawn block and gives you access to debug features (Reset Tier, Current Tier, Toggle Amulet of Flight, Toggle Legendary Weapons).
Default: true

## Visual

#### DebugTextScale (float)

The scale of the debug text in the bottom left corner of the screen. Also affects the Dev Build text.
Default: 0.5

### Message Options (Rejected item, Aestus Flask used)

#### MessageTime (float)

The time, in seconds, the message lasts on screen. The Aestus Flask message lasts for twice this time.
Default: 3.7

#### MessageScale (float)

The scale of the message.
Default: 2.4

#### MessageVelocity (Vector2)

The velocity of the message. The Aestus Flask message moves at half this velocity.
Default: { X:0, Y:-20 }

#### Rejection Message Color (Color)

The color of the standard rejection message (The {Item} rejects your abilities...).
Default: { R:240, G:22, B:15, A:255 }

#### AestusMessageColor (Color)

The color of the Aestus Flask message.
Default: { R:202 G:104 B:31 A:255 }