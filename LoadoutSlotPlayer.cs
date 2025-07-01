using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LoadoutLock
{
    internal class LoadoutSlotPlayer : ModPlayer
    {
        //public EquipmentLoadout[] ExtraLoadouts;
        //public int SavedSlot;

        public override void Initialize()
        {
            //int extralsot = ModContent.GetInstance<LoadoutConfig>().ExtraLoadoutSlot;
            //if (Player.Loadouts.Length < 3 + extralsot)
            //{
            //    EquipmentLoadout[] temp = new EquipmentLoadout[Player.Loadouts.Length + extralsot];
            //    Array.Copy(Player.Loadouts, temp, Player.Loadouts.Length);
            //    Player.Loadouts = temp;
            //    for (int i = 0; i < Player.Loadouts.Length; i++)
            //    {
            //        Player.Loadouts[i] = new EquipmentLoadout();
            //    }
            //}
            //if (Player.name != "")
            //{
            //    PlayerFileData.CreateAndSave(Player);
            //}
            //if(LoadoutLock.SavePlayerMethod != null)
            //{
            //    var tag = LoadoutLock.SavePlayerMethod.Invoke(null, new object[] { Player });
            //}
            base.Initialize();
        }
        public override void Load()
        {
            //int extralsot = ModContent.GetInstance<LoadoutConfig>().ExtraLoadoutSlot;
            //ExtraLoadouts = new EquipmentLoadout[extralsot];
            //for(int i = 0; i < extralsot; i++)
            //{
            //    ExtraLoadouts[i] = new EquipmentLoadout();
            //}
            base.Load();
        }
        public override void LoadData(TagCompound tag)
        {
            //if(tag.TryGet<int>("SavedSlot",out int slot))
            //{
            //    SavedSlot = slot;
            //}
            //int extralsot = ModContent.GetInstance<LoadoutConfig>().ExtraLoadoutSlot;
            //if (Player.Loadouts.Length < 3 + extralsot)
            //{
            //    EquipmentLoadout[] temp = new EquipmentLoadout[Player.Loadouts.Length + extralsot];
            //    Array.Copy(Player.Loadouts, temp, Player.Loadouts.Length);
            //    Player.Loadouts = temp;
            //    for (int i = 3; i < Player.Loadouts.Length; i++)
            //    {
            //        Player.Loadouts[i] = new EquipmentLoadout();
            //    }
            //}
            //int savedslot = Player.GetModPlayer<LoadoutSlotPlayer>().SavedSlot;
            //for (int i = 0; i < Math.Min(SavedSlot, extralsot); i++)
            //{
            //    LockUtils.LoadInventory(Player.Loadouts[3 + i].Armor, tag.GetList<TagCompound>($"loadout{i + 3}Armor"));
            //    LockUtils.LoadInventory(Player.Loadouts[3 + i].Dye, tag.GetList<TagCompound>($"loadout{i + 3}Dye"));
            //}
            //if (tag.TryGet<int>("CurrentLoadoutIndex", out int index))
            //{
            //    Player.CurrentLoadoutIndex = index;
            //}
            base.LoadData(tag);
        }
        public override void SaveData(TagCompound tag)
        {
            //int extralsot = ModContent.GetInstance<LoadoutLockConfig>().ExtraLoadoutSlot;
            //tag["SavedSlot"] = extralsot;
            //tag["CurrentLoadoutIndex"] = Player.CurrentLoadoutIndex;
            base.SaveData(tag);
        }
    }
}
