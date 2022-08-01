using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;

namespace DaveTheMonitor
{
    public class RebalanceData
    {
        /// <summary>
        /// The highest tier reached. Used for mob spawns.
        /// <para>0 = None</para>
        /// <para>1 = Wood</para>
        /// <para>2 = Iron</para>
        /// <para>3 = Steel</para>
        /// <para>4 = GSG</para>
        /// <para>5 = Diamond</para>
        /// <para>6 = Ruby</para>
        /// <para>7 = Titanium</para>
        /// </summary>
        public int HighestTier = 0;
        public bool AmuletOfFlight = false;
        public bool LegendaryItems = false;
    }

    public class RebalanceConfigXML
    {
        public string[] EnemyActors;
        public string[] LegendaryItems;
        public bool EnableTierSpawns;
        public bool NewCombat;
        public float CritChance;
        public float AmuletOfFuryCritChance;
        public bool LockLegendaryItems;
        public bool LockAmuletOfFlight;
        public float TierUpdateInterval;
        public bool DebugMode;
        
        public float DebugTextScale;
        public float MessageTime;
        public float MessageScale;
        public Vector2 MessageVelocity;
        public Color RejectionMessageColor;
        public Color AestusMessageColor;
        public Color CritMessageColor;
        public int CritMessageSpeed;
    }

    public struct RebalanceItemData
    {
        public Item ID;
        public string IDString;
        public string Name;
        public RebalanceItemData(Item id, string idString, string name)
        {
            ID = id;
            IDString = idString;
            Name = name;
        }
    }

    public class RebalanceTierXML
    {
        public string[] Items;
        public string[] Spawns;
    }

    public struct RebalanceTier
    {
        public List<Item> Items;
        public List<ActorType> Spawns;
        public RebalanceTier(List<Item> items, List<ActorType> spawns)
        {
            Items = items;
            Spawns = spawns;
        }
    }

    public struct ActorTypes
    {
        public ActorAIType AIType;
        public ActorLevelType LevelType;
        public ActorPhysicsType PhysicsType;

        public ActorTypes(ActorAIType aiType, ActorLevelType levelType, ActorPhysicsType physicsType)
        {
            AIType = aiType;
            LevelType = levelType;
            PhysicsType = physicsType;
        }
    }

    public struct NewCombatData
    {
        public float Damage;
        public float Reach;
        public StudioForge.BlockWorld.SkillType SkillType;

        public NewCombatData(float damage, float reach, StudioForge.BlockWorld.SkillType skillType)
        {
            Damage = damage;
            Reach = reach;
            SkillType = skillType;
            
        }
    }
}
