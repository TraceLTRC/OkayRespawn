using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace OkayRespawn
{

	public class OkayRespawnPlayer : ModPlayer
	{
        private const float MAXIMUM_DISTANCE = 500f;

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            
        }

        public override void OnRespawn(Player player)
        {
            
            
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.NurseDistanceKeybind.JustPressed)
            {
                NurseHealMode mode = GetNurseHealMode();
                Main.NewText($"The current mode is {mode}");
            }
        }   

        private bool IsBossActive()
        {
            return Main.npc.Any(npc => npc.active && npc.boss);
        }

        private int GetNursePrice()
        {
            Player player = Main.LocalPlayer;
            
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

        private bool IsNurseNearby()
        {
            Player player = Main.LocalPlayer;
            NPC nurse = GetNurse();
            if (nurse == null) return false;

            return Vector2.Distance(player.Center, nurse.Center) > MAXIMUM_DISTANCE;
        }

        private float GetNurseDistance()
        {
            Player player = Main.LocalPlayer;
            NPC nurse = GetNurse();
            if (nurse == null) return 0;

            return Vector2.Distance(player.Center, nurse.Center);
        }

        private NPC GetNurse()
        {
            return Main.npc.FirstOrDefault(npc => npc.active && npc.type == NPCID.Nurse);
        }

        private NurseHealMode GetNurseHealMode()
        {
            return OkayRespawnConfig.NurseModeFromString(ModContent.GetInstance<OkayRespawnConfig>().nurseHealMode);
        }

        private float GetNurseQuickHealRange()
        {
            return ModContent.GetInstance<OkayRespawnConfig>().nurseQuickHealRange;
        }
    }
}