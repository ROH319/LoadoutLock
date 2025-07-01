using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace LoadoutLock
{
    public class LoadoutLockConfig : ModConfig
    {

        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static LoadoutLockConfig Instance;
        [Header("LoadoutLock")]
        [DefaultValue(true)]
        public bool SwitchItem;

        public Vector2 VanillaLockOffset;

        public float VanillaLockScale;

        public Vector2 ModdedLockOffset;

        public float ModdedLockScale;

        //[Header("ExtraLoadoutSlot")]
        //[DefaultValue(0)]
        //[ReloadRequired]
        //public int ExtraLoadoutSlot;
    }
}
