using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OkayRespawn
{

	public class OkayRespawnPlayer : ModPlayer
	{

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            OkayRespawnConfig config = GetConfigInstance();

            // Respawn handling
            int respawnTimer = (int)(OkayRespawnConfig.DefaultRespawnTimer * config.RespawnScale);
            
            // Using exact logic from vanilla
            bool isBossAlive = false;
            if (Main.netMode != 0 && !pvp)
            {
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && (Main.npc[k].boss || Main.npc[k].type == 13 || Main.npc[k].type == 14 || Main.npc[k].type == 15) && Math.Abs(Player.Center.X - Main.npc[k].Center.X) + Math.Abs(Player.Center.Y - Main.npc[k].Center.Y) < 4000f)
                    {
                        isBossAlive = true;
                        break;
                    }
                }
            }

            if (isBossAlive)
            {
                respawnTimer += (int)(OkayRespawnConfig.DefaultBossTimerAddition * GetConfigInstance().RespawnScaleBoss);
            }
            if (Main.expertMode)
            {
                respawnTimer = (int)((double)respawnTimer * GetConfigInstance().RespawnScaleExpert);
            }

            Player.respawnTimer = respawnTimer;
        }

        public override void OnRespawn(Player player)
        {
            if (player.whoAmI != Main.myPlayer) return;

            // Auto nurse on respawn
            OkayRespawnConfig config = GetConfigInstance();
            NurseHealMode healMode = OkayRespawnConfig.NurseModeFromString(config.AutoNurseMode);

            if ((healMode == NurseHealMode.OnDeathWithBoss && IsBossActive()) || healMode == NurseHealMode.OnDeath)
            {
                NPC nurse = GetNurse();
                if (nurse == null) return;

                NurseHeal(player, nurse);
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.NurseDistanceKeybind.JustPressed)
            {
                Player player = Main.LocalPlayer;
                NPC nurse = GetNurse();
                if (nurse == null) return;

                if (GetConfigInstance().QuickNurseRange > Vector2.Distance(player.Center, nurse.Center))
                {
                    NurseHeal(player, nurse);
                } 
                else
                {
                    Main.NewText("You are too far from the nurse!", Color.Red);
                }
            }
        }

        /// <summary>
        /// Heals the player as if it was healed by a nurse
        /// </summary>
        /// <param name="nurse">The NPC instance of the nurse</param>
        /// <param name="player">The plyaer to heal</param>
        /// <param name="removeDebuffs">If debuffs should be removed after heal</param>
        /// <param name="nurseReply">If the nurse should reply in chat</param>
        private void NurseHeal(Player player, NPC nurse, bool nurseReply = true)
        {
            int price = GetNursePrice(player);
            int health = player.statLifeMax2 - player.statLife;
            bool removeDebuffs = true;
            string reason = Language.GetTextValue("tModLoader.DefaultNurseCantHealChat");

            bool canHeal = PlayerLoader.ModifyNurseHeal(player, nurse, ref health, ref removeDebuffs, ref reason);
            PlayerLoader.ModifyNursePrice(player, nurse, health, removeDebuffs, ref price);

            if (!canHeal)
            {
                Main.NewText($"[Nurse]: {reason}");
            }
            else if (player.BuyItem(price))
            {
                AchievementsHelper.HandleNurseService(price);
                SoundEngine.PlaySound(in SoundID.Item4);
                player.HealEffect(health);

                if (nurseReply)
                {
                    string response = Lang.dialog(230);

                    if ((double)player.statLife < (double)player.statLifeMax2 * 0.25)
                    {
                        response = Lang.dialog(227);
                    }
                    else if ((double)player.statLife < (double)player.statLifeMax2 * 0.5)
                    {
                        response = Lang.dialog(228);
                    }
                    else if ((double)player.statLife < (double)player.statLifeMax2 * 0.75)
                    {
                        response = Lang.dialog(229);
                    }

                    Main.NewText($"[Nurse]: {response}");
                }

                player.statLife += health;

                if (removeDebuffs)
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int buffType = player.buffType[l];
                        if (Main.debuff[buffType] && player.buffTime[l] > 0 && (buffType < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[buffType]))
                        {
                            player.DelBuff(l);
                            l = -1;
                        }
                    }
                }
            }
        }

        private bool IsBossActive()
        {
            // Different from vanilla, doesn't consider range of the player.
            return Main.npc.Any(npc => npc.active && (npc.boss || npc.type == 13 || npc.type == 14 || npc.type == 15));
        }

        private int GetNursePrice(Player player)
        {
            
            int health = player.statLifeMax2 - player.statLife;
            int price = health;

            // Handle debuff cleansing
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int buffType = player.buffType[i];
                if (Main.debuff[buffType] && player.buffTime[i] > 60 && (buffType < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[buffType]))
                {
                    price += 100;
                }
            }
            
            // Handle boss price increase
            if (NPC.downedGolemBoss)
            {
                price *= 200;
            }
            else if (NPC.downedPlantBoss)
            {
                price *= 150;
            } else if (NPC.downedMechBossAny)
            {
                price *= 100;
            }
            else if (Main.hardMode)
            {
                price *= 60;
            }
            else if (NPC.downedBoss3 || NPC.downedQueenBee)
            {
                price *= 25;
            }
            else if (NPC.downedBoss2)
            {
                price *= 10;
            }
            else if (NPC.downedBoss1)
            {
                price *= 3;
            }
            if (Main.expertMode)
            {
                price *= 2;
            }

            // Happiness & Accesories
            price = (int)((double)price * player.currentShoppingSettings.PriceAdjustment);

            return price;
        }

        private NPC GetNurse()
        {
            return Main.npc.FirstOrDefault(npc => npc.active && npc.type == NPCID.Nurse);
        }

        private OkayRespawnConfig GetConfigInstance()
        {
            return ModContent.GetInstance<OkayRespawnConfig>();
        }
    }
}