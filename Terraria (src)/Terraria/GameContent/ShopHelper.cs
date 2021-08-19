using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;

namespace Terraria.GameContent
{
	public class ShopHelper
	{
		public const float LowestPossiblePriceMultiplier = 0.75f;

		public const float MaxHappinessAchievementPriceMultiplier = 0.82f;

		public const float HighestPossiblePriceMultiplier = 1.5f;

		private string _currentHappiness;

		private float _currentPriceAdjustment;

		private NPC _currentNPCBeingTalkedTo;

		private Player _currentPlayerTalking;

		private const float likeValue = 0.94f;

		private const float dislikeValue = 1.06f;

		private const float loveValue = 0.88f;

		private const float hateValue = 1.12f;

		public ShoppingSettings GetShoppingSettings(Player player, NPC npc)
		{
			ShoppingSettings shoppingSettings = default(ShoppingSettings);
			shoppingSettings.PriceAdjustment = 1.0;
			shoppingSettings.HappinessReport = "";
			ShoppingSettings result = shoppingSettings;
			_currentNPCBeingTalkedTo = npc;
			_currentPlayerTalking = player;
			ProcessMood(player, npc);
			result.PriceAdjustment = _currentPriceAdjustment;
			result.HappinessReport = _currentHappiness;
			return result;
		}

		private float GetSkeletonMerchantPrices(NPC npc)
		{
			float num = 1f;
			if (Main.moonPhase == 1 || Main.moonPhase == 7)
			{
				num = 1.1f;
			}
			if (Main.moonPhase == 2 || Main.moonPhase == 6)
			{
				num = 1.2f;
			}
			if (Main.moonPhase == 3 || Main.moonPhase == 5)
			{
				num = 1.3f;
			}
			if (Main.moonPhase == 4)
			{
				num = 1.4f;
			}
			if (Main.dayTime)
			{
				num += 0.1f;
			}
			return num;
		}

		private float GetTravelingMerchantPrices(NPC npc)
		{
			Vector2 value = npc.Center / 16f;
			Vector2 value2 = new Vector2(Main.spawnTileX, Main.spawnTileY);
			float num = Vector2.Distance(value, value2) / (float)(Main.maxTilesX / 2);
			num = 1.5f - num;
			return (2f + num) / 3f;
		}

		private void ProcessMood(Player player, NPC npc)
		{
			_currentHappiness = "";
			_currentPriceAdjustment = 1f;
			if (npc.type == 368)
			{
				_currentPriceAdjustment = 1f;
			}
			else if (npc.type == 453)
			{
				_currentPriceAdjustment = 1f;
			}
			else
			{
				if (npc.type == 656 || npc.type == 637 || npc.type == 638)
				{
					return;
				}
				if (IsNotReallyTownNPC(npc))
				{
					_currentPriceAdjustment = 1f;
					return;
				}
				if (RuinMoodIfHomeless(npc))
				{
					_currentPriceAdjustment = 1000f;
				}
				else if (IsFarFromHome(npc))
				{
					_currentPriceAdjustment = 1000f;
				}
				if (IsPlayerInEvilBiomes(player))
				{
					_currentPriceAdjustment = 1000f;
				}
				int npcsWithinHouse;
				int npcsWithinVillage;
				List<NPC> nearbyResidentNPCs = GetNearbyResidentNPCs(npc, out npcsWithinHouse, out npcsWithinVillage);
				bool flag = true;
				float num = 1.05f;
				if (npc.type == 663)
				{
					flag = false;
					num = 1f;
					if (npcsWithinHouse < 2 && npcsWithinVillage < 2)
					{
						AddHappinessReportText("HateLonely");
						_currentPriceAdjustment = 1000f;
					}
				}
				if (true && npcsWithinHouse > 2)
				{
					for (int i = 2; i < npcsWithinHouse; i++)
					{
						_currentPriceAdjustment *= num;
					}
					if (npcsWithinHouse > 5)
					{
						AddHappinessReportText("HateCrowded");
					}
					else
					{
						AddHappinessReportText("DislikeCrowded");
					}
				}
				if (flag && npcsWithinHouse <= 2 && npcsWithinVillage < 4)
				{
					AddHappinessReportText("LoveSpace");
					_currentPriceAdjustment *= 0.95f;
				}
				bool[] array = new bool[668];
				foreach (NPC item in nearbyResidentNPCs)
				{
					array[item.type] = true;
				}
				HelperInfo helperInfo = default(HelperInfo);
				helperInfo.player = player;
				helperInfo.npc = npc;
				helperInfo.NearbyNPCs = nearbyResidentNPCs;
				helperInfo.PrimaryPlayerBiome = player.GetPrimaryBiome();
				helperInfo.nearbyNPCsByType = array;
				HelperInfo info = helperInfo;
				new AllPersonalitiesModifier().ModifyShopPrice(info, this);
				if (_currentHappiness == "")
				{
					AddHappinessReportText("Content");
				}
				_currentPriceAdjustment = LimitAndRoundMultiplier(_currentPriceAdjustment);
			}
		}

		private float LimitAndRoundMultiplier(float priceAdjustment)
		{
			priceAdjustment = MathHelper.Clamp(priceAdjustment, 0.75f, 1.5f);
			priceAdjustment = (float)Math.Round(priceAdjustment * 100f) / 100f;
			return priceAdjustment;
		}

		private static string BiomeNameByKey(int biomeID)
		{
			string text = null;
			return Language.GetTextValue("TownNPCMoodBiomes." + biomeID switch
			{
				1 => "NormalUnderground", 
				2 => "Snow", 
				3 => "Desert", 
				4 => "Jungle", 
				5 => "Ocean", 
				6 => "Hallow", 
				7 => "Mushroom", 
				8 => "Dungeon", 
				9 => "Corruption", 
				10 => "Crimson", 
				_ => "Forest", 
			});
		}

		private void AddHappinessReportText(string textKeyInCategory, object substitutes = null)
		{
			string text = "TownNPCMood_" + NPCID.Search.GetName(_currentNPCBeingTalkedTo.netID);
			if (_currentNPCBeingTalkedTo.type == 633 && _currentNPCBeingTalkedTo.altTexture == 2)
			{
				text += "Transformed";
			}
			string textValueWith = Language.GetTextValueWith(text + "." + textKeyInCategory, substitutes);
			_currentHappiness = _currentHappiness + textValueWith + " ";
		}

		public void LikeBiome(int biomeID)
		{
			AddHappinessReportText("LikeBiome", new
			{
				BiomeName = BiomeNameByKey(biomeID)
			});
			_currentPriceAdjustment *= 0.94f;
		}

		public void LoveBiome(int biomeID)
		{
			AddHappinessReportText("LoveBiome", new
			{
				BiomeName = BiomeNameByKey(biomeID)
			});
			_currentPriceAdjustment *= 0.88f;
		}

		public void DislikeBiome(int biomeID)
		{
			AddHappinessReportText("DislikeBiome", new
			{
				BiomeName = BiomeNameByKey(biomeID)
			});
			_currentPriceAdjustment *= 1.06f;
		}

		public void HateBiome(int biomeID)
		{
			AddHappinessReportText("HateBiome", new
			{
				BiomeName = BiomeNameByKey(biomeID)
			});
			_currentPriceAdjustment *= 1.12f;
		}

		public void LikeNPC(int npcType)
		{
			AddHappinessReportText("LikeNPC", new
			{
				NPCName = NPC.GetFullnameByID(npcType)
			});
			_currentPriceAdjustment *= 0.94f;
		}

		public void LoveNPCByTypeName(int npcType)
		{
			AddHappinessReportText("LoveNPC_" + NPCID.Search.GetName(npcType), new
			{
				NPCName = NPC.GetFullnameByID(npcType)
			});
			_currentPriceAdjustment *= 0.88f;
		}

		public void LikePrincess()
		{
			AddHappinessReportText("LikeNPC_Princess", new
			{
				NPCName = NPC.GetFullnameByID(663)
			});
			_currentPriceAdjustment *= 0.94f;
		}

		public void LoveNPC(int npcType)
		{
			AddHappinessReportText("LoveNPC", new
			{
				NPCName = NPC.GetFullnameByID(npcType)
			});
			_currentPriceAdjustment *= 0.88f;
		}

		public void DislikeNPC(int npcType)
		{
			AddHappinessReportText("DislikeNPC", new
			{
				NPCName = NPC.GetFullnameByID(npcType)
			});
			_currentPriceAdjustment *= 1.06f;
		}

		public void HateNPC(int npcType)
		{
			AddHappinessReportText("HateNPC", new
			{
				NPCName = NPC.GetFullnameByID(npcType)
			});
			_currentPriceAdjustment *= 1.12f;
		}

		private List<NPC> GetNearbyResidentNPCs(NPC npc, out int npcsWithinHouse, out int npcsWithinVillage)
		{
			List<NPC> list = new List<NPC>();
			npcsWithinHouse = 0;
			npcsWithinVillage = 0;
			Vector2 value = new Vector2(npc.homeTileX, npc.homeTileY);
			if (npc.homeless)
			{
				value = new Vector2(npc.Center.X / 16f, npc.Center.Y / 16f);
			}
			for (int i = 0; i < 200; i++)
			{
				if (i == npc.whoAmI)
				{
					continue;
				}
				NPC nPC = Main.npc[i];
				if (nPC.active && nPC.townNPC && !IsNotReallyTownNPC(nPC) && !WorldGen.TownManager.CanNPCsLiveWithEachOther_ShopHelper(npc, nPC))
				{
					Vector2 value2 = new Vector2(nPC.homeTileX, nPC.homeTileY);
					if (nPC.homeless)
					{
						value2 = nPC.Center / 16f;
					}
					float num = Vector2.Distance(value, value2);
					if (num < 25f)
					{
						list.Add(nPC);
						npcsWithinHouse++;
					}
					else if (num < 120f)
					{
						npcsWithinVillage++;
					}
				}
			}
			return list;
		}

		private bool RuinMoodIfHomeless(NPC npc)
		{
			if (npc.homeless)
			{
				AddHappinessReportText("NoHome");
			}
			return npc.homeless;
		}

		private bool IsFarFromHome(NPC npc)
		{
			Vector2 value = new Vector2(npc.homeTileX, npc.homeTileY);
			Vector2 value2 = new Vector2(npc.Center.X / 16f, npc.Center.Y / 16f);
			if (Vector2.Distance(value, value2) > 120f)
			{
				AddHappinessReportText("FarFromHome");
				return true;
			}
			return false;
		}

		private bool IsPlayerInEvilBiomes(Player player)
		{
			if (player.ZoneCorrupt)
			{
				AddHappinessReportText("HateBiome", new
				{
					BiomeName = BiomeNameByKey(9)
				});
				return true;
			}
			if (player.ZoneCrimson)
			{
				AddHappinessReportText("HateBiome", new
				{
					BiomeName = BiomeNameByKey(10)
				});
				return true;
			}
			if (player.ZoneDungeon)
			{
				AddHappinessReportText("HateBiome", new
				{
					BiomeName = BiomeNameByKey(8)
				});
				return true;
			}
			return false;
		}

		private bool IsNotReallyTownNPC(NPC npc)
		{
			int type = npc.type;
			if (type == 37 || type == 368 || type == 453)
			{
				return true;
			}
			return false;
		}
	}
}
