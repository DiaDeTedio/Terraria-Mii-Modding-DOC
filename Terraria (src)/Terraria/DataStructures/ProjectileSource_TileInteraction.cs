namespace Terraria.DataStructures
{
	public class ProjectileSource_TileInteraction : AProjectileSource_Tile
	{
		public readonly Player Player;

		public ProjectileSource_TileInteraction(Player player, int tileCoordsX, int tileCoordsY)
			: base(tileCoordsX, tileCoordsY)
		{
			Player = player;
		}
	}
}
