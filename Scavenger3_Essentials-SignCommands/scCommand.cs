using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using MySqlX.XDevAPI.Common;
using static Mysqlx.Datatypes.Scalar.Types;
using System.Linq;

namespace Scavenger3_Essentials_SignCommands
{
    public class scCommand
    {
        public string command { get; set; }
        public List<string> args { get; set; }

        public scCommand(scSign parent, string command, List<string> args)
        {
            this.command = command.ToLower();
            this.args = args;
        }

        #region AmIValid
        public bool AmIValid()
        {
            switch (command)
            {
                case "time":
                case "heal":
                case "show":
                case "damage":
                case "boss":
                case "spawnmob":
                case "warp":
                case "item":
                case "buff":
                case "command":
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region ExecuteCommand
        public void ExecuteCommand(scPlayer sPly)
        {
            switch (command)
            {
                case "time":
                    CMDtime(sPly, args);
                    break;
                case "heal":
                    CMDheal(sPly, args);
                    break;
                case "show":
                    CMDshow(sPly, args);
                    break;
                case "damage":
                    CMDdamage(sPly, args);
                    break;
                case "boss":
                    CMDboss(sPly, args);
                    break;
                case "spawnmob":
                    CMDspawnmob(sPly, args);
                    break;
                case "warp":
                    CMDwarp(sPly, args);
                    break;
                case "item":
                    CMDitem(sPly, args);
                    break;
                case "buff":
                    CMDbuff(sPly, args);
                    break;
                case "command":
                    CMDcommand(sPly, args);
                    break;
            }
        }
        #endregion

        #region CMDtime
        public static void CMDtime(scPlayer sPly, List<string> args)
        {
            if (args.Count < 1) return;
            string time = args[0].ToLower();
            switch (time)
            {
                case "day":
                    TSPlayer.Server.SetTime(true, 150.0);
                    break;
                case "night":
                    TSPlayer.Server.SetTime(false, 0.0);
                    break;
                case "dusk":
                    TSPlayer.Server.SetTime(false, 0.0);
                    break;
                case "noon":
                    TSPlayer.Server.SetTime(true, 27000.0);
                    break;
                case "midnight":
                    TSPlayer.Server.SetTime(false, 16200.0);
                    break;
                case "fullmoon":
                    TSPlayer.Server.SetFullMoon();
                    break;
                case "bloodmoon":
                    TSPlayer.Server.SetBloodMoon(true);
                    break;
            }
        }
        #endregion

        #region CMDheal
        public static void CMDheal(scPlayer sPly, List<string> args)
        {
            Item heart = TShock.Utils.GetItemById(58);
            Item star = TShock.Utils.GetItemById(184);
            for (int ic = 0; ic < 20; ic++)
                sPly.TSPlayer.GiveItem(heart.type, heart.stack, heart.prefix);
            for (int ic = 0; ic < 10; ic++)
                sPly.TSPlayer.GiveItem(star.type, star.stack, star.prefix);
        }
        #endregion

        #region CMDshow
        public static void CMDshow(scPlayer sPly, List<string> args)
        {
            if (args.Count < 1) return;
            string file = args[0].ToLower();
            switch (file)
            {
                case "motd":
                    ShowFileToUser(sPly.TSPlayer, "motd.txt");
                    break;
                case "rules":
                    ShowFileToUser(sPly.TSPlayer, "rules.txt");
                    break;
                case "playing":
                    Commands.HandleCommand(sPly.TSPlayer, "/playing");
                    break;
                default:
                    {
                        if (File.Exists(args[0]))
                            ShowFileToUser(sPly.TSPlayer, args[0]);
                        else
                            sPly.TSPlayer.SendErrorMessage("Could not find file.");
                    }
                    break;
            }
        }
        #endregion

        public static void ShowFileToUser(TSPlayer player, string file)
        {
            string empty = string.Empty;
            using StreamReader streamReader = new StreamReader(Path.Combine(TShock.SavePath, file));
            while ((empty = streamReader.ReadLine()) != null)
            {
                empty = empty.Replace("%map%", Main.worldName);
                empty = empty.Replace("%players%", GetPlayers());
                if (empty.Substring(0, 1) == "%" && empty.Substring(12, 1) == "%")
                {
                    string text = empty.Substring(0, 13);
                    empty = empty.Remove(0, 13);
                    float[] array = new float[3];
                    text = text.Replace("%", string.Empty);
                    string[] array2 = text.Split(',');
                    if (array2.Length == 3)
                    {
                        try
                        {
                            player.SendMessage(empty, (byte)Convert.ToInt32(array2[0]), (byte)Convert.ToInt32(array2[1]), (byte)Convert.ToInt32(array2[2]));
                        }
                        catch (Exception ex)
                        {
                            TShock.Log.Error(ex.ToString());
                            goto IL_00f4;
                        }

                        continue;
                    }
                }

                goto IL_00f4;
            IL_00f4:
                player.SendMessage(empty, Color.WhiteSmoke);
            }
            string GetPlayers()
            {
                StringBuilder stringBuilder = new StringBuilder();
                TSPlayer[] players = TShock.Players;
                foreach (TSPlayer tSPlayer in players)
                {
                    if (tSPlayer != null && tSPlayer.Active)
                    {
                        if (stringBuilder.Length != 0)
                        {
                            stringBuilder.Append(", ");
                        }

                        stringBuilder.Append(tSPlayer.Name);
                    }
                }

                return stringBuilder.ToString();
            }
        }

        #region CMDdamage
        public static void CMDdamage(scPlayer sPly, List<string> args)
        {
            int amount = 10;
            if (args.Count > 0)
                int.TryParse(args[0], out amount);
            if (amount < 1)
                amount = 10;
            sPly.TSPlayer.DamagePlayer(amount);
        }
        #endregion

        #region CMDboss
        public static void CMDboss(scPlayer sPly, List<string> args)
        {
            if (args.Count < 1) return;
            string boss = args[0].ToLower();

            int result = 1;
            if (args.Count > 1)
                int.TryParse(args[1], out result);

            result = Math.Min(result, Main.maxNPCs);
            if (result < 1)
                result = 1;

            NPC nPC = new NPC();

            switch (boss)
            {
                case "*":
                case "all":
                    {
                        int[] obj = new int[17]
                        {
                4, 13, 35, 50, 125, 126, 127, 134, 222, 245,
                262, 266, 370, 398, 439, 636, 657
                        };
                        TSPlayer.Server.SetTime(dayTime: false, 0.0);
                        int[] array = obj;
                        foreach (int type in array)
                        {
                            nPC.SetDefaults(type);
                            TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                        }
                        break;
                    }
                case "brain":
                case "brain of cthulhu":
                case "boc":
                    nPC.SetDefaults(266);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "destroyer":
                    nPC.SetDefaults(134);
                    TSPlayer.Server.SetTime(dayTime: false, 0.0);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "duke":
                case "duke fishron":
                case "fishron":
                    nPC.SetDefaults(370);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "eater":
                case "eater of worlds":
                case "eow":
                    nPC.SetDefaults(13);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "eye":
                case "eye of cthulhu":
                case "eoc":
                    nPC.SetDefaults(4);
                    TSPlayer.Server.SetTime(dayTime: false, 0.0);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "golem":
                    nPC.SetDefaults(245);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "king":
                case "king slime":
                case "ks":
                    nPC.SetDefaults(50);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "plantera":
                    nPC.SetDefaults(262);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "prime":
                case "skeletron prime":
                    nPC.SetDefaults(127);
                    TSPlayer.Server.SetTime(dayTime: false, 0.0);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "queen bee":
                case "qb":
                    nPC.SetDefaults(222);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "skeletron":
                    nPC.SetDefaults(35);
                    TSPlayer.Server.SetTime(dayTime: false, 0.0);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "twins":
                    TSPlayer.Server.SetTime(dayTime: false, 0.0);
                    nPC.SetDefaults(125);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    nPC.SetDefaults(126);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "wof":
                case "wall of flesh":
                    if (Main.wofNPCIndex != -1)
                    {
                        return;
                    }

                    if (sPly.TSPlayer.Y / 16f < (float)(Main.maxTilesY - 205))
                    {
                        return;
                    }

                    NPC.SpawnWOF(new Vector2(sPly.TSPlayer.X, sPly.TSPlayer.Y));
                    break;
                case "moon":
                case "moon lord":
                case "ml":
                    nPC.SetDefaults(398);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "empress":
                case "empress of light":
                case "eol":
                    nPC.SetDefaults(636);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "queen slime":
                case "qs":
                    nPC.SetDefaults(657);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "lunatic":
                case "lunatic cultist":
                case "cultist":
                case "lc":
                    nPC.SetDefaults(439);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "betsy":
                    nPC.SetDefaults(551);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "flying dutchman":
                case "flying":
                case "dutchman":
                    nPC.SetDefaults(491);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "mourning wood":
                    nPC.SetDefaults(325);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "pumpking":
                    nPC.SetDefaults(327);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "everscream":
                    nPC.SetDefaults(344);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "santa-nk1":
                case "santa":
                    nPC.SetDefaults(346);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "ice queen":
                    nPC.SetDefaults(345);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "martian saucer":
                    nPC.SetDefaults(392);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "solar pillar":
                    nPC.SetDefaults(517);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "nebula pillar":
                    nPC.SetDefaults(507);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "vortex pillar":
                    nPC.SetDefaults(422);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "stardust pillar":
                    nPC.SetDefaults(493);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                case "deerclops":
                    nPC.SetDefaults(668);
                    TSPlayer.Server.SpawnNPC(nPC.type, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY);
                    break;
                default:
                    return;
            }
        }
        #endregion

        #region CMDspawnmob
        public static void CMDspawnmob(scPlayer sPly, List<string> args)
        {
            int result = 1;
            if (args.Count == 2 && !int.TryParse(args[1], out result))
            {
                return;
            }

            result = Math.Min(result, 200);
            List<NPC> nPCByIdOrName = TShock.Utils.GetNPCByIdOrName(args[0]);
            if (nPCByIdOrName.Count == 0)
            {
                return;
            }

            if (nPCByIdOrName.Count > 1)
            {
                return;
            }

            NPC nPC = nPCByIdOrName[0];
            if (nPC.type >= 1 && nPC.type < NPCID.Count && nPC.type != 113)
            {
                TSPlayer.Server.SpawnNPC(nPC.netID, nPC.FullName, result, sPly.TSPlayer.TileX, sPly.TSPlayer.TileY, 50, 20);
            }
            else if (nPC.type == 113)
            {
                if (Main.wofNPCIndex != -1 || sPly.TSPlayer.Y / 16f < (float)(Main.maxTilesY - 205))
                {
                    return;
                }

                NPC.SpawnWOF(new Vector2(sPly.TSPlayer.X, sPly.TSPlayer.Y));
            }
        }
        #endregion

        #region CMDwarp
        public static void CMDwarp(scPlayer sPly, List<string> args)
        {
            if (args.Count < 1) return;

            string WarpName = args[0];

            var Warp = TShock.Warps.Find(WarpName);

            if (Warp != null && Warp.Name != "" && Warp.Position.X > 0 && Warp.Position.Y > 0)
                sPly.TSPlayer.Teleport(Warp.Position.X * 16F, Warp.Position.Y * 16F);
        }
        #endregion

        #region CMDitem
        public static void CMDitem(scPlayer sPly, List<string> args)
        {
            if (args.Count < 1) return;
            string itemname = args[0];

            int amount = 1;
            if (args.Count > 1)
                int.TryParse(args[1], out amount);
            if (amount < 1)
                amount = 1;

            int prefix = 0;
            if (args.Count > 2)
                int.TryParse(args[2], out prefix);
            if (prefix < 0)
                prefix = 0;

            List<Item> items = TShock.Utils.GetItemByIdOrName(itemname);
            if (items.Count == 1 && items[0].type >= 1 && items[0].type < ItemID.Count)
            {
                Item item = items[0];
                if (sPly.TSPlayer.InventorySlotAvailable || item.Name.Contains("Coin"))
                {
                    if (amount == 0 || amount > item.maxStack)
                        amount = item.maxStack;
                    sPly.TSPlayer.GiveItemCheck(item.type, item.Name, amount, prefix);
                }
            }
        }
        #endregion

        #region CMDbuff
        public static void CMDbuff(scPlayer sPly, List<string> args)
        {
            if (args.Count < 1) return;
            string buffname = args[0];

            int duration = 60;
            if (args.Count > 1)
                int.TryParse(args[1], out duration);

            int buffid = -1;
            if (!int.TryParse(buffname, out buffid))
            {
                List<int> buffs = TShock.Utils.GetBuffByName(buffname);
                if (buffs.Count == 1)
                    buffid = buffs[0];
                else
                    return;
            }
            if (buffid > 0 && buffid < BuffID.Count)
            {
                if (duration < 1 || duration > short.MaxValue)
                    duration = 60;
                sPly.TSPlayer.SetBuff(buffid, duration * 60);
            }
        }
        #endregion

        #region CMDcommand
        public static void CMDcommand(scPlayer sPly, List<string> args)
        {
            if (args.Count < 1) return;
            string command = args[0];
            Commands.HandleCommand(sPly.TSPlayer, command);
        }
        #endregion
    }
}
