using Microsoft.Xna.Framework;

namespace Terraria.DataStructures
{
	public abstract class AProjectileSource_Tile : IProjectileSource
	{
		public readonly Point TileCoords;

		public AProjectileSource_Tile(int tileCoordsX, int tileCoordsY)
		{
			TileCoords = new Point(tileCoordsX, tileCoordsY);
		}
	}
}
