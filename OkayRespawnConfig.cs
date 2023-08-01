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
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Nurse Options")]
        [Label("Auto Nurse Mode")]
        [Tooltip("When should nurses auto heal?")]
        [OptionStrings(new string[] { "Never", "Only when fighting a boss", "Always" })]
        [DefaultValue("Only when fighting a boss")]
        public string nurseHealMode;

        [Label("Quick Nurse Range")]
        [Tooltip("Maximum range between player and nurse to allow quick nurse healing")]
        [Range(100f, 1000f)]
        [DefaultValue(500f)]
        [Slider]
        public float nurseQuickHealRange;

        [Header("Respawn Options")]
        [Label("Respawn Timer with Boss")]
        [Tooltip("How fast the respawn timer should be compared to vanilla when a boss is active. 0% being instant, 100% being vanilla.")]
        [Range(0, 200)]
        [DefaultValue(100)]
        [Slider]
        public int respawnPercBoss;

        [Label("Respawn Timer")]
        [Tooltip("How fast the respawn timer should be compared to vanilla. 0% being instant, 100% being vanilla.")]
        [Range(0, 200)]
        [DefaultValue(100)]
        [Slider]
        public int respawnPerc;

        public static NurseHealMode NurseModeFromString(string str)
        {
            switch (str)
            {
                case "Never":
                    return NurseHealMode.Never;
                case "Only when fighting a boss":
                    return NurseHealMode.OnDeathWithBoss;
                case "Always":
                    return NurseHealMode.OnDeath;
                default:
                    throw new ArgumentException($"String {str} is not a valid NurseHealMode");
            }
        }
    }
}
