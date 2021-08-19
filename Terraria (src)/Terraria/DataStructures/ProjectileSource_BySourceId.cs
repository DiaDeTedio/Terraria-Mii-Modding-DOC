namespace Terraria.DataStructures
{
	public class ProjectileSource_BySourceId : IProjectileSource
	{
		public readonly int SourceId;

		public ProjectileSource_BySourceId(int projectileSourceId)
		{
			SourceId = projectileSourceId;
		}
	}
}
