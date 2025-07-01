using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace LoadoutLock
{
    public class LockSystem : ModSystem
    {
        
        
    }
    public class LoadoutConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static LoadoutConfig Instance;
        [Header("LoadoutLock")]
        [DefaultValue(true)]
        public bool SwitchItem;

        [Header("ExtraLoadoutSlot")]
        [DefaultValue(0)]
        [ReloadRequired]
        public int ExtraLoadoutSlot;
    }
}
