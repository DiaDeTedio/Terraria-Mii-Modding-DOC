using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.GameContent.Golf;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Net;
using Terraria.Testing;
using Terraria.UI;

namespace Terraria
{
	public class MessageBuffer
	{
		public const int readBufferMax = 131070;

		public const int writeBufferMax = 131070;

		public bool broadcast;

		public byte[] readBuffer = new byte[131070];

		public byte[] writeBuffer = new byte[131070];

		public bool writeLocked;

		public int messageLength;

		public int totalData;

		public int whoAmI;

		public int spamCount;

		public int maxSpam;

		public bool checkBytes;

		public MemoryStream readerStream;

		public MemoryStream writerStream;

		public BinaryReader reader;

		public BinaryWriter writer;

		public PacketHistory History = new PacketHistory();

		public static event TileChangeReceivedEvent OnTileChangeReceived;

		public void Reset()
		{
			Array.Clear(readBuffer, 0, readBuffer.Length);
			Array.Clear(writeBuffer, 0, writeBuffer.Length);
			writeLocked = false;
			messageLength = 0;
			totalData = 0;
			spamCount = 0;
			broadcast = false;
			checkBytes = false;
			ResetReader();
			ResetWriter();
		}

		public void ResetReader()
		{
			if (readerStream != null)
			{
				readerStream.Close();
			}
			readerStream = new MemoryStream(readBuffer);
			reader = new BinaryReader(readerStream);
		}

		public void ResetWriter()
		{
			if (writerStream != null)
			{
				writerStream.Close();
			}
			writerStream = new MemoryStream(writeBuffer);
			writer = new BinaryWriter(writerStream);
		}

		public void GetData(int start, int length, out int messageType)
		{
			if (whoAmI < 256)
			{
				Netplay.Clients[whoAmI].TimeOutTimer = 0;
			}
			else
			{
				Netplay.Connection.TimeOutTimer = 0;
			}
			byte b = 0;
			int num = 0;
			num = start + 1;
			b = (byte)(messageType = readBuffer[start]);
			if (b >= 141)
			{
				return;
			}
			Main.ActiveNetDiagnosticsUI.CountReadMessage(b, length);
			if (Main.netMode == 1 && Netplay.Connection.StatusMax > 0)
			{
				Netplay.Connection.StatusCount++;
			}
			if (Main.verboseNetplay)
			{
				for (int i = start; i < start + length; i++)
				{
				}
				for (int j = start; j < start + length; j++)
				{
					_ = readBuffer[j];
				}
			}
			if (Main.netMode == 2 && b != 38 && Netplay.Clients[whoAmI].State == -1)
			{
				NetMessage.TrySendData(2, whoAmI, -1, Lang.mp[1].ToNetworkText());
				return;
			}
			if (Main.netMode == 2)
			{
				if (Netplay.Clients[whoAmI].State < 10 && b > 12 && b != 93 && b != 16 && b != 42 && b != 50 && b != 38 && b != 68)
				{
					NetMessage.BootPlayer(whoAmI, Lang.mp[2].ToNetworkText());
				}
				if (Netplay.Clients[whoAmI].State == 0 && b != 1)
				{
					NetMessage.BootPlayer(whoAmI, Lang.mp[2].ToNetworkText());
				}
			}
			if (reader == null)
			{
				ResetReader();
			}
			reader.BaseStream.Position = num;
			NPCSpawnParams spawnparams;
			switch (b)
			{
			case 1:
				if (Main.netMode != 2)
				{
					break;
				}
				if (Main.dedServ && Netplay.IsBanned(Netplay.Clients[whoAmI].Socket.GetRemoteAddress()))
				{
					NetMessage.TrySendData(2, whoAmI, -1, Lang.mp[3].ToNetworkText());
				}
				else
				{
					if (Netplay.Clients[whoAmI].State != 0)
					{
						break;
					}
					if (reader.ReadString() == "Terraria" + 238)
					{
						if (string.IsNullOrEmpty(Netplay.ServerPassword))
						{
							Netplay.Clients[whoAmI].State = 1;
							NetMessage.TrySendData(3, whoAmI);
						}
						else
						{
							Netplay.Clients[whoAmI].State = -1;
							NetMessage.TrySendData(37, whoAmI);
						}
					}
					else
					{
						NetMessage.TrySendData(2, whoAmI, -1, Lang.mp[4].ToNetworkText());
					}
				}
				break;
			case 2:
				if (Main.netMode == 1)
				{
					Netplay.Disconnect = true;
					Main.statusText = NetworkText.Deserialize(reader).ToString();
				}
				break;
			case 3:
				if (Main.netMode == 1)
				{
					if (Netplay.Connection.State == 1)
					{
						Netplay.Connection.State = 2;
					}
					int num71 = reader.ReadByte();
					bool value4 = reader.ReadBoolean();
					Netplay.Connection.ServerSpecialFlags[2] = value4;
					if (num71 != Main.myPlayer)
					{
						Main.player[num71] = Main.ActivePlayerFileData.Player;
						Main.player[Main.myPlayer] = new Player();
					}
					Main.player[num71].whoAmI = num71;
					Main.myPlayer = num71;
					Player player8 = Main.player[num71];
					NetMessage.TrySendData(4, -1, -1, null, num71);
					NetMessage.TrySendData(68, -1, -1, null, num71);
					NetMessage.TrySendData(16, -1, -1, null, num71);
					NetMessage.TrySendData(42, -1, -1, null, num71);
					NetMessage.TrySendData(50, -1, -1, null, num71);
					for (int num72 = 0; num72 < 59; num72++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, num72, (int)player8.inventory[num72].prefix);
					}
					for (int num73 = 0; num73 < player8.armor.Length; num73++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, 59 + num73, (int)player8.armor[num73].prefix);
					}
					for (int num74 = 0; num74 < player8.dye.Length; num74++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, 79 + num74, (int)player8.dye[num74].prefix);
					}
					for (int num75 = 0; num75 < player8.miscEquips.Length; num75++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, 89 + num75, (int)player8.miscEquips[num75].prefix);
					}
					for (int num76 = 0; num76 < player8.miscDyes.Length; num76++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, 94 + num76, (int)player8.miscDyes[num76].prefix);
					}
					for (int num77 = 0; num77 < player8.bank.item.Length; num77++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, 99 + num77, (int)player8.bank.item[num77].prefix);
					}
					for (int num78 = 0; num78 < player8.bank2.item.Length; num78++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, 139 + num78, (int)player8.bank2.item[num78].prefix);
					}
					NetMessage.TrySendData(5, -1, -1, null, num71, 179f, (int)player8.trashItem.prefix);
					for (int num79 = 0; num79 < player8.bank3.item.Length; num79++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, 180 + num79, (int)player8.bank3.item[num79].prefix);
					}
					for (int num80 = 0; num80 < player8.bank4.item.Length; num80++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num71, 220 + num80, (int)player8.bank4.item[num80].prefix);
					}
					NetMessage.TrySendData(6);
					if (Netplay.Connection.State == 2)
					{
						Netplay.Connection.State = 3;
					}
				}
				break;
			case 4:
			{
				int num22 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num22 = whoAmI;
				}
				if (num22 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					break;
				}
				Player player2 = Main.player[num22];
				player2.whoAmI = num22;
				player2.skinVariant = reader.ReadByte();
				player2.skinVariant = (int)MathHelper.Clamp(player2.skinVariant, 0f, 11f);
				player2.hair = reader.ReadByte();
				if (player2.hair >= 163)
				{
					player2.hair = 0;
				}
				player2.name = reader.ReadString().Trim().Trim();
				player2.hairDye = reader.ReadByte();
				BitsByte bitsByte2 = reader.ReadByte();
				for (int m = 0; m < 8; m++)
				{
					player2.hideVisibleAccessory[m] = bitsByte2[m];
				}
				bitsByte2 = reader.ReadByte();
				for (int n = 0; n < 2; n++)
				{
					player2.hideVisibleAccessory[n + 8] = bitsByte2[n];
				}
				player2.hideMisc = reader.ReadByte();
				player2.hairColor = reader.ReadRGB();
				player2.skinColor = reader.ReadRGB();
				player2.eyeColor = reader.ReadRGB();
				player2.shirtColor = reader.ReadRGB();
				player2.underShirtColor = reader.ReadRGB();
				player2.pantsColor = reader.ReadRGB();
				player2.shoeColor = reader.ReadRGB();
				BitsByte bitsByte3 = reader.ReadByte();
				player2.difficulty = 0;
				if (bitsByte3[0])
				{
					player2.difficulty = 1;
				}
				if (bitsByte3[1])
				{
					player2.difficulty = 2;
				}
				if (bitsByte3[3])
				{
					player2.difficulty = 3;
				}
				if (player2.difficulty > 3)
				{
					player2.difficulty = 3;
				}
				player2.extraAccessory = bitsByte3[2];
				BitsByte bitsByte4 = reader.ReadByte();
				player2.UsingBiomeTorches = bitsByte4[0];
				player2.happyFunTorchTime = bitsByte4[1];
				player2.unlockedBiomeTorches = bitsByte4[2];
				if (Main.netMode != 2)
				{
					break;
				}
				bool flag2 = false;
				if (Netplay.Clients[whoAmI].State < 10)
				{
					for (int num23 = 0; num23 < 255; num23++)
					{
						if (num23 != num22 && player2.name == Main.player[num23].name && Netplay.Clients[num23].IsActive)
						{
							flag2 = true;
						}
					}
				}
				if (flag2)
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey(Lang.mp[5].Key, player2.name));
				}
				else if (player2.name.Length > Player.nameLen)
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey("Net.NameTooLong"));
				}
				else if (player2.name == "")
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey("Net.EmptyName"));
				}
				else if (player2.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"));
				}
				else if (player2.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"));
				}
				else
				{
					Netplay.Clients[whoAmI].Name = player2.name;
					Netplay.Clients[whoAmI].Name = player2.name;
					NetMessage.TrySendData(4, -1, whoAmI, null, num22);
				}
				break;
			}
			case 5:
			{
				int num231 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num231 = whoAmI;
				}
				if (num231 == Main.myPlayer && !Main.ServerSideCharacter && !Main.player[num231].HasLockedInventory())
				{
					break;
				}
				Player player14 = Main.player[num231];
				lock (player14)
				{
					int num232 = reader.ReadInt16();
					int stack6 = reader.ReadInt16();
					int num233 = reader.ReadByte();
					int type14 = reader.ReadInt16();
					Item[] array3 = null;
					Item[] array4 = null;
					int num234 = 0;
					bool flag14 = false;
					if (num232 >= 220)
					{
						num234 = num232 - 220;
						array3 = player14.bank4.item;
						array4 = Main.clientPlayer.bank4.item;
					}
					else if (num232 >= 180)
					{
						num234 = num232 - 180;
						array3 = player14.bank3.item;
						array4 = Main.clientPlayer.bank3.item;
					}
					else if (num232 >= 179)
					{
						flag14 = true;
					}
					else if (num232 >= 139)
					{
						num234 = num232 - 139;
						array3 = player14.bank2.item;
						array4 = Main.clientPlayer.bank2.item;
					}
					else if (num232 >= 99)
					{
						num234 = num232 - 99;
						array3 = player14.bank.item;
						array4 = Main.clientPlayer.bank.item;
					}
					else if (num232 >= 94)
					{
						num234 = num232 - 94;
						array3 = player14.miscDyes;
						array4 = Main.clientPlayer.miscDyes;
					}
					else if (num232 >= 89)
					{
						num234 = num232 - 89;
						array3 = player14.miscEquips;
						array4 = Main.clientPlayer.miscEquips;
					}
					else if (num232 >= 79)
					{
						num234 = num232 - 79;
						array3 = player14.dye;
						array4 = Main.clientPlayer.dye;
					}
					else if (num232 >= 59)
					{
						num234 = num232 - 59;
						array3 = player14.armor;
						array4 = Main.clientPlayer.armor;
					}
					else
					{
						num234 = num232;
						array3 = player14.inventory;
						array4 = Main.clientPlayer.inventory;
					}
					if (flag14)
					{
						player14.trashItem = new Item();
						player14.trashItem.netDefaults(type14);
						player14.trashItem.stack = stack6;
						player14.trashItem.Prefix(num233);
						if (num231 == Main.myPlayer && !Main.ServerSideCharacter)
						{
							Main.clientPlayer.trashItem = player14.trashItem.Clone();
						}
					}
					else if (num232 <= 58)
					{
						int type15 = array3[num234].type;
						int stack7 = array3[num234].stack;
						array3[num234] = new Item();
						array3[num234].netDefaults(type14);
						array3[num234].stack = stack6;
						array3[num234].Prefix(num233);
						if (num231 == Main.myPlayer && !Main.ServerSideCharacter)
						{
							array4[num234] = array3[num234].Clone();
						}
						if (num231 == Main.myPlayer && num234 == 58)
						{
							Main.mouseItem = array3[num234].Clone();
						}
						if (num231 == Main.myPlayer && Main.netMode == 1)
						{
							Main.player[num231].inventoryChestStack[num232] = false;
							if (array3[num234].stack != stack7 || array3[num234].type != type15)
							{
								Recipe.FindRecipes(canDelayCheck: true);
								SoundEngine.PlaySound(7);
							}
						}
					}
					else
					{
						array3[num234] = new Item();
						array3[num234].netDefaults(type14);
						array3[num234].stack = stack6;
						array3[num234].Prefix(num233);
						if (num231 == Main.myPlayer && !Main.ServerSideCharacter)
						{
							array4[num234] = array3[num234].Clone();
						}
					}
					if (Main.netMode == 2 && num231 == whoAmI && num232 <= 58 + player14.armor.Length + player14.dye.Length + player14.miscEquips.Length + player14.miscDyes.Length)
					{
						NetMessage.TrySendData(5, -1, whoAmI, null, num231, num232, num233);
					}
				}
				break;
			}
			case 6:
				if (Main.netMode == 2)
				{
					if (Netplay.Clients[whoAmI].State == 1)
					{
						Netplay.Clients[whoAmI].State = 2;
					}
					NetMessage.TrySendData(7, whoAmI);
					Main.SyncAnInvasion(whoAmI);
				}
				break;
			case 7:
				if (Main.netMode == 1)
				{
					Main.time = reader.ReadInt32();
					BitsByte bitsByte20 = reader.ReadByte();
					Main.dayTime = bitsByte20[0];
					Main.bloodMoon = bitsByte20[1];
					Main.eclipse = bitsByte20[2];
					Main.moonPhase = reader.ReadByte();
					Main.maxTilesX = reader.ReadInt16();
					Main.maxTilesY = reader.ReadInt16();
					Main.spawnTileX = reader.ReadInt16();
					Main.spawnTileY = reader.ReadInt16();
					Main.worldSurface = reader.ReadInt16();
					Main.rockLayer = reader.ReadInt16();
					Main.worldID = reader.ReadInt32();
					Main.worldName = reader.ReadString();
					Main.GameMode = reader.ReadByte();
					Main.ActiveWorldFileData.UniqueId = new Guid(reader.ReadBytes(16));
					Main.ActiveWorldFileData.WorldGeneratorVersion = reader.ReadUInt64();
					Main.moonType = reader.ReadByte();
					WorldGen.setBG(0, reader.ReadByte());
					WorldGen.setBG(10, reader.ReadByte());
					WorldGen.setBG(11, reader.ReadByte());
					WorldGen.setBG(12, reader.ReadByte());
					WorldGen.setBG(1, reader.ReadByte());
					WorldGen.setBG(2, reader.ReadByte());
					WorldGen.setBG(3, reader.ReadByte());
					WorldGen.setBG(4, reader.ReadByte());
					WorldGen.setBG(5, reader.ReadByte());
					WorldGen.setBG(6, reader.ReadByte());
					WorldGen.setBG(7, reader.ReadByte());
					WorldGen.setBG(8, reader.ReadByte());
					WorldGen.setBG(9, reader.ReadByte());
					Main.iceBackStyle = reader.ReadByte();
					Main.jungleBackStyle = reader.ReadByte();
					Main.hellBackStyle = reader.ReadByte();
					Main.windSpeedTarget = reader.ReadSingle();
					Main.numClouds = reader.ReadByte();
					for (int num249 = 0; num249 < 3; num249++)
					{
						Main.treeX[num249] = reader.ReadInt32();
					}
					for (int num250 = 0; num250 < 4; num250++)
					{
						Main.treeStyle[num250] = reader.ReadByte();
					}
					for (int num251 = 0; num251 < 3; num251++)
					{
						Main.caveBackX[num251] = reader.ReadInt32();
					}
					for (int num252 = 0; num252 < 4; num252++)
					{
						Main.caveBackStyle[num252] = reader.ReadByte();
					}
					WorldGen.TreeTops.SyncReceive(reader);
					WorldGen.BackgroundsCache.UpdateCache();
					Main.maxRaining = reader.ReadSingle();
					Main.raining = Main.maxRaining > 0f;
					BitsByte bitsByte21 = reader.ReadByte();
					WorldGen.shadowOrbSmashed = bitsByte21[0];
					NPC.downedBoss1 = bitsByte21[1];
					NPC.downedBoss2 = bitsByte21[2];
					NPC.downedBoss3 = bitsByte21[3];
					Main.hardMode = bitsByte21[4];
					NPC.downedClown = bitsByte21[5];
					Main.ServerSideCharacter = bitsByte21[6];
					NPC.downedPlantBoss = bitsByte21[7];
					BitsByte bitsByte22 = reader.ReadByte();
					NPC.downedMechBoss1 = bitsByte22[0];
					NPC.downedMechBoss2 = bitsByte22[1];
					NPC.downedMechBoss3 = bitsByte22[2];
					NPC.downedMechBossAny = bitsByte22[3];
					Main.cloudBGActive = (bitsByte22[4] ? 1 : 0);
					WorldGen.crimson = bitsByte22[5];
					Main.pumpkinMoon = bitsByte22[6];
					Main.snowMoon = bitsByte22[7];
					BitsByte bitsByte23 = reader.ReadByte();
					Main.fastForwardTime = bitsByte23[1];
					Main.UpdateTimeRate();
					bool num253 = bitsByte23[2];
					NPC.downedSlimeKing = bitsByte23[3];
					NPC.downedQueenBee = bitsByte23[4];
					NPC.downedFishron = bitsByte23[5];
					NPC.downedMartians = bitsByte23[6];
					NPC.downedAncientCultist = bitsByte23[7];
					BitsByte bitsByte24 = reader.ReadByte();
					NPC.downedMoonlord = bitsByte24[0];
					NPC.downedHalloweenKing = bitsByte24[1];
					NPC.downedHalloweenTree = bitsByte24[2];
					NPC.downedChristmasIceQueen = bitsByte24[3];
					NPC.downedChristmasSantank = bitsByte24[4];
					NPC.downedChristmasTree = bitsByte24[5];
					NPC.downedGolemBoss = bitsByte24[6];
					BirthdayParty.ManualParty = bitsByte24[7];
					BitsByte bitsByte25 = reader.ReadByte();
					NPC.downedPirates = bitsByte25[0];
					NPC.downedFrost = bitsByte25[1];
					NPC.downedGoblins = bitsByte25[2];
					Sandstorm.Happening = bitsByte25[3];
					DD2Event.Ongoing = bitsByte25[4];
					DD2Event.DownedInvasionT1 = bitsByte25[5];
					DD2Event.DownedInvasionT2 = bitsByte25[6];
					DD2Event.DownedInvasionT3 = bitsByte25[7];
					BitsByte bitsByte26 = reader.ReadByte();
					NPC.combatBookWasUsed = bitsByte26[0];
					LanternNight.ManualLanterns = bitsByte26[1];
					NPC.downedTowerSolar = bitsByte26[2];
					NPC.downedTowerVortex = bitsByte26[3];
					NPC.downedTowerNebula = bitsByte26[4];
					NPC.downedTowerStardust = bitsByte26[5];
					Main.forceHalloweenForToday = bitsByte26[6];
					Main.forceXMasForToday = bitsByte26[7];
					BitsByte bitsByte27 = reader.ReadByte();
					NPC.boughtCat = bitsByte27[0];
					NPC.boughtDog = bitsByte27[1];
					NPC.boughtBunny = bitsByte27[2];
					NPC.freeCake = bitsByte27[3];
					Main.drunkWorld = bitsByte27[4];
					NPC.downedEmpressOfLight = bitsByte27[5];
					NPC.downedQueenSlime = bitsByte27[6];
					Main.getGoodWorld = bitsByte27[7];
					Main.tenthAnniversaryWorld = ((BitsByte)reader.ReadByte())[0];
					WorldGen.SavedOreTiers.Copper = reader.ReadInt16();
					WorldGen.SavedOreTiers.Iron = reader.ReadInt16();
					WorldGen.SavedOreTiers.Silver = reader.ReadInt16();
					WorldGen.SavedOreTiers.Gold = reader.ReadInt16();
					WorldGen.SavedOreTiers.Cobalt = reader.ReadInt16();
					WorldGen.SavedOreTiers.Mythril = reader.ReadInt16();
					WorldGen.SavedOreTiers.Adamantite = reader.ReadInt16();
					if (num253)
					{
						Main.StartSlimeRain();
					}
					else
					{
						Main.StopSlimeRain();
					}
					Main.invasionType = reader.ReadSByte();
					Main.LobbyId = reader.ReadUInt64();
					Sandstorm.IntendedSeverity = reader.ReadSingle();
					if (Netplay.Connection.State == 3)
					{
						Main.windSpeedCurrent = Main.windSpeedTarget;
						Netplay.Connection.State = 4;
					}
					Main.checkHalloween();
					Main.checkXMas();
				}
				break;
			case 8:
			{
				if (Main.netMode != 2)
				{
					break;
				}
				int num103 = reader.ReadInt32();
				int num104 = reader.ReadInt32();
				bool flag6 = true;
				if (num103 == -1 || num104 == -1)
				{
					flag6 = false;
				}
				else if (num103 < 10 || num103 > Main.maxTilesX - 10)
				{
					flag6 = false;
				}
				else if (num104 < 10 || num104 > Main.maxTilesY - 10)
				{
					flag6 = false;
				}
				int num105 = Netplay.GetSectionX(Main.spawnTileX) - 2;
				int num106 = Netplay.GetSectionY(Main.spawnTileY) - 1;
				int num107 = num105 + 5;
				int num108 = num106 + 3;
				if (num105 < 0)
				{
					num105 = 0;
				}
				if (num107 >= Main.maxSectionsX)
				{
					num107 = Main.maxSectionsX - 1;
				}
				if (num106 < 0)
				{
					num106 = 0;
				}
				if (num108 >= Main.maxSectionsY)
				{
					num108 = Main.maxSectionsY - 1;
				}
				int num109 = (num107 - num105) * (num108 - num106);
				List<Point> list = new List<Point>();
				for (int num110 = num105; num110 < num107; num110++)
				{
					for (int num111 = num106; num111 < num108; num111++)
					{
						list.Add(new Point(num110, num111));
					}
				}
				int num112 = -1;
				int num113 = -1;
				if (flag6)
				{
					num103 = Netplay.GetSectionX(num103) - 2;
					num104 = Netplay.GetSectionY(num104) - 1;
					num112 = num103 + 5;
					num113 = num104 + 3;
					if (num103 < 0)
					{
						num103 = 0;
					}
					if (num112 >= Main.maxSectionsX)
					{
						num112 = Main.maxSectionsX - 1;
					}
					if (num104 < 0)
					{
						num104 = 0;
					}
					if (num113 >= Main.maxSectionsY)
					{
						num113 = Main.maxSectionsY - 1;
					}
					for (int num114 = num103; num114 <= num112; num114++)
					{
						for (int num115 = num104; num115 <= num113; num115++)
						{
							if (num114 < num105 || num114 >= num107 || num115 < num106 || num115 >= num108)
							{
								list.Add(new Point(num114, num115));
								num109++;
							}
						}
					}
				}
				int num116 = 1;
				PortalHelper.SyncPortalsOnPlayerJoin(whoAmI, 1, list, out var portals, out var portalCenters);
				num109 += portals.Count;
				if (Netplay.Clients[whoAmI].State == 2)
				{
					Netplay.Clients[whoAmI].State = 3;
				}
				NetMessage.TrySendData(9, whoAmI, -1, Lang.inter[44].ToNetworkText(), num109);
				Netplay.Clients[whoAmI].StatusText2 = Language.GetTextValue("Net.IsReceivingTileData");
				Netplay.Clients[whoAmI].StatusMax += num109;
				for (int num117 = num105; num117 < num107; num117++)
				{
					for (int num118 = num106; num118 < num108; num118++)
					{
						NetMessage.SendSection(whoAmI, num117, num118);
					}
				}
				NetMessage.TrySendData(11, whoAmI, -1, null, num105, num106, num107 - 1, num108 - 1);
				if (flag6)
				{
					for (int num119 = num103; num119 <= num112; num119++)
					{
						for (int num120 = num104; num120 <= num113; num120++)
						{
							NetMessage.SendSection(whoAmI, num119, num120, skipSent: true);
						}
					}
					NetMessage.TrySendData(11, whoAmI, -1, null, num103, num104, num112, num113);
				}
				for (int num121 = 0; num121 < portals.Count; num121++)
				{
					NetMessage.SendSection(whoAmI, portals[num121].X, portals[num121].Y, skipSent: true);
				}
				for (int num122 = 0; num122 < portalCenters.Count; num122++)
				{
					NetMessage.TrySendData(11, whoAmI, -1, null, portalCenters[num122].X - num116, portalCenters[num122].Y - num116, portalCenters[num122].X + num116 + 1, portalCenters[num122].Y + num116 + 1);
				}
				for (int num123 = 0; num123 < 400; num123++)
				{
					if (Main.item[num123].active)
					{
						NetMessage.TrySendData(21, whoAmI, -1, null, num123);
						NetMessage.TrySendData(22, whoAmI, -1, null, num123);
					}
				}
				for (int num124 = 0; num124 < 200; num124++)
				{
					if (Main.npc[num124].active)
					{
						NetMessage.TrySendData(23, whoAmI, -1, null, num124);
					}
				}
				for (int num125 = 0; num125 < 1000; num125++)
				{
					if (Main.projectile[num125].active && (Main.projPet[Main.projectile[num125].type] || Main.projectile[num125].netImportant))
					{
						NetMessage.TrySendData(27, whoAmI, -1, null, num125);
					}
				}
				for (int num126 = 0; num126 < 289; num126++)
				{
					NetMessage.TrySendData(83, whoAmI, -1, null, num126);
				}
				NetMessage.TrySendData(49, whoAmI);
				NetMessage.TrySendData(57, whoAmI);
				NetMessage.TrySendData(7, whoAmI);
				NetMessage.TrySendData(103, -1, -1, null, NPC.MoonLordCountdown);
				NetMessage.TrySendData(101, whoAmI);
				NetMessage.TrySendData(136, whoAmI);
				Main.BestiaryTracker.OnPlayerJoining(whoAmI);
				CreativePowerManager.Instance.SyncThingsToJoiningPlayer(whoAmI);
				Main.PylonSystem.OnPlayerJoining(whoAmI);
				break;
			}
			case 9:
				if (Main.netMode == 1)
				{
					Netplay.Connection.StatusMax += reader.ReadInt32();
					Netplay.Connection.StatusText = NetworkText.Deserialize(reader).ToString();
					BitsByte bitsByte9 = reader.ReadByte();
					BitsByte serverSpecialFlags = Netplay.Connection.ServerSpecialFlags;
					serverSpecialFlags[0] = bitsByte9[0];
					serverSpecialFlags[1] = bitsByte9[1];
					Netplay.Connection.ServerSpecialFlags = serverSpecialFlags;
				}
				break;
			case 10:
				if (Main.netMode == 1)
				{
					NetMessage.DecompressTileBlock(readBuffer, num, length);
				}
				break;
			case 11:
				if (Main.netMode == 1)
				{
					WorldGen.SectionTileFrame(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
				}
				break;
			case 12:
			{
				int num229 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num229 = whoAmI;
				}
				Player player13 = Main.player[num229];
				player13.SpawnX = reader.ReadInt16();
				player13.SpawnY = reader.ReadInt16();
				player13.respawnTimer = reader.ReadInt32();
				if (player13.respawnTimer > 0)
				{
					player13.dead = true;
				}
				PlayerSpawnContext playerSpawnContext = (PlayerSpawnContext)reader.ReadByte();
				player13.Spawn(playerSpawnContext);
				if (num229 == Main.myPlayer && Main.netMode != 2)
				{
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.Hooks.EnterWorld(Main.myPlayer);
				}
				if (Main.netMode != 2 || Netplay.Clients[whoAmI].State < 3)
				{
					break;
				}
				if (Netplay.Clients[whoAmI].State == 3)
				{
					Netplay.Clients[whoAmI].State = 10;
					NetMessage.buffer[whoAmI].broadcast = true;
					NetMessage.SyncConnectedPlayer(whoAmI);
					bool flag13 = NetMessage.DoesPlayerSlotCountAsAHost(whoAmI);
					Main.countsAsHostForGameplay[whoAmI] = flag13;
					if (NetMessage.DoesPlayerSlotCountAsAHost(whoAmI))
					{
						NetMessage.TrySendData(139, whoAmI, -1, null, whoAmI, flag13.ToInt());
					}
					NetMessage.TrySendData(12, -1, whoAmI, null, whoAmI, (int)(byte)playerSpawnContext);
					NetMessage.TrySendData(74, whoAmI, -1, NetworkText.FromLiteral(Main.player[whoAmI].name), Main.anglerQuest);
					NetMessage.TrySendData(129, whoAmI);
					NetMessage.greetPlayer(whoAmI);
					if (Main.player[num229].unlockedBiomeTorches)
					{
						NPC nPC5 = new NPC();
						spawnparams = default(NPCSpawnParams);
						nPC5.SetDefaults(664, spawnparams);
						Main.BestiaryTracker.Kills.RegisterKill(nPC5);
					}
				}
				else
				{
					NetMessage.TrySendData(12, -1, whoAmI, null, whoAmI, (int)(byte)playerSpawnContext);
				}
				break;
			}
			case 13:
			{
				int num207 = reader.ReadByte();
				if (num207 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num207 = whoAmI;
					}
					Player player11 = Main.player[num207];
					BitsByte bitsByte14 = reader.ReadByte();
					BitsByte bitsByte15 = reader.ReadByte();
					BitsByte bitsByte16 = reader.ReadByte();
					BitsByte bitsByte17 = reader.ReadByte();
					player11.controlUp = bitsByte14[0];
					player11.controlDown = bitsByte14[1];
					player11.controlLeft = bitsByte14[2];
					player11.controlRight = bitsByte14[3];
					player11.controlJump = bitsByte14[4];
					player11.controlUseItem = bitsByte14[5];
					player11.direction = (bitsByte14[6] ? 1 : (-1));
					if (bitsByte15[0])
					{
						player11.pulley = true;
						player11.pulleyDir = (byte)((!bitsByte15[1]) ? 1u : 2u);
					}
					else
					{
						player11.pulley = false;
					}
					player11.vortexStealthActive = bitsByte15[3];
					player11.gravDir = (bitsByte15[4] ? 1 : (-1));
					player11.TryTogglingShield(bitsByte15[5]);
					player11.ghost = bitsByte15[6];
					player11.selectedItem = reader.ReadByte();
					player11.position = reader.ReadVector2();
					if (bitsByte15[2])
					{
						player11.velocity = reader.ReadVector2();
					}
					else
					{
						player11.velocity = Vector2.Zero;
					}
					if (bitsByte16[6])
					{
						player11.PotionOfReturnOriginalUsePosition = reader.ReadVector2();
						player11.PotionOfReturnHomePosition = reader.ReadVector2();
					}
					else
					{
						player11.PotionOfReturnOriginalUsePosition = null;
						player11.PotionOfReturnHomePosition = null;
					}
					player11.tryKeepingHoveringUp = bitsByte16[0];
					player11.IsVoidVaultEnabled = bitsByte16[1];
					player11.sitting.isSitting = bitsByte16[2];
					player11.downedDD2EventAnyDifficulty = bitsByte16[3];
					player11.isPettingAnimal = bitsByte16[4];
					player11.isTheAnimalBeingPetSmall = bitsByte16[5];
					player11.tryKeepingHoveringDown = bitsByte16[7];
					player11.sleeping.SetIsSleepingAndAdjustPlayerRotation(player11, bitsByte17[0]);
					if (Main.netMode == 2 && Netplay.Clients[whoAmI].State == 10)
					{
						NetMessage.TrySendData(13, -1, whoAmI, null, num207);
					}
				}
				break;
			}
			case 14:
			{
				int num65 = reader.ReadByte();
				int num66 = reader.ReadByte();
				if (Main.netMode != 1)
				{
					break;
				}
				bool active = Main.player[num65].active;
				if (num66 == 1)
				{
					if (!Main.player[num65].active)
					{
						Main.player[num65] = new Player();
					}
					Main.player[num65].active = true;
				}
				else
				{
					Main.player[num65].active = false;
				}
				if (active != Main.player[num65].active)
				{
					if (Main.player[num65].active)
					{
						Player.Hooks.PlayerConnect(num65);
					}
					else
					{
						Player.Hooks.PlayerDisconnect(num65);
					}
				}
				break;
			}
			case 16:
			{
				int num221 = reader.ReadByte();
				if (num221 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num221 = whoAmI;
					}
					Player player12 = Main.player[num221];
					player12.statLife = reader.ReadInt16();
					player12.statLifeMax = reader.ReadInt16();
					if (player12.statLifeMax < 100)
					{
						player12.statLifeMax = 100;
					}
					player12.dead = player12.statLife <= 0;
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(16, -1, whoAmI, null, num221);
					}
				}
				break;
			}
			case 17:
			{
				byte b12 = reader.ReadByte();
				int num178 = reader.ReadInt16();
				int num179 = reader.ReadInt16();
				short num180 = reader.ReadInt16();
				int num181 = reader.ReadByte();
				bool flag9 = num180 == 1;
				if (!WorldGen.InWorld(num178, num179, 3))
				{
					break;
				}
				if (Main.tile[num178, num179] == null)
				{
					Main.tile[num178, num179] = new Tile();
				}
				if (Main.netMode == 2)
				{
					if (!flag9)
					{
						if (b12 == 0 || b12 == 2 || b12 == 4)
						{
							Netplay.Clients[whoAmI].SpamDeleteBlock += 1f;
						}
						if (b12 == 1 || b12 == 3)
						{
							Netplay.Clients[whoAmI].SpamAddBlock += 1f;
						}
					}
					if (!Netplay.Clients[whoAmI].TileSections[Netplay.GetSectionX(num178), Netplay.GetSectionY(num179)])
					{
						flag9 = true;
					}
				}
				if (b12 == 0)
				{
					WorldGen.KillTile(num178, num179, flag9);
					if (Main.netMode == 1 && !flag9)
					{
						HitTile.ClearAllTilesAtThisLocation(num178, num179);
					}
				}
				if (b12 == 1)
				{
					WorldGen.PlaceTile(num178, num179, num180, mute: false, forced: true, -1, num181);
				}
				if (b12 == 2)
				{
					WorldGen.KillWall(num178, num179, flag9);
				}
				if (b12 == 3)
				{
					WorldGen.PlaceWall(num178, num179, num180);
				}
				if (b12 == 4)
				{
					WorldGen.KillTile(num178, num179, flag9, effectOnly: false, noItem: true);
				}
				if (b12 == 5)
				{
					WorldGen.PlaceWire(num178, num179);
				}
				if (b12 == 6)
				{
					WorldGen.KillWire(num178, num179);
				}
				if (b12 == 7)
				{
					WorldGen.PoundTile(num178, num179);
				}
				if (b12 == 8)
				{
					WorldGen.PlaceActuator(num178, num179);
				}
				if (b12 == 9)
				{
					WorldGen.KillActuator(num178, num179);
				}
				if (b12 == 10)
				{
					WorldGen.PlaceWire2(num178, num179);
				}
				if (b12 == 11)
				{
					WorldGen.KillWire2(num178, num179);
				}
				if (b12 == 12)
				{
					WorldGen.PlaceWire3(num178, num179);
				}
				if (b12 == 13)
				{
					WorldGen.KillWire3(num178, num179);
				}
				if (b12 == 14)
				{
					WorldGen.SlopeTile(num178, num179, num180);
				}
				if (b12 == 15)
				{
					Minecart.FrameTrack(num178, num179, pound: true);
				}
				if (b12 == 16)
				{
					WorldGen.PlaceWire4(num178, num179);
				}
				if (b12 == 17)
				{
					WorldGen.KillWire4(num178, num179);
				}
				switch (b12)
				{
				case 18:
					Wiring.SetCurrentUser(whoAmI);
					Wiring.PokeLogicGate(num178, num179);
					Wiring.SetCurrentUser();
					return;
				case 19:
					Wiring.SetCurrentUser(whoAmI);
					Wiring.Actuate(num178, num179);
					Wiring.SetCurrentUser();
					return;
				case 20:
					if (WorldGen.InWorld(num178, num179, 2))
					{
						int type9 = Main.tile[num178, num179].type;
						WorldGen.KillTile(num178, num179, flag9);
						num180 = (short)((Main.tile[num178, num179].active() && Main.tile[num178, num179].type == type9) ? 1 : 0);
						if (Main.netMode == 2)
						{
							NetMessage.TrySendData(17, -1, -1, null, b12, num178, num179, num180, num181);
						}
					}
					return;
				case 21:
					WorldGen.ReplaceTile(num178, num179, (ushort)num180, num181);
					break;
				}
				if (b12 == 22)
				{
					WorldGen.ReplaceWall(num178, num179, (ushort)num180);
				}
				if (b12 == 23)
				{
					WorldGen.SlopeTile(num178, num179, num180);
					WorldGen.PoundTile(num178, num179);
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(17, -1, whoAmI, null, b12, num178, num179, num180, num181);
					if ((b12 == 1 || b12 == 21) && TileID.Sets.Falling[num180])
					{
						NetMessage.SendTileSquare(-1, num178, num179);
					}
				}
				break;
			}
			case 18:
				if (Main.netMode == 1)
				{
					Main.dayTime = reader.ReadByte() == 1;
					Main.time = reader.ReadInt32();
					Main.sunModY = reader.ReadInt16();
					Main.moonModY = reader.ReadInt16();
				}
				break;
			case 19:
			{
				byte b2 = reader.ReadByte();
				int num3 = reader.ReadInt16();
				int num4 = reader.ReadInt16();
				if (WorldGen.InWorld(num3, num4, 3))
				{
					int num5 = ((reader.ReadByte() != 0) ? 1 : (-1));
					switch (b2)
					{
					case 0:
						WorldGen.OpenDoor(num3, num4, num5);
						break;
					case 1:
						WorldGen.CloseDoor(num3, num4, forced: true);
						break;
					case 2:
						WorldGen.ShiftTrapdoor(num3, num4, num5 == 1, 1);
						break;
					case 3:
						WorldGen.ShiftTrapdoor(num3, num4, num5 == 1, 0);
						break;
					case 4:
						WorldGen.ShiftTallGate(num3, num4, closing: false, forced: true);
						break;
					case 5:
						WorldGen.ShiftTallGate(num3, num4, closing: true, forced: true);
						break;
					}
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(19, -1, whoAmI, null, b2, num3, num4, (num5 == 1) ? 1 : 0);
					}
				}
				break;
			}
			case 20:
			{
				int num44 = reader.ReadInt16();
				int num45 = reader.ReadInt16();
				ushort num46 = reader.ReadByte();
				ushort num47 = reader.ReadByte();
				byte b6 = reader.ReadByte();
				if (!WorldGen.InWorld(num44, num45, 3))
				{
					break;
				}
				TileChangeType type4 = TileChangeType.None;
				if (Enum.IsDefined(typeof(TileChangeType), b6))
				{
					type4 = (TileChangeType)b6;
				}
				if (MessageBuffer.OnTileChangeReceived != null)
				{
					MessageBuffer.OnTileChangeReceived(num44, num45, Math.Max(num46, num47), type4);
				}
				BitsByte bitsByte7 = (byte)0;
				BitsByte bitsByte8 = (byte)0;
				Tile tile4 = null;
				for (int num48 = num44; num48 < num44 + num46; num48++)
				{
					for (int num49 = num45; num49 < num45 + num47; num49++)
					{
						if (Main.tile[num48, num49] == null)
						{
							Main.tile[num48, num49] = new Tile();
						}
						tile4 = Main.tile[num48, num49];
						bool flag4 = tile4.active();
						bitsByte7 = reader.ReadByte();
						bitsByte8 = reader.ReadByte();
						tile4.active(bitsByte7[0]);
						tile4.wall = (byte)(bitsByte7[2] ? 1u : 0u);
						bool flag5 = bitsByte7[3];
						if (Main.netMode != 2)
						{
							tile4.liquid = (byte)(flag5 ? 1u : 0u);
						}
						tile4.wire(bitsByte7[4]);
						tile4.halfBrick(bitsByte7[5]);
						tile4.actuator(bitsByte7[6]);
						tile4.inActive(bitsByte7[7]);
						tile4.wire2(bitsByte8[0]);
						tile4.wire3(bitsByte8[1]);
						if (bitsByte8[2])
						{
							tile4.color(reader.ReadByte());
						}
						if (bitsByte8[3])
						{
							tile4.wallColor(reader.ReadByte());
						}
						if (tile4.active())
						{
							int type5 = tile4.type;
							tile4.type = reader.ReadUInt16();
							if (Main.tileFrameImportant[tile4.type])
							{
								tile4.frameX = reader.ReadInt16();
								tile4.frameY = reader.ReadInt16();
							}
							else if (!flag4 || tile4.type != type5)
							{
								tile4.frameX = -1;
								tile4.frameY = -1;
							}
							byte b7 = 0;
							if (bitsByte8[4])
							{
								b7 = (byte)(b7 + 1);
							}
							if (bitsByte8[5])
							{
								b7 = (byte)(b7 + 2);
							}
							if (bitsByte8[6])
							{
								b7 = (byte)(b7 + 4);
							}
							tile4.slope(b7);
						}
						tile4.wire4(bitsByte8[7]);
						if (tile4.wall > 0)
						{
							tile4.wall = reader.ReadUInt16();
						}
						if (flag5)
						{
							tile4.liquid = reader.ReadByte();
							tile4.liquidType(reader.ReadByte());
						}
					}
				}
				WorldGen.RangeFrame(num44, num45, num44 + num46, num45 + num47);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(b, -1, whoAmI, null, num44, num45, (int)num46, (int)num47, b6);
				}
				break;
			}
			case 21:
			case 90:
			{
				int num156 = reader.ReadInt16();
				Vector2 position2 = reader.ReadVector2();
				Vector2 velocity3 = reader.ReadVector2();
				int stack4 = reader.ReadInt16();
				int pre2 = reader.ReadByte();
				int num157 = reader.ReadByte();
				int num158 = reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num158 == 0)
					{
						Main.item[num156].active = false;
						break;
					}
					int num159 = num156;
					Item item2 = Main.item[num159];
					ItemSyncPersistentStats itemSyncPersistentStats = default(ItemSyncPersistentStats);
					itemSyncPersistentStats.CopyFrom(item2);
					bool newAndShiny = (item2.newAndShiny || item2.netID != num158) && ItemSlot.Options.HighlightNewItems && (num158 < 0 || num158 >= 5088 || !ItemID.Sets.NeverAppearsAsNewInInventory[num158]);
					item2.netDefaults(num158);
					item2.newAndShiny = newAndShiny;
					item2.Prefix(pre2);
					item2.stack = stack4;
					item2.position = position2;
					item2.velocity = velocity3;
					item2.active = true;
					if (b == 90)
					{
						item2.instanced = true;
						item2.playerIndexTheItemIsReservedFor = Main.myPlayer;
						item2.keepTime = 600;
					}
					item2.wet = Collision.WetCollision(item2.position, item2.width, item2.height);
					itemSyncPersistentStats.PasteInto(item2);
				}
				else
				{
					if (Main.timeItemSlotCannotBeReusedFor[num156] > 0)
					{
						break;
					}
					if (num158 == 0)
					{
						if (num156 < 400)
						{
							Main.item[num156].active = false;
							NetMessage.TrySendData(21, -1, -1, null, num156);
						}
						break;
					}
					bool flag8 = false;
					if (num156 == 400)
					{
						flag8 = true;
					}
					if (flag8)
					{
						Item item3 = new Item();
						item3.netDefaults(num158);
						num156 = Item.NewItem((int)position2.X, (int)position2.Y, item3.width, item3.height, item3.type, stack4, noBroadcast: true);
					}
					Item obj4 = Main.item[num156];
					obj4.netDefaults(num158);
					obj4.Prefix(pre2);
					obj4.stack = stack4;
					obj4.position = position2;
					obj4.velocity = velocity3;
					obj4.active = true;
					obj4.playerIndexTheItemIsReservedFor = Main.myPlayer;
					if (flag8)
					{
						NetMessage.TrySendData(21, -1, -1, null, num156);
						if (num157 == 0)
						{
							Main.item[num156].ownIgnore = whoAmI;
							Main.item[num156].ownTime = 100;
						}
						Main.item[num156].FindOwner(num156);
					}
					else
					{
						NetMessage.TrySendData(21, -1, whoAmI, null, num156);
					}
				}
				break;
			}
			case 22:
			{
				int num154 = reader.ReadInt16();
				int num155 = reader.ReadByte();
				if (Main.netMode != 2 || Main.item[num154].playerIndexTheItemIsReservedFor == whoAmI)
				{
					Main.item[num154].playerIndexTheItemIsReservedFor = num155;
					if (num155 == Main.myPlayer)
					{
						Main.item[num154].keepTime = 15;
					}
					else
					{
						Main.item[num154].keepTime = 0;
					}
					if (Main.netMode == 2)
					{
						Main.item[num154].playerIndexTheItemIsReservedFor = 255;
						Main.item[num154].keepTime = 15;
						NetMessage.TrySendData(22, -1, -1, null, num154);
					}
				}
				break;
			}
			case 23:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num138 = reader.ReadInt16();
				Vector2 vector5 = reader.ReadVector2();
				Vector2 velocity2 = reader.ReadVector2();
				int num139 = reader.ReadUInt16();
				if (num139 == 65535)
				{
					num139 = 0;
				}
				BitsByte bitsByte10 = reader.ReadByte();
				BitsByte bitsByte11 = reader.ReadByte();
				float[] array = new float[NPC.maxAI];
				for (int num140 = 0; num140 < NPC.maxAI; num140++)
				{
					if (bitsByte10[num140 + 2])
					{
						array[num140] = reader.ReadSingle();
					}
					else
					{
						array[num140] = 0f;
					}
				}
				int num141 = reader.ReadInt16();
				int? playerCountForMultiplayerDifficultyOverride = 1;
				if (bitsByte11[0])
				{
					playerCountForMultiplayerDifficultyOverride = reader.ReadByte();
				}
				float value6 = 1f;
				if (bitsByte11[2])
				{
					value6 = reader.ReadSingle();
				}
				int num142 = 0;
				if (!bitsByte10[7])
				{
					num142 = reader.ReadByte() switch
					{
						2 => reader.ReadInt16(), 
						4 => reader.ReadInt32(), 
						_ => reader.ReadSByte(), 
					};
				}
				int num143 = -1;
				NPC nPC4 = Main.npc[num138];
				if (nPC4.active && Main.multiplayerNPCSmoothingRange > 0 && Vector2.DistanceSquared(nPC4.position, vector5) < 640000f)
				{
					nPC4.netOffset += nPC4.position - vector5;
				}
				if (!nPC4.active || nPC4.netID != num141)
				{
					nPC4.netOffset *= 0f;
					if (nPC4.active)
					{
						num143 = nPC4.type;
					}
					nPC4.active = true;
					spawnparams = new NPCSpawnParams
					{
						playerCountForMultiplayerDifficultyOverride = playerCountForMultiplayerDifficultyOverride,
						strengthMultiplierOverride = value6
					};
					nPC4.SetDefaults(num141, spawnparams);
				}
				nPC4.position = vector5;
				nPC4.velocity = velocity2;
				nPC4.target = num139;
				nPC4.direction = (bitsByte10[0] ? 1 : (-1));
				nPC4.directionY = (bitsByte10[1] ? 1 : (-1));
				nPC4.spriteDirection = (bitsByte10[6] ? 1 : (-1));
				if (bitsByte10[7])
				{
					num142 = (nPC4.life = nPC4.lifeMax);
				}
				else
				{
					nPC4.life = num142;
				}
				if (num142 <= 0)
				{
					nPC4.active = false;
				}
				nPC4.SpawnedFromStatue = bitsByte11[0];
				if (nPC4.SpawnedFromStatue)
				{
					nPC4.value = 0f;
				}
				for (int num144 = 0; num144 < NPC.maxAI; num144++)
				{
					nPC4.ai[num144] = array[num144];
				}
				if (num143 > -1 && num143 != nPC4.type)
				{
					nPC4.TransformVisuals(num143, nPC4.type);
				}
				if (num141 == 262)
				{
					NPC.plantBoss = num138;
				}
				if (num141 == 245)
				{
					NPC.golemBoss = num138;
				}
				if (nPC4.type >= 0 && nPC4.type < 668 && Main.npcCatchable[nPC4.type])
				{
					nPC4.releaseOwner = reader.ReadByte();
				}
				break;
			}
			case 24:
			{
				int num56 = reader.ReadInt16();
				int num57 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num57 = whoAmI;
				}
				Player player6 = Main.player[num57];
				Main.npc[num56].StrikeNPC(player6.inventory[player6.selectedItem].damage, player6.inventory[player6.selectedItem].knockBack, player6.direction);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(24, -1, whoAmI, null, num56, num57);
					NetMessage.TrySendData(23, -1, -1, null, num56);
				}
				break;
			}
			case 27:
			{
				int num162 = reader.ReadInt16();
				Vector2 position3 = reader.ReadVector2();
				Vector2 velocity4 = reader.ReadVector2();
				int num163 = reader.ReadByte();
				int num164 = reader.ReadInt16();
				BitsByte bitsByte12 = reader.ReadByte();
				float[] array2 = new float[Projectile.maxAI];
				for (int num165 = 0; num165 < Projectile.maxAI; num165++)
				{
					if (bitsByte12[num165])
					{
						array2[num165] = reader.ReadSingle();
					}
					else
					{
						array2[num165] = 0f;
					}
				}
				int bannerIdToRespondTo = (bitsByte12[3] ? reader.ReadUInt16() : 0);
				int damage2 = (bitsByte12[4] ? reader.ReadInt16() : 0);
				float knockBack2 = (bitsByte12[5] ? reader.ReadSingle() : 0f);
				int originalDamage = (bitsByte12[6] ? reader.ReadInt16() : 0);
				int num166 = (bitsByte12[7] ? reader.ReadInt16() : (-1));
				if (num166 >= 1000)
				{
					num166 = -1;
				}
				if (Main.netMode == 2)
				{
					if (num164 == 949)
					{
						num163 = 255;
					}
					else
					{
						num163 = whoAmI;
						if (Main.projHostile[num164])
						{
							break;
						}
					}
				}
				int num167 = 1000;
				for (int num168 = 0; num168 < 1000; num168++)
				{
					if (Main.projectile[num168].owner == num163 && Main.projectile[num168].identity == num162 && Main.projectile[num168].active)
					{
						num167 = num168;
						break;
					}
				}
				if (num167 == 1000)
				{
					for (int num169 = 0; num169 < 1000; num169++)
					{
						if (!Main.projectile[num169].active)
						{
							num167 = num169;
							break;
						}
					}
				}
				if (num167 == 1000)
				{
					num167 = Projectile.FindOldestProjectile();
				}
				Projectile projectile = Main.projectile[num167];
				if (!projectile.active || projectile.type != num164)
				{
					projectile.SetDefaults(num164);
					if (Main.netMode == 2)
					{
						Netplay.Clients[whoAmI].SpamProjectile += 1f;
					}
				}
				projectile.identity = num162;
				projectile.position = position3;
				projectile.velocity = velocity4;
				projectile.type = num164;
				projectile.damage = damage2;
				projectile.bannerIdToRespondTo = bannerIdToRespondTo;
				projectile.originalDamage = originalDamage;
				projectile.knockBack = knockBack2;
				projectile.owner = num163;
				for (int num170 = 0; num170 < Projectile.maxAI; num170++)
				{
					projectile.ai[num170] = array2[num170];
				}
				if (num166 >= 0)
				{
					projectile.projUUID = num166;
					Main.projectileIdentity[num163, num166] = num167;
				}
				projectile.ProjectileFixDesperation();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(27, -1, whoAmI, null, num167);
				}
				break;
			}
			case 28:
			{
				int num257 = reader.ReadInt16();
				int num258 = reader.ReadInt16();
				float num259 = reader.ReadSingle();
				int num260 = reader.ReadByte() - 1;
				byte b13 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					if (num258 < 0)
					{
						num258 = 0;
					}
					Main.npc[num257].PlayerInteraction(whoAmI);
				}
				if (num258 >= 0)
				{
					Main.npc[num257].StrikeNPC(num258, num259, num260, b13 == 1, noEffect: false, fromNet: true);
				}
				else
				{
					Main.npc[num257].life = 0;
					Main.npc[num257].HitEffect();
					Main.npc[num257].active = false;
				}
				if (Main.netMode != 2)
				{
					break;
				}
				NetMessage.TrySendData(28, -1, whoAmI, null, num257, num258, num259, num260, b13);
				if (Main.npc[num257].life <= 0)
				{
					NetMessage.TrySendData(23, -1, -1, null, num257);
				}
				else
				{
					Main.npc[num257].netUpdate = true;
				}
				if (Main.npc[num257].realLife >= 0)
				{
					if (Main.npc[Main.npc[num257].realLife].life <= 0)
					{
						NetMessage.TrySendData(23, -1, -1, null, Main.npc[num257].realLife);
					}
					else
					{
						Main.npc[Main.npc[num257].realLife].netUpdate = true;
					}
				}
				break;
			}
			case 29:
			{
				int num204 = reader.ReadInt16();
				int num205 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num205 = whoAmI;
				}
				for (int num206 = 0; num206 < 1000; num206++)
				{
					if (Main.projectile[num206].owner == num205 && Main.projectile[num206].identity == num204 && Main.projectile[num206].active)
					{
						Main.projectile[num206].Kill();
						break;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(29, -1, whoAmI, null, num204, num205);
				}
				break;
			}
			case 30:
			{
				int num211 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num211 = whoAmI;
				}
				bool flag11 = reader.ReadBoolean();
				Main.player[num211].hostile = flag11;
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(30, -1, whoAmI, null, num211);
					LocalizedText obj7 = (flag11 ? Lang.mp[11] : Lang.mp[12]);
					ChatHelper.BroadcastChatMessage(color: Main.teamColor[Main.player[num211].team], text: NetworkText.FromKey(obj7.Key, Main.player[num211].name));
				}
				break;
			}
			case 31:
			{
				if (Main.netMode != 2)
				{
					break;
				}
				int num52 = reader.ReadInt16();
				int num53 = reader.ReadInt16();
				int num54 = Chest.FindChest(num52, num53);
				if (num54 > -1 && Chest.UsingChest(num54) == -1)
				{
					for (int num55 = 0; num55 < 40; num55++)
					{
						NetMessage.TrySendData(32, whoAmI, -1, null, num54, num55);
					}
					NetMessage.TrySendData(33, whoAmI, -1, null, num54);
					Main.player[whoAmI].chest = num54;
					if (Main.myPlayer == whoAmI)
					{
						Main.recBigList = false;
					}
					NetMessage.TrySendData(80, -1, whoAmI, null, whoAmI, num54);
					if (Main.netMode == 2 && WorldGen.IsChestRigged(num52, num53))
					{
						Wiring.SetCurrentUser(whoAmI);
						Wiring.HitSwitch(num52, num53);
						Wiring.SetCurrentUser();
						NetMessage.TrySendData(59, -1, whoAmI, null, num52, num53);
					}
				}
				break;
			}
			case 32:
			{
				int num242 = reader.ReadInt16();
				int num243 = reader.ReadByte();
				int stack8 = reader.ReadInt16();
				int pre3 = reader.ReadByte();
				int type16 = reader.ReadInt16();
				if (num242 >= 0 && num242 < 8000)
				{
					if (Main.chest[num242] == null)
					{
						Main.chest[num242] = new Chest();
					}
					if (Main.chest[num242].item[num243] == null)
					{
						Main.chest[num242].item[num243] = new Item();
					}
					Main.chest[num242].item[num243].netDefaults(type16);
					Main.chest[num242].item[num243].Prefix(pre3);
					Main.chest[num242].item[num243].stack = stack8;
					Recipe.FindRecipes(canDelayCheck: true);
				}
				break;
			}
			case 33:
			{
				int num61 = reader.ReadInt16();
				int num62 = reader.ReadInt16();
				int num63 = reader.ReadInt16();
				int num64 = reader.ReadByte();
				string name = string.Empty;
				if (num64 != 0)
				{
					if (num64 <= 20)
					{
						name = reader.ReadString();
					}
					else if (num64 != 255)
					{
						num64 = 0;
					}
				}
				if (Main.netMode == 1)
				{
					Player player7 = Main.player[Main.myPlayer];
					if (player7.chest == -1)
					{
						Main.playerInventory = true;
						SoundEngine.PlaySound(10);
					}
					else if (player7.chest != num61 && num61 != -1)
					{
						Main.playerInventory = true;
						SoundEngine.PlaySound(12);
						Main.recBigList = false;
					}
					else if (player7.chest != -1 && num61 == -1)
					{
						SoundEngine.PlaySound(11);
						Main.recBigList = false;
					}
					player7.chest = num61;
					player7.chestX = num62;
					player7.chestY = num63;
					Recipe.FindRecipes(canDelayCheck: true);
					if (Main.tile[num62, num63].frameX >= 36 && Main.tile[num62, num63].frameX < 72)
					{
						AchievementsHelper.HandleSpecialEvent(Main.player[Main.myPlayer], 16);
					}
				}
				else
				{
					if (num64 != 0)
					{
						int chest = Main.player[whoAmI].chest;
						Chest chest2 = Main.chest[chest];
						chest2.name = name;
						NetMessage.TrySendData(69, -1, whoAmI, null, chest, chest2.x, chest2.y);
					}
					Main.player[whoAmI].chest = num61;
					Recipe.FindRecipes(canDelayCheck: true);
					NetMessage.TrySendData(80, -1, whoAmI, null, whoAmI, num61);
				}
				break;
			}
			case 34:
			{
				byte b3 = reader.ReadByte();
				int num12 = reader.ReadInt16();
				int num13 = reader.ReadInt16();
				int num14 = reader.ReadInt16();
				int num15 = reader.ReadInt16();
				if (Main.netMode == 2)
				{
					num15 = 0;
				}
				if (Main.netMode == 2)
				{
					switch (b3)
					{
					case 0:
					{
						int num18 = WorldGen.PlaceChest(num12, num13, 21, notNearOtherChests: false, num14);
						if (num18 == -1)
						{
							NetMessage.TrySendData(34, whoAmI, -1, null, b3, num12, num13, num14, num18);
							Item.NewItem(num12 * 16, num13 * 16, 32, 32, Chest.chestItemSpawn[num14], 1, noBroadcast: true);
						}
						else
						{
							NetMessage.TrySendData(34, -1, -1, null, b3, num12, num13, num14, num18);
						}
						break;
					}
					case 1:
						if (Main.tile[num12, num13].type == 21)
						{
							Tile tile = Main.tile[num12, num13];
							if (tile.frameX % 36 != 0)
							{
								num12--;
							}
							if (tile.frameY % 36 != 0)
							{
								num13--;
							}
							int number = Chest.FindChest(num12, num13);
							WorldGen.KillTile(num12, num13);
							if (!tile.active())
							{
								NetMessage.TrySendData(34, -1, -1, null, b3, num12, num13, 0f, number);
							}
							break;
						}
						goto default;
					default:
						switch (b3)
						{
						case 2:
						{
							int num16 = WorldGen.PlaceChest(num12, num13, 88, notNearOtherChests: false, num14);
							if (num16 == -1)
							{
								NetMessage.TrySendData(34, whoAmI, -1, null, b3, num12, num13, num14, num16);
								Item.NewItem(num12 * 16, num13 * 16, 32, 32, Chest.dresserItemSpawn[num14], 1, noBroadcast: true);
							}
							else
							{
								NetMessage.TrySendData(34, -1, -1, null, b3, num12, num13, num14, num16);
							}
							break;
						}
						case 3:
							if (Main.tile[num12, num13].type == 88)
							{
								Tile tile2 = Main.tile[num12, num13];
								num12 -= tile2.frameX % 54 / 18;
								if (tile2.frameY % 36 != 0)
								{
									num13--;
								}
								int number2 = Chest.FindChest(num12, num13);
								WorldGen.KillTile(num12, num13);
								if (!tile2.active())
								{
									NetMessage.TrySendData(34, -1, -1, null, b3, num12, num13, 0f, number2);
								}
								break;
							}
							goto default;
						default:
							switch (b3)
							{
							case 4:
							{
								int num17 = WorldGen.PlaceChest(num12, num13, 467, notNearOtherChests: false, num14);
								if (num17 == -1)
								{
									NetMessage.TrySendData(34, whoAmI, -1, null, b3, num12, num13, num14, num17);
									Item.NewItem(num12 * 16, num13 * 16, 32, 32, Chest.chestItemSpawn2[num14], 1, noBroadcast: true);
								}
								else
								{
									NetMessage.TrySendData(34, -1, -1, null, b3, num12, num13, num14, num17);
								}
								break;
							}
							case 5:
								if (Main.tile[num12, num13].type == 467)
								{
									Tile tile3 = Main.tile[num12, num13];
									if (tile3.frameX % 36 != 0)
									{
										num12--;
									}
									if (tile3.frameY % 36 != 0)
									{
										num13--;
									}
									int number3 = Chest.FindChest(num12, num13);
									WorldGen.KillTile(num12, num13);
									if (!tile3.active())
									{
										NetMessage.TrySendData(34, -1, -1, null, b3, num12, num13, 0f, number3);
									}
								}
								break;
							}
							break;
						}
						break;
					}
					break;
				}
				switch (b3)
				{
				case 0:
					if (num15 == -1)
					{
						WorldGen.KillTile(num12, num13);
						break;
					}
					SoundEngine.PlaySound(0, num12 * 16, num13 * 16);
					WorldGen.PlaceChestDirect(num12, num13, 21, num14, num15);
					break;
				case 2:
					if (num15 == -1)
					{
						WorldGen.KillTile(num12, num13);
						break;
					}
					SoundEngine.PlaySound(0, num12 * 16, num13 * 16);
					WorldGen.PlaceDresserDirect(num12, num13, 88, num14, num15);
					break;
				case 4:
					if (num15 == -1)
					{
						WorldGen.KillTile(num12, num13);
						break;
					}
					SoundEngine.PlaySound(0, num12 * 16, num13 * 16);
					WorldGen.PlaceChestDirect(num12, num13, 467, num14, num15);
					break;
				default:
					Chest.DestroyChestDirect(num12, num13, num15);
					WorldGen.KillTile(num12, num13);
					break;
				}
				break;
			}
			case 35:
			{
				int num238 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num238 = whoAmI;
				}
				int num239 = reader.ReadInt16();
				if (num238 != Main.myPlayer || Main.ServerSideCharacter)
				{
					Main.player[num238].HealEffect(num239);
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(35, -1, whoAmI, null, num238, num239);
				}
				break;
			}
			case 36:
			{
				int num228 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num228 = whoAmI;
				}
				Player obj8 = Main.player[num228];
				obj8.zone1 = reader.ReadByte();
				obj8.zone2 = reader.ReadByte();
				obj8.zone3 = reader.ReadByte();
				obj8.zone4 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(36, -1, whoAmI, null, num228);
				}
				break;
			}
			case 37:
				if (Main.netMode == 1)
				{
					if (Main.autoPass)
					{
						NetMessage.TrySendData(38);
						Main.autoPass = false;
					}
					else
					{
						Netplay.ServerPassword = "";
						Main.menuMode = 31;
					}
				}
				break;
			case 38:
				if (Main.netMode == 2)
				{
					if (reader.ReadString() == Netplay.ServerPassword)
					{
						Netplay.Clients[whoAmI].State = 1;
						NetMessage.TrySendData(3, whoAmI);
					}
					else
					{
						NetMessage.TrySendData(2, whoAmI, -1, Lang.mp[1].ToNetworkText());
					}
				}
				break;
			case 39:
				if (Main.netMode == 1)
				{
					int num182 = reader.ReadInt16();
					Main.item[num182].playerIndexTheItemIsReservedFor = 255;
					NetMessage.TrySendData(22, -1, -1, null, num182);
				}
				break;
			case 40:
			{
				int num174 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num174 = whoAmI;
				}
				int npcIndex = reader.ReadInt16();
				Main.player[num174].SetTalkNPC(npcIndex, fromNet: true);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(40, -1, whoAmI, null, num174);
				}
				break;
			}
			case 41:
			{
				int num98 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num98 = whoAmI;
				}
				Player player9 = Main.player[num98];
				float itemRotation = reader.ReadSingle();
				int itemAnimation = reader.ReadInt16();
				player9.itemRotation = itemRotation;
				player9.itemAnimation = itemAnimation;
				player9.channel = player9.inventory[player9.selectedItem].channel;
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(41, -1, whoAmI, null, num98);
				}
				break;
			}
			case 42:
			{
				int num60 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num60 = whoAmI;
				}
				else if (Main.myPlayer == num60 && !Main.ServerSideCharacter)
				{
					break;
				}
				int statMana = reader.ReadInt16();
				int statManaMax = reader.ReadInt16();
				Main.player[num60].statMana = statMana;
				Main.player[num60].statManaMax = statManaMax;
				break;
			}
			case 43:
			{
				int num7 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num7 = whoAmI;
				}
				int num8 = reader.ReadInt16();
				if (num7 != Main.myPlayer)
				{
					Main.player[num7].ManaEffect(num8);
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(43, -1, whoAmI, null, num7, num8);
				}
				break;
			}
			case 45:
			{
				int num235 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num235 = whoAmI;
				}
				int num236 = reader.ReadByte();
				Player player15 = Main.player[num235];
				int team = player15.team;
				player15.team = num236;
				Color color4 = Main.teamColor[num236];
				if (Main.netMode != 2)
				{
					break;
				}
				NetMessage.TrySendData(45, -1, whoAmI, null, num235);
				LocalizedText localizedText = Lang.mp[13 + num236];
				if (num236 == 5)
				{
					localizedText = Lang.mp[22];
				}
				for (int num237 = 0; num237 < 255; num237++)
				{
					if (num237 == whoAmI || (team > 0 && Main.player[num237].team == team) || (num236 > 0 && Main.player[num237].team == num236))
					{
						ChatHelper.SendChatMessageToClient(NetworkText.FromKey(localizedText.Key, player15.name), color4, num237);
					}
				}
				break;
			}
			case 46:
				if (Main.netMode == 2)
				{
					short i3 = reader.ReadInt16();
					int j3 = reader.ReadInt16();
					int num230 = Sign.ReadSign(i3, j3);
					if (num230 >= 0)
					{
						NetMessage.TrySendData(47, whoAmI, -1, null, num230, whoAmI);
					}
				}
				break;
			case 47:
			{
				int num190 = reader.ReadInt16();
				int x10 = reader.ReadInt16();
				int y9 = reader.ReadInt16();
				string text3 = reader.ReadString();
				int num191 = reader.ReadByte();
				BitsByte bitsByte13 = reader.ReadByte();
				if (num190 >= 0 && num190 < 1000)
				{
					string text4 = null;
					if (Main.sign[num190] != null)
					{
						text4 = Main.sign[num190].text;
					}
					Main.sign[num190] = new Sign();
					Main.sign[num190].x = x10;
					Main.sign[num190].y = y9;
					Sign.TextSign(num190, text3);
					if (Main.netMode == 2 && text4 != text3)
					{
						num191 = whoAmI;
						NetMessage.TrySendData(47, -1, whoAmI, null, num190, num191);
					}
					if (Main.netMode == 1 && num191 == Main.myPlayer && Main.sign[num190] != null && !bitsByte13[0])
					{
						Main.playerInventory = false;
						Main.player[Main.myPlayer].SetTalkNPC(-1, fromNet: true);
						Main.npcChatCornerItem = 0;
						Main.editSign = false;
						SoundEngine.PlaySound(10);
						Main.player[Main.myPlayer].sign = num190;
						Main.npcChatText = Main.sign[num190].text;
					}
				}
				break;
			}
			case 48:
			{
				int num81 = reader.ReadInt16();
				int num82 = reader.ReadInt16();
				byte liquid = reader.ReadByte();
				byte liquidType = reader.ReadByte();
				if (Main.netMode == 2 && Netplay.SpamCheck)
				{
					int num83 = whoAmI;
					int num84 = (int)(Main.player[num83].position.X + (float)(Main.player[num83].width / 2));
					int num85 = (int)(Main.player[num83].position.Y + (float)(Main.player[num83].height / 2));
					int num86 = 10;
					int num87 = num84 - num86;
					int num88 = num84 + num86;
					int num89 = num85 - num86;
					int num90 = num85 + num86;
					if (num81 < num87 || num81 > num88 || num82 < num89 || num82 > num90)
					{
						Netplay.Clients[whoAmI].SpamWater += 1f;
					}
				}
				if (Main.tile[num81, num82] == null)
				{
					Main.tile[num81, num82] = new Tile();
				}
				lock (Main.tile[num81, num82])
				{
					Main.tile[num81, num82].liquid = liquid;
					Main.tile[num81, num82].liquidType(liquidType);
					if (Main.netMode == 2)
					{
						WorldGen.SquareTileFrame(num81, num82);
					}
				}
				break;
			}
			case 49:
				if (Netplay.Connection.State == 6)
				{
					Netplay.Connection.State = 10;
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.Hooks.EnterWorld(Main.myPlayer);
					Main.player[Main.myPlayer].Spawn(PlayerSpawnContext.SpawningIntoWorld);
				}
				break;
			case 50:
			{
				int num50 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num50 = whoAmI;
				}
				else if (num50 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					break;
				}
				Player player5 = Main.player[num50];
				for (int num51 = 0; num51 < 22; num51++)
				{
					player5.buffType[num51] = reader.ReadUInt16();
					if (player5.buffType[num51] > 0)
					{
						player5.buffTime[num51] = 60;
					}
					else
					{
						player5.buffTime[num51] = 0;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(50, -1, whoAmI, null, num50);
				}
				break;
			}
			case 51:
			{
				byte b4 = reader.ReadByte();
				byte b5 = reader.ReadByte();
				switch (b5)
				{
				case 1:
					NPC.SpawnSkeletron();
					break;
				case 2:
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(51, -1, whoAmI, null, b4, (int)b5);
					}
					else
					{
						SoundEngine.PlaySound(SoundID.Item1, (int)Main.player[b4].position.X, (int)Main.player[b4].position.Y);
					}
					break;
				case 3:
					if (Main.netMode == 2)
					{
						Main.Sundialing();
					}
					break;
				case 4:
					Main.npc[b4].BigMimicSpawnSmoke();
					break;
				case 5:
					if (Main.netMode == 2)
					{
						NPC nPC = new NPC();
						spawnparams = default(NPCSpawnParams);
						nPC.SetDefaults(664, spawnparams);
						Main.BestiaryTracker.Kills.RegisterKill(nPC);
					}
					break;
				}
				break;
			}
			case 52:
			{
				int num261 = reader.ReadByte();
				int num262 = reader.ReadInt16();
				int num263 = reader.ReadInt16();
				if (num261 == 1)
				{
					Chest.Unlock(num262, num263);
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(52, -1, whoAmI, null, 0, num261, num262, num263);
						NetMessage.SendTileSquare(-1, num262, num263, 2);
					}
				}
				if (num261 == 2)
				{
					WorldGen.UnlockDoor(num262, num263);
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(52, -1, whoAmI, null, 0, num261, num262, num263);
						NetMessage.SendTileSquare(-1, num262, num263, 2);
					}
				}
				break;
			}
			case 53:
			{
				int num264 = reader.ReadInt16();
				int type18 = reader.ReadUInt16();
				int time2 = reader.ReadInt16();
				Main.npc[num264].AddBuff(type18, time2, quiet: true);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(54, -1, -1, null, num264);
				}
				break;
			}
			case 54:
				if (Main.netMode == 1)
				{
					int num240 = reader.ReadInt16();
					NPC nPC6 = Main.npc[num240];
					for (int num241 = 0; num241 < 5; num241++)
					{
						nPC6.buffType[num241] = reader.ReadUInt16();
						nPC6.buffTime[num241] = reader.ReadInt16();
					}
				}
				break;
			case 55:
			{
				int num225 = reader.ReadByte();
				int num226 = reader.ReadUInt16();
				int num227 = reader.ReadInt32();
				if (Main.netMode != 2 || num225 == whoAmI || Main.pvpBuff[num226])
				{
					if (Main.netMode == 1 && num225 == Main.myPlayer)
					{
						Main.player[num225].AddBuff(num226, num227);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.TrySendData(55, num225, -1, null, num225, num226, num227);
					}
				}
				break;
			}
			case 56:
			{
				int num217 = reader.ReadInt16();
				if (num217 >= 0 && num217 < 200)
				{
					if (Main.netMode == 1)
					{
						string givenName = reader.ReadString();
						Main.npc[num217].GivenName = givenName;
						int townNpcVariationIndex = reader.ReadInt32();
						Main.npc[num217].townNpcVariationIndex = townNpcVariationIndex;
					}
					else if (Main.netMode == 2)
					{
						NetMessage.TrySendData(56, whoAmI, -1, null, num217);
					}
				}
				break;
			}
			case 57:
				if (Main.netMode == 1)
				{
					WorldGen.tGood = reader.ReadByte();
					WorldGen.tEvil = reader.ReadByte();
					WorldGen.tBlood = reader.ReadByte();
				}
				break;
			case 58:
			{
				int num202 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num202 = whoAmI;
				}
				float num203 = reader.ReadSingle();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(58, -1, whoAmI, null, whoAmI, num203);
					break;
				}
				Player player10 = Main.player[num202];
				int type11 = player10.inventory[player10.selectedItem].type;
				switch (type11)
				{
				case 4057:
				case 4372:
				case 4715:
					player10.PlayGuitarChord(num203);
					break;
				case 4673:
					player10.PlayDrums(num203);
					break;
				default:
				{
					Main.musicPitch = num203;
					LegacySoundStyle type12 = SoundID.Item26;
					if (type11 == 507)
					{
						type12 = SoundID.Item35;
					}
					if (type11 == 1305)
					{
						type12 = SoundID.Item47;
					}
					SoundEngine.PlaySound(type12, player10.position);
					break;
				}
				}
				break;
			}
			case 59:
			{
				int num188 = reader.ReadInt16();
				int num189 = reader.ReadInt16();
				Wiring.SetCurrentUser(whoAmI);
				Wiring.HitSwitch(num188, num189);
				Wiring.SetCurrentUser();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(59, -1, whoAmI, null, num188, num189);
				}
				break;
			}
			case 60:
			{
				int num151 = reader.ReadInt16();
				int num152 = reader.ReadInt16();
				int num153 = reader.ReadInt16();
				byte b11 = reader.ReadByte();
				if (num151 >= 200)
				{
					NetMessage.BootPlayer(whoAmI, NetworkText.FromKey("Net.CheatingInvalid"));
				}
				else if (Main.netMode == 1)
				{
					Main.npc[num151].homeless = b11 == 1;
					Main.npc[num151].homeTileX = num152;
					Main.npc[num151].homeTileY = num153;
					switch (b11)
					{
					case 1:
						WorldGen.TownManager.KickOut(Main.npc[num151].type);
						break;
					case 2:
						WorldGen.TownManager.SetRoom(Main.npc[num151].type, num152, num153);
						break;
					}
				}
				else if (b11 == 1)
				{
					WorldGen.kickOut(num151);
				}
				else
				{
					WorldGen.moveRoom(num152, num153, num151);
				}
				break;
			}
			case 61:
			{
				int plr = reader.ReadInt16();
				int num200 = reader.ReadInt16();
				if (Main.netMode != 2)
				{
					break;
				}
				if (num200 >= 0 && num200 < 668 && NPCID.Sets.MPAllowedEnemies[num200])
				{
					if (!NPC.AnyNPCs(num200))
					{
						NPC.SpawnOnPlayer(plr, num200);
					}
				}
				else if (num200 == -4)
				{
					if (!Main.dayTime && !DD2Event.Ongoing)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[31].Key), new Color(50, 255, 130));
						Main.startPumpkinMoon();
						NetMessage.TrySendData(7);
						NetMessage.TrySendData(78, -1, -1, null, 0, 1f, 2f, 1f);
					}
				}
				else if (num200 == -5)
				{
					if (!Main.dayTime && !DD2Event.Ongoing)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[34].Key), new Color(50, 255, 130));
						Main.startSnowMoon();
						NetMessage.TrySendData(7);
						NetMessage.TrySendData(78, -1, -1, null, 0, 1f, 1f, 1f);
					}
				}
				else if (num200 == -6)
				{
					if (Main.dayTime && !Main.eclipse)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[20].Key), new Color(50, 255, 130));
						Main.eclipse = true;
						NetMessage.TrySendData(7);
					}
				}
				else if (num200 == -7)
				{
					Main.invasionDelay = 0;
					Main.StartInvasion(4);
					NetMessage.TrySendData(7);
					NetMessage.TrySendData(78, -1, -1, null, 0, 1f, Main.invasionType + 3);
				}
				else if (num200 == -8)
				{
					if (NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
					{
						WorldGen.StartImpendingDoom();
						NetMessage.TrySendData(7);
					}
				}
				else if (num200 == -10)
				{
					if (!Main.dayTime && !Main.bloodMoon)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[8].Key), new Color(50, 255, 130));
						Main.bloodMoon = true;
						if (Main.GetMoonPhase() == MoonPhase.Empty)
						{
							Main.moonPhase = 5;
						}
						AchievementsHelper.NotifyProgressionEvent(4);
						NetMessage.TrySendData(7);
					}
				}
				else if (num200 == -11)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.CombatBookUsed"), new Color(50, 255, 130));
					NPC.combatBookWasUsed = true;
					NetMessage.TrySendData(7);
				}
				else if (num200 == -12)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.LicenseCatUsed"), new Color(50, 255, 130));
					NPC.boughtCat = true;
					NetMessage.TrySendData(7);
				}
				else if (num200 == -13)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.LicenseDogUsed"), new Color(50, 255, 130));
					NPC.boughtDog = true;
					NetMessage.TrySendData(7);
				}
				else if (num200 == -14)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.LicenseBunnyUsed"), new Color(50, 255, 130));
					NPC.boughtBunny = true;
					NetMessage.TrySendData(7);
				}
				else if (num200 < 0)
				{
					int num201 = 1;
					if (num200 > -5)
					{
						num201 = -num200;
					}
					if (num201 > 0 && Main.invasionType == 0)
					{
						Main.invasionDelay = 0;
						Main.StartInvasion(num201);
					}
					NetMessage.TrySendData(78, -1, -1, null, 0, 1f, Main.invasionType + 3);
				}
				break;
			}
			case 62:
			{
				int num149 = reader.ReadByte();
				int num150 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num149 = whoAmI;
				}
				if (num150 == 1)
				{
					Main.player[num149].NinjaDodge();
				}
				if (num150 == 2)
				{
					Main.player[num149].ShadowDodge();
				}
				if (num150 == 4)
				{
					Main.player[num149].BrainOfConfusionDodge();
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(62, -1, whoAmI, null, num149, num150);
				}
				break;
			}
			case 63:
			{
				int num145 = reader.ReadInt16();
				int num146 = reader.ReadInt16();
				byte b10 = reader.ReadByte();
				WorldGen.paintTile(num145, num146, b10);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(63, -1, whoAmI, null, num145, num146, (int)b10);
				}
				break;
			}
			case 64:
			{
				int num136 = reader.ReadInt16();
				int num137 = reader.ReadInt16();
				byte b9 = reader.ReadByte();
				WorldGen.paintWall(num136, num137, b9);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(64, -1, whoAmI, null, num136, num137, (int)b9);
				}
				break;
			}
			case 65:
			{
				BitsByte bitsByte6 = reader.ReadByte();
				int num27 = reader.ReadInt16();
				if (Main.netMode == 2)
				{
					num27 = whoAmI;
				}
				Vector2 vector = reader.ReadVector2();
				int num28 = 0;
				num28 = reader.ReadByte();
				int num29 = 0;
				if (bitsByte6[0])
				{
					num29++;
				}
				if (bitsByte6[1])
				{
					num29 += 2;
				}
				bool flag3 = false;
				if (bitsByte6[2])
				{
					flag3 = true;
				}
				int num30 = 0;
				if (bitsByte6[3])
				{
					num30 = reader.ReadInt32();
				}
				if (flag3)
				{
					vector = Main.player[num27].position;
				}
				switch (num29)
				{
				case 0:
					Main.player[num27].Teleport(vector, num28, num30);
					break;
				case 1:
					Main.npc[num27].Teleport(vector, num28, num30);
					break;
				case 2:
				{
					Main.player[num27].Teleport(vector, num28, num30);
					if (Main.netMode != 2)
					{
						break;
					}
					RemoteClient.CheckSection(whoAmI, vector);
					NetMessage.TrySendData(65, -1, -1, null, 0, num27, vector.X, vector.Y, num28, flag3.ToInt(), num30);
					int num31 = -1;
					float num32 = 9999f;
					for (int num33 = 0; num33 < 255; num33++)
					{
						if (Main.player[num33].active && num33 != whoAmI)
						{
							Vector2 vector2 = Main.player[num33].position - Main.player[whoAmI].position;
							if (vector2.Length() < num32)
							{
								num32 = vector2.Length();
								num31 = num33;
							}
						}
					}
					if (num31 >= 0)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Game.HasTeleportedTo", Main.player[whoAmI].name, Main.player[num31].name), new Color(250, 250, 0));
					}
					break;
				}
				}
				if (Main.netMode == 2 && num29 == 0)
				{
					NetMessage.TrySendData(65, -1, whoAmI, null, num29, num27, vector.X, vector.Y, num28, flag3.ToInt(), num30);
				}
				break;
			}
			case 66:
			{
				int num10 = reader.ReadByte();
				int num11 = reader.ReadInt16();
				if (num11 > 0)
				{
					Player player = Main.player[num10];
					player.statLife += num11;
					if (player.statLife > player.statLifeMax2)
					{
						player.statLife = player.statLifeMax2;
					}
					player.HealEffect(num11, broadcast: false);
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(66, -1, whoAmI, null, num10, num11);
					}
				}
				break;
			}
			case 68:
				reader.ReadString();
				break;
			case 69:
			{
				int num222 = reader.ReadInt16();
				int num223 = reader.ReadInt16();
				int num224 = reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num222 >= 0 && num222 < 8000)
					{
						Chest chest3 = Main.chest[num222];
						if (chest3 == null)
						{
							chest3 = new Chest();
							chest3.x = num223;
							chest3.y = num224;
							Main.chest[num222] = chest3;
						}
						else if (chest3.x != num223 || chest3.y != num224)
						{
							break;
						}
						chest3.name = reader.ReadString();
					}
				}
				else
				{
					if (num222 < -1 || num222 >= 8000)
					{
						break;
					}
					if (num222 == -1)
					{
						num222 = Chest.FindChest(num223, num224);
						if (num222 == -1)
						{
							break;
						}
					}
					Chest chest4 = Main.chest[num222];
					if (chest4.x == num223 && chest4.y == num224)
					{
						NetMessage.TrySendData(69, whoAmI, -1, null, num222, num223, num224);
					}
				}
				break;
			}
			case 70:
				if (Main.netMode == 2)
				{
					int num213 = reader.ReadInt16();
					int who = reader.ReadByte();
					if (Main.netMode == 2)
					{
						who = whoAmI;
					}
					if (num213 < 200 && num213 >= 0)
					{
						NPC.CatchNPC(num213, who);
					}
				}
				break;
			case 71:
				if (Main.netMode == 2)
				{
					int x12 = reader.ReadInt32();
					int y11 = reader.ReadInt32();
					int type13 = reader.ReadInt16();
					byte style3 = reader.ReadByte();
					NPC.ReleaseNPC(x12, y11, type13, style3, whoAmI);
				}
				break;
			case 72:
				if (Main.netMode == 1)
				{
					for (int num208 = 0; num208 < 40; num208++)
					{
						Main.travelShop[num208] = reader.ReadInt16();
					}
				}
				break;
			case 73:
				switch (reader.ReadByte())
				{
				case 0:
					Main.player[whoAmI].TeleportationPotion();
					break;
				case 1:
					Main.player[whoAmI].MagicConch();
					break;
				case 2:
					Main.player[whoAmI].DemonConch();
					break;
				}
				break;
			case 74:
				if (Main.netMode == 1)
				{
					Main.anglerQuest = reader.ReadByte();
					Main.anglerQuestFinished = reader.ReadBoolean();
				}
				break;
			case 75:
				if (Main.netMode == 2)
				{
					string name2 = Main.player[whoAmI].name;
					if (!Main.anglerWhoFinishedToday.Contains(name2))
					{
						Main.anglerWhoFinishedToday.Add(name2);
					}
				}
				break;
			case 76:
			{
				int num187 = reader.ReadByte();
				if (num187 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num187 = whoAmI;
					}
					Player obj6 = Main.player[num187];
					obj6.anglerQuestsFinished = reader.ReadInt32();
					obj6.golferScoreAccumulated = reader.ReadInt32();
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(76, -1, whoAmI, null, num187);
					}
				}
				break;
			}
			case 77:
			{
				short type10 = reader.ReadInt16();
				ushort tileType = reader.ReadUInt16();
				short x9 = reader.ReadInt16();
				short y8 = reader.ReadInt16();
				Animation.NewTemporaryAnimation(type10, tileType, x9, y8);
				break;
			}
			case 78:
				if (Main.netMode == 1)
				{
					Main.ReportInvasionProgress(reader.ReadInt32(), reader.ReadInt32(), reader.ReadSByte(), reader.ReadSByte());
				}
				break;
			case 79:
			{
				int x7 = reader.ReadInt16();
				int y6 = reader.ReadInt16();
				short type8 = reader.ReadInt16();
				int style2 = reader.ReadInt16();
				int num148 = reader.ReadByte();
				int random = reader.ReadSByte();
				int direction = (reader.ReadBoolean() ? 1 : (-1));
				if (Main.netMode == 2)
				{
					Netplay.Clients[whoAmI].SpamAddBlock += 1f;
					if (!WorldGen.InWorld(x7, y6, 10) || !Netplay.Clients[whoAmI].TileSections[Netplay.GetSectionX(x7), Netplay.GetSectionY(y6)])
					{
						break;
					}
				}
				WorldGen.PlaceObject(x7, y6, type8, mute: false, style2, num148, random, direction);
				if (Main.netMode == 2)
				{
					NetMessage.SendObjectPlacment(whoAmI, x7, y6, type8, style2, num148, random, direction);
				}
				break;
			}
			case 80:
				if (Main.netMode == 1)
				{
					int num130 = reader.ReadByte();
					int num131 = reader.ReadInt16();
					if (num131 >= -3 && num131 < 8000)
					{
						Main.player[num130].chest = num131;
						Recipe.FindRecipes(canDelayCheck: true);
					}
				}
				break;
			case 81:
				if (Main.netMode == 1)
				{
					int x6 = (int)reader.ReadSingle();
					int y5 = (int)reader.ReadSingle();
					CombatText.NewText(color: reader.ReadRGB(), amount: reader.ReadInt32(), location: new Rectangle(x6, y5, 0, 0));
				}
				break;
			case 119:
				if (Main.netMode == 1)
				{
					int x5 = (int)reader.ReadSingle();
					int y4 = (int)reader.ReadSingle();
					CombatText.NewText(color: reader.ReadRGB(), text: NetworkText.Deserialize(reader).ToString(), location: new Rectangle(x5, y4, 0, 0));
				}
				break;
			case 82:
				NetManager.Instance.Read(reader, whoAmI, length);
				break;
			case 83:
				if (Main.netMode == 1)
				{
					int num96 = reader.ReadInt16();
					int num97 = reader.ReadInt32();
					if (num96 >= 0 && num96 < 289)
					{
						NPC.killCount[num96] = num97;
					}
				}
				break;
			case 84:
			{
				int num67 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num67 = whoAmI;
				}
				float stealth = reader.ReadSingle();
				Main.player[num67].stealth = stealth;
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(84, -1, whoAmI, null, num67);
				}
				break;
			}
			case 85:
			{
				int num59 = whoAmI;
				byte b8 = reader.ReadByte();
				if (Main.netMode == 2 && num59 < 255 && b8 < 58)
				{
					Chest.ServerPlaceItem(whoAmI, b8);
				}
				break;
			}
			case 86:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num34 = reader.ReadInt32();
				if (!reader.ReadBoolean())
				{
					if (TileEntity.ByID.TryGetValue(num34, out var value2))
					{
						TileEntity.ByID.Remove(num34);
						TileEntity.ByPosition.Remove(value2.Position);
					}
				}
				else
				{
					TileEntity tileEntity = TileEntity.Read(reader, networkSend: true);
					tileEntity.ID = num34;
					TileEntity.ByID[tileEntity.ID] = tileEntity;
					TileEntity.ByPosition[tileEntity.Position] = tileEntity;
				}
				break;
			}
			case 87:
				if (Main.netMode == 2)
				{
					int x2 = reader.ReadInt16();
					int y2 = reader.ReadInt16();
					int type2 = reader.ReadByte();
					if (WorldGen.InWorld(x2, y2) && !TileEntity.ByPosition.ContainsKey(new Point16(x2, y2)))
					{
						TileEntity.PlaceEntityNet(x2, y2, type2);
					}
				}
				break;
			case 88:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num212 = reader.ReadInt16();
				if (num212 < 0 || num212 > 400)
				{
					break;
				}
				Item item4 = Main.item[num212];
				BitsByte bitsByte18 = reader.ReadByte();
				if (bitsByte18[0])
				{
					item4.color.PackedValue = reader.ReadUInt32();
				}
				if (bitsByte18[1])
				{
					item4.damage = reader.ReadUInt16();
				}
				if (bitsByte18[2])
				{
					item4.knockBack = reader.ReadSingle();
				}
				if (bitsByte18[3])
				{
					item4.useAnimation = reader.ReadUInt16();
				}
				if (bitsByte18[4])
				{
					item4.useTime = reader.ReadUInt16();
				}
				if (bitsByte18[5])
				{
					item4.shoot = reader.ReadInt16();
				}
				if (bitsByte18[6])
				{
					item4.shootSpeed = reader.ReadSingle();
				}
				if (bitsByte18[7])
				{
					bitsByte18 = reader.ReadByte();
					if (bitsByte18[0])
					{
						item4.width = reader.ReadInt16();
					}
					if (bitsByte18[1])
					{
						item4.height = reader.ReadInt16();
					}
					if (bitsByte18[2])
					{
						item4.scale = reader.ReadSingle();
					}
					if (bitsByte18[3])
					{
						item4.ammo = reader.ReadInt16();
					}
					if (bitsByte18[4])
					{
						item4.useAmmo = reader.ReadInt16();
					}
					if (bitsByte18[5])
					{
						item4.notAmmo = reader.ReadBoolean();
					}
				}
				break;
			}
			case 89:
				if (Main.netMode == 2)
				{
					short x11 = reader.ReadInt16();
					int y10 = reader.ReadInt16();
					int netid3 = reader.ReadInt16();
					int prefix3 = reader.ReadByte();
					int stack5 = reader.ReadInt16();
					TEItemFrame.TryPlacing(x11, y10, netid3, prefix3, stack5);
				}
				break;
			case 91:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num195 = reader.ReadInt32();
				int num196 = reader.ReadByte();
				if (num196 == 255)
				{
					if (EmoteBubble.byID.ContainsKey(num195))
					{
						EmoteBubble.byID.Remove(num195);
					}
					break;
				}
				int num197 = reader.ReadUInt16();
				int num198 = reader.ReadUInt16();
				int num199 = reader.ReadByte();
				int metadata = 0;
				if (num199 < 0)
				{
					metadata = reader.ReadInt16();
				}
				WorldUIAnchor worldUIAnchor = EmoteBubble.DeserializeNetAnchor(num196, num197);
				if (num196 == 1)
				{
					Main.player[num197].emoteTime = 360;
				}
				lock (EmoteBubble.byID)
				{
					if (!EmoteBubble.byID.ContainsKey(num195))
					{
						EmoteBubble.byID[num195] = new EmoteBubble(num199, worldUIAnchor, num198);
					}
					else
					{
						EmoteBubble.byID[num195].lifeTime = num198;
						EmoteBubble.byID[num195].lifeTimeStart = num198;
						EmoteBubble.byID[num195].emote = num199;
						EmoteBubble.byID[num195].anchor = worldUIAnchor;
					}
					EmoteBubble.byID[num195].ID = num195;
					EmoteBubble.byID[num195].metadata = metadata;
					EmoteBubble.OnBubbleChange(num195);
				}
				break;
			}
			case 92:
			{
				int num183 = reader.ReadInt16();
				int num184 = reader.ReadInt32();
				float num185 = reader.ReadSingle();
				float num186 = reader.ReadSingle();
				if (num183 >= 0 && num183 <= 200)
				{
					if (Main.netMode == 1)
					{
						Main.npc[num183].moneyPing(new Vector2(num185, num186));
						Main.npc[num183].extraValue = num184;
					}
					else
					{
						Main.npc[num183].extraValue += num184;
						NetMessage.TrySendData(92, -1, -1, null, num183, Main.npc[num183].extraValue, num185, num186);
					}
				}
				break;
			}
			case 95:
			{
				ushort num175 = reader.ReadUInt16();
				int num176 = reader.ReadByte();
				if (Main.netMode != 2)
				{
					break;
				}
				for (int num177 = 0; num177 < 1000; num177++)
				{
					if (Main.projectile[num177].owner == num175 && Main.projectile[num177].active && Main.projectile[num177].type == 602 && Main.projectile[num177].ai[1] == (float)num176)
					{
						Main.projectile[num177].Kill();
						NetMessage.TrySendData(29, -1, -1, null, Main.projectile[num177].identity, (int)num175);
						break;
					}
				}
				break;
			}
			case 96:
			{
				int num171 = reader.ReadByte();
				Player obj5 = Main.player[num171];
				int num172 = reader.ReadInt16();
				Vector2 newPos2 = reader.ReadVector2();
				Vector2 velocity5 = reader.ReadVector2();
				int num173 = (obj5.lastPortalColorIndex = num172 + ((num172 % 2 == 0) ? 1 : (-1)));
				obj5.Teleport(newPos2, 4, num172);
				obj5.velocity = velocity5;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(96, -1, -1, null, num171, newPos2.X, newPos2.Y, num172);
				}
				break;
			}
			case 97:
				if (Main.netMode == 1)
				{
					AchievementsHelper.NotifyNPCKilledDirect(Main.player[Main.myPlayer], reader.ReadInt16());
				}
				break;
			case 98:
				if (Main.netMode == 1)
				{
					AchievementsHelper.NotifyProgressionEvent(reader.ReadInt16());
				}
				break;
			case 99:
			{
				int num147 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num147 = whoAmI;
				}
				Main.player[num147].MinionRestTargetPoint = reader.ReadVector2();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(99, -1, whoAmI, null, num147);
				}
				break;
			}
			case 115:
			{
				int num135 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num135 = whoAmI;
				}
				Main.player[num135].MinionAttackTargetNPC = reader.ReadInt16();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(115, -1, whoAmI, null, num135);
				}
				break;
			}
			case 100:
			{
				int num127 = reader.ReadUInt16();
				NPC obj3 = Main.npc[num127];
				int num128 = reader.ReadInt16();
				Vector2 newPos = reader.ReadVector2();
				Vector2 velocity = reader.ReadVector2();
				int num129 = (obj3.lastPortalColorIndex = num128 + ((num128 % 2 == 0) ? 1 : (-1)));
				obj3.Teleport(newPos, 4, num128);
				obj3.velocity = velocity;
				obj3.netOffset *= 0f;
				break;
			}
			case 101:
				if (Main.netMode != 2)
				{
					NPC.ShieldStrengthTowerSolar = reader.ReadUInt16();
					NPC.ShieldStrengthTowerVortex = reader.ReadUInt16();
					NPC.ShieldStrengthTowerNebula = reader.ReadUInt16();
					NPC.ShieldStrengthTowerStardust = reader.ReadUInt16();
					if (NPC.ShieldStrengthTowerSolar < 0)
					{
						NPC.ShieldStrengthTowerSolar = 0;
					}
					if (NPC.ShieldStrengthTowerVortex < 0)
					{
						NPC.ShieldStrengthTowerVortex = 0;
					}
					if (NPC.ShieldStrengthTowerNebula < 0)
					{
						NPC.ShieldStrengthTowerNebula = 0;
					}
					if (NPC.ShieldStrengthTowerStardust < 0)
					{
						NPC.ShieldStrengthTowerStardust = 0;
					}
					if (NPC.ShieldStrengthTowerSolar > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerSolar = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerVortex > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerVortex = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerNebula > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerNebula = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerStardust > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerStardust = NPC.LunarShieldPowerExpert;
					}
				}
				break;
			case 102:
			{
				int num35 = reader.ReadByte();
				ushort num36 = reader.ReadUInt16();
				Vector2 other = reader.ReadVector2();
				if (Main.netMode == 2)
				{
					num35 = whoAmI;
					NetMessage.TrySendData(102, -1, -1, null, num35, (int)num36, other.X, other.Y);
					break;
				}
				Player player3 = Main.player[num35];
				for (int num37 = 0; num37 < 255; num37++)
				{
					Player player4 = Main.player[num37];
					if (!player4.active || player4.dead || (player3.team != 0 && player3.team != player4.team) || !(player4.Distance(other) < 700f))
					{
						continue;
					}
					Vector2 value3 = player3.Center - player4.Center;
					Vector2 vector3 = Vector2.Normalize(value3);
					if (!vector3.HasNaNs())
					{
						int type3 = 90;
						float num38 = 0f;
						float num39 = (float)Math.PI / 15f;
						Vector2 spinningpoint = new Vector2(0f, -8f);
						Vector2 vector4 = new Vector2(-3f);
						float num40 = 0f;
						float num41 = 0.005f;
						switch (num36)
						{
						case 179:
							type3 = 86;
							break;
						case 173:
							type3 = 90;
							break;
						case 176:
							type3 = 88;
							break;
						}
						for (int num42 = 0; (float)num42 < value3.Length() / 6f; num42++)
						{
							Vector2 position = player4.Center + 6f * (float)num42 * vector3 + spinningpoint.RotatedBy(num38) + vector4;
							num38 += num39;
							int num43 = Dust.NewDust(position, 6, 6, type3, 0f, 0f, 100, default(Color), 1.5f);
							Main.dust[num43].noGravity = true;
							Main.dust[num43].velocity = Vector2.Zero;
							num40 = (Main.dust[num43].fadeIn = num40 + num41);
							Main.dust[num43].velocity += vector3 * 1.5f;
						}
					}
					player4.NebulaLevelup(num36);
				}
				break;
			}
			case 103:
				if (Main.netMode == 1)
				{
					NPC.MoonLordCountdown = reader.ReadInt32();
				}
				break;
			case 104:
				if (Main.netMode == 1 && Main.npcShop > 0)
				{
					Item[] item = Main.instance.shop[Main.npcShop].item;
					int num21 = reader.ReadByte();
					int type = reader.ReadInt16();
					int stack = reader.ReadInt16();
					int pre = reader.ReadByte();
					int value = reader.ReadInt32();
					BitsByte bitsByte = reader.ReadByte();
					if (num21 < item.Length)
					{
						item[num21] = new Item();
						item[num21].netDefaults(type);
						item[num21].stack = stack;
						item[num21].Prefix(pre);
						item[num21].value = value;
						item[num21].buyOnce = bitsByte[0];
					}
				}
				break;
			case 105:
				if (Main.netMode != 1)
				{
					short i2 = reader.ReadInt16();
					int j2 = reader.ReadInt16();
					bool on = reader.ReadBoolean();
					WorldGen.ToggleGemLock(i2, j2, on);
				}
				break;
			case 106:
				if (Main.netMode == 1)
				{
					HalfVector2 halfVector = default(HalfVector2);
					halfVector.PackedValue = reader.ReadUInt32();
					Utils.PoofOfSmoke(halfVector.ToVector2());
				}
				break;
			case 107:
				if (Main.netMode == 1)
				{
					Color c = reader.ReadRGB();
					string text = NetworkText.Deserialize(reader).ToString();
					int widthLimit = reader.ReadInt16();
					Main.NewTextMultiline(text, force: false, c, widthLimit);
				}
				break;
			case 108:
				if (Main.netMode == 1)
				{
					int damage = reader.ReadInt16();
					float knockBack = reader.ReadSingle();
					int x = reader.ReadInt16();
					int y = reader.ReadInt16();
					int angle = reader.ReadInt16();
					int ammo = reader.ReadInt16();
					int num2 = reader.ReadByte();
					if (num2 == Main.myPlayer)
					{
						WorldGen.ShootFromCannon(x, y, angle, ammo, damage, knockBack, num2, fromWire: true);
					}
				}
				break;
			case 109:
				if (Main.netMode == 2)
				{
					short x14 = reader.ReadInt16();
					int y13 = reader.ReadInt16();
					int x15 = reader.ReadInt16();
					int y14 = reader.ReadInt16();
					byte toolMode = reader.ReadByte();
					int num265 = whoAmI;
					WiresUI.Settings.MultiToolMode toolMode2 = WiresUI.Settings.ToolMode;
					WiresUI.Settings.ToolMode = (WiresUI.Settings.MultiToolMode)toolMode;
					Wiring.MassWireOperation(new Point(x14, y13), new Point(x15, y14), Main.player[num265]);
					WiresUI.Settings.ToolMode = toolMode2;
				}
				break;
			case 110:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int type17 = reader.ReadInt16();
				int num254 = reader.ReadInt16();
				int num255 = reader.ReadByte();
				if (num255 == Main.myPlayer)
				{
					Player player16 = Main.player[num255];
					for (int num256 = 0; num256 < num254; num256++)
					{
						player16.ConsumeItem(type17);
					}
					player16.wireOperationsCooldown = 0;
				}
				break;
			}
			case 111:
				if (Main.netMode == 2)
				{
					BirthdayParty.ToggleManualParty();
				}
				break;
			case 112:
			{
				int num244 = reader.ReadByte();
				int num245 = reader.ReadInt32();
				int num246 = reader.ReadInt32();
				int num247 = reader.ReadByte();
				int num248 = reader.ReadInt16();
				switch (num244)
				{
				case 1:
					if (Main.netMode == 1)
					{
						WorldGen.TreeGrowFX(num245, num246, num247, num248);
					}
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(b, -1, -1, null, num244, num245, num246, num247, num248);
					}
					break;
				case 2:
					NPC.FairyEffects(new Vector2(num245, num246), num248);
					break;
				}
				break;
			}
			case 113:
			{
				int x13 = reader.ReadInt16();
				int y12 = reader.ReadInt16();
				if (Main.netMode == 2 && !Main.snowMoon && !Main.pumpkinMoon)
				{
					if (DD2Event.WouldFailSpawningHere(x13, y12))
					{
						DD2Event.FailureMessage(whoAmI);
					}
					DD2Event.SummonCrystal(x13, y12);
				}
				break;
			}
			case 114:
				if (Main.netMode == 1)
				{
					DD2Event.WipeEntities();
				}
				break;
			case 116:
				if (Main.netMode == 1)
				{
					DD2Event.TimeLeftBetweenWaves = reader.ReadInt32();
				}
				break;
			case 117:
			{
				int num218 = reader.ReadByte();
				if (Main.netMode != 2 || whoAmI == num218 || (Main.player[num218].hostile && Main.player[whoAmI].hostile))
				{
					PlayerDeathReason playerDeathReason2 = PlayerDeathReason.FromReader(reader);
					int damage3 = reader.ReadInt16();
					int num219 = reader.ReadByte() - 1;
					BitsByte bitsByte19 = reader.ReadByte();
					bool flag12 = bitsByte19[0];
					bool pvp2 = bitsByte19[1];
					int num220 = reader.ReadSByte();
					Main.player[num218].Hurt(playerDeathReason2, damage3, num219, pvp2, quiet: true, flag12, num220);
					if (Main.netMode == 2)
					{
						NetMessage.SendPlayerHurt(num218, playerDeathReason2, damage3, num219, flag12, pvp2, num220, -1, whoAmI);
					}
				}
				break;
			}
			case 118:
			{
				int num214 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num214 = whoAmI;
				}
				PlayerDeathReason playerDeathReason = PlayerDeathReason.FromReader(reader);
				int num215 = reader.ReadInt16();
				int num216 = reader.ReadByte() - 1;
				bool pvp = ((BitsByte)reader.ReadByte())[0];
				Main.player[num214].KillMe(playerDeathReason, num215, num216, pvp);
				if (Main.netMode == 2)
				{
					NetMessage.SendPlayerDeath(num214, playerDeathReason, num215, num216, pvp, -1, whoAmI);
				}
				break;
			}
			case 120:
			{
				int num209 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num209 = whoAmI;
				}
				int num210 = reader.ReadByte();
				if (num210 >= 0 && num210 < 146 && Main.netMode == 2)
				{
					EmoteBubble.NewBubble(num210, new WorldUIAnchor(Main.player[num209]), 360);
					EmoteBubble.CheckForNPCsToReactToEmoteBubble(num210, Main.player[num209]);
				}
				break;
			}
			case 121:
			{
				int num192 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num192 = whoAmI;
				}
				int num193 = reader.ReadInt32();
				int num194 = reader.ReadByte();
				bool flag10 = false;
				if (num194 >= 8)
				{
					flag10 = true;
					num194 -= 8;
				}
				if (!TileEntity.ByID.TryGetValue(num193, out var value9))
				{
					reader.ReadInt32();
					reader.ReadByte();
					break;
				}
				if (num194 >= 8)
				{
					value9 = null;
				}
				TEDisplayDoll tEDisplayDoll = value9 as TEDisplayDoll;
				if (tEDisplayDoll != null)
				{
					tEDisplayDoll.ReadItem(num194, reader, flag10);
				}
				else
				{
					reader.ReadInt32();
					reader.ReadByte();
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(b, -1, num192, null, num192, num193, num194, flag10.ToInt());
				}
				break;
			}
			case 122:
			{
				int num160 = reader.ReadInt32();
				int num161 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num161 = whoAmI;
				}
				if (Main.netMode == 2)
				{
					if (num160 == -1)
					{
						Main.player[num161].tileEntityAnchor.Clear();
						NetMessage.TrySendData(b, -1, -1, null, num160, num161);
						break;
					}
					if (!TileEntity.IsOccupied(num160, out var _) && TileEntity.ByID.TryGetValue(num160, out var value7))
					{
						Main.player[num161].tileEntityAnchor.Set(num160, value7.Position.X, value7.Position.Y);
						NetMessage.TrySendData(b, -1, -1, null, num160, num161);
					}
				}
				if (Main.netMode == 1)
				{
					TileEntity value8;
					if (num160 == -1)
					{
						Main.player[num161].tileEntityAnchor.Clear();
					}
					else if (TileEntity.ByID.TryGetValue(num160, out value8))
					{
						TileEntity.SetInteractionAnchor(Main.player[num161], value8.Position.X, value8.Position.Y, num160);
					}
				}
				break;
			}
			case 123:
				if (Main.netMode == 2)
				{
					short x8 = reader.ReadInt16();
					int y7 = reader.ReadInt16();
					int netid2 = reader.ReadInt16();
					int prefix2 = reader.ReadByte();
					int stack3 = reader.ReadInt16();
					TEWeaponsRack.TryPlacing(x8, y7, netid2, prefix2, stack3);
				}
				break;
			case 124:
			{
				int num132 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num132 = whoAmI;
				}
				int num133 = reader.ReadInt32();
				int num134 = reader.ReadByte();
				bool flag7 = false;
				if (num134 >= 2)
				{
					flag7 = true;
					num134 -= 2;
				}
				if (!TileEntity.ByID.TryGetValue(num133, out var value5))
				{
					reader.ReadInt32();
					reader.ReadByte();
					break;
				}
				if (num134 >= 2)
				{
					value5 = null;
				}
				TEHatRack tEHatRack = value5 as TEHatRack;
				if (tEHatRack != null)
				{
					tEHatRack.ReadItem(num134, reader, flag7);
				}
				else
				{
					reader.ReadInt32();
					reader.ReadByte();
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(b, -1, num132, null, num132, num133, num134, flag7.ToInt());
				}
				break;
			}
			case 125:
			{
				int num99 = reader.ReadByte();
				int num100 = reader.ReadInt16();
				int num101 = reader.ReadInt16();
				int num102 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num99 = whoAmI;
				}
				if (Main.netMode == 1)
				{
					Main.player[Main.myPlayer].GetOtherPlayersPickTile(num100, num101, num102);
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(125, -1, num99, null, num99, num100, num101, num102);
				}
				break;
			}
			case 126:
				if (Main.netMode == 1)
				{
					NPC.RevengeManager.AddMarkerFromReader(reader);
				}
				break;
			case 127:
			{
				int markerUniqueID = reader.ReadInt32();
				if (Main.netMode == 1)
				{
					NPC.RevengeManager.DestroyMarker(markerUniqueID);
				}
				break;
			}
			case 128:
			{
				int num91 = reader.ReadByte();
				int num92 = reader.ReadUInt16();
				int num93 = reader.ReadUInt16();
				int num94 = reader.ReadUInt16();
				int num95 = reader.ReadUInt16();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(128, -1, num91, null, num91, num94, num95, 0f, num92, num93);
				}
				else
				{
					GolfHelper.ContactListener.PutBallInCup_TextAndEffects(new Point(num92, num93), num91, num94, num95);
				}
				break;
			}
			case 129:
				if (Main.netMode == 1)
				{
					Main.FixUIScale();
					Main.TrySetPreparationState(Main.WorldPreparationState.ProcessingData);
				}
				break;
			case 130:
				if (Main.netMode == 2)
				{
					ushort num68 = reader.ReadUInt16();
					int num69 = reader.ReadUInt16();
					int type6 = reader.ReadInt16();
					int x4 = num68 * 16;
					num69 *= 16;
					NPC nPC3 = new NPC();
					spawnparams = default(NPCSpawnParams);
					nPC3.SetDefaults(type6, spawnparams);
					int type7 = nPC3.type;
					int netID = nPC3.netID;
					int num70 = NPC.NewNPC(x4, num69, type6);
					if (netID != type7)
					{
						NPC obj2 = Main.npc[num70];
						spawnparams = default(NPCSpawnParams);
						obj2.SetDefaults(netID, spawnparams);
						NetMessage.TrySendData(23, -1, -1, null, num70);
					}
				}
				break;
			case 131:
				if (Main.netMode == 1)
				{
					int num58 = reader.ReadUInt16();
					NPC nPC2 = null;
					nPC2 = ((num58 >= 200) ? new NPC() : Main.npc[num58]);
					if (reader.ReadByte() == 1)
					{
						int time = reader.ReadInt32();
						int fromWho = reader.ReadInt16();
						nPC2.GetImmuneTime(fromWho, time);
					}
				}
				break;
			case 132:
				if (Main.netMode == 1)
				{
					Point point = reader.ReadVector2().ToPoint();
					ushort key = reader.ReadUInt16();
					LegacySoundStyle legacySoundStyle = SoundID.SoundByIndex[key];
					BitsByte bitsByte5 = reader.ReadByte();
					int num24 = -1;
					float num25 = 1f;
					float num26 = 0f;
					SoundEngine.PlaySound(Style: (!bitsByte5[0]) ? legacySoundStyle.Style : reader.ReadInt32(), volumeScale: (!bitsByte5[1]) ? legacySoundStyle.Volume : MathHelper.Clamp(reader.ReadSingle(), 0f, 1f), pitchOffset: (!bitsByte5[2]) ? legacySoundStyle.GetRandomPitch() : MathHelper.Clamp(reader.ReadSingle(), -1f, 1f), type: legacySoundStyle.SoundId, x: point.X, y: point.Y);
				}
				break;
			case 133:
				if (Main.netMode == 2)
				{
					short x3 = reader.ReadInt16();
					int y3 = reader.ReadInt16();
					int netid = reader.ReadInt16();
					int prefix = reader.ReadByte();
					int stack2 = reader.ReadInt16();
					TEFoodPlatter.TryPlacing(x3, y3, netid, prefix, stack2);
				}
				break;
			case 134:
			{
				int num20 = reader.ReadByte();
				int ladyBugLuckTimeLeft = reader.ReadInt32();
				float torchLuck = reader.ReadSingle();
				byte luckPotion = reader.ReadByte();
				bool hasGardenGnomeNearby = reader.ReadBoolean();
				if (Main.netMode == 2)
				{
					num20 = whoAmI;
				}
				Player obj = Main.player[num20];
				obj.ladyBugLuckTimeLeft = ladyBugLuckTimeLeft;
				obj.torchLuck = torchLuck;
				obj.luckPotion = luckPotion;
				obj.HasGardenGnomeNearby = hasGardenGnomeNearby;
				obj.RecalculateLuck();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(134, -1, num20, null, num20);
				}
				break;
			}
			case 135:
			{
				int num19 = reader.ReadByte();
				if (Main.netMode == 1)
				{
					Main.player[num19].immuneAlpha = 255;
				}
				break;
			}
			case 136:
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = 0; l < 3; l++)
					{
						NPC.cavernMonsterType[k, l] = reader.ReadUInt16();
					}
				}
				break;
			}
			case 137:
				if (Main.netMode == 2)
				{
					int num9 = reader.ReadInt16();
					int buffTypeToRemove = reader.ReadUInt16();
					if (num9 >= 0 && num9 < 200)
					{
						Main.npc[num9].RequestBuffRemoval(buffTypeToRemove);
					}
				}
				break;
			case 139:
				if (Main.netMode != 2)
				{
					int num6 = reader.ReadByte();
					bool flag = reader.ReadBoolean();
					Main.countsAsHostForGameplay[num6] = flag;
				}
				break;
			case 140:
				if (Main.netMode == 1)
				{
					reader.ReadByte();
					CreditsRollEvent.SetRemainingTimeDirect(reader.ReadInt32());
				}
				break;
			default:
				if (Netplay.Clients[whoAmI].State == 0)
				{
					NetMessage.BootPlayer(whoAmI, Lang.mp[2].ToNetworkText());
				}
				break;
			case 15:
			case 25:
			case 26:
			case 44:
			case 67:
			case 93:
				break;
			}
		}
	}
}
