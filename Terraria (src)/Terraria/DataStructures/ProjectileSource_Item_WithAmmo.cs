namespace Terraria.DataStructures
{
	public class ProjectileSource_Item_WithAmmo : ProjectileSource_Item
	{
		public readonly int AmmoItemIdUsed;

		public ProjectileSource_Item_WithAmmo(Player player, Item item, int ammoItemIdUsed)
			: base(player, item)
		{
			AmmoItemIdUsed = ammoItemIdUsed;
		}
	}
}
