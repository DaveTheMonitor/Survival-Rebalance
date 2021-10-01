# DON'T USE THIS BUILD

I was an idiot and left in the player damage every tier update. Use v0.1.1

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

#### NewCombat (bool)

Whether or not the Survival Rebalance experimental combat system is used.
By default, attacks have a chance to deal semi-random damage against targets, the chance of which decreases the higher the target's defence is. Eg. an NPC with 60 defence will take damage from attacks less often than an NPC with 20 defence. The chance of dealing damage against an enemy increases the higher the player's attack skill/level, and the damage dealt increases with the player's strength skill/level.

The experimental combat system replaces the existing system with a custom one, in which weapons deal damage equal to their base damage, modified by slight randomness and the target's defence. This combat system only affects your attacks, not the enemy's. This combat system also does not consider your skills when attacking. This combat system also works with modded weapons. This combat system can be disabled in the config if you wish. The calculations for damage are detailed below.
This combat system is very experimental, and I'm open to feedback.
Notes:
 \- At the moment, the experimental combat system does not use the Crit hitbox of the target.
 \- The target will take knockback in the direction you are facing based on how much damage you deal.
 \- The difference in damage against defence is based off your damage and the target's defence. The higher your damage, the less affected it is by the target's defence.
 \- \- Calculated Damage = Damage² * 0.8 / Defence (Min 5%, Max 100%)
 \- \- If your damage is 125% or more of the target's defence, you will deal 100% damage.
 \- \- If your damage is 100% of the target's defence, you will deal 80% damage.
 \- \- If your damage is 75% of the target's defence, you will deal 60% damage.
 \- \- If your damage is 62.5% of the target's defence, you will deal 50% damage.
 \- \- If your damage is 50% of the target's defence, you will deal 40% damage.
 \- \- If your damage is 25% of the target's defence, you will deal 20% damage.

Known Issues:
 \- Enemies do not target the player when attacked. This could be a bug with the game, and not the mod.
 \- Orange damage numbers (crits) seem to appear even if no crit is landed, due to the game using a different crit system. This does not appear to affect damage.
 \- This combat system sets every weapon's reach to 0. As far as I know, this is required.

Crit chance = 5% OR 15% w/ Amulet of Fury (Crit damage = 1.5x)
Damage = Random(WeaponDamage * 0.9, WeaponDamage * 1.1) * Crit? * Clamp(Damage * 0.8f / TargetDefence, 0.05, 1)

C# Code:
bool crit = player.IsItemEquippedAndUsable(Item.AmuletOfFury) ? rand.Next(0, 100) < 15 : rand.Next(0, 100) < 5;
float damage = crit ? rand.Next((int)(data.Damage * 0.9f), (int)(data.Damage * 1.1f)) * 1.5f : rand.Next((int)(data.Damage * 0.9f), (int)(data.Damage * 1.1f));
int defence = npcLevelData\[prevDifficulty\].ContainsKey(ActorTypes\[target.ActorType\].LevelType) ? npcLevelData\[prevDifficulty\]\[ActorTypes\[target.ActorType\].LevelType\].DefenceLevel : Globals1.NpcLevelData\[(int)ActorTypes\[target.ActorType\].LevelType\].DefenceLevel;
damage *= MathHelper.Clamp(damage * 0.8f / defence, 0.05f, 1);

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

#### TierUpdateInterval

The time, in seconds, between each tier update check. Increasing this value will cause the world's tier to update less frequently, but will be more performant.

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
