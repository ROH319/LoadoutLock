using Humanizer;
using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Gamepad;
using Terraria.Utilities;

namespace LoadoutLock
{
    public class LoadoutLock : Mod
    {
        public static MethodInfo SavePlayerMethod;
        public static MethodInfo TryLoadDataMethod;
        public delegate bool DrawDelegate(object acc, int skip, bool modded, int slot, Color color);
        public bool DrawHook(DrawDelegate orig,object acc, int skip, bool modded, int slot, Color color)
        {
            var lockacc = Main.LocalPlayer.GetModPlayer<LockPlayer>().LockAccessory;
            var origindex = Main.LocalPlayer.GetModPlayer<LockPlayer>().OriginalIndex;
            bool flag = orig.Invoke(acc, skip, modded, slot, color);
            int yLoc = 0, xLoc = 0;
            if (!modded && Main.EquipPage == 0)
            {
                if (!SetDrawLocation(slot - 3, skip, ref xLoc, ref yLoc))
                {
                    return true;
                }
                int xloc2 = xLoc - 58 + 64 + 28 + 4;
                int yloc2 = yLoc - 2 + 20;
                bool hover = false;
                Texture2D tex = TextureAssets.HbLock[lockacc[slot] ? 0 : 1].Value;
                Rectangle rectangle = new Rectangle(xloc2, yloc2, tex.Width/2, tex.Height);
                if(rectangle.Contains(new Point(Main.mouseX,Main.mouseY)) && !PlayerInput.IgnoreMouseInterface)
                {
                    hover = true;
                    Main.LocalPlayer.mouseInterface = true;
                    if(Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        lockacc[slot] = !lockacc[slot];
                        if (lockacc[slot]) origindex[slot] = Main.LocalPlayer.CurrentLoadoutIndex;
                        //else origindex[slot] = 0;
                        SoundEngine.PlaySound(SoundID.Unlock);
                    }
                }
                
                Main.spriteBatch.Draw(tex, new Vector2(xloc2, yloc2), new Rectangle(0, 0, 22, 22), Color.White);
                if (hover)
                {
                    Main.spriteBatch.Draw(tex, new Vector2(xloc2, yloc2), new Rectangle(26, 0, 22, 22), Color.Yellow);
                }
            }
            return flag;
        }
        public override void Load()
        {
            var accessoryslotloader = typeof(Main).Assembly.GetTypes().First(t => t.Name == "AccessorySlotLoader");
            var drawMethod = accessoryslotloader.GetMethod("Draw");
            if (drawMethod is not null)
            {
                MonoModHooks.Add(drawMethod, DrawHook);
                
            }
            var setdrawlocation = accessoryslotloader.GetMethod("SetDrawLocation");
            On_EquipmentLoadout.Swap += On_EquipmentLoadout_Swap;
            On_Main.DrawInterface_27_Inventory += On_Main_DrawInterface_27_Inventory;
            //On_Main.DrawLoadoutButtons += On_Main_DrawLoadoutButtons;
            //On_ItemSlot.TryGetSlotColor += On_ItemSlot_TryGetSlotColor;
            //On_Player.Deserialize_PlayerFileData_Player_BinaryReader_int_refBoolean += On_Player_Deserialize_PlayerFileData_Player_BinaryReader_int_refBoolean;
            //On_Player.Deserialize_PlayerFileData_Player_BinaryReader_TagCompound_int_refBoolean += On_Player_Deserialize_PlayerFileData_Player_BinaryReader_TagCompound_int_refBoolean;
            //On_Player.Serialize += On_Player_Serialize;
            //Type playertype = typeof(Player);
            //ConstructorInfo[] constructorInfo = playertype.GetConstructors();
            //MonoModHooks.Add(constructorInfo[0],
            //var player = typeof(Main).Assembly.GetTypes().First(t => t.Name == "Player");
            
            //var savedata = player.GetMethod("SavePlayer", BindingFlags.Static | BindingFlags.NonPublic);
            //SavePlayerMethod = savedata;
            //var playerio = typeof(Main).Assembly.GetTypes().First(t => t.Name == "PlayerIO");
            //var loadloadout = playerio.GetMethod("LoadLoadouts", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            //if (loadloadout is not null)
            //{
            //    MonoModHooks.Add(loadloadout, LoadLoadoutHook);
            //}
            //var loadplayer = playerio.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic);
            //if(loadplayer is not null)
            //{
            //    MonoModHooks.Add(loadplayer, LoadPlayerHook);
            //}
            //var tryloaddata = playerio.GetMethod("TryLoadData", BindingFlags.Static);
            //if(tryloaddata is not null)
            //{
            //    TryLoadDataMethod = tryloaddata;
            //}
            base.Load();
        }

        public override void Unload()
        {
            On_EquipmentLoadout.Swap -= On_EquipmentLoadout_Swap;
            On_Main.DrawInterface_27_Inventory -= On_Main_DrawInterface_27_Inventory;
            //On_Main.DrawLoadoutButtons -= On_Main_DrawLoadoutButtons;
            //On_ItemSlot.TryGetSlotColor -= On_ItemSlot_TryGetSlotColor;
            //On_Player.Deserialize_PlayerFileData_Player_BinaryReader_int_refBoolean -= On_Player_Deserialize_PlayerFileData_Player_BinaryReader_int_refBoolean;
            //On_Player.Deserialize_PlayerFileData_Player_BinaryReader_TagCompound_int_refBoolean -= On_Player_Deserialize_PlayerFileData_Player_BinaryReader_TagCompound_int_refBoolean;
            //On_Player.Serialize -= On_Player_Serialize;
            base.Unload();
        }
        private void On_Main_DrawInterface_27_Inventory(On_Main.orig_DrawInterface_27_Inventory orig, Main self)
        {
            orig(self);
            if (Main.playerInventory && Main.EquipPage == 0)
            {
                int xLoc = 0, yLoc = 0; 
                var lockacc = Main.LocalPlayer.GetModPlayer<LockPlayer>().LockAccessory;
                for (int i = 0; i < 3; i++)
                {
                    yLoc = (int)((float)(AccessorySlotLoader.DrawVerticalAlignment) + (float)(i * 80) * Main.inventoryScale);
                    xLoc = Main.screenWidth - 64 - 28;

                    int xloc2 = xLoc - 58 + 64 + 28 + 4;
                    int yloc2 = yLoc - 2 + 20;
                    bool hover = false;
                    Texture2D tex = TextureAssets.HbLock[lockacc[i] ? 0 : 1].Value;
                    Rectangle rectangle = new Rectangle(xloc2, yloc2, tex.Width / 2, tex.Height);
                    if (rectangle.Contains(new Point(Main.mouseX, Main.mouseY)) && !PlayerInput.IgnoreMouseInterface)
                    {
                        hover = true;
                        Main.LocalPlayer.mouseInterface = true;
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            lockacc[i] = !lockacc[i];
                            SoundEngine.PlaySound(SoundID.Unlock);
                        }
                    }

                    Main.spriteBatch.Draw(tex, new Vector2(xloc2, yloc2), new Rectangle(0, 0, 22, 22), Color.White);
                    if (hover)
                    {
                        Main.spriteBatch.Draw(tex, new Vector2(xloc2, yloc2), new Rectangle(26, 0, 22, 22), Color.Yellow);
                    }
                }
            }
        }

        private void On_EquipmentLoadout_Swap(On_EquipmentLoadout.orig_Swap orig, EquipmentLoadout self, Player player)
        {
            var lockacc = player.GetModPlayer<LockPlayer>().LockAccessory;
            Item[] armor = player.armor;
            for (int i = 0; i < armor.Length; i++)
            {
                if (!lockacc[i > 9 ? i - 10 : i]) Utils.Swap(ref armor[i], ref self.Armor[i]);
            }

            Item[] dye = player.dye;
            for (int j = 0; j < dye.Length; j++)
            {
                if (!lockacc[j]) Utils.Swap(ref dye[j], ref self.Dye[j]);
            }

            bool[] hideVisibleAccessory = player.hideVisibleAccessory;
            for (int k = 0; k < hideVisibleAccessory.Length; k++)
            {
                if (!lockacc[k]) Utils.Swap(ref hideVisibleAccessory[k], ref self.Hide[k]);
            }
        }
        public int GetAccessorySlotPerColumn()
        {
            float minimumClearance = AccessorySlotLoader.DrawVerticalAlignment + 2 * 56 * Main.inventoryScale + 4;
            return (int)((Main.screenHeight - minimumClearance) / (56 * Main.inventoryScale) - 1.8f);
        }
        public bool SetDrawLocation(int trueSlot, int skip, ref int xLoc,ref int yLoc)
        {
            int accessoryPerColumn = GetAccessorySlotPerColumn();
            int xColumn = (int)(trueSlot / accessoryPerColumn);
            int yRow = trueSlot % accessoryPerColumn;
            int row = yRow, tempSlot = trueSlot, col = xColumn;
            if (skip > 0)
            {
                tempSlot -= skip;
                row = tempSlot % accessoryPerColumn;
                col = tempSlot / accessoryPerColumn;
            }

            yLoc = (int)((float)(AccessorySlotLoader.DrawVerticalAlignment) + (float)((row +3) * 56) * Main.inventoryScale) + 4;
            if (col > 0)
            {
                xLoc = Main.screenWidth - 64 - 28 - 47 * 3 * col - 50;
            }
            else
            {
                xLoc = Main.screenWidth - 64 - 28 - 47 * 3 * col;
            }
            return true;
        }
        public static int GetHoveredIndex()
        {
            var main = typeof(Main);
            var hover = main.GetField("_lastHoveredLoadoutIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (int)hover.GetValue(main);
        }
        public static void SetHoveredIndex(int index)
        {
            var main = typeof(Main);
            var hover = main.GetField("_lastHoveredLoadoutIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            hover.SetValue(main, index);
        }

        #region Wasted

        private bool On_ItemSlot_TryGetSlotColor(On_ItemSlot.orig_TryGetSlotColor orig, int loadoutIndex, int context, out Color color)
        {
            loadoutIndex %= 3;
            return orig(loadoutIndex, context, out color);
        }

        private void On_Main_DrawLoadoutButtons(On_Main.orig_DrawLoadoutButtons orig, int inventoryTop, bool demonHeartSlotAvailable, bool masterModeSlotAvailable)
        {
            int accSlot = 10;
            Player player = Main.player[Main.myPlayer];
            if (!demonHeartSlotAvailable)
                accSlot--;

            if (!masterModeSlotAvailable)
                accSlot--;

            int x = Main.screenWidth - 58 + 14;
            int num2 = (int)((float)(inventoryTop - 2) + 0f * Main.inventoryScale);
            int num3 = (int)((float)(inventoryTop - 2) + (float)(accSlot * 56) * Main.inventoryScale);
            Texture2D value = TextureAssets.Extra[259].Value;
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(x, num2 + 2, 4, num3 - num2);
            //ItemSlot.GetLoadoutColor(player.CurrentLoadoutIndex);
            int num4 = player.Loadouts.Length;
            int num5 = 32;
            int num6 = 4;
            int num7 = -1;
            for (int i = 0; i < num4; i++)
            {
                Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(rectangle.X + rectangle.Width, rectangle.Y + (num5 + num6) * i, 32, num5);
                Microsoft.Xna.Framework.Color loadoutColor = ItemSlot.GetLoadoutColor(i);
                int frameX = ((i == player.CurrentLoadoutIndex) ? 1 : 0);
                bool flag = false;
                if (rectangle2.Contains(Main.MouseScreen.ToPoint()))
                {
                    flag = true;
                    //loadoutColor = Microsoft.Xna.Framework.Color.Lerp(loadoutColor, Microsoft.Xna.Framework.Color.White, 0.8f);
                    loadoutColor = Color.Blue;
                    player.mouseInterface = true;
                    if (!Main.mouseText)
                    {
                        Main.instance.MouseText(Language.GetTextValue("UI.Loadout" + (i + 1)), 0, 0);
                        Main.mouseText = true;
                    }

                    if (Main.mouseLeft && Main.mouseLeftRelease)
                        player.TrySwitchingLoadout(i);
                }

                Microsoft.Xna.Framework.Rectangle rectangle3 = value.Frame(3, 3, frameX, i % 3);
                Main.spriteBatch.Draw(value, rectangle2.Center.ToVector2(), rectangle3, Microsoft.Xna.Framework.Color.White, 0f, rectangle3.Size() / 2f, 1f, SpriteEffects.None, 0f);
                if (flag)
                {
                    rectangle3 = value.Frame(3, 3, 2, i % 3);
                    Main.spriteBatch.Draw(value, rectangle2.Center.ToVector2(), rectangle3, Main.OurFavoriteColor, 0f, rectangle3.Size() / 2f, 1f, SpriteEffects.None, 0f);
                }

                UILinkPointNavigator.SetPosition(312 + i > 3 ? 3 : i, rectangle2.Center.ToVector2());
            }

            if (GetHoveredIndex() != num7)
            {
                SetHoveredIndex(num7);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        public delegate void LoadPlayerDelegate(Player player, TagCompound tag);
        //public void LoadPlayerHook(LoadPlayerDelegate orig, Player player, TagCompound tag)
        //{
        //    orig(player, tag);
        //    if (player.name != "")
        //    {
        //        int extralsot = ModContent.GetInstance<LoadoutLockConfig>().ExtraLoadoutSlot;
        //        int savedslot = player.GetModPlayer<LoadoutSlotPlayer>().SavedSlot;
        //        if (player.Loadouts.Length < 3 + extralsot)
        //        {
        //            EquipmentLoadout[] temp = new EquipmentLoadout[player.Loadouts.Length + extralsot];
        //            Array.Copy(player.Loadouts, temp, player.Loadouts.Length);
        //            player.Loadouts = temp;
        //            for (int i = 3 + savedslot; i < player.Loadouts.Length; i++)
        //            {
        //                player.Loadouts[i] = new EquipmentLoadout();
        //            }
        //        }
        //        for(int i = 0; i < player.Loadouts.Length; i++)
        //        {
        //            if (player.Loadouts[i] == null) player.Loadouts[i] = new EquipmentLoadout();
        //        }
        //        int a = player.Loadouts.Length;
        //        int b = 0;
        //        //for (int i = 0; i < Math.Min(savedslot, extralsot); i++)
        //        //{
        //        //    LockUtils.LoadInventory(player.Loadouts[3 + i].Armor, tag.GetList<TagCompound>($"loadout{i + 3}Armor"));
        //        //    LockUtils.LoadInventory(player.Loadouts[3 + i].Dye, tag.GetList<TagCompound>($"loadout{i + 3}Dye"));
        //        //}
        //        //if (tag.GetList<TagCompound>("modData").First(t => t.GetString("mod") == "LoadoutLock").GetCompound("data").TryGet<int>("CurrentLoadoutIndex", out int index))
        //        //{
        //        //    player.CurrentLoadoutIndex = index;
        //        //}
        //    }
        //}
        public delegate void LoadLoadoutDelegate(EquipmentLoadout[] loadouts, TagCompound tag);
        public void LoadLoadoutHook(LoadLoadoutDelegate orig, EquipmentLoadout[] loadouts, TagCompound tag)
        {
            orig(loadouts, tag);
        }

        //private void On_Player_Serialize(On_Player.orig_Serialize orig, Terraria.IO.PlayerFileData playerFile, Player newPlayer, System.IO.BinaryWriter fileIO)
        //{
        //    orig(playerFile, newPlayer, fileIO);
        //    int extraslot = LoadoutLockConfig.Instance.ExtraLoadoutSlot;
        //    int previousslot = newPlayer.Loadouts.Length;
        //    if(previousslot < 3 + extraslot)
        //    {
        //        EquipmentLoadout[] temp = new EquipmentLoadout[previousslot + extraslot];
        //        Array.Copy(newPlayer.Loadouts, temp, previousslot);
        //        newPlayer.Loadouts = temp;
        //        for (int i = previousslot; i < newPlayer.Loadouts.Length; i++)
        //        {
        //            newPlayer.Loadouts[i] = new EquipmentLoadout();
        //        }
        //    }
        //}

        private void On_Player_Deserialize_PlayerFileData_Player_BinaryReader_TagCompound_int_refBoolean(On_Player.orig_Deserialize_PlayerFileData_Player_BinaryReader_TagCompound_int_refBoolean orig, Terraria.IO.PlayerFileData data, Player newPlayer, System.IO.BinaryReader fileIO, TagCompound modData, int release, out bool gotToReadName)
        {
            int a = newPlayer.Loadouts.Length;
            int b = 0;
            orig(data, newPlayer, fileIO, modData, release, out gotToReadName);
            int c = newPlayer.Loadouts.Length;
            int d = 0;
        }

        private void On_Player_Deserialize_PlayerFileData_Player_BinaryReader_int_refBoolean(On_Player.orig_Deserialize_PlayerFileData_Player_BinaryReader_int_refBoolean orig, Terraria.IO.PlayerFileData data, Player newPlayer, System.IO.BinaryReader fileIO, int release, out bool gotToReadName)
        {
            if (!LockUtils.TryLoadData(data.Path, data.IsCloudSave, out var tag))
            {
                //tag = null;
            }
            if (tag != null)
            {
                var mp = newPlayer.GetModPlayer<LoadoutSlotPlayer>();
                var modlist = tag.GetList<TagCompound>("modData");
                var moddata = modlist.FirstOrDefault(t => t.GetString("mod") == "LoadoutLock" && t.GetString("name") == "LoadoutSlotPlayer")?.GetCompound("data");
                if (moddata != default(TagCompound) && moddata != null)
                {
                    mp.LoadData(moddata);
                    //int savedslot = mp.SavedSlot;
                    //if (newPlayer.Loadouts.Length < 3 + savedslot)
                    //{
                    //    EquipmentLoadout[] temp = new EquipmentLoadout[newPlayer.Loadouts.Length + savedslot];
                    //    newPlayer.Loadouts = temp;
                    //    for (int i = 0; i < newPlayer.Loadouts.Length; i++)
                    //    {
                    //        newPlayer.Loadouts[i] = new EquipmentLoadout();
                    //    }
                    //}
                }
            }
            orig(data, newPlayer, fileIO, release, out gotToReadName);
        }

        #endregion
    }
}