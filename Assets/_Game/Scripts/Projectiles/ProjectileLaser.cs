using Enemies;
using UnityEditor.Graphs;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileLaser : Projectile
    {
        protected override void OnHit(EnemyController enemy)
        {
            enemy.TakeDamage(ProjectileDamage * DamageModifier, Color.white);
            Destroy(gameObject);
        }
    }
}