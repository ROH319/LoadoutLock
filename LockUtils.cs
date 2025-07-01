using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace LoadoutLock
{
    public static class LockUtils
    {
        public static void SwapCertainSlotWith(this EquipmentLoadout eq, EquipmentLoadout target, int index)
        {
            int altindex = 0;
            if (index > 9) altindex = index - 10;
            else altindex = index + 10;
            Utils.Swap<Item>(ref eq.Armor[index], ref target.Armor[index]);
            Utils.Swap<Item>(ref eq.Armor[altindex], ref target.Armor[altindex]);
            Utils.Swap<Item>(ref eq.Dye[index], ref target.Dye[index]);
            Utils.Swap<bool>(ref eq.Hide[index], ref target.Hide[index]);
        }
        public static void LoadLoadouts(EquipmentLoadout[] loadouts, TagCompound loadoutTag)
        {
            for (int i = 0; i < loadouts.Length; i++)
            {
                LoadInventory(loadouts[i].Armor, loadoutTag.GetList<TagCompound>($"loadout{i}Armor"));
                LoadInventory(loadouts[i].Dye, loadoutTag.GetList<TagCompound>($"loadout{i}Dye"));
            }
        }
        public static void LoadInventory(Item[] inv, IList<TagCompound> list)
        {
            foreach (var tag in list)
                inv[tag.GetShort("slot")] = ItemIO.Load(tag);
        }
        public static bool TryLoadData(string path, bool isCloudSave, out TagCompound tag)
        {
            path = Path.ChangeExtension(path, ".tplr");
            tag = new TagCompound();

            if (!FileUtilities.Exists(path, isCloudSave))
                return false;

            byte[] buf = FileUtilities.ReadAllBytes(path, isCloudSave);

            if (buf[0] != 0x1F || buf[1] != 0x8B)
            {
                throw new IOException($"{Path.GetFileName(path)}:: File Corrupted during Last Save Step. Aborting... ERROR: Missing NBT Header");
            }

            tag = TagIO.FromStream(buf.ToMemoryStream());
            return true;
        }
        public static void CloneLoadOuts(this Player player, Player clonePlayer)
        {
            Item[] array = player.armor;
            Item[] array2 = clonePlayer.armor;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].CopyNetStateTo(array2[i]);
            }

            array = player.dye;
            array2 = clonePlayer.dye;
            for (int j = 0; j < array.Length; j++)
            {
                array[j].CopyNetStateTo(array2[j]);
            }

            for (int k = 0; k < player.Loadouts.Length; k++)
            {
                if(clonePlayer.Loadouts.Length < player.Loadouts.Length)
                {
                    //int extra = player.Loadouts.Length - clonePlayer.Loadouts.Length;
                    //Array.Resize(ref clonePlayer.Loadouts, player.Loadouts.Length);
                    clonePlayer.Loadouts = new EquipmentLoadout[player.Loadouts.Length];
                    for(int i = 0; i < clonePlayer.Loadouts.Length; i++)
                    {
                        clonePlayer.Loadouts[i] = new EquipmentLoadout();
                    }
                }
                array = player.Loadouts[k].Armor;
                array2 = clonePlayer.Loadouts[k].Armor;
                for (int l = 0; l < array.Length; l++)
                {
                    array[l].CopyNetStateTo(array2[l]);
                }

                array = player.Loadouts[k].Dye;
                array2 = clonePlayer.Loadouts[k].Dye;
                for (int m = 0; m < array.Length; m++)
                {
                    array[m].CopyNetStateTo(array2[m]);
                }
            }
        }
    }
}
