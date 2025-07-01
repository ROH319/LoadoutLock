
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace LoadoutLock
{
    public class LockPlayer : ModPlayer
    {
        public bool[] LockAccessory = new bool[10];
        public int[] OriginalIndex = new int[10];
        public override void Load()
        {
            On_Player.TrySwitchingLoadout += On_Player_TrySwitchingLoadout;
            
            base.Load();
        }
        public override void Initialize()
        {
            OriginalIndex = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            base.Initialize();
        }

        private void On_Player_TrySwitchingLoadout(On_Player.orig_TrySwitchingLoadout orig, Player self, int loadoutIndex)
        {
            if (ModContent.GetInstance<LoadoutConfig>().SwitchItem)
            {
                var lockAcc = self.GetModPlayer<LockPlayer>().LockAccessory;
                var oriIndex = self.GetModPlayer<LockPlayer>().OriginalIndex;
                bool flag = self.itemTime > 0 || self.itemAnimation > 0;
                if ((self.whoAmI != Main.myPlayer || (!flag && !self.CCed && !self.dead)) && loadoutIndex != self.CurrentLoadoutIndex && loadoutIndex >= 0 && loadoutIndex < self.Loadouts.Length)
                {
                    self.Loadouts[self.CurrentLoadoutIndex].Swap(self);
                    self.Loadouts[loadoutIndex].Swap(self);
                    for(int i = 0; i < 10; i++)
                    {
                        if (lockAcc[i] && oriIndex[i] != -1)
                        {
                            self.Loadouts[self.CurrentLoadoutIndex].SwapCertainSlotWith(self.Loadouts[oriIndex[i]], i);
                            self.Loadouts[oriIndex[i]].SwapCertainSlotWith(self.Loadouts[loadoutIndex], i);
                        }
                    }
                    self.CurrentLoadoutIndex = loadoutIndex;
                    if (self.whoAmI == Main.myPlayer)
                    {
                        self.CloneLoadOuts(Main.clientPlayer);
                        Main.mouseLeftRelease = false;
                        ItemSlot.RecordLoadoutChange();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                        NetMessage.TrySendData(147, -1, -1, null, self.whoAmI, loadoutIndex);
                        ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.LoadoutChange, new ParticleOrchestraSettings
                        {
                            PositionInWorld = self.Center,
                            UniqueInfoPiece = loadoutIndex
                        }, self.whoAmI);
                    }
                }
                return;
            }
            orig(self, loadoutIndex);
        }

        public override void Unload()
        {
            On_Player.TrySwitchingLoadout -= On_Player_TrySwitchingLoadout;
            base.Unload();
        }
        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < 10; i++)
            {
                if (tag.TryGet<bool>($"LockAccessory{i}", out bool value))
                {
                    LockAccessory[i] = value;
                }
                if(tag.TryGet<int>($"OriginalIndex{i}",out int index))
                {
                    OriginalIndex[i] = index;
                }
            }
            
            base.LoadData(tag);
        }
        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < 10; i++)
            {
                tag[$"LockAccessory{i}"] = LockAccessory[i];
                tag[$"OriginalIndex{i}"] = OriginalIndex[i]; 
            }
            base.SaveData(tag);
        }
    }
}
