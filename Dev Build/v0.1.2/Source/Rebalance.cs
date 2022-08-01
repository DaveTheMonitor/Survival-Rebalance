using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using StudioForge.TotalMiner;
using StudioForge.Engine;

namespace DaveTheMonitor
{
    public static class Rebalance
    {
        public static List<ActorType> EnabledActors = new List<ActorType>();
        public static void SaveData(string file, RebalanceData data)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
                {
                    writer.Write(data.HighestTier);
                    writer.Write(data.AmuletOfFlight);
                    writer.Write(data.LegendaryItems);
                }
            }
        }
        public static RebalanceData LoadData(string file)
        {
            if (!File.Exists(file))
                return new RebalanceData();
            try
            {
                using (FileStream stream = new FileStream(file, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                    {
                        RebalanceData data = new RebalanceData();
                        data.HighestTier = reader.ReadInt32();
                        data.AmuletOfFlight = reader.ReadBoolean();
                        data.LegendaryItems = reader.ReadBoolean();
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                CoreGlobals.LogErrorMessage("Failed to load Rebalance data.", $"There was a problem loading Rebalance save data, and this world's Rebalance data was reset. It's possible the \"rebalance.dat\" file was corrupted.\n\nError:{ex.Message}");
                return new RebalanceData();
            }
        }

        public static void SetNPCData(ActorLevelDataXML[] actorLevelData)
        {
            foreach (ActorLevelDataXML data in actorLevelData)
                Globals1.NpcLevelData[(int)data.ActorLevelType] = data;
        }

        public static void SetNPCData(ActorPhysicsDataXML[] actorPhysicsData)
        {
            foreach (ActorPhysicsDataXML data in actorPhysicsData)
                Globals1.NpcPhysicsData[(int)data.ActorPhysicsType] = data;
        }

        public static void UpdateSpawns(int tier)
        {
            EnabledActors.Clear();
            foreach (ActorType actor in RebalancePlugin.Tiers[tier].Spawns)
            {
                EnabledActors.Add(actor);
            }
        }

        public static Dictionary<ActorLevelType, ActorLevelDataXML> XMLToDictionary(ActorLevelDataXML[] dataXML)
        {
            Dictionary<ActorLevelType, ActorLevelDataXML> dictionary = new Dictionary<ActorLevelType, ActorLevelDataXML>();
            foreach (ActorLevelDataXML data in dataXML)
                dictionary.Add(data.ActorLevelType, data);
            return dictionary;
        }

        public static List<RebalanceTier> ConvertToTiers(RebalanceTierXML[] xml)
        {
            List<RebalanceTier> tiers = new List<RebalanceTier>();
            foreach (RebalanceTierXML data in xml)
            {
                RebalanceTier tier = new RebalanceTier(new List<Item>(), new List<ActorType>());
                foreach (ItemDataXML item in Globals1.ItemData)
                {
                    if (data.Items.Contains(item.IDString))
                        tier.Items.Add(item.ItemID);
                }

                foreach (ActorTypeDataXML actor in Globals1.NpcTypeData)
                {
                    if (data.Spawns.Contains(actor.IDString))
                        tier.Spawns.Add(actor.ActorType);
                }
                tiers.Add(tier);
            }
            return tiers;
        }
    }
}
