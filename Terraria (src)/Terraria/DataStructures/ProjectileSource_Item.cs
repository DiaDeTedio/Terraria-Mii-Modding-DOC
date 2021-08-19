namespace Terraria.DataStructures
{
	public class ProjectileSource_Item : IProjectileSource
	{
		public readonly Player Player;

		public readonly Item Item;

		public ProjectileSource_Item(Player player, Item item)
		{
			Player = player;
			Item = item;
		}
	}
}
