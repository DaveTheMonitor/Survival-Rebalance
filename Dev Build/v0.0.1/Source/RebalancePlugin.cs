using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner;
using StudioForge.Engine.Core;
using StudioForge.Engine;

namespace DaveTheMonitor
{
    public class RebalancePlugin : ITMPlugin
    {
        ITMGame game;
        ITMWorld world;
        ITMNpcManager npcManager;
        float timer = 0;
        float tierUpdateInterval = 10;

        #region Fields
        List<ITMPlayer> players = new List<ITMPlayer>();
        GameDifficulty prevDifficulty;
        RebalanceData rebalanceData;
        RebalanceConfigXML config;
        public static RebalanceTierXML[] Tiers;
        public static ActorType[] EnemyActors;
        int npcCount = 0;
        string rootPath;
        bool devMessageShown = false;
        #endregion

        #region GUI
        string indevText = "Survival Rebalance Dev Build v0.0.1";
        string debugText = "R - Reset Tier\nT - Current Tier\nK - Toggle Amulet of Flight\nL - Toggle Legendary Items";
        string itemRejection = "The {0} rejects your abilities...";
        Vector2 viewportSize = new Vector2(CoreGlobals.GraphicsDevice.Viewport.Width, CoreGlobals.GraphicsDevice.Viewport.Height);
        SpriteBatchSafe spriteBatch = CoreGlobals.SpriteBatch;
        SpriteFont font = CoreGlobals.GameFont;
        Vector2 devTextPos = Vector2.Zero;
        Vector2 debugTextPos = Vector2.Zero;
        #endregion

        #region NPC Data
        Dictionary<GameDifficulty, ActorLevelDataXML[]> npcLevelData = new Dictionary<GameDifficulty, ActorLevelDataXML[]>();
        Dictionary<GameDifficulty, ActorPhysicsDataXML[]> npcPhysicsData = new Dictionary<GameDifficulty, ActorPhysicsDataXML[]>();
        #endregion
        void ITMPlugin.PlayerJoined(ITMPlayer player) { }
        void ITMPlugin.PlayerLeft(ITMPlayer player) { }
        void ITMPlugin.WorldSaved(int version)
        {
            Rebalance.SaveData(Path.Combine(rootPath, world.WorldPath, "rebalance.dat"), rebalanceData);
        }
        void ITMPlugin.Initialize(ITMPluginManager mgr, string path)
        {
            rootPath = FileSystem.RootPath;
            config = FileSystem.Deserialize<RebalanceConfigXML>(Path.Combine(path, "RebalanceConfig.xml"));
            EnemyActors = config.EnemyActors;
            Tiers = FileSystem.Deserialize<RebalanceTierXML[]>(Path.Combine(path, "RebalanceTiers.xml"));

            if (config.DebugMode)
            {
                Globals1.ItemData[(int)Item.NPCSpawn].IsValid = true;
                Globals1.ItemData[(int)Item.NPCSpawn].IsEnabled = true;
            }
            else
            {
                Globals1.ItemData[(int)Item.NPCSpawn].IsValid = false;
                Globals1.ItemData[(int)Item.NPCSpawn].IsEnabled = false;
            }

            npcLevelData[GameDifficulty.Easy] = FileSystem.Deserialize<ActorLevelDataXML[]>(Path.Combine(path, "Easy\\ActorLevelData.xml"));
            npcLevelData[GameDifficulty.Normal] = FileSystem.Deserialize<ActorLevelDataXML[]>(Path.Combine(path, "Normal\\ActorLevelData.xml"));
            npcLevelData[GameDifficulty.Legendary] = FileSystem.Deserialize<ActorLevelDataXML[]>(Path.Combine(path, "Legendary\\ActorLevelData.xml"));

            npcPhysicsData[GameDifficulty.Easy] = FileSystem.Deserialize<ActorPhysicsDataXML[]>(Path.Combine(path, "Easy\\ActorPhysicsData.xml"));
            npcPhysicsData[GameDifficulty.Normal] = FileSystem.Deserialize<ActorPhysicsDataXML[]>(Path.Combine(path, "Normal\\ActorPhysicsData.xml"));
            npcPhysicsData[GameDifficulty.Legendary] = FileSystem.Deserialize<ActorPhysicsDataXML[]>(Path.Combine(path, "Legendary\\ActorPhysicsData.xml"));
        }
        void ITMPlugin.InitializeGame(ITMGame game)
        {
            this.game = game;
            world = game.World;
            npcManager = world.NpcManager;

            game.AddEventItemSwing(Item.AestusFlask, new Action<Item, ITMHand>(UnlockLegendaryItems));

            rebalanceData = Rebalance.LoadData(rootPath + world.WorldPath + "rebalance.dat");

            prevDifficulty = world.IsPeacefulDifficulty ? GameDifficulty.Peaceful :
                world.IsEasyDifficulty ? GameDifficulty.Easy :
                world.IsNormalDifficulty ? GameDifficulty.Normal :
                GameDifficulty.Legendary;

            SetActorData(prevDifficulty);
            Rebalance.UpdateSpawns(rebalanceData.HighestTier);

            game.GetAllPlayers(players);

            devTextPos = new Vector2(viewportSize.X - 10 - (font.MeasureString(indevText).X * config.DebugTextScale), viewportSize.Y - 10 - (font.MeasureString(indevText).Y * config.DebugTextScale));
            debugTextPos = new Vector2(viewportSize.X - 10 - (font.MeasureString(indevText).X * config.DebugTextScale), viewportSize.Y - 10 - (font.MeasureString(indevText).Y * config.DebugTextScale) - (font.MeasureString(debugText).Y * config.DebugTextScale));
        }
        void SetActorData(GameDifficulty difficulty)
        {
            if (difficulty == GameDifficulty.Peaceful) difficulty = GameDifficulty.Easy;
            Rebalance.SetNPCData(npcLevelData[difficulty]);
            Rebalance.SetNPCData(npcPhysicsData[difficulty]);
        }
        void ITMPlugin.UnloadMod() { }
        bool ITMPlugin.HandleInput(ITMPlayer player)
        {
            if (config.DebugMode)
            {
                if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.R))
                {
                    game.SendTextMessage("Reset Tier", player, null, false, false);
                    rebalanceData.HighestTier = 0;
                    return true;
                }
                else if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.T))
                {
                    game.SendTextMessage($"Current Tier: {rebalanceData.HighestTier}", player, null, false, false);
                    return true;
                }
                else if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.L))
                {
                    if (rebalanceData.LegendaryItems)
                    {
                        game.SendTextMessage("Locked Legendary Items", player, null, false, false);
                        rebalanceData.LegendaryItems = false;
                        return true;
                    }
                    game.SendTextMessage("Unlocked Legendary Items", player, null, false, false);
                    rebalanceData.LegendaryItems = true;
                    return true;
                }
                else if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.K))
                {
                    if (rebalanceData.AmuletOfFlight)
                    {
                        game.SendTextMessage("Locked AoF", player, null, false, false);
                        rebalanceData.AmuletOfFlight = false;
                        return true;
                    }
                    game.SendTextMessage("Unlocked AoF", player, null, false, false);
                    rebalanceData.AmuletOfFlight = true;
                    return true;
                }
            }
            return false;
        }
        void ITMPlugin.Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CoreGlobals.GraphicsDevice.Viewport.Width != viewportSize.X || CoreGlobals.GraphicsDevice.Viewport.Height != viewportSize.Y)
            {
                viewportSize = new Vector2(CoreGlobals.GraphicsDevice.Viewport.Width, CoreGlobals.GraphicsDevice.Viewport.Height);
                devTextPos = new Vector2(viewportSize.X - 10 - (font.MeasureString(indevText).X * config.DebugTextScale), viewportSize.Y - 10 - (font.MeasureString(indevText).Y * config.DebugTextScale));
                debugTextPos = new Vector2(viewportSize.X - 10 - (font.MeasureString(indevText).X * config.DebugTextScale), viewportSize.Y - 10 - (font.MeasureString(indevText).Y * config.DebugTextScale) - (font.MeasureString(debugText).Y * config.DebugTextScale));
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, indevText, devTextPos, Color.White, 0, Vector2.Zero, config.DebugTextScale, SpriteEffects.None, 0);
            if (config.DebugMode) spriteBatch.DrawString(font, debugText, debugTextPos, Color.White, 0, Vector2.Zero, config.DebugTextScale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        void ITMPlugin.Update()
        {
            if (world.IsPeacefulDifficulty && prevDifficulty != GameDifficulty.Peaceful)
            {
                prevDifficulty = GameDifficulty.Peaceful;
                SetActorData(GameDifficulty.Peaceful);
            }
            else if (world.IsEasyDifficulty && prevDifficulty != GameDifficulty.Easy)
            {
                prevDifficulty = GameDifficulty.Easy;
                SetActorData(GameDifficulty.Easy);
            }
            else if (world.IsNormalDifficulty && prevDifficulty != GameDifficulty.Normal)
            {
                prevDifficulty = GameDifficulty.Normal;
                SetActorData(GameDifficulty.Normal);
            }
            else if (world.IsLegendaryDifficulty && prevDifficulty != GameDifficulty.Legendary)
            {
                prevDifficulty = GameDifficulty.Legendary;
                SetActorData(GameDifficulty.Legendary);
            }

            if (config.EnableTierSpawns && world.NpcManager.NpcList.Count != npcCount)
            {
                foreach (ITMActor actor in npcManager.NpcList)
                {
                    if (EnemyActors.Contains(actor.ActorType) && !Rebalance.EnabledActors.Contains(actor.ActorType) && !actor.IsDeadOrInactiveOrDisabled)
                        npcManager.DeactivateNpc(actor);
                }
            }

            if (timer > tierUpdateInterval)
            {
                game.GetAllPlayers(players);
                if (rebalanceData.HighestTier != 7)
                {
                    int tier = rebalanceData.HighestTier;
                    foreach (ITMPlayer player in players)
                    {
                        foreach (InventoryItem invItem in player.Inventory.Items)
                        {
                            Item item = invItem.ItemID;
                            int i = 0;
                            foreach (RebalanceTierXML tierData in Tiers)
                            {
                                if (tier <= i && tierData.Items.Contains(item)) tier = i;
                                i++;
                            }
                        }
                    }
                    if (rebalanceData.HighestTier != tier)
                    {
                        rebalanceData.HighestTier = tier;
                        Rebalance.UpdateSpawns(tier);
                    }
                }
                timer = 0;
            }

            npcCount = world.NpcManager.NpcList.Count;
            timer += Services.ElapsedTime;
        }
        void ITMPlugin.Update(ITMPlayer player)
        {
            if (!devMessageShown)
            {
                game.RunSingleScriptCommand("MessageBox [Survival Rebalance Development Build. Some features may be unoptimized or just not work. If you encounter any problems, let me know.\nDiscord: Dave The Monitor#6212]", player);
                devMessageShown = true;
            }
            if (config.LockAmuletOfFlight && !rebalanceData.AmuletOfFlight && player.IsItemEquipped(Item.AmuletOfFlight))
            {
                bool unequipped = player.UnequipToInventory(EquipIndex.Neck);
                if (!unequipped) player.DropItem((int)EquipIndex.Neck);
                CoreGlobals.Message.ShowMessage(string.Format(itemRejection, "amulet"), config.MessageVelocity, config.MessageTime, config.MessageScale, config.RejectionMessageColor);
            }
            if (config.LockLegendaryWeapons && !rebalanceData.LegendaryItems && (config.LegendaryWeapons.Contains(player.LeftHand.ItemID) || config.LegendaryWeapons.Contains(player.RightHand.ItemID)))
            {
                int index = config.LegendaryWeapons.Contains(player.LeftHand.ItemID) ? player.LeftHand.HandIndex : player.RightHand.HandIndex;
                player.DropItem(index);
                CoreGlobals.Message.ShowMessage(string.Format(itemRejection, "legendary weapon"), config.MessageVelocity, config.MessageTime, config.MessageScale, config.RejectionMessageColor);
            }
            else if (config.LockAllLegendaryItems && !rebalanceData.LegendaryItems)
                foreach (Item item in config.LegendaryItems)
                {
                    if (player.IsItemEquipped(item))
                    {
                        int index = player.Inventory.FindItem(item);
                        EquipIndex equipIndex = Globals1.ItemTypeData[(int)item].Equip;
                        bool equippedInHand = equipIndex == EquipIndex.RightHand || equipIndex == EquipIndex.LeftHand;
                        bool unequipped = player.UnequipToInventory(equippedInHand ? EquipIndex.RightHand : equipIndex);
                        if (!unequipped && equippedInHand) unequipped = player.UnequipToInventory(EquipIndex.LeftHand);
                        if (!unequipped) player.DropItem(index);
                        CoreGlobals.Message.ShowMessage(string.Format(itemRejection, item), config.MessageVelocity, config.MessageTime, config.MessageScale, config.RejectionMessageColor);
                    }
                }
        }

        void UnlockLegendaryItems(Item item, ITMHand hand)
        {
            if (hand.Player != null && !rebalanceData.LegendaryItems)
            {
                rebalanceData.LegendaryItems = true;
                CoreGlobals.Message.ShowMessage("The flask cracks as Aestus essence is released...", config.MessageVelocity / 2, config.MessageTime * 2, config.MessageScale, config.AestusMessageColor);
                CoreGlobals.Message.ShowMessage("\n\nYou can feel an immense power radiating from underground...", config.MessageVelocity / 2, config.MessageTime * 2, config.MessageScale, config.AestusMessageColor);
                hand.Player.Inventory.DecrementItem(item, 1);
            }
        }
    }
}