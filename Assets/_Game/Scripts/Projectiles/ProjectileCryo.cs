using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileCryo : Projectile
    {
        [SerializeField] private float HitIceEnemyModifier = 0.5f;

        private float FrozenSlowModifier;
        
        public void Setup(Vector2 direction, float damageModifier, float frozenModifier)
        {
            Direction = direction;
            DamageModifier = damageModifier;
            Active = true;
            FrozenSlowModifier = frozenModifier;
        }
        
        protected override void OnHit(EnemyController enemy)
        {
            if (enemy.GetElement() == Element.Ice)
            {
                enemy.TakeDamage(ProjectileDamage * DamageModifier * HitIceEnemyModifier, Element.Ice, Color.gray);
            }
            else
            {
                enemy.TakeDamage(ProjectileDamage * DamageModifier, Element.Ice, GameManager.Instance.GetElementColor(Element.Ice));
                enemy.HitByCryo(FrozenSlowModifier);
            }
            
            Destroy(gameObject);
        }
    }
}