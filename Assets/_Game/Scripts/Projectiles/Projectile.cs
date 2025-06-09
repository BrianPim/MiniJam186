using Enemies;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public float ProjectileDamage = 10;
        public float ProjectileSpeed = 10;
        public float Lifetime = 5;

        public Rigidbody2D RigidBody;
    
        protected float DamageModifier;
        protected Vector2 Direction;
        protected bool Active;

        private float Elapsed;
    
        public void Setup(Vector2 direction, float damageModifier)
        {
            Direction = direction;
            DamageModifier = damageModifier;
            Active = true;
        }

        public void Update()
        {
            if (Elapsed > Lifetime)
            {
                Destroy(gameObject);
            }
            else
            {
                Elapsed += Time.deltaTime;
            }
        }

        public void FixedUpdate()
        {
            if (!Active)
                return;

            RigidBody.linearVelocity = Direction.normalized * ProjectileSpeed;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<EnemyController>();

            if (enemy == null || !Active)
                return;

            if (Destroying || enemy.IsBeingDestroyed()) return;
            
            OnHit(enemy);
        }

        protected virtual void OnHit(EnemyController enemy)
        {
            
        }

        private bool Destroying;
        public void DoDestroy()
        {
            Destroying = true;
            Active = false;

            RigidBody.linearVelocity = Vector2.zero;
            
            foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>())
            {
                rend.enabled = false;
            }
            
            Destroy(gameObject, 1);
        }
    }
}