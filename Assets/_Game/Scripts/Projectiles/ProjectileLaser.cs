using Enemies;
using UnityEditor.Graphs;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileLaser : Projectile
    {
        protected override void OnHit(EnemyController enemy)
        {
            enemy.TakeDamage(ProjectileDamage * DamageModifier, Element.Normal, Color.white);
            Destroy(gameObject);
        }
    }
}