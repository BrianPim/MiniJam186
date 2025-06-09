using Enemies;
using UnityEngine;

namespace Projectiles
{
    public class EnemyProjectile : MonoBehaviour
    {
        public int ProjectileDamage = 1;
        public float ProjectileSpeed = 10;
        public float Lifetime = 5;

        public Rigidbody2D RigidBody;
    
        protected Vector2 Direction;
        protected bool Active;

        private float Elapsed;
    
        public void Setup(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
            
            Direction = direction;
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
            var player = collision.GetComponent<PlayerController>();

            if (player == null || !Active || player.IsDead || player.IsDodging)
                return;

            OnHit();
        }

        protected virtual void OnHit()
        {
            GameManager.Instance.Player.TakeDamage(ProjectileDamage);
            Destroy(gameObject);
        }
    }
}