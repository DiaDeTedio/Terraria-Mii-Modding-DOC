namespace Terraria.DataStructures
{
	public class ProjectileSource_Buff : IProjectileSource
	{
		public readonly Player Player;

		public readonly int BuffId;

		public readonly int BuffIndex;

		public ProjectileSource_Buff(Player player, int buffId, int buffIndex)
		{
			Player = player;
			BuffId = buffId;
			BuffIndex = buffIndex;
		}
	}
}
