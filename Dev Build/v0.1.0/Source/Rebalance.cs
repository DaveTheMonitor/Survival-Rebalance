using System;
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
            ActorType[] spawns = RebalancePlugin.Tiers[tier].Spawns;
            foreach (ActorType actor in spawns)
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
