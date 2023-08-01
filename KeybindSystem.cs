using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace OkayRespawn
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind NurseDistanceKeybind { get; private set; }

        public override void Load()
        {
            NurseDistanceKeybind = KeybindLoader.RegisterKeybind(Mod, "Quick Nurse Heal", "K");
        }

        public override void Unload()
        {
            NurseDistanceKeybind = null;
        }
    }
}
