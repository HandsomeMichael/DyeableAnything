using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Graphics;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.Localization; 
using Terraria.Utilities;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DyeableAnything
{
	public class DyeableAnything : Mod 
	{
		public static List<byte> dyeList = new List<byte>();
		public static List<int> dyeListType = new List<int>();
		public override void PostSetupContent() {
			for (int i = 0; i < ItemLoader.ItemCount; i++)
			{
				Item item = new Item();
				item.SetDefaults(i);
				if (item.dye > 0) {
					dyeList.Add(item.dye);
					dyeListType.Add(i);
				}
			}
		}
		public override void Unload() {
			dyeList = null;
			dyeListType = null;
		}
	}
	[Label("Dye Sprayer")]
	public class PepeConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public static PepeConfig get => ModContent.GetInstance<PepeConfig>();

		[Header("Dye Sprayer")]

		[Label("Dye sprayer Consumes Dye")]
		[Tooltip("Dye sprayer will consumes dye upon use")]
		[DefaultValue(true)]
		public bool useDye;

		[Label("Dyeable Projectile")]
		[Tooltip("Dye sprayer can dye projectiles")]
		[DefaultValue(true)]
		public bool projDye;

		[Header("Others")]

		[Label("Dye dye item")]
		[Tooltip("dye the dye item")]
		[DefaultValue(false)]
		public bool dyeDyeItem;

		[Label("Dye weak to water")]
		[Tooltip("Removes dye if got hit by water gun")]
		[DefaultValue(true)]
		public bool weakWater;

		[Label("Password")]
		[Tooltip("Insert password here")]
		[DefaultValue("Twitter")]
		public string password;
	}
	public class RandomDye : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Random Dye");
			Tooltip.SetDefault("'amongus not included'");
		}
		public override void SetDefaults() {
			item.CloneDefaults(ItemID.GelDye);
		}
		public int dye;
		public int timer;
		public override void UpdateInventory(Player player) {
			timer++;
			if (timer > 30) {
				item.dye = Main.rand.Next(DyeableAnything.dyeList);
				dye = Main.rand.Next(DyeableAnything.dyeListType);
				timer = 0;
			}
		}
		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
			GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(dye), item, null);
			return true;
		}
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			Main.spriteBatch.End(); 
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
		}
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
			Main.spriteBatch.End(); 
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
			GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(dye), item, null);
			return true;
		}
	}
	public class MagicDyer : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Magic Dyer");
			Tooltip.SetDefault("Dye your projectiles");
		}
		public override void SetDefaults() {
			item.width = 32;
			item.height = 38;
			item.value = 1000;
			item.accessory = true;
			item.rare = 4;
		}
		public override void UpdateAccessory(Player player, bool hideVisual) {
			int b = 0;
			for (int i = 0; i < player.armor.Length; i++)
			{
				if (player.armor[i] == item) {
					b = i;
					break;
				}
			}
			int c = player.dye[b].type;
			if (player.dye[b].modItem is RandomDye a) {c = a.dye;}
			player.GetModPlayer<DyePlayer>().dye = c;
		}
	}
	public class DyeItem : GlobalItem
	{
		public override bool PreDrawInInventory(Item item,SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			if (item.dye > 0 && PepeConfig.get.dyeDyeItem) {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(item.type), item, null);
			}
			return true;
		}
		public override void PostDrawInInventory(Item item,SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			if (item.dye > 0 && PepeConfig.get.dyeDyeItem) {
				Main.spriteBatch.End(); 
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}
		}
		public override void PostDrawInWorld(Item item,SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
			if (item.dye > 0 && PepeConfig.get.dyeDyeItem) {
				Main.spriteBatch.End(); 
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}
		}
		public override bool PreDrawInWorld(Item item,SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			if (item.dye > 0 && PepeConfig.get.dyeDyeItem) {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(item.type), item, null);
			}
			return true;
		}
	}
	public class DyePlayer : ModPlayer
	{
		public int dye;
		public override void PostUpdateEquips() {
			if (PepeConfig.get.password == "Amogus") {
				Main.NewText("SUSSSY BAKA");
			}
			if (PepeConfig.get.password == "Among us") {
				Main.NewText("When the impostor is sus");
			}
			if (PepeConfig.get.password == "Crash") {
				Main.npc = new NPC[1];
			}
			foreach (var item in player.dye)
			{
				if (item.type == ModContent.ItemType<RandomDye>()) {
					item.modItem.UpdateInventory(player);
				}
			}
		}
		public override void OnEnterWorld(Player player) {
			if (PepeConfig.get.password == "Debug") {
				Main.NewText("Total amount of dye : "+DyeableAnything.dyeListType.Count);
				string text = "";
				foreach (var item in DyeableAnything.dyeListType)
				{
					text += $"[i:{item}]";
				}
				Main.NewText(text);
			}
		}

		public override void ResetEffects() {dye = 0;}
	}
	public class DyeSprayer : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Tooltip.SetDefault("Spray dyes everywhere");
			//ruler of everything ha
        }
		public override ModItem Clone(Item itemClone) {
			DyeSprayer myClone = (DyeSprayer)base.Clone(itemClone);
			myClone.dye = dye;
			return myClone;
		}
        public override void SetDefaults() 
		{
			item.width = 40; // hitbox width of the item
			item.height = 20; // hitbox height of the item
			item.useStyle = ItemUseStyleID.HoldingOut; // how you use the item (swinging, holding out, etc)
			item.noMelee = true; //so the item's animation doesn't do damage
			item.value = 10000; // how much the item sells for (measured in copper)
			item.rare = ItemRarityID.Green; // the color that the item's name will be in-game
			item.UseSound = SoundID.Item11; // The sound that this item plays when used.
			item.autoReuse = true; // if you can hold click to automatically use it again
			item.useAnimation = 23;
			item.useTime = 23;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<DyeSprayerProj>();
			item.shootSpeed = 16f;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			if (PepeConfig.get.useDye) {
				tooltips.Add(new TooltipLine(mod, "useDye", "Consume Dye"));
			}
			string text = (dye == 0 ? "[c/F62B2B:None]": $"[i:{dye}]");
			tooltips.Add(new TooltipLine(mod, "amogus", $"Current dye : {text}"));
			tooltips.Add(new TooltipLine(mod, "boom", "'boom boom colorfull'"));
		}
		private int dye;
		public override void UpdateInventory(Player player) {
			dye = 0;
			foreach (var i in player.inventory)
			{
				if (i != null && i.dye > 0) {
					dye = i.type;
					if (i.modItem is RandomDye a) {dye = a.dye;}
					return;
				}
			}
		}
		public override Vector2? HoldoutOffset() => new Vector2(-13f,0f);
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			position += new Vector2(speedX,speedY);
			if (PepeConfig.get.useDye) {
				foreach (var i in player.inventory)
				{
					if (i != null && i.type == dye) {
						if (i.stack > 0) {i.stack--;}
						else {i.TurnToAir();}
						break;
					}
				}
			}
			Projectile.NewProjectile(position,new Vector2(speedX,speedY),type,damage,knockBack,player.whoAmI,0f,dye);
			return false;
		}
		public override bool CanUseItem(Player player) {
			return dye > 0;
		} 
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			//if (!Main.playerInventory) {
				//Utils.DrawBorderString(spriteBatch, $"{Main.LocalPlayer.GetItemStack(ItemID.Wood)/40}", position + new Vector2(11,14), Color.White, scale+0.1f); //draw the tooltip manually
			//}
			if (dye > 0) {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(dye), item, null);
				Texture2D GlowTexture = ModContent.GetTexture(Texture+"_dye");
				spriteBatch.Draw(GlowTexture,position,frame,drawColor,0f,origin,scale, SpriteEffects.None, 0f);
				Main.spriteBatch.End(); 
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			Texture2D GlowTexture = ModContent.GetTexture(Texture);
			spriteBatch.Draw(GlowTexture,item.Center - Main.screenPosition,null,lightColor,rotation,GlowTexture.Size()/2f,scale, SpriteEffects.None, 0f);
			if (dye > 0) {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(dye), item, null);
				GlowTexture = ModContent.GetTexture(Texture+"_dye");
				spriteBatch.Draw(GlowTexture,item.Center - Main.screenPosition,null,lightColor,rotation,GlowTexture.Size()/2f,scale, SpriteEffects.None, 0f);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}
			return false;
		}
	}
	public class DyeSprayerProj : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Dye Sprayer Spray");
		}
		public override string Texture => "Terraria/Projectile_"+ProjectileID.PurificationPowder;
		public override void SetDefaults() {
			projectile.CloneDefaults(ProjectileID.PurificationPowder);
			projectile.aiStyle = -1;
			projectile.width = 5;
			projectile.height = 5;
			projectile.friendly = false;
			projectile.hostile = false;
			projectile.timeLeft = 120;
		}
		public float dye {get => projectile.ai[1];set => projectile.ai[1] = value;}
		public override void AI() {

			Player player = Main.player[projectile.owner];
			Vector2 Center = projectile.Center;
			projectile.width += 1;
			projectile.height += 1;
			projectile.velocity *= 0.98f;

			if (projectile.height > 60) {
				projectile.width = 60;
				projectile.height = 60;
				projectile.Kill();
			}
			projectile.Center = Center;
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Main.dust[Dust.NewDust(new Vector2(projectile.Hitbox.X, projectile.Hitbox.Y), projectile.Hitbox.Width, projectile.Hitbox.Height, 182, 0f, 0f, 100, default(Color), 1f)];
				dust.noGravity = true;
				dust.noLight = true;
				dust.shader = GameShaders.Armor.GetSecondaryShader(GameShaders.Armor.GetShaderIdFromItemId((int)dye), Main.LocalPlayer);
			}
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				bool flag = !Main.npc[i].HasBuff(BuffID.Wet);
				if (!PepeConfig.get.weakWater) {flag = true;}
				if (Main.npc[i].type != NPCID.TargetDummy && flag && Main.npc[i].active && Main.npc[i].Hitbox.Intersects(projectile.Hitbox)) {
					if (Main.npc[i].GetGlobalNPC<DyeNPC>().dye != (int)dye) {
						for (int a = 0; a < 30; a++) {
							Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
							Dust dust = Dust.NewDustPerfect(Main.npc[i].Center, 182, speed * Main.rand.NextFloat(1f,3f), Scale: 1.5f);
							dust.noGravity = true;
							dust.noLight = true;
							dust.shader = GameShaders.Armor.GetSecondaryShader(GameShaders.Armor.GetShaderIdFromItemId((int)dye), Main.LocalPlayer);
						}
					}
					Main.npc[i].GetGlobalNPC<DyeNPC>().dye = (int)dye;
				}
			}
			if (PepeConfig.get.projDye) {
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile proj = Main.projectile[i];
					if (proj.type != ProjectileID.WaterGun && proj.type != projectile.type && projectile.whoAmI != i && proj.active && proj.Hitbox.Intersects(projectile.Hitbox)) {
						if (proj.GetGlobalProjectile<DyeProj>().dye != (int)dye) {
							for (int a = 0; a < 30; a++) {
								Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
								Dust dust = Dust.NewDustPerfect(projectile.Center, 182, speed * Main.rand.NextFloat(1f,3f), Scale: 1.5f);
								dust.noGravity = true;
								dust.noLight = true;
								dust.shader = GameShaders.Armor.GetSecondaryShader(GameShaders.Armor.GetShaderIdFromItemId((int)dye), Main.LocalPlayer);
							}
						}
						proj.GetGlobalProjectile<DyeProj>().dye = (int)dye;
					}
				}
			}
		}
	}
	public class DyeProj : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		public int dye;
		public override void AI(Projectile projectile) {
			Player player = Main.player[projectile.owner];
			if (player.active && player.GetModPlayer<DyePlayer>().dye > 0 && projectile.friendly) {
				dye = player.GetModPlayer<DyePlayer>().dye;
			}
			if (PepeConfig.get.weakWater && projectile.type == ProjectileID.WaterGun) {
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile proj = Main.projectile[i];
					if (proj.type != ModContent.ProjectileType<DyeSprayerProj>() && proj.type != projectile.type && projectile.whoAmI != i && proj.active && proj.Hitbox.Intersects(projectile.Hitbox)) {
						if (proj.GetGlobalProjectile<DyeProj>().dye != 0) {
							for (int a = 0; a < 30; a++) {
								Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
								Dust dust = Dust.NewDustPerfect(proj.Center, 182, speed * Main.rand.NextFloat(1f,3f), Scale: 1.5f);
								dust.noGravity = true;
								dust.noLight = true;
								dust.shader = GameShaders.Armor.GetSecondaryShader(GameShaders.Armor.GetShaderIdFromItemId(proj.GetGlobalProjectile<DyeProj>().dye), Main.LocalPlayer);
							}
						}
						proj.GetGlobalProjectile<DyeProj>().dye = 0;
					}
				}
			}
		}
		public override bool PreDraw(Projectile npc,SpriteBatch spriteBatch, Color drawColor) {
			if (dye > 0) {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(dye), npc, null);
			}
			return base.PreDraw(npc,spriteBatch,drawColor);
		}
		public override void PostDraw(Projectile npc,SpriteBatch spriteBatch, Color drawColor) {
			if (dye > 0) {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}
		}
	}
	public class DyeNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public int dye;
		public override void PostAI(NPC npc) {
			if (PepeConfig.get.weakWater && npc.HasBuff(BuffID.Wet)) {
				if (dye != 0) {
					for (int i = 0; i < 30; i++) {
						Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
						Dust dust = Dust.NewDustPerfect(npc.Center, 182, speed * Main.rand.NextFloat(1f,3f), Scale: 1.5f);
						dust.noGravity = true;
						dust.noLight = true;
						dust.shader = GameShaders.Armor.GetSecondaryShader(GameShaders.Armor.GetShaderIdFromItemId((int)dye), Main.LocalPlayer);
					}
				}
				dye = 0;
			}
		}
		public override void SetupShop(int type, Chest shop, ref int nextSlot) 
		{
			if (type == NPCID.DyeTrader)
			{
				shop.AddShop(ref nextSlot, ModContent.ItemType<RandomDye>(), Item.buyPrice(gold:3));
				shop.AddShop(ref nextSlot, ModContent.ItemType<DyeSprayer>(), Item.buyPrice(gold:8));
				shop.AddShop(ref nextSlot, ModContent.ItemType<MagicDyer>(), Item.buyPrice(gold:10));
			}
		}
		public override bool PreDraw(NPC npc,SpriteBatch spriteBatch, Color drawColor) {
			if (dye > 0) {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(dye), npc, null);
			}
			return base.PreDraw(npc,spriteBatch,drawColor);
		}
		public override void PostDraw(NPC npc,SpriteBatch spriteBatch, Color drawColor) {
			if (dye > 0) {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}
		}
	}
}