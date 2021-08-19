namespace Terraria.DataStructures
{
	public class ProjectileSource_NPC : IProjectileSource
	{
		public readonly NPC NPC;

		public ProjectileSource_NPC(NPC npc)
		{
			NPC = npc;
		}
	}
}
