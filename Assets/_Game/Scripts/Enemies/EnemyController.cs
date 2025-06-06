using System.Linq;
using UnityEngine;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        public float Health;

        protected virtual void Awake()
        {
            if (GameManager.Instance && !GameManager.Instance.Enemies.Contains(this))
                GameManager.Instance.Enemies.Add(this);
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance && GameManager.Instance.Enemies.Contains(this))
                GameManager.Instance.Enemies.Remove(this);
        }
    }
}
