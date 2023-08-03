using Terraria.ModLoader;

namespace OkayRespawn
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind NurseDistanceKeybind { get; private set; }

        public override void Load()
        {
            NurseDistanceKeybind = KeybindLoader.RegisterKeybind(Mod, "Quick Nurse", "K");
        }

        public override void Unload()
        {
            NurseDistanceKeybind = null;
        }
    }
}
