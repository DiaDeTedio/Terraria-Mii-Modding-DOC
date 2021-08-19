using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using ReLogic.Content.Readers;
using ReLogic.Graphics;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.IO;
using Terraria.Utilities;

namespace Terraria.Initializers
{
	public static class AssetInitializer
	{
		public static void CreateAssetServices(GameServiceContainer services)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected O, but got Unknown
			AssetReaderCollection val = new AssetReaderCollection();
			val.RegisterReader((IAssetReader)new PngReader(XnaExtensions.Get<IGraphicsDeviceService>((IServiceProvider)services).GraphicsDevice), new string[1] { ".png" });
			val.RegisterReader((IAssetReader)new XnbReader((IServiceProvider)services), new string[1] { ".xnb" });
			AsyncAssetLoader val2 = new AsyncAssetLoader(val, 20);
			val2.RequireTypeCreationOnTransfer(typeof(Texture2D));
			val2.RequireTypeCreationOnTransfer(typeof(DynamicSpriteFont));
			val2.RequireTypeCreationOnTransfer(typeof(SpriteFont));
			IAssetRepository provider = (IAssetRepository)new AssetRepository((IAssetLoader)new AssetLoader(val), (IAsyncAssetLoader)(object)val2);
			services.AddService(typeof(AssetReaderCollection), val);
			services.AddService(typeof(IAssetRepository), provider);
		}

		public static ResourcePackList CreateResourcePackList(IServiceProvider services)
		{
			GetResourcePacksFolderPathAndConfirmItExists(out var resourcePackJson, out var resourcePackFolder);
			return ResourcePackList.FromJson(resourcePackJson, services, resourcePackFolder);
		}

		public static void GetResourcePacksFolderPathAndConfirmItExists(out JArray resourcePackJson, out string resourcePackFolder)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			resourcePackJson = Main.Configuration.Get<JArray>("ResourcePacks", new JArray());
			resourcePackFolder = Path.Combine(Main.SavePath, "ResourcePacks");
			Utils.TryCreatingDirectory(resourcePackFolder);
		}

		public static void LoadSplashAssets(bool asyncLoadForSounds)
		{
			TextureAssets.SplashTexture16x9 = LoadAsset<Texture2D>("Images\\SplashScreens\\Splash_1", (AssetRequestMode)1);
			TextureAssets.SplashTexture4x3 = LoadAsset<Texture2D>("Images\\logo_" + new UnifiedRandom().Next(1, 9), (AssetRequestMode)1);
			TextureAssets.SplashTextureLegoResonanace = LoadAsset<Texture2D>("Images\\SplashScreens\\ResonanceArray", (AssetRequestMode)1);
			int num = new UnifiedRandom().Next(1, 10);
			TextureAssets.SplashTextureLegoBack = LoadAsset<Texture2D>("Images\\SplashScreens\\Splash_" + num + "_0", (AssetRequestMode)1);
			TextureAssets.SplashTextureLegoTree = LoadAsset<Texture2D>("Images\\SplashScreens\\Splash_" + num + "_1", (AssetRequestMode)1);
			TextureAssets.SplashTextureLegoFront = LoadAsset<Texture2D>("Images\\SplashScreens\\Splash_" + num + "_2", (AssetRequestMode)1);
			TextureAssets.Item[75] = LoadAsset<Texture2D>("Images\\Item_" + (short)75, (AssetRequestMode)1);
			TextureAssets.LoadingSunflower = LoadAsset<Texture2D>("Images\\UI\\Sunflower_Loading", (AssetRequestMode)1);
		}

		public static void LoadAssetsWhileInInitialBlackScreen()
		{
			LoadFonts((AssetRequestMode)1);
			LoadTextures((AssetRequestMode)1);
			LoadRenderTargetAssets((AssetRequestMode)1);
			LoadSounds((AssetRequestMode)1);
		}

		public static void Load(bool asyncLoad)
		{
		}

		private static void LoadFonts(AssetRequestMode mode)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			FontAssets.ItemStack = LoadAsset<DynamicSpriteFont>("Fonts/Item_Stack", mode);
			FontAssets.MouseText = LoadAsset<DynamicSpriteFont>("Fonts/Mouse_Text", mode);
			FontAssets.DeathText = LoadAsset<DynamicSpriteFont>("Fonts/Death_Text", mode);
			FontAssets.CombatText[0] = LoadAsset<DynamicSpriteFont>("Fonts/Combat_Text", mode);
			FontAssets.CombatText[1] = LoadAsset<DynamicSpriteFont>("Fonts/Combat_Crit", mode);
		}

		private static void LoadSounds(AssetRequestMode mode)
		{
			SoundEngine.Load(Main.instance.Services);
		}

		private static void LoadRenderTargetAssets(AssetRequestMode mode)
		{
			RegisterRenderTargetAsset(TextureAssets.RenderTargets.PlayerRainbowWings = new PlayerRainbowWingsTextureContent());
			RegisterRenderTargetAsset(TextureAssets.RenderTargets.PlayerTitaniumStormBuff = new PlayerTitaniumStormBuffTextureContent());
			RegisterRenderTargetAsset(TextureAssets.RenderTargets.QueenSlimeMount = new PlayerQueenSlimeMountTextureContent());
		}

		private static void RegisterRenderTargetAsset(INeedRenderTargetContent content)
		{
			Main.ContentThatNeedsRenderTargets.Add(content);
		}

		private static void LoadTextures(AssetRequestMode mode)
		{
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_078f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_082e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0905: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_0945: Unknown result type (might be due to invalid IL or missing references)
			//IL_0955: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_0975: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_0995: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff6: Unknown result type (might be due to invalid IL or missing references)
			//IL_100c: Unknown result type (might be due to invalid IL or missing references)
			//IL_101e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1030: Unknown result type (might be due to invalid IL or missing references)
			//IL_1042: Unknown result type (might be due to invalid IL or missing references)
			//IL_104e: Unknown result type (might be due to invalid IL or missing references)
			//IL_105e: Unknown result type (might be due to invalid IL or missing references)
			//IL_106e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1084: Unknown result type (might be due to invalid IL or missing references)
			//IL_1096: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_10de: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1102: Unknown result type (might be due to invalid IL or missing references)
			//IL_1114: Unknown result type (might be due to invalid IL or missing references)
			//IL_1126: Unknown result type (might be due to invalid IL or missing references)
			//IL_1138: Unknown result type (might be due to invalid IL or missing references)
			//IL_114a: Unknown result type (might be due to invalid IL or missing references)
			//IL_115c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1180: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1214: Unknown result type (might be due to invalid IL or missing references)
			//IL_1233: Unknown result type (might be due to invalid IL or missing references)
			//IL_1260: Unknown result type (might be due to invalid IL or missing references)
			//IL_1272: Unknown result type (might be due to invalid IL or missing references)
			//IL_1284: Unknown result type (might be due to invalid IL or missing references)
			//IL_1296: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_12c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1306: Unknown result type (might be due to invalid IL or missing references)
			//IL_1316: Unknown result type (might be due to invalid IL or missing references)
			//IL_1326: Unknown result type (might be due to invalid IL or missing references)
			//IL_1336: Unknown result type (might be due to invalid IL or missing references)
			//IL_1346: Unknown result type (might be due to invalid IL or missing references)
			//IL_1356: Unknown result type (might be due to invalid IL or missing references)
			//IL_1366: Unknown result type (might be due to invalid IL or missing references)
			//IL_1376: Unknown result type (might be due to invalid IL or missing references)
			//IL_1386: Unknown result type (might be due to invalid IL or missing references)
			//IL_1396: Unknown result type (might be due to invalid IL or missing references)
			//IL_13a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_140b: Unknown result type (might be due to invalid IL or missing references)
			//IL_141b: Unknown result type (might be due to invalid IL or missing references)
			//IL_142b: Unknown result type (might be due to invalid IL or missing references)
			//IL_143b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1451: Unknown result type (might be due to invalid IL or missing references)
			//IL_1463: Unknown result type (might be due to invalid IL or missing references)
			//IL_1475: Unknown result type (might be due to invalid IL or missing references)
			//IL_1487: Unknown result type (might be due to invalid IL or missing references)
			//IL_1499: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_14bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1511: Unknown result type (might be due to invalid IL or missing references)
			//IL_1546: Unknown result type (might be due to invalid IL or missing references)
			//IL_157b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1598: Unknown result type (might be due to invalid IL or missing references)
			//IL_15a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1608: Unknown result type (might be due to invalid IL or missing references)
			//IL_1618: Unknown result type (might be due to invalid IL or missing references)
			//IL_1628: Unknown result type (might be due to invalid IL or missing references)
			//IL_1638: Unknown result type (might be due to invalid IL or missing references)
			//IL_1648: Unknown result type (might be due to invalid IL or missing references)
			//IL_1658: Unknown result type (might be due to invalid IL or missing references)
			//IL_1668: Unknown result type (might be due to invalid IL or missing references)
			//IL_1678: Unknown result type (might be due to invalid IL or missing references)
			//IL_1688: Unknown result type (might be due to invalid IL or missing references)
			//IL_1698: Unknown result type (might be due to invalid IL or missing references)
			//IL_16a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_16b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_16c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_16d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_16e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_16f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1708: Unknown result type (might be due to invalid IL or missing references)
			//IL_1718: Unknown result type (might be due to invalid IL or missing references)
			//IL_1728: Unknown result type (might be due to invalid IL or missing references)
			//IL_1738: Unknown result type (might be due to invalid IL or missing references)
			//IL_1748: Unknown result type (might be due to invalid IL or missing references)
			//IL_175d: Unknown result type (might be due to invalid IL or missing references)
			//IL_176d: Unknown result type (might be due to invalid IL or missing references)
			//IL_177d: Unknown result type (might be due to invalid IL or missing references)
			//IL_178d: Unknown result type (might be due to invalid IL or missing references)
			//IL_179d: Unknown result type (might be due to invalid IL or missing references)
			//IL_17ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_17bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_17cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_17dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_17ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_17fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1825: Unknown result type (might be due to invalid IL or missing references)
			//IL_183d: Unknown result type (might be due to invalid IL or missing references)
			//IL_184d: Unknown result type (might be due to invalid IL or missing references)
			//IL_185d: Unknown result type (might be due to invalid IL or missing references)
			//IL_186d: Unknown result type (might be due to invalid IL or missing references)
			//IL_187d: Unknown result type (might be due to invalid IL or missing references)
			//IL_188d: Unknown result type (might be due to invalid IL or missing references)
			//IL_189d: Unknown result type (might be due to invalid IL or missing references)
			//IL_18ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_18bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_18cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_18dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_18ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_18fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_190d: Unknown result type (might be due to invalid IL or missing references)
			//IL_191d: Unknown result type (might be due to invalid IL or missing references)
			//IL_192d: Unknown result type (might be due to invalid IL or missing references)
			//IL_193d: Unknown result type (might be due to invalid IL or missing references)
			//IL_194d: Unknown result type (might be due to invalid IL or missing references)
			//IL_195d: Unknown result type (might be due to invalid IL or missing references)
			//IL_196d: Unknown result type (might be due to invalid IL or missing references)
			//IL_197d: Unknown result type (might be due to invalid IL or missing references)
			//IL_198d: Unknown result type (might be due to invalid IL or missing references)
			//IL_199d: Unknown result type (might be due to invalid IL or missing references)
			//IL_19ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_19bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_19cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_19dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_19ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_19fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1aad: Unknown result type (might be due to invalid IL or missing references)
			//IL_1abd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1acd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1add: Unknown result type (might be due to invalid IL or missing references)
			//IL_1aed: Unknown result type (might be due to invalid IL or missing references)
			//IL_1afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_1baa: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1be7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1bf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c07: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c17: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c27: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c37: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c42: Unknown result type (might be due to invalid IL or missing references)
			//IL_1c48: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < TextureAssets.Item.Length; i++)
			{
				int num = ItemID.Sets.TextureCopyLoad[i];
				if (num != -1)
				{
					TextureAssets.Item[i] = TextureAssets.Item[num];
				}
				else
				{
					TextureAssets.Item[i] = LoadAsset<Texture2D>("Images/Item_" + i, (AssetRequestMode)0);
				}
			}
			for (int j = 0; j < TextureAssets.Npc.Length; j++)
			{
				TextureAssets.Npc[j] = LoadAsset<Texture2D>("Images/NPC_" + j, (AssetRequestMode)0);
			}
			for (int k = 0; k < TextureAssets.Projectile.Length; k++)
			{
				TextureAssets.Projectile[k] = LoadAsset<Texture2D>("Images/Projectile_" + k, (AssetRequestMode)0);
			}
			for (int l = 0; l < TextureAssets.Gore.Length; l++)
			{
				TextureAssets.Gore[l] = LoadAsset<Texture2D>("Images/Gore_" + l, (AssetRequestMode)0);
			}
			for (int m = 0; m < TextureAssets.Wall.Length; m++)
			{
				TextureAssets.Wall[m] = LoadAsset<Texture2D>("Images/Wall_" + m, (AssetRequestMode)0);
			}
			for (int n = 0; n < TextureAssets.Tile.Length; n++)
			{
				TextureAssets.Tile[n] = LoadAsset<Texture2D>("Images/Tiles_" + n, (AssetRequestMode)0);
			}
			for (int num2 = 0; num2 < TextureAssets.ItemFlame.Length; num2++)
			{
				TextureAssets.ItemFlame[num2] = LoadAsset<Texture2D>("Images/ItemFlame_" + num2, (AssetRequestMode)0);
			}
			for (int num3 = 0; num3 < TextureAssets.Wings.Length; num3++)
			{
				TextureAssets.Wings[num3] = LoadAsset<Texture2D>("Images/Wings_" + num3, (AssetRequestMode)0);
			}
			for (int num4 = 0; num4 < TextureAssets.PlayerHair.Length; num4++)
			{
				TextureAssets.PlayerHair[num4] = LoadAsset<Texture2D>("Images/Player_Hair_" + (num4 + 1), (AssetRequestMode)0);
			}
			for (int num5 = 0; num5 < TextureAssets.PlayerHairAlt.Length; num5++)
			{
				TextureAssets.PlayerHairAlt[num5] = LoadAsset<Texture2D>("Images/Player_HairAlt_" + (num5 + 1), (AssetRequestMode)0);
			}
			for (int num6 = 0; num6 < TextureAssets.ArmorHead.Length; num6++)
			{
				TextureAssets.ArmorHead[num6] = LoadAsset<Texture2D>("Images/Armor_Head_" + num6, (AssetRequestMode)0);
			}
			for (int num7 = 0; num7 < TextureAssets.FemaleBody.Length; num7++)
			{
				TextureAssets.FemaleBody[num7] = LoadAsset<Texture2D>("Images/Female_Body_" + num7, (AssetRequestMode)0);
			}
			for (int num8 = 0; num8 < TextureAssets.ArmorBody.Length; num8++)
			{
				TextureAssets.ArmorBody[num8] = LoadAsset<Texture2D>("Images/Armor_Body_" + num8, (AssetRequestMode)0);
			}
			for (int num9 = 0; num9 < TextureAssets.ArmorBodyComposite.Length; num9++)
			{
				TextureAssets.ArmorBodyComposite[num9] = LoadAsset<Texture2D>("Images/Armor/Armor_" + num9, (AssetRequestMode)0);
			}
			for (int num10 = 0; num10 < TextureAssets.ArmorArm.Length; num10++)
			{
				TextureAssets.ArmorArm[num10] = LoadAsset<Texture2D>("Images/Armor_Arm_" + num10, (AssetRequestMode)0);
			}
			for (int num11 = 0; num11 < TextureAssets.ArmorLeg.Length; num11++)
			{
				TextureAssets.ArmorLeg[num11] = LoadAsset<Texture2D>("Images/Armor_Legs_" + num11, (AssetRequestMode)0);
			}
			for (int num12 = 0; num12 < TextureAssets.AccHandsOn.Length; num12++)
			{
				TextureAssets.AccHandsOn[num12] = LoadAsset<Texture2D>("Images/Acc_HandsOn_" + num12, (AssetRequestMode)0);
			}
			for (int num13 = 0; num13 < TextureAssets.AccHandsOff.Length; num13++)
			{
				TextureAssets.AccHandsOff[num13] = LoadAsset<Texture2D>("Images/Acc_HandsOff_" + num13, (AssetRequestMode)0);
			}
			for (int num14 = 0; num14 < TextureAssets.AccHandsOnComposite.Length; num14++)
			{
				TextureAssets.AccHandsOnComposite[num14] = LoadAsset<Texture2D>("Images/Accessories/Acc_HandsOn_" + num14, (AssetRequestMode)0);
			}
			for (int num15 = 0; num15 < TextureAssets.AccHandsOffComposite.Length; num15++)
			{
				TextureAssets.AccHandsOffComposite[num15] = LoadAsset<Texture2D>("Images/Accessories/Acc_HandsOff_" + num15, (AssetRequestMode)0);
			}
			for (int num16 = 0; num16 < TextureAssets.AccBack.Length; num16++)
			{
				TextureAssets.AccBack[num16] = LoadAsset<Texture2D>("Images/Acc_Back_" + num16, (AssetRequestMode)0);
			}
			for (int num17 = 0; num17 < TextureAssets.AccFront.Length; num17++)
			{
				TextureAssets.AccFront[num17] = LoadAsset<Texture2D>("Images/Acc_Front_" + num17, (AssetRequestMode)0);
			}
			for (int num18 = 0; num18 < TextureAssets.AccShoes.Length; num18++)
			{
				TextureAssets.AccShoes[num18] = LoadAsset<Texture2D>("Images/Acc_Shoes_" + num18, (AssetRequestMode)0);
			}
			for (int num19 = 0; num19 < TextureAssets.AccWaist.Length; num19++)
			{
				TextureAssets.AccWaist[num19] = LoadAsset<Texture2D>("Images/Acc_Waist_" + num19, (AssetRequestMode)0);
			}
			for (int num20 = 0; num20 < TextureAssets.AccShield.Length; num20++)
			{
				TextureAssets.AccShield[num20] = LoadAsset<Texture2D>("Images/Acc_Shield_" + num20, (AssetRequestMode)0);
			}
			for (int num21 = 0; num21 < TextureAssets.AccNeck.Length; num21++)
			{
				TextureAssets.AccNeck[num21] = LoadAsset<Texture2D>("Images/Acc_Neck_" + num21, (AssetRequestMode)0);
			}
			for (int num22 = 0; num22 < TextureAssets.AccFace.Length; num22++)
			{
				TextureAssets.AccFace[num22] = LoadAsset<Texture2D>("Images/Acc_Face_" + num22, (AssetRequestMode)0);
			}
			for (int num23 = 0; num23 < TextureAssets.AccBalloon.Length; num23++)
			{
				TextureAssets.AccBalloon[num23] = LoadAsset<Texture2D>("Images/Acc_Balloon_" + num23, (AssetRequestMode)0);
			}
			for (int num24 = 0; num24 < TextureAssets.Background.Length; num24++)
			{
				TextureAssets.Background[num24] = LoadAsset<Texture2D>("Images/Background_" + num24, (AssetRequestMode)0);
			}
			TextureAssets.FlameRing = LoadAsset<Texture2D>("Images/FlameRing", (AssetRequestMode)0);
			TextureAssets.TileCrack = LoadAsset<Texture2D>("Images\\TileCracks", mode);
			TextureAssets.ChestStack[0] = LoadAsset<Texture2D>("Images\\ChestStack_0", mode);
			TextureAssets.ChestStack[1] = LoadAsset<Texture2D>("Images\\ChestStack_1", mode);
			TextureAssets.SmartDig = LoadAsset<Texture2D>("Images\\SmartDig", mode);
			TextureAssets.IceBarrier = LoadAsset<Texture2D>("Images\\IceBarrier", mode);
			TextureAssets.Frozen = LoadAsset<Texture2D>("Images\\Frozen", mode);
			for (int num25 = 0; num25 < TextureAssets.Pvp.Length; num25++)
			{
				TextureAssets.Pvp[num25] = LoadAsset<Texture2D>("Images\\UI\\PVP_" + num25, mode);
			}
			for (int num26 = 0; num26 < TextureAssets.EquipPage.Length; num26++)
			{
				TextureAssets.EquipPage[num26] = LoadAsset<Texture2D>("Images\\UI\\DisplaySlots_" + num26, mode);
			}
			TextureAssets.HouseBanner = LoadAsset<Texture2D>("Images\\UI\\House_Banner", mode);
			for (int num27 = 0; num27 < TextureAssets.CraftToggle.Length; num27++)
			{
				TextureAssets.CraftToggle[num27] = LoadAsset<Texture2D>("Images\\UI\\Craft_Toggle_" + num27, mode);
			}
			for (int num28 = 0; num28 < TextureAssets.InventorySort.Length; num28++)
			{
				TextureAssets.InventorySort[num28] = LoadAsset<Texture2D>("Images\\UI\\Sort_" + num28, mode);
			}
			for (int num29 = 0; num29 < TextureAssets.TextGlyph.Length; num29++)
			{
				TextureAssets.TextGlyph[num29] = LoadAsset<Texture2D>("Images\\UI\\Glyphs_" + num29, mode);
			}
			for (int num30 = 0; num30 < TextureAssets.HotbarRadial.Length; num30++)
			{
				TextureAssets.HotbarRadial[num30] = LoadAsset<Texture2D>("Images\\UI\\HotbarRadial_" + num30, mode);
			}
			for (int num31 = 0; num31 < TextureAssets.InfoIcon.Length; num31++)
			{
				TextureAssets.InfoIcon[num31] = LoadAsset<Texture2D>("Images\\UI\\InfoIcon_" + num31, mode);
			}
			for (int num32 = 0; num32 < TextureAssets.Reforge.Length; num32++)
			{
				TextureAssets.Reforge[num32] = LoadAsset<Texture2D>("Images\\UI\\Reforge_" + num32, mode);
			}
			for (int num33 = 0; num33 < TextureAssets.Camera.Length; num33++)
			{
				TextureAssets.Camera[num33] = LoadAsset<Texture2D>("Images\\UI\\Camera_" + num33, mode);
			}
			for (int num34 = 0; num34 < TextureAssets.WireUi.Length; num34++)
			{
				TextureAssets.WireUi[num34] = LoadAsset<Texture2D>("Images\\UI\\Wires_" + num34, mode);
			}
			TextureAssets.BuilderAcc = LoadAsset<Texture2D>("Images\\UI\\BuilderIcons", mode);
			TextureAssets.QuicksIcon = LoadAsset<Texture2D>("Images\\UI\\UI_quickicon1", mode);
			TextureAssets.CraftUpButton = LoadAsset<Texture2D>("Images\\RecUp", mode);
			TextureAssets.CraftDownButton = LoadAsset<Texture2D>("Images\\RecDown", mode);
			TextureAssets.ScrollLeftButton = LoadAsset<Texture2D>("Images\\RecLeft", mode);
			TextureAssets.ScrollRightButton = LoadAsset<Texture2D>("Images\\RecRight", mode);
			TextureAssets.OneDropLogo = LoadAsset<Texture2D>("Images\\OneDropLogo", mode);
			TextureAssets.Pulley = LoadAsset<Texture2D>("Images\\PlayerPulley", mode);
			TextureAssets.Timer = LoadAsset<Texture2D>("Images\\Timer", mode);
			TextureAssets.EmoteMenuButton = LoadAsset<Texture2D>("Images\\UI\\Emotes", mode);
			TextureAssets.BestiaryMenuButton = LoadAsset<Texture2D>("Images\\UI\\Bestiary", mode);
			TextureAssets.Wof = LoadAsset<Texture2D>("Images\\WallOfFlesh", mode);
			TextureAssets.WallOutline = LoadAsset<Texture2D>("Images\\Wall_Outline", mode);
			TextureAssets.Fade = LoadAsset<Texture2D>("Images\\fade-out", mode);
			TextureAssets.Ghost = LoadAsset<Texture2D>("Images\\Ghost", mode);
			TextureAssets.EvilCactus = LoadAsset<Texture2D>("Images\\Evil_Cactus", mode);
			TextureAssets.GoodCactus = LoadAsset<Texture2D>("Images\\Good_Cactus", mode);
			TextureAssets.CrimsonCactus = LoadAsset<Texture2D>("Images\\Crimson_Cactus", mode);
			TextureAssets.WraithEye = LoadAsset<Texture2D>("Images\\Wraith_Eyes", mode);
			TextureAssets.Firefly = LoadAsset<Texture2D>("Images\\Firefly", mode);
			TextureAssets.FireflyJar = LoadAsset<Texture2D>("Images\\FireflyJar", mode);
			TextureAssets.Lightningbug = LoadAsset<Texture2D>("Images\\LightningBug", mode);
			TextureAssets.LightningbugJar = LoadAsset<Texture2D>("Images\\LightningBugJar", mode);
			for (int num35 = 1; num35 <= 3; num35++)
			{
				TextureAssets.JellyfishBowl[num35 - 1] = LoadAsset<Texture2D>("Images\\jellyfishBowl" + num35, mode);
			}
			TextureAssets.GlowSnail = LoadAsset<Texture2D>("Images\\GlowSnail", mode);
			TextureAssets.IceQueen = LoadAsset<Texture2D>("Images\\IceQueen", mode);
			TextureAssets.SantaTank = LoadAsset<Texture2D>("Images\\SantaTank", mode);
			TextureAssets.JackHat = LoadAsset<Texture2D>("Images\\JackHat", mode);
			TextureAssets.TreeFace = LoadAsset<Texture2D>("Images\\TreeFace", mode);
			TextureAssets.PumpkingFace = LoadAsset<Texture2D>("Images\\PumpkingFace", mode);
			TextureAssets.ReaperEye = LoadAsset<Texture2D>("Images\\Reaper_Eyes", mode);
			TextureAssets.MapDeath = LoadAsset<Texture2D>("Images\\MapDeath", mode);
			TextureAssets.DukeFishron = LoadAsset<Texture2D>("Images\\DukeFishron", mode);
			TextureAssets.MiniMinotaur = LoadAsset<Texture2D>("Images\\MiniMinotaur", mode);
			TextureAssets.Map = LoadAsset<Texture2D>("Images\\Map", mode);
			for (int num36 = 0; num36 < TextureAssets.MapBGs.Length; num36++)
			{
				TextureAssets.MapBGs[num36] = LoadAsset<Texture2D>("Images\\MapBG" + (num36 + 1), mode);
			}
			TextureAssets.Hue = LoadAsset<Texture2D>("Images\\Hue", mode);
			TextureAssets.ColorSlider = LoadAsset<Texture2D>("Images\\ColorSlider", mode);
			TextureAssets.ColorBar = LoadAsset<Texture2D>("Images\\ColorBar", mode);
			TextureAssets.ColorBlip = LoadAsset<Texture2D>("Images\\ColorBlip", mode);
			TextureAssets.ColorHighlight = LoadAsset<Texture2D>("Images\\UI\\Slider_Highlight", mode);
			TextureAssets.LockOnCursor = LoadAsset<Texture2D>("Images\\UI\\LockOn_Cursor", mode);
			TextureAssets.Rain = LoadAsset<Texture2D>("Images\\Rain", mode);
			for (int num37 = 0; num37 < 310; num37++)
			{
				TextureAssets.GlowMask[num37] = LoadAsset<Texture2D>("Images\\Glow_" + num37, mode);
			}
			for (int num38 = 0; num38 < TextureAssets.HighlightMask.Length; num38++)
			{
				if (TileID.Sets.HasOutlines[num38])
				{
					TextureAssets.HighlightMask[num38] = LoadAsset<Texture2D>("Images\\Misc\\TileOutlines\\Tiles_" + num38, mode);
				}
			}
			for (int num39 = 0; num39 < 244; num39++)
			{
				TextureAssets.Extra[num39] = LoadAsset<Texture2D>("Images\\Extra_" + num39, mode);
			}
			for (int num40 = 0; num40 < 4; num40++)
			{
				TextureAssets.Coin[num40] = LoadAsset<Texture2D>("Images\\Coin_" + num40, mode);
			}
			TextureAssets.MagicPixel = LoadAsset<Texture2D>("Images\\MagicPixel", mode);
			TextureAssets.SettingsPanel = LoadAsset<Texture2D>("Images\\UI\\Settings_Panel", mode);
			TextureAssets.SettingsPanel2 = LoadAsset<Texture2D>("Images\\UI\\Settings_Panel_2", mode);
			for (int num41 = 0; num41 < TextureAssets.XmasTree.Length; num41++)
			{
				TextureAssets.XmasTree[num41] = LoadAsset<Texture2D>("Images\\Xmas_" + num41, mode);
			}
			for (int num42 = 0; num42 < 6; num42++)
			{
				TextureAssets.Clothes[num42] = LoadAsset<Texture2D>("Images\\Clothes_" + num42, mode);
			}
			for (int num43 = 0; num43 < TextureAssets.Flames.Length; num43++)
			{
				TextureAssets.Flames[num43] = LoadAsset<Texture2D>("Images\\Flame_" + num43, mode);
			}
			for (int num44 = 0; num44 < 8; num44++)
			{
				TextureAssets.MapIcon[num44] = LoadAsset<Texture2D>("Images\\Map_" + num44, mode);
			}
			for (int num45 = 0; num45 < TextureAssets.Underworld.Length; num45++)
			{
				TextureAssets.Underworld[num45] = LoadAsset<Texture2D>("Images/Backgrounds/Underworld " + num45, (AssetRequestMode)0);
			}
			TextureAssets.Dest[0] = LoadAsset<Texture2D>("Images\\Dest1", mode);
			TextureAssets.Dest[1] = LoadAsset<Texture2D>("Images\\Dest2", mode);
			TextureAssets.Dest[2] = LoadAsset<Texture2D>("Images\\Dest3", mode);
			TextureAssets.Actuator = LoadAsset<Texture2D>("Images\\Actuator", mode);
			TextureAssets.Wire = LoadAsset<Texture2D>("Images\\Wires", mode);
			TextureAssets.Wire2 = LoadAsset<Texture2D>("Images\\Wires2", mode);
			TextureAssets.Wire3 = LoadAsset<Texture2D>("Images\\Wires3", mode);
			TextureAssets.Wire4 = LoadAsset<Texture2D>("Images\\Wires4", mode);
			TextureAssets.WireNew = LoadAsset<Texture2D>("Images\\WiresNew", mode);
			TextureAssets.FlyingCarpet = LoadAsset<Texture2D>("Images\\FlyingCarpet", mode);
			TextureAssets.Hb1 = LoadAsset<Texture2D>("Images\\HealthBar1", mode);
			TextureAssets.Hb2 = LoadAsset<Texture2D>("Images\\HealthBar2", mode);
			for (int num46 = 0; num46 < TextureAssets.NpcHead.Length; num46++)
			{
				TextureAssets.NpcHead[num46] = LoadAsset<Texture2D>("Images\\NPC_Head_" + num46, mode);
			}
			for (int num47 = 0; num47 < TextureAssets.NpcHeadBoss.Length; num47++)
			{
				TextureAssets.NpcHeadBoss[num47] = LoadAsset<Texture2D>("Images\\NPC_Head_Boss_" + num47, mode);
			}
			for (int num48 = 1; num48 < TextureAssets.BackPack.Length; num48++)
			{
				TextureAssets.BackPack[num48] = LoadAsset<Texture2D>("Images\\BackPack_" + num48, mode);
			}
			for (int num49 = 1; num49 < 327; num49++)
			{
				TextureAssets.Buff[num49] = LoadAsset<Texture2D>("Images\\Buff_" + num49, mode);
			}
			Main.instance.LoadBackground(0);
			Main.instance.LoadBackground(49);
			TextureAssets.MinecartMount = LoadAsset<Texture2D>("Images\\Mount_Minecart", mode);
			for (int num50 = 0; num50 < TextureAssets.RudolphMount.Length; num50++)
			{
				TextureAssets.RudolphMount[num50] = LoadAsset<Texture2D>("Images\\Rudolph_" + num50, mode);
			}
			TextureAssets.BunnyMount = LoadAsset<Texture2D>("Images\\Mount_Bunny", mode);
			TextureAssets.PigronMount = LoadAsset<Texture2D>("Images\\Mount_Pigron", mode);
			TextureAssets.SlimeMount = LoadAsset<Texture2D>("Images\\Mount_Slime", mode);
			TextureAssets.TurtleMount = LoadAsset<Texture2D>("Images\\Mount_Turtle", mode);
			TextureAssets.UnicornMount = LoadAsset<Texture2D>("Images\\Mount_Unicorn", mode);
			TextureAssets.BasiliskMount = LoadAsset<Texture2D>("Images\\Mount_Basilisk", mode);
			TextureAssets.MinecartMechMount[0] = LoadAsset<Texture2D>("Images\\Mount_MinecartMech", mode);
			TextureAssets.MinecartMechMount[1] = LoadAsset<Texture2D>("Images\\Mount_MinecartMechGlow", mode);
			TextureAssets.CuteFishronMount[0] = LoadAsset<Texture2D>("Images\\Mount_CuteFishron1", mode);
			TextureAssets.CuteFishronMount[1] = LoadAsset<Texture2D>("Images\\Mount_CuteFishron2", mode);
			TextureAssets.MinecartWoodMount = LoadAsset<Texture2D>("Images\\Mount_MinecartWood", mode);
			TextureAssets.DesertMinecartMount = LoadAsset<Texture2D>("Images\\Mount_MinecartDesert", mode);
			TextureAssets.FishMinecartMount = LoadAsset<Texture2D>("Images\\Mount_MinecartMineCarp", mode);
			TextureAssets.BeeMount[0] = LoadAsset<Texture2D>("Images\\Mount_Bee", mode);
			TextureAssets.BeeMount[1] = LoadAsset<Texture2D>("Images\\Mount_BeeWings", mode);
			TextureAssets.UfoMount[0] = LoadAsset<Texture2D>("Images\\Mount_UFO", mode);
			TextureAssets.UfoMount[1] = LoadAsset<Texture2D>("Images\\Mount_UFOGlow", mode);
			TextureAssets.DrillMount[0] = LoadAsset<Texture2D>("Images\\Mount_DrillRing", mode);
			TextureAssets.DrillMount[1] = LoadAsset<Texture2D>("Images\\Mount_DrillSeat", mode);
			TextureAssets.DrillMount[2] = LoadAsset<Texture2D>("Images\\Mount_DrillDiode", mode);
			TextureAssets.DrillMount[3] = LoadAsset<Texture2D>("Images\\Mount_Glow_DrillRing", mode);
			TextureAssets.DrillMount[4] = LoadAsset<Texture2D>("Images\\Mount_Glow_DrillSeat", mode);
			TextureAssets.DrillMount[5] = LoadAsset<Texture2D>("Images\\Mount_Glow_DrillDiode", mode);
			TextureAssets.ScutlixMount[0] = LoadAsset<Texture2D>("Images\\Mount_Scutlix", mode);
			TextureAssets.ScutlixMount[1] = LoadAsset<Texture2D>("Images\\Mount_ScutlixEyes", mode);
			TextureAssets.ScutlixMount[2] = LoadAsset<Texture2D>("Images\\Mount_ScutlixEyeGlow", mode);
			for (int num51 = 0; num51 < TextureAssets.Gem.Length; num51++)
			{
				TextureAssets.Gem[num51] = LoadAsset<Texture2D>("Images\\Gem_" + num51, mode);
			}
			for (int num52 = 0; num52 < 37; num52++)
			{
				TextureAssets.Cloud[num52] = LoadAsset<Texture2D>("Images\\Cloud_" + num52, mode);
			}
			for (int num53 = 0; num53 < 4; num53++)
			{
				TextureAssets.Star[num53] = LoadAsset<Texture2D>("Images\\Star_" + num53, mode);
			}
			for (int num54 = 0; num54 < 13; num54++)
			{
				TextureAssets.Liquid[num54] = LoadAsset<Texture2D>("Images\\Liquid_" + num54, mode);
				TextureAssets.LiquidSlope[num54] = LoadAsset<Texture2D>("Images\\LiquidSlope_" + num54, mode);
			}
			Main.instance.waterfallManager.LoadContent();
			TextureAssets.NpcToggle[0] = LoadAsset<Texture2D>("Images\\House_1", mode);
			TextureAssets.NpcToggle[1] = LoadAsset<Texture2D>("Images\\House_2", mode);
			TextureAssets.HbLock[0] = LoadAsset<Texture2D>("Images\\Lock_0", mode);
			TextureAssets.HbLock[1] = LoadAsset<Texture2D>("Images\\Lock_1", mode);
			TextureAssets.blockReplaceIcon[0] = LoadAsset<Texture2D>("Images\\UI\\BlockReplace_0", mode);
			TextureAssets.blockReplaceIcon[1] = LoadAsset<Texture2D>("Images\\UI\\BlockReplace_1", mode);
			TextureAssets.Grid = LoadAsset<Texture2D>("Images\\Grid", mode);
			TextureAssets.Trash = LoadAsset<Texture2D>("Images\\Trash", mode);
			TextureAssets.Cd = LoadAsset<Texture2D>("Images\\CoolDown", mode);
			TextureAssets.Logo = LoadAsset<Texture2D>("Images\\Logo", mode);
			TextureAssets.Logo2 = LoadAsset<Texture2D>("Images\\Logo2", mode);
			TextureAssets.Logo3 = LoadAsset<Texture2D>("Images\\Logo3", mode);
			TextureAssets.Logo4 = LoadAsset<Texture2D>("Images\\Logo4", mode);
			TextureAssets.Dust = LoadAsset<Texture2D>("Images\\Dust", mode);
			TextureAssets.Sun = LoadAsset<Texture2D>("Images\\Sun", mode);
			TextureAssets.Sun2 = LoadAsset<Texture2D>("Images\\Sun2", mode);
			TextureAssets.Sun3 = LoadAsset<Texture2D>("Images\\Sun3", mode);
			TextureAssets.BlackTile = LoadAsset<Texture2D>("Images\\Black_Tile", mode);
			TextureAssets.Heart = LoadAsset<Texture2D>("Images\\Heart", mode);
			TextureAssets.Heart2 = LoadAsset<Texture2D>("Images\\Heart2", mode);
			TextureAssets.Bubble = LoadAsset<Texture2D>("Images\\Bubble", mode);
			TextureAssets.Flame = LoadAsset<Texture2D>("Images\\Flame", mode);
			TextureAssets.Mana = LoadAsset<Texture2D>("Images\\Mana", mode);
			for (int num55 = 0; num55 < TextureAssets.Cursors.Length; num55++)
			{
				TextureAssets.Cursors[num55] = LoadAsset<Texture2D>("Images\\UI\\Cursor_" + num55, mode);
			}
			TextureAssets.CursorRadial = LoadAsset<Texture2D>("Images\\UI\\Radial", mode);
			TextureAssets.Ninja = LoadAsset<Texture2D>("Images\\Ninja", mode);
			TextureAssets.AntLion = LoadAsset<Texture2D>("Images\\AntlionBody", mode);
			TextureAssets.SpikeBase = LoadAsset<Texture2D>("Images\\Spike_Base", mode);
			TextureAssets.Wood[0] = LoadAsset<Texture2D>("Images\\Tiles_5_0", mode);
			TextureAssets.Wood[1] = LoadAsset<Texture2D>("Images\\Tiles_5_1", mode);
			TextureAssets.Wood[2] = LoadAsset<Texture2D>("Images\\Tiles_5_2", mode);
			TextureAssets.Wood[3] = LoadAsset<Texture2D>("Images\\Tiles_5_3", mode);
			TextureAssets.Wood[4] = LoadAsset<Texture2D>("Images\\Tiles_5_4", mode);
			TextureAssets.Wood[5] = LoadAsset<Texture2D>("Images\\Tiles_5_5", mode);
			TextureAssets.Wood[6] = LoadAsset<Texture2D>("Images\\Tiles_5_6", mode);
			TextureAssets.SmileyMoon = LoadAsset<Texture2D>("Images\\Moon_Smiley", mode);
			TextureAssets.PumpkinMoon = LoadAsset<Texture2D>("Images\\Moon_Pumpkin", mode);
			TextureAssets.SnowMoon = LoadAsset<Texture2D>("Images\\Moon_Snow", mode);
			for (int num56 = 0; num56 < TextureAssets.Moon.Length; num56++)
			{
				TextureAssets.Moon[num56] = LoadAsset<Texture2D>("Images\\Moon_" + num56, mode);
			}
			for (int num57 = 0; num57 < TextureAssets.TreeTop.Length; num57++)
			{
				TextureAssets.TreeTop[num57] = LoadAsset<Texture2D>("Images\\Tree_Tops_" + num57, mode);
			}
			for (int num58 = 0; num58 < TextureAssets.TreeBranch.Length; num58++)
			{
				TextureAssets.TreeBranch[num58] = LoadAsset<Texture2D>("Images\\Tree_Branches_" + num58, mode);
			}
			TextureAssets.ShroomCap = LoadAsset<Texture2D>("Images\\Shroom_Tops", mode);
			TextureAssets.InventoryBack = LoadAsset<Texture2D>("Images\\Inventory_Back", mode);
			TextureAssets.InventoryBack2 = LoadAsset<Texture2D>("Images\\Inventory_Back2", mode);
			TextureAssets.InventoryBack3 = LoadAsset<Texture2D>("Images\\Inventory_Back3", mode);
			TextureAssets.InventoryBack4 = LoadAsset<Texture2D>("Images\\Inventory_Back4", mode);
			TextureAssets.InventoryBack5 = LoadAsset<Texture2D>("Images\\Inventory_Back5", mode);
			TextureAssets.InventoryBack6 = LoadAsset<Texture2D>("Images\\Inventory_Back6", mode);
			TextureAssets.InventoryBack7 = LoadAsset<Texture2D>("Images\\Inventory_Back7", mode);
			TextureAssets.InventoryBack8 = LoadAsset<Texture2D>("Images\\Inventory_Back8", mode);
			TextureAssets.InventoryBack9 = LoadAsset<Texture2D>("Images\\Inventory_Back9", mode);
			TextureAssets.InventoryBack10 = LoadAsset<Texture2D>("Images\\Inventory_Back10", mode);
			TextureAssets.InventoryBack11 = LoadAsset<Texture2D>("Images\\Inventory_Back11", mode);
			TextureAssets.InventoryBack12 = LoadAsset<Texture2D>("Images\\Inventory_Back12", mode);
			TextureAssets.InventoryBack13 = LoadAsset<Texture2D>("Images\\Inventory_Back13", mode);
			TextureAssets.InventoryBack14 = LoadAsset<Texture2D>("Images\\Inventory_Back14", mode);
			TextureAssets.InventoryBack15 = LoadAsset<Texture2D>("Images\\Inventory_Back15", mode);
			TextureAssets.InventoryBack16 = LoadAsset<Texture2D>("Images\\Inventory_Back16", mode);
			TextureAssets.InventoryBack17 = LoadAsset<Texture2D>("Images\\Inventory_Back17", mode);
			TextureAssets.InventoryBack18 = LoadAsset<Texture2D>("Images\\Inventory_Back18", mode);
			TextureAssets.HairStyleBack = LoadAsset<Texture2D>("Images\\HairStyleBack", mode);
			TextureAssets.ClothesStyleBack = LoadAsset<Texture2D>("Images\\ClothesStyleBack", mode);
			TextureAssets.InventoryTickOff = LoadAsset<Texture2D>("Images\\Inventory_Tick_Off", mode);
			TextureAssets.InventoryTickOn = LoadAsset<Texture2D>("Images\\Inventory_Tick_On", mode);
			TextureAssets.TextBack = LoadAsset<Texture2D>("Images\\Text_Back", mode);
			TextureAssets.Chat = LoadAsset<Texture2D>("Images\\Chat", mode);
			TextureAssets.Chat2 = LoadAsset<Texture2D>("Images\\Chat2", mode);
			TextureAssets.ChatBack = LoadAsset<Texture2D>("Images\\Chat_Back", mode);
			TextureAssets.Team = LoadAsset<Texture2D>("Images\\Team", mode);
			PlayerDataInitializer.Load();
			TextureAssets.Chaos = LoadAsset<Texture2D>("Images\\Chaos", mode);
			TextureAssets.EyeLaser = LoadAsset<Texture2D>("Images\\Eye_Laser", mode);
			TextureAssets.BoneEyes = LoadAsset<Texture2D>("Images\\Bone_Eyes", mode);
			TextureAssets.BoneLaser = LoadAsset<Texture2D>("Images\\Bone_Laser", mode);
			TextureAssets.LightDisc = LoadAsset<Texture2D>("Images\\Light_Disc", mode);
			TextureAssets.Confuse = LoadAsset<Texture2D>("Images\\Confuse", mode);
			TextureAssets.Probe = LoadAsset<Texture2D>("Images\\Probe", mode);
			TextureAssets.SunOrb = LoadAsset<Texture2D>("Images\\SunOrb", mode);
			TextureAssets.SunAltar = LoadAsset<Texture2D>("Images\\SunAltar", mode);
			TextureAssets.XmasLight = LoadAsset<Texture2D>("Images\\XmasLight", mode);
			TextureAssets.Beetle = LoadAsset<Texture2D>("Images\\BeetleOrb", mode);
			for (int num59 = 0; num59 < 17; num59++)
			{
				TextureAssets.Chains[num59] = LoadAsset<Texture2D>("Images\\Chains_" + num59, mode);
			}
			TextureAssets.Chain20 = LoadAsset<Texture2D>("Images\\Chain20", mode);
			TextureAssets.FishingLine = LoadAsset<Texture2D>("Images\\FishingLine", mode);
			TextureAssets.Chain = LoadAsset<Texture2D>("Images\\Chain", mode);
			TextureAssets.Chain2 = LoadAsset<Texture2D>("Images\\Chain2", mode);
			TextureAssets.Chain3 = LoadAsset<Texture2D>("Images\\Chain3", mode);
			TextureAssets.Chain4 = LoadAsset<Texture2D>("Images\\Chain4", mode);
			TextureAssets.Chain5 = LoadAsset<Texture2D>("Images\\Chain5", mode);
			TextureAssets.Chain6 = LoadAsset<Texture2D>("Images\\Chain6", mode);
			TextureAssets.Chain7 = LoadAsset<Texture2D>("Images\\Chain7", mode);
			TextureAssets.Chain8 = LoadAsset<Texture2D>("Images\\Chain8", mode);
			TextureAssets.Chain9 = LoadAsset<Texture2D>("Images\\Chain9", mode);
			TextureAssets.Chain10 = LoadAsset<Texture2D>("Images\\Chain10", mode);
			TextureAssets.Chain11 = LoadAsset<Texture2D>("Images\\Chain11", mode);
			TextureAssets.Chain12 = LoadAsset<Texture2D>("Images\\Chain12", mode);
			TextureAssets.Chain13 = LoadAsset<Texture2D>("Images\\Chain13", mode);
			TextureAssets.Chain14 = LoadAsset<Texture2D>("Images\\Chain14", mode);
			TextureAssets.Chain15 = LoadAsset<Texture2D>("Images\\Chain15", mode);
			TextureAssets.Chain16 = LoadAsset<Texture2D>("Images\\Chain16", mode);
			TextureAssets.Chain17 = LoadAsset<Texture2D>("Images\\Chain17", mode);
			TextureAssets.Chain18 = LoadAsset<Texture2D>("Images\\Chain18", mode);
			TextureAssets.Chain19 = LoadAsset<Texture2D>("Images\\Chain19", mode);
			TextureAssets.Chain20 = LoadAsset<Texture2D>("Images\\Chain20", mode);
			TextureAssets.Chain21 = LoadAsset<Texture2D>("Images\\Chain21", mode);
			TextureAssets.Chain22 = LoadAsset<Texture2D>("Images\\Chain22", mode);
			TextureAssets.Chain23 = LoadAsset<Texture2D>("Images\\Chain23", mode);
			TextureAssets.Chain24 = LoadAsset<Texture2D>("Images\\Chain24", mode);
			TextureAssets.Chain25 = LoadAsset<Texture2D>("Images\\Chain25", mode);
			TextureAssets.Chain26 = LoadAsset<Texture2D>("Images\\Chain26", mode);
			TextureAssets.Chain27 = LoadAsset<Texture2D>("Images\\Chain27", mode);
			TextureAssets.Chain28 = LoadAsset<Texture2D>("Images\\Chain28", mode);
			TextureAssets.Chain29 = LoadAsset<Texture2D>("Images\\Chain29", mode);
			TextureAssets.Chain30 = LoadAsset<Texture2D>("Images\\Chain30", mode);
			TextureAssets.Chain31 = LoadAsset<Texture2D>("Images\\Chain31", mode);
			TextureAssets.Chain32 = LoadAsset<Texture2D>("Images\\Chain32", mode);
			TextureAssets.Chain33 = LoadAsset<Texture2D>("Images\\Chain33", mode);
			TextureAssets.Chain34 = LoadAsset<Texture2D>("Images\\Chain34", mode);
			TextureAssets.Chain35 = LoadAsset<Texture2D>("Images\\Chain35", mode);
			TextureAssets.Chain36 = LoadAsset<Texture2D>("Images\\Chain36", mode);
			TextureAssets.Chain37 = LoadAsset<Texture2D>("Images\\Chain37", mode);
			TextureAssets.Chain38 = LoadAsset<Texture2D>("Images\\Chain38", mode);
			TextureAssets.Chain39 = LoadAsset<Texture2D>("Images\\Chain39", mode);
			TextureAssets.Chain40 = LoadAsset<Texture2D>("Images\\Chain40", mode);
			TextureAssets.Chain41 = LoadAsset<Texture2D>("Images\\Chain41", mode);
			TextureAssets.Chain42 = LoadAsset<Texture2D>("Images\\Chain42", mode);
			TextureAssets.Chain43 = LoadAsset<Texture2D>("Images\\Chain43", mode);
			TextureAssets.EyeLaserSmall = LoadAsset<Texture2D>("Images\\Eye_Laser_Small", mode);
			TextureAssets.BoneArm = LoadAsset<Texture2D>("Images\\Arm_Bone", mode);
			TextureAssets.PumpkingArm = LoadAsset<Texture2D>("Images\\PumpkingArm", mode);
			TextureAssets.PumpkingCloak = LoadAsset<Texture2D>("Images\\PumpkingCloak", mode);
			TextureAssets.BoneArm2 = LoadAsset<Texture2D>("Images\\Arm_Bone_2", mode);
			for (int num60 = 1; num60 < TextureAssets.GemChain.Length; num60++)
			{
				TextureAssets.GemChain[num60] = LoadAsset<Texture2D>("Images\\GemChain_" + num60, mode);
			}
			for (int num61 = 1; num61 < TextureAssets.Golem.Length; num61++)
			{
				TextureAssets.Golem[num61] = LoadAsset<Texture2D>("Images\\GolemLights" + num61, mode);
			}
			TextureAssets.GolfSwingBarFill = LoadAsset<Texture2D>("Images\\UI\\GolfSwingBarFill", mode);
			TextureAssets.GolfSwingBarPanel = LoadAsset<Texture2D>("Images\\UI\\GolfSwingBarPanel", mode);
			TextureAssets.SpawnPoint = LoadAsset<Texture2D>("Images\\UI\\SpawnPoint", mode);
			TextureAssets.SpawnBed = LoadAsset<Texture2D>("Images\\UI\\SpawnBed", mode);
			TextureAssets.MapPing = LoadAsset<Texture2D>("Images\\UI\\MapPing", mode);
			TextureAssets.GolfBallArrow = LoadAsset<Texture2D>("Images\\UI\\GolfBall_Arrow", mode);
			TextureAssets.GolfBallArrowShadow = LoadAsset<Texture2D>("Images\\UI\\GolfBall_Arrow_Shadow", mode);
			TextureAssets.GolfBallOutline = LoadAsset<Texture2D>("Images\\Misc\\GolfBallOutline", mode);
			LoadMinimapFrames(mode);
			LoadPlayerResourceSets(mode);
			Main.AchievementAdvisor.LoadContent();
		}

		private static void LoadMinimapFrames(AssetRequestMode mode)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			float num = 2f;
			float num2 = 6f;
			LoadMinimap("Default", new Vector2(-8f, -15f), new Vector2(148f + num, 234f + num2), new Vector2(200f + num, 234f + num2), new Vector2(174f + num, 234f + num2), mode);
			LoadMinimap("Golden", new Vector2(-10f, -10f), new Vector2(136f, 248f), new Vector2(96f, 248f), new Vector2(116f, 248f), mode);
			LoadMinimap("Remix", new Vector2(-10f, -10f), new Vector2(200f, 234f), new Vector2(148f, 234f), new Vector2(174f, 234f), mode);
			LoadMinimap("Sticks", new Vector2(-10f, -10f), new Vector2(148f, 234f), new Vector2(200f, 234f), new Vector2(174f, 234f), mode);
			LoadMinimap("StoneGold", new Vector2(-15f, -15f), new Vector2(220f, 244f), new Vector2(244f, 188f), new Vector2(244f, 216f), mode);
			LoadMinimap("TwigLeaf", new Vector2(-20f, -20f), new Vector2(206f, 242f), new Vector2(162f, 242f), new Vector2(184f, 242f), mode);
			LoadMinimap("Leaf", new Vector2(-20f, -20f), new Vector2(212f, 244f), new Vector2(168f, 246f), new Vector2(190f, 246f), mode);
			LoadMinimap("Retro", new Vector2(-10f, -10f), new Vector2(150f, 236f), new Vector2(202f, 236f), new Vector2(176f, 236f), mode);
			LoadMinimap("Valkyrie", new Vector2(-10f, -10f), new Vector2(154f, 242f), new Vector2(206f, 240f), new Vector2(180f, 244f), mode);
			string frameName = Main.Configuration.Get("MinimapFrame", "Default");
			Main.ActiveMinimapFrame = Main.MinimapFrames.FirstOrDefault((KeyValuePair<string, MinimapFrame> pair) => pair.Key == frameName).Value;
			if (Main.ActiveMinimapFrame == null)
			{
				Main.ActiveMinimapFrame = Main.MinimapFrames.Values.First();
			}
			Main.Configuration.OnSave += Configuration_OnSave_MinimapFrame;
		}

		private static void Configuration_OnSave_MinimapFrame(Preferences obj)
		{
			string text = Main.MinimapFrames.FirstOrDefault((KeyValuePair<string, MinimapFrame> pair) => pair.Value == Main.ActiveMinimapFrame).Key;
			if (text == null)
			{
				text = "Default";
			}
			obj.Put("MinimapFrame", text);
		}

		private static void LoadMinimap(string name, Vector2 frameOffset, Vector2 resetPosition, Vector2 zoomInPosition, Vector2 zoomOutPosition, AssetRequestMode mode)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			MinimapFrame minimapFrame = new MinimapFrame(LoadAsset<Texture2D>("Images\\UI\\Minimap\\" + name + "\\MinimapFrame", mode), frameOffset);
			minimapFrame.SetResetButton(LoadAsset<Texture2D>("Images\\UI\\Minimap\\" + name + "\\MinimapButton_Reset", mode), resetPosition);
			minimapFrame.SetZoomOutButton(LoadAsset<Texture2D>("Images\\UI\\Minimap\\" + name + "\\MinimapButton_ZoomOut", mode), zoomOutPosition);
			minimapFrame.SetZoomInButton(LoadAsset<Texture2D>("Images\\UI\\Minimap\\" + name + "\\MinimapButton_ZoomIn", mode), zoomInPosition);
			Main.MinimapFrames[name] = minimapFrame;
		}

		private static void LoadPlayerResourceSets(AssetRequestMode mode)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Main.PlayerResourcesSets["Default"] = new ClassicPlayerResourcesDisplaySet();
			Main.PlayerResourcesSets["New"] = new FancyClassicPlayerResourcesDisplaySet("FancyClassic", mode);
			Main.PlayerResourcesSets["HorizontalBars"] = new HorizontalBarsPlayerReosurcesDisplaySet("HorizontalBars", mode);
			string frameName = Main.Configuration.Get("PlayerResourcesSet", "New");
			Main.ActivePlayerResourcesSet = Main.PlayerResourcesSets.FirstOrDefault((KeyValuePair<string, IPlayerResourcesDisplaySet> pair) => pair.Key == frameName).Value;
			if (Main.ActivePlayerResourcesSet == null)
			{
				Main.ActivePlayerResourcesSet = Main.PlayerResourcesSets.Values.First();
			}
			Main.Configuration.OnSave += Configuration_OnSave_PlayerResourcesSet;
		}

		private static void Configuration_OnSave_PlayerResourcesSet(Preferences obj)
		{
			string text = Main.PlayerResourcesSets.FirstOrDefault((KeyValuePair<string, IPlayerResourcesDisplaySet> pair) => pair.Value == Main.ActivePlayerResourcesSet).Key;
			if (text == null)
			{
				text = "New";
			}
			obj.Put("PlayerResourcesSet", text);
		}

		private static Asset<T> LoadAsset<T>(string assetName, AssetRequestMode mode) where T : class
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Main.Assets.Request<T>(assetName, mode);
		}
	}
}
