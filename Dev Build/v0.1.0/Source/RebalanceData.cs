using System;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;

namespace DaveTheMonitor
{
    /// <summary>
    /// Data for Rebalance that can be saved.
    /// Everything that should persist between sessions should be set using an instance of this class, which should serialized and written to "rebalance.dat" in the world's directory every time the world is saved, which should be read and deserialized into an instance of this class every time the world is loaded.
    /// </summary>
    [Serializable]
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
        public ActorType[] EnemyActors;
        public Item[] LegendaryWeapons;
        public Item[] LegendaryItems;
        public bool EnableTierSpawns;
        public bool NewCombat;
        public bool LockLegendaryWeapons;
        public bool LockAmuletOfFlight;
        public bool LockAllLegendaryItems;
        public float TierUpdateInterval;
        public bool DebugMode;
        
        public float DebugTextScale;
        public float MessageTime;
        public float MessageScale;
        public Vector2 MessageVelocity;
        public Color RejectionMessageColor;
        public Color AestusMessageColor;
    }

    public class RebalanceTierXML
    {
        public Item[] Items;
        public ActorType[] Spawns;
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
