using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Graphics.Shaders;
using Terraria.ObjectData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DyeableAnything
{
    public static class Helpme
    {
        
        public static int GetItemStack(this Player player,int id) {
            int a = 0;
            foreach(var item in player.inventory){if(item.type == id) {a += item.stack;}}
            return a;
        }
        public static bool HasAmmo(this Player player,int id) {
            foreach (var item in player.inventory){if (item.ammo == id) {return true;}}
            return false;
        }

        public static void AddLoot(this NPC npc ,int id, int npcid , int ran = 1, int stack = 1, bool a = true) {
			if (Main.rand.Next(ran) == 0 && a) {
                if (npcid == -1) {Item.NewItem(npc.getRect(), id, stack);}
				else if (npc.type == npcid) {Item.NewItem(npc.getRect(), id, stack);}
			}
		}
        public static void Resize(this Projectile projectile, int newWidth = 0, int newHeight = 0){
            Vector2 oldCenter = projectile.Center;
            projectile.width = newWidth;
            projectile.height = newHeight;
            projectile.Center = oldCenter;
        }
        public static void AddShop(this Chest shop, ref int nextSlot, int item, int gold = -1){
            shop.item[nextSlot].SetDefaults(item);
            if (gold > -1) {shop.item[nextSlot].shopCustomPrice = gold;}
            nextSlot++;
        }
        //(this SoundEffectInstance asdf) => asdf.State == SoundState.Playing;
        public static bool AnyBoss() {
			for (int i = 0; i < Main.maxNPCs; i++){
                NPC n = Main.npc[i];
                if (n.active && (n.boss || n.type == NPCID.EaterofWorldsHead))
                {return true;}
			}
			return false;
		}
        public static void BeginNormal(this SpriteBatch spriteBatch) =>spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        public static void BeginGlow(this SpriteBatch spriteBatch) =>spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

    }
}
