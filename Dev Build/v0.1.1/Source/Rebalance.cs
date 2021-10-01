using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
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
            FileStream fileStream = new FileStream(file, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, data);
            fileStream.Dispose();
        }
        public static RebalanceData LoadData(string file)
        {
            if (!File.Exists(file))
                return new RebalanceData();
            try
            {
                FileStream fileStream = new FileStream(file, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new RebalanceBinder();
                formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
                RebalanceData data = formatter.Deserialize(fileStream) as RebalanceData;
                fileStream.Dispose();
                return data;
            }
            catch
            {
                CoreGlobals.LogErrorMessage("Failed to load Rebalance data.", "There was a problem loading Rebalance save data, and this world's Rebalance data was reset. It's possible the \"rebalance.dat\" file was corrupted.");
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

        private sealed class RebalanceBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName) => typeof(RebalanceData);

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                base.BindToName(serializedType, out assemblyName, out typeName);
                assemblyName = typeof(RebalanceData).Assembly.FullName;
            }
        }
    }
}
