namespace Terraria.DataStructures
{
	public class ProjectileSource_Mount : IProjectileSource
	{
		public readonly Player Player;

		public readonly int MountId;

		public ProjectileSource_Mount(Player player, int mountId)
		{
			Player = player;
			MountId = mountId;
		}
	}
}
