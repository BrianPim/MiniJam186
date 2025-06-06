using Enemies;

namespace Projectiles
{
    public class ProjectileLaser : Projectile
    {
        protected override void OnHit(EnemyController enemy)
        {
            enemy.TakeDamage(ProjectileDamage * DamageModifier);
            Destroy(gameObject);
        }
    }
}