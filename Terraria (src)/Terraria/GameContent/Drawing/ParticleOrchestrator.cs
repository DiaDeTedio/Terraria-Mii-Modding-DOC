using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.NetModules;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.Net;

namespace Terraria.GameContent.Drawing
{
	public class ParticleOrchestrator
	{
		private static ParticlePool<FadingParticle> _poolFading = new ParticlePool<FadingParticle>(200, GetNewFadingParticle);

		private static ParticlePool<FlameParticle> _poolFlame = new ParticlePool<FlameParticle>(200, GetNewFlameParticle);

		private static ParticlePool<RandomizedFrameParticle> _poolRandomizedFrame = new ParticlePool<RandomizedFrameParticle>(200, GetNewRandomizedFrameParticle);

		private static ParticlePool<PrettySparkleParticle> _poolPrettySparkle = new ParticlePool<PrettySparkleParticle>(200, GetNewPrettySparkleParticle);

		public static void RequestParticleSpawn(bool clientOnly, ParticleOrchestraType type, ParticleOrchestraSettings settings, int? overrideInvokingPlayerIndex = null)
		{
			settings.IndexOfPlayerWhoInvokedThis = (byte)Main.myPlayer;
			if (overrideInvokingPlayerIndex.HasValue)
			{
				settings.IndexOfPlayerWhoInvokedThis = (byte)overrideInvokingPlayerIndex.Value;
			}
			if (clientOnly)
			{
				SpawnParticlesDirect(type, settings);
			}
			else
			{
				NetManager.Instance.SendToServerOrLoopback(NetParticlesModule.Serialize(type, settings));
			}
		}

		private static FadingParticle GetNewFadingParticle()
		{
			return new FadingParticle();
		}

		private static FlameParticle GetNewFlameParticle()
		{
			return new FlameParticle();
		}

		private static RandomizedFrameParticle GetNewRandomizedFrameParticle()
		{
			return new RandomizedFrameParticle();
		}

		private static PrettySparkleParticle GetNewPrettySparkleParticle()
		{
			return new PrettySparkleParticle();
		}

		public static void SpawnParticlesDirect(ParticleOrchestraType type, ParticleOrchestraSettings settings)
		{
			if (Main.netMode != 2)
			{
				switch (type)
				{
				case ParticleOrchestraType.Keybrand:
					Spawn_Keybrand(settings);
					break;
				case ParticleOrchestraType.FlameWaders:
					Spawn_FlameWaders(settings);
					break;
				case ParticleOrchestraType.StellarTune:
					Spawn_StellarTune(settings);
					break;
				case ParticleOrchestraType.WallOfFleshGoatMountFlames:
					Spawn_WallOfFleshGoatMountFlames(settings);
					break;
				case ParticleOrchestraType.BlackLightningHit:
					Spawn_BlackLightningHit(settings);
					break;
				case ParticleOrchestraType.RainbowRodHit:
					Spawn_RainbowRodHit(settings);
					break;
				case ParticleOrchestraType.BlackLightningSmall:
					Spawn_BlackLightningSmall(settings);
					break;
				case ParticleOrchestraType.StardustPunch:
					Spawn_StardustPunch(settings);
					break;
				case ParticleOrchestraType.PrincessWeapon:
					Spawn_PrincessWeapon(settings);
					break;
				}
			}
		}

		private static void Spawn_PrincessWeapon(ParticleOrchestraSettings settings)
		{
			float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float num2 = 1f;
			for (float num3 = 0f; num3 < 1f; num3 += 1f / num2)
			{
				Vector2 vector = settings.MovementVector * (0.6f + Main.rand.NextFloat() * 0.35f);
				Vector2 vector2 = new Vector2(Main.rand.NextFloat() * 0.4f + 0.2f);
				float f = num + Main.rand.NextFloat() * ((float)Math.PI * 2f);
				float rotation = (float)Math.PI / 2f;
				Vector2 vector3 = 0.1f * vector2;
				float num4 = 60f;
				Vector2 vector4 = Main.rand.NextVector2Circular(8f, 8f) * vector2;
				PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
				prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num4) - vector * 1f / 30f;
				prettySparkleParticle.AccelerationPerFrame = -vector * 1f / 60f;
				prettySparkleParticle.Velocity = vector * 0.66f;
				prettySparkleParticle.ColorTint = Main.hslToRgb((0.92f + Main.rand.NextFloat() * 0.02f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
				prettySparkleParticle.ColorTint.A = 0;
				prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
				prettySparkleParticle.Rotation = rotation;
				prettySparkleParticle.Scale = vector2;
				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
				prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
				prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num4) - vector * 1f / 15f;
				prettySparkleParticle.AccelerationPerFrame = -vector * 1f / 60f;
				prettySparkleParticle.Velocity = vector * 0.66f;
				prettySparkleParticle.ColorTint = new Color(255, 255, 255, 0);
				prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
				prettySparkleParticle.Rotation = rotation;
				prettySparkleParticle.Scale = vector2 * 0.6f;
				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			}
			for (int i = 0; i < 2; i++)
			{
				Color newColor = Main.hslToRgb((0.92f + Main.rand.NextFloat() * 0.02f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
				int num5 = Dust.NewDust(settings.PositionInWorld, 0, 0, 267, 0f, 0f, 0, newColor);
				Main.dust[num5].velocity = Main.rand.NextVector2Circular(2f, 2f);
				Main.dust[num5].velocity += settings.MovementVector * (0.5f + 0.5f * Main.rand.NextFloat()) * 1.4f;
				Main.dust[num5].noGravity = true;
				Main.dust[num5].scale = 0.1f;
				Main.dust[num5].position += Main.rand.NextVector2Circular(16f, 16f);
				Main.dust[num5].velocity = settings.MovementVector;
				if (num5 != 6000)
				{
					Dust dust = Dust.CloneDust(num5);
					dust.scale /= 2f;
					dust.fadeIn *= 0.75f;
					dust.color = new Color(255, 255, 255, 255);
				}
			}
		}

		private static void Spawn_StardustPunch(ParticleOrchestraSettings settings)
		{
			float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float num2 = 1f;
			for (float num3 = 0f; num3 < 1f; num3 += 1f / num2)
			{
				Vector2 vector = settings.MovementVector * (0.3f + Main.rand.NextFloat() * 0.35f);
				Vector2 vector2 = new Vector2(Main.rand.NextFloat() * 0.4f + 0.4f);
				float f = num + Main.rand.NextFloat() * ((float)Math.PI * 2f);
				float rotation = (float)Math.PI / 2f;
				Vector2 vector3 = 0.1f * vector2;
				float num4 = 60f;
				Vector2 vector4 = Main.rand.NextVector2Circular(8f, 8f) * vector2;
				PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
				prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num4) - vector * 1f / 60f;
				prettySparkleParticle.ColorTint = Main.hslToRgb((0.6f + Main.rand.NextFloat() * 0.05f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
				prettySparkleParticle.ColorTint.A = 0;
				prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
				prettySparkleParticle.Rotation = rotation;
				prettySparkleParticle.Scale = vector2;
				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
				prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
				prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num4) - vector * 1f / 30f;
				prettySparkleParticle.ColorTint = new Color(255, 255, 255, 0);
				prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
				prettySparkleParticle.Rotation = rotation;
				prettySparkleParticle.Scale = vector2 * 0.6f;
				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			}
			for (int i = 0; i < 2; i++)
			{
				Color newColor = Main.hslToRgb((0.59f + Main.rand.NextFloat() * 0.05f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
				int num5 = Dust.NewDust(settings.PositionInWorld, 0, 0, 267, 0f, 0f, 0, newColor);
				Main.dust[num5].velocity = Main.rand.NextVector2Circular(2f, 2f);
				Main.dust[num5].velocity += settings.MovementVector * (0.5f + 0.5f * Main.rand.NextFloat()) * 1.4f;
				Main.dust[num5].noGravity = true;
				Main.dust[num5].scale = 0.6f + Main.rand.NextFloat() * 2f;
				Main.dust[num5].position += Main.rand.NextVector2Circular(16f, 16f);
				if (num5 != 6000)
				{
					Dust dust = Dust.CloneDust(num5);
					dust.scale /= 2f;
					dust.fadeIn *= 0.75f;
					dust.color = new Color(255, 255, 255, 255);
				}
			}
		}

		private static void Spawn_RainbowRodHit(ParticleOrchestraSettings settings)
		{
			float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float num2 = 6f;
			float num3 = Main.rand.NextFloat();
			for (float num4 = 0f; num4 < 1f; num4 += 1f / num2)
			{
				Vector2 vector = settings.MovementVector * Main.rand.NextFloatDirection() * 0.15f;
				Vector2 vector2 = new Vector2(Main.rand.NextFloat() * 0.4f + 0.4f);
				float f = num + Main.rand.NextFloat() * ((float)Math.PI * 2f);
				float rotation = (float)Math.PI / 2f;
				Vector2 vector3 = 1.5f * vector2;
				float num5 = 60f;
				Vector2 vector4 = Main.rand.NextVector2Circular(8f, 8f) * vector2;
				PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
				prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num5) - vector * 1f / 60f;
				prettySparkleParticle.ColorTint = Main.hslToRgb((num3 + Main.rand.NextFloat() * 0.33f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
				prettySparkleParticle.ColorTint.A = 0;
				prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
				prettySparkleParticle.Rotation = rotation;
				prettySparkleParticle.Scale = vector2;
				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
				prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
				prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num5) - vector * 1f / 60f;
				prettySparkleParticle.ColorTint = new Color(255, 255, 255, 0);
				prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
				prettySparkleParticle.Rotation = rotation;
				prettySparkleParticle.Scale = vector2 * 0.6f;
				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			}
			for (int i = 0; i < 12; i++)
			{
				Color newColor = Main.hslToRgb((num3 + Main.rand.NextFloat() * 0.12f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
				int num6 = Dust.NewDust(settings.PositionInWorld, 0, 0, 267, 0f, 0f, 0, newColor);
				Main.dust[num6].velocity = Main.rand.NextVector2Circular(1f, 1f);
				Main.dust[num6].velocity += settings.MovementVector * Main.rand.NextFloatDirection() * 0.5f;
				Main.dust[num6].noGravity = true;
				Main.dust[num6].scale = 0.6f + Main.rand.NextFloat() * 0.9f;
				Main.dust[num6].fadeIn = 0.7f + Main.rand.NextFloat() * 0.8f;
				if (num6 != 6000)
				{
					Dust dust = Dust.CloneDust(num6);
					dust.scale /= 2f;
					dust.fadeIn *= 0.75f;
					dust.color = new Color(255, 255, 255, 255);
				}
			}
		}

		private static void Spawn_BlackLightningSmall(ParticleOrchestraSettings settings)
		{
			float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float num2 = Main.rand.Next(1, 3);
			float num3 = 0.7f;
			int num4 = 916;
			Main.instance.LoadProjectile(num4);
			Color value = new Color(255, 255, 255, 255);
			Color indigo = Color.Indigo;
			indigo.A = 0;
			for (float num5 = 0f; num5 < 1f; num5 += 1f / num2)
			{
				float f = (float)Math.PI * 2f * num5 + num + Main.rand.NextFloatDirection() * 0.25f;
				float num6 = Main.rand.NextFloat() * 4f + 0.1f;
				Vector2 vector = Main.rand.NextVector2Circular(12f, 12f) * num3;
				Color.Lerp(Color.Lerp(Color.Black, indigo, Main.rand.NextFloat() * 0.5f), value, Main.rand.NextFloat() * 0.6f);
				Color colorTint = new Color(0, 0, 0, 255);
				int num7 = Main.rand.Next(4);
				if (num7 == 1)
				{
					colorTint = Color.Lerp(new Color(106, 90, 205, 127), Color.Black, 0.1f + 0.7f * Main.rand.NextFloat());
				}
				if (num7 == 2)
				{
					colorTint = Color.Lerp(new Color(106, 90, 205, 60), Color.Black, 0.1f + 0.8f * Main.rand.NextFloat());
				}
				RandomizedFrameParticle randomizedFrameParticle = _poolRandomizedFrame.RequestParticle();
				randomizedFrameParticle.SetBasicInfo(TextureAssets.Projectile[num4], null, Vector2.Zero, vector);
				randomizedFrameParticle.SetTypeInfo(Main.projFrames[num4], 2, 24f);
				randomizedFrameParticle.Velocity = f.ToRotationVector2() * num6 * new Vector2(1f, 0.5f) * 0.2f + settings.MovementVector;
				randomizedFrameParticle.ColorTint = colorTint;
				randomizedFrameParticle.LocalPosition = settings.PositionInWorld + vector;
				randomizedFrameParticle.Rotation = randomizedFrameParticle.Velocity.ToRotation();
				randomizedFrameParticle.Scale = Vector2.One * 0.5f;
				randomizedFrameParticle.FadeInNormalizedTime = 0.01f;
				randomizedFrameParticle.FadeOutNormalizedTime = 0.5f;
				randomizedFrameParticle.ScaleVelocity = new Vector2(0.025f);
				Main.ParticleSystem_World_OverPlayers.Add(randomizedFrameParticle);
			}
		}

		private static void Spawn_BlackLightningHit(ParticleOrchestraSettings settings)
		{
			float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float num2 = 7f;
			float num3 = 0.7f;
			int num4 = 916;
			Main.instance.LoadProjectile(num4);
			Color value = new Color(255, 255, 255, 255);
			Color indigo = Color.Indigo;
			indigo.A = 0;
			for (float num5 = 0f; num5 < 1f; num5 += 1f / num2)
			{
				float num6 = (float)Math.PI * 2f * num5 + num + Main.rand.NextFloatDirection() * 0.25f;
				float num7 = Main.rand.NextFloat() * 4f + 0.1f;
				Vector2 vector = Main.rand.NextVector2Circular(12f, 12f) * num3;
				Color.Lerp(Color.Lerp(Color.Black, indigo, Main.rand.NextFloat() * 0.5f), value, Main.rand.NextFloat() * 0.6f);
				Color colorTint = new Color(0, 0, 0, 255);
				int num8 = Main.rand.Next(4);
				if (num8 == 1)
				{
					colorTint = Color.Lerp(new Color(106, 90, 205, 127), Color.Black, 0.1f + 0.7f * Main.rand.NextFloat());
				}
				if (num8 == 2)
				{
					colorTint = Color.Lerp(new Color(106, 90, 205, 60), Color.Black, 0.1f + 0.8f * Main.rand.NextFloat());
				}
				RandomizedFrameParticle randomizedFrameParticle = _poolRandomizedFrame.RequestParticle();
				randomizedFrameParticle.SetBasicInfo(TextureAssets.Projectile[num4], null, Vector2.Zero, vector);
				randomizedFrameParticle.SetTypeInfo(Main.projFrames[num4], 2, 24f);
				randomizedFrameParticle.Velocity = num6.ToRotationVector2() * num7 * new Vector2(1f, 0.5f);
				randomizedFrameParticle.ColorTint = colorTint;
				randomizedFrameParticle.LocalPosition = settings.PositionInWorld + vector;
				randomizedFrameParticle.Rotation = num6;
				randomizedFrameParticle.Scale = Vector2.One;
				randomizedFrameParticle.FadeInNormalizedTime = 0.01f;
				randomizedFrameParticle.FadeOutNormalizedTime = 0.5f;
				randomizedFrameParticle.ScaleVelocity = new Vector2(0.05f);
				Main.ParticleSystem_World_OverPlayers.Add(randomizedFrameParticle);
			}
		}

		private static void Spawn_StellarTune(ParticleOrchestraSettings settings)
		{
			float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float num2 = 5f;
			Vector2 vector = new Vector2(0.7f);
			for (float num3 = 0f; num3 < 1f; num3 += 1f / num2)
			{
				float num4 = (float)Math.PI * 2f * num3 + num + Main.rand.NextFloatDirection() * 0.25f;
				Vector2 vector2 = 1.5f * vector;
				float num5 = 60f;
				Vector2 vector3 = Main.rand.NextVector2Circular(12f, 12f) * vector;
				Color colorTint = Color.Lerp(Color.Gold, Color.HotPink, Main.rand.NextFloat());
				if (Main.rand.Next(2) == 0)
				{
					colorTint = Color.Lerp(Color.Violet, Color.HotPink, Main.rand.NextFloat());
				}
				PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				prettySparkleParticle.Velocity = num4.ToRotationVector2() * vector2;
				prettySparkleParticle.AccelerationPerFrame = num4.ToRotationVector2() * -(vector2 / num5);
				prettySparkleParticle.ColorTint = colorTint;
				prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector3;
				prettySparkleParticle.Rotation = num4;
				prettySparkleParticle.Scale = vector * (Main.rand.NextFloat() * 0.8f + 0.2f);
				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			}
			num2 = 1f;
		}

		private static void Spawn_Keybrand(ParticleOrchestraSettings settings)
		{
			float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float num2 = 3f;
			Vector2 vector = new Vector2(0.7f);
			for (float num3 = 0f; num3 < 1f; num3 += 1f / num2)
			{
				float num4 = (float)Math.PI * 2f * num3 + num + Main.rand.NextFloatDirection() * 0.1f;
				Vector2 vector2 = 1.5f * vector;
				float num5 = 60f;
				Vector2 vector3 = Main.rand.NextVector2Circular(4f, 4f) * vector;
				PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				prettySparkleParticle.Velocity = num4.ToRotationVector2() * vector2;
				prettySparkleParticle.AccelerationPerFrame = num4.ToRotationVector2() * -(vector2 / num5);
				prettySparkleParticle.ColorTint = Color.Lerp(Color.Gold, Color.OrangeRed, Main.rand.NextFloat());
				prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector3;
				prettySparkleParticle.Rotation = num4;
				prettySparkleParticle.Scale = vector * 0.8f;
				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			}
			num += 1f / num2 / 2f * ((float)Math.PI * 2f);
			num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			for (float num6 = 0f; num6 < 1f; num6 += 1f / num2)
			{
				float num7 = (float)Math.PI * 2f * num6 + num + Main.rand.NextFloatDirection() * 0.1f;
				Vector2 vector4 = 1f * vector;
				float num8 = 30f;
				Color value = Color.Lerp(Color.Gold, Color.OrangeRed, Main.rand.NextFloat());
				value = Color.Lerp(Color.White, value, 0.5f);
				value.A = 0;
				Vector2 vector5 = Main.rand.NextVector2Circular(4f, 4f) * vector;
				FadingParticle fadingParticle = _poolFading.RequestParticle();
				fadingParticle.SetBasicInfo(TextureAssets.Extra[98], null, Vector2.Zero, Vector2.Zero);
				fadingParticle.SetTypeInfo(num8);
				fadingParticle.Velocity = num7.ToRotationVector2() * vector4;
				fadingParticle.AccelerationPerFrame = num7.ToRotationVector2() * -(vector4 / num8);
				fadingParticle.ColorTint = value;
				fadingParticle.LocalPosition = settings.PositionInWorld + num7.ToRotationVector2() * vector4 * vector * num8 * 0.2f + vector5;
				fadingParticle.Rotation = num7 + (float)Math.PI / 2f;
				fadingParticle.FadeInNormalizedTime = 0.3f;
				fadingParticle.FadeOutNormalizedTime = 0.4f;
				fadingParticle.Scale = new Vector2(0.5f, 1.2f) * 0.8f * vector;
				Main.ParticleSystem_World_OverPlayers.Add(fadingParticle);
			}
			num2 = 1f;
			num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			for (float num9 = 0f; num9 < 1f; num9 += 1f / num2)
			{
				float num10 = (float)Math.PI * 2f * num9 + num;
				float typeInfo = 30f;
				Color colorTint = Color.Lerp(Color.CornflowerBlue, Color.White, Main.rand.NextFloat());
				colorTint.A = 127;
				Vector2 vector6 = Main.rand.NextVector2Circular(4f, 4f) * vector;
				Vector2 vector7 = Main.rand.NextVector2Square(0.7f, 1.3f);
				FadingParticle fadingParticle2 = _poolFading.RequestParticle();
				fadingParticle2.SetBasicInfo(TextureAssets.Extra[174], null, Vector2.Zero, Vector2.Zero);
				fadingParticle2.SetTypeInfo(typeInfo);
				fadingParticle2.ColorTint = colorTint;
				fadingParticle2.LocalPosition = settings.PositionInWorld + vector6;
				fadingParticle2.Rotation = num10 + (float)Math.PI / 2f;
				fadingParticle2.FadeInNormalizedTime = 0.1f;
				fadingParticle2.FadeOutNormalizedTime = 0.4f;
				fadingParticle2.Scale = new Vector2(0.1f, 0.1f) * vector;
				fadingParticle2.ScaleVelocity = vector7 * 1f / 60f;
				fadingParticle2.ScaleAcceleration = vector7 * -0.0166666675f / 60f;
				Main.ParticleSystem_World_OverPlayers.Add(fadingParticle2);
			}
		}

		private static void Spawn_FlameWaders(ParticleOrchestraSettings settings)
		{
			float num = 60f;
			for (int i = -1; i <= 1; i++)
			{
				int num2 = Main.rand.NextFromList(new short[3] { 326, 327, 328 });
				Main.instance.LoadProjectile(num2);
				Player player = Main.player[settings.IndexOfPlayerWhoInvokedThis];
				float num3 = Main.rand.NextFloat() * 0.9f + 0.1f;
				Vector2 vector = settings.PositionInWorld + new Vector2((float)i * 5.33333349f, 0f);
				FlameParticle flameParticle = _poolFlame.RequestParticle();
				flameParticle.SetBasicInfo(TextureAssets.Projectile[num2], null, Vector2.Zero, vector);
				flameParticle.SetTypeInfo(num, settings.IndexOfPlayerWhoInvokedThis, player.cShoe);
				flameParticle.FadeOutNormalizedTime = 0.4f;
				flameParticle.ScaleAcceleration = Vector2.One * num3 * -0.0166666675f / num;
				flameParticle.Scale = Vector2.One * num3;
				Main.ParticleSystem_World_BehindPlayers.Add(flameParticle);
				if (Main.rand.Next(16) == 0)
				{
					Dust dust = Dust.NewDustDirect(vector, 4, 4, 6, 0f, 0f, 100);
					if (Main.rand.Next(2) == 0)
					{
						dust.noGravity = true;
						dust.fadeIn = 1.15f;
					}
					else
					{
						dust.scale = 0.6f;
					}
					dust.velocity *= 0.6f;
					dust.velocity.Y -= 1.2f;
					dust.noLight = true;
					dust.position.Y -= 4f;
					dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
				}
			}
		}

		private static void Spawn_WallOfFleshGoatMountFlames(ParticleOrchestraSettings settings)
		{
			float num = 50f;
			for (int i = -1; i <= 1; i++)
			{
				int num2 = Main.rand.NextFromList(new short[3] { 326, 327, 328 });
				Main.instance.LoadProjectile(num2);
				Player player = Main.player[settings.IndexOfPlayerWhoInvokedThis];
				float num3 = Main.rand.NextFloat() * 0.9f + 0.1f;
				Vector2 vector = settings.PositionInWorld + new Vector2((float)i * 5.33333349f, 0f);
				FlameParticle flameParticle = _poolFlame.RequestParticle();
				flameParticle.SetBasicInfo(TextureAssets.Projectile[num2], null, Vector2.Zero, vector);
				flameParticle.SetTypeInfo(num, settings.IndexOfPlayerWhoInvokedThis, player.cMount);
				flameParticle.FadeOutNormalizedTime = 0.3f;
				flameParticle.ScaleAcceleration = Vector2.One * num3 * -0.0166666675f / num;
				flameParticle.Scale = Vector2.One * num3;
				Main.ParticleSystem_World_BehindPlayers.Add(flameParticle);
				if (Main.rand.Next(8) == 0)
				{
					Dust dust = Dust.NewDustDirect(vector, 4, 4, 6, 0f, 0f, 100);
					if (Main.rand.Next(2) == 0)
					{
						dust.noGravity = true;
						dust.fadeIn = 1.15f;
					}
					else
					{
						dust.scale = 0.6f;
					}
					dust.velocity *= 0.6f;
					dust.velocity.Y -= 1.2f;
					dust.noLight = true;
					dust.position.Y -= 4f;
					dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMount, player);
				}
			}
		}
	}
}
