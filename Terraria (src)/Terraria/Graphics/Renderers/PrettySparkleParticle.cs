using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Terraria.Graphics.Renderers
{
	public class PrettySparkleParticle : ABasicParticle
	{
		public Color ColorTint;

		public float Opacity;

		private float _timeSinceSpawn;

		public override void FetchFromPool()
		{
			base.FetchFromPool();
			ColorTint = Color.Transparent;
			_timeSinceSpawn = 0f;
			Opacity = 0f;
		}

		public override void Update(ref ParticleRendererSettings settings)
		{
			base.Update(ref settings);
			_timeSinceSpawn += 1f;
			Opacity = Utils.GetLerpValue(0f, 0.05f, _timeSinceSpawn / 60f, clamped: true) * Utils.GetLerpValue(1f, 0.9f, _timeSinceSpawn / 60f, clamped: true);
			if (_timeSinceSpawn >= 60f)
			{
				base.ShouldBeRemovedFromRenderer = true;
			}
		}

		public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
		{
			Color color = Color.White * Opacity * 0.9f;
			color.A /= 2;
			Texture2D value = TextureAssets.Extra[98].get_Value();
			Color color2 = ColorTint * Opacity * 0.5f;
			color2.A = 0;
			Vector2 origin = value.Size() / 2f;
			Color color3 = color * 0.5f;
			float num = Utils.GetLerpValue(0f, 20f, _timeSinceSpawn, clamped: true) * Utils.GetLerpValue(45f, 30f, _timeSinceSpawn, clamped: true);
			Vector2 vector = new Vector2(0.3f, 2f) * num * Scale;
			Vector2 vector2 = new Vector2(0.3f, 1f) * num * Scale;
			color2 *= num;
			color3 *= num;
			Vector2 position = settings.AnchorPosition + LocalPosition;
			SpriteEffects effects = SpriteEffects.None;
			spritebatch.Draw(value, position, null, color2, (float)Math.PI / 2f + Rotation, origin, vector, effects, 0f);
			spritebatch.Draw(value, position, null, color2, 0f + Rotation, origin, vector2, effects, 0f);
			spritebatch.Draw(value, position, null, color3, (float)Math.PI / 2f + Rotation, origin, vector * 0.6f, effects, 0f);
			spritebatch.Draw(value, position, null, color3, 0f + Rotation, origin, vector2 * 0.6f, effects, 0f);
		}
	}
}
