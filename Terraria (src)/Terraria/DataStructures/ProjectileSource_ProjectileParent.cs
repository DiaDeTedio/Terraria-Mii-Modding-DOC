namespace Terraria.DataStructures
{
	public class ProjectileSource_ProjectileParent : IProjectileSource
	{
		public readonly Projectile ParentProjectile;

		public ProjectileSource_ProjectileParent(Projectile projectile)
		{
			ParentProjectile = projectile;
		}
	}
}
