using Enemies;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileLaser : Projectile
    {
        public ParticleSystem HitParticle;
        
        protected override void OnHit(EnemyController enemy)
        {
            base.OnHit(enemy);
            
            enemy.TakeDamage(ProjectileDamage * DamageModifier, Element.Normal, GameManager.Instance.GetElementColor(Element.Normal));

            HitParticle.Play();
            DoDestroy();
        }
    }
}