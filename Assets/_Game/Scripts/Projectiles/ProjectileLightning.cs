using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileLightning : Projectile
    {
        [SerializeField] private float HitElectricEnemyModifier = 0.5f;
        
        private List<EnemyController> IllegalTargets = new List<EnemyController>();
        private int Chain;
        private int EnemiesHit;
    
        public void Setup(Vector2 direction, float damageModifier, int chain)
        {
            Direction = direction;
            DamageModifier = damageModifier;
            Active = true;
            Chain = chain;
        }

        protected override void OnHit(EnemyController enemy)
        {
            base.OnHit(enemy);
            
            if (IllegalTargets.Contains(enemy)) return;
            
            if (enemy.GetElement() == Element.Electric)
            {
                enemy.TakeDamage(ProjectileDamage * DamageModifier * HitElectricEnemyModifier, Element.Electric, Color.gray);
                
                Destroy(gameObject);
            }
            else
            {
                if (EnemiesHit < Chain && GameManager.Instance.Enemies.Count > 1)
                {
                    IllegalTargets.Add(enemy);
                    var newEnemy = GetNearestEnemy();
                
                    Direction = newEnemy.transform.position - transform.position;
                
                    EnemiesHit++;
                }
                else
                {
                    Destroy(gameObject);
                }
                
                enemy.TakeDamage(ProjectileDamage * DamageModifier, Element.Electric, GameManager.Instance.GetElementColor(Element.Electric));
            }
        }
        
        //Returns the closest valid enemy target.
        private EnemyController GetNearestEnemy()
        {
            EnemyController nearest = null;

            if (IllegalTargets.Count == GameManager.Instance.Enemies.Count)
            {
                IllegalTargets.Clear();
            }

            foreach (var enemy in GameManager.Instance.Enemies)
            {
                if (IllegalTargets.Contains(enemy)) continue;
                
                if (nearest == null || Vector2.Distance(enemy.transform.position, transform.position) < Vector2.Distance(nearest.transform.position, transform.position))
                {
                    nearest = enemy;
                }
            }

            return nearest;
        }
    }
}