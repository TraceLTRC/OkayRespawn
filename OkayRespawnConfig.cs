using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace OkayRespawn
{
    public enum NurseHealMode
    {
        Never,
        OnDeath,
        OnDeathWithBoss
    }

    public class OkayRespawnConfig : ModConfig
    {
        public static int DefaultRespawnTimer = 600;
        public static int DefaultBossTimerAddition = 600;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Nurse Options")]
        [Label("Auto Nurse Mode")]
        [Tooltip("After death, when should nurses auto heal?")]
        [OptionStrings(new string[] { "Never", "Only when fighting a boss", "Always" })]
        [DefaultValue("Only when fighting a boss")]
        public string AutoNurseMode;

        [Label("Quick Nurse Range")]
        [Tooltip("Maximum range between player and nurse to allow quick nurse healing")]
        [Range(100f, 1000f)]
        [DefaultValue(500f)]
        [Slider]
        public float QuickNurseRange;

        [Header("Respawn Options")]
        [Label("Respawn Boss Timer Scale")]
        [Tooltip("How fast the respawn timer should be compared to vanilla when a boss is active. 0% being instant, 100% being vanilla.")]
        [Range(0f, 3f)]
        [DefaultValue(2f)]
        [Slider]
        public float RespawnScaleBoss;

        [Label("Respawn Timer Scale")]
        [Tooltip("How fast the respawn timer should be compared to vanilla. 0% being instant, 100% being vanilla.")]
        [Range(0f, 3f)]
        [DefaultValue(1f)]
        [Slider]
        public float RespawnScale;

        [Label("Respawn Timer Expert Scale")]
        [Tooltip("How fast the respawn timer should be compared to vanilla in expert mode. 0% being instant, 100% being vanilla.")]
        [Range(0f, 3f)]
        [DefaultValue(1.5f)]
        [Slider]
        public float RespawnScaleExpert;

        public static NurseHealMode NurseModeFromString(string str)
        {
            switch (str)
            {
                case "Never":
                    return OkayRespawn.NurseHealMode.Never;
                case "Only when fighting a boss":
                    return OkayRespawn.NurseHealMode.OnDeathWithBoss;
                case "Always":
                    return OkayRespawn.NurseHealMode.OnDeath;
                default:
                    throw new ArgumentException($"String {str} is not a valid NurseHealMode");
            }
        }
    }
}
