using Enemies;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileLaser : Projectile
    {
        public ParticleSystem HitParticle;
        
        protected override void OnHit(EnemyController enemy)
        {
            enemy.TakeDamage(ProjectileDamage * DamageModifier, Element.Normal, Color.white);

            HitParticle.Play();
            DoDestroy();
        }
    }
}