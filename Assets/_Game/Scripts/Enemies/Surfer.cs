using System.Collections;
using Projectiles;
using UnityEngine;

namespace Enemies
{
    public class Surfer : EnemyBehaviour
    {
        public EnemyProjectile Projectile;

        public override void Awake()
        {
            base.Awake();

            Controller.EnemySpawningComplete();
            Controller.SetOverrideDestination(new Vector3(EnemySpawner.Instance.EnemyDespawnPoint.position.x, transform.position.y, 0));
        }

        public override void Update()
        {
            base.Update();

            if (transform.position.x <= EnemySpawner.Instance.EnemyDespawnPoint.position.x)
            {
                Destroy(gameObject);
                
                EvolutionManager.Instance.Increment(Element.Normal);
                EnemySpawner.Instance.AddDifficulty();
                EnemyDirector.Instance.EnemyKilled();
            }
        }

        public override void DoAction()
        {
            base.DoAction();
            
            //Controller.Animator.SetTrigger("shoot");
            //Controller.SfxShoot.Play();
        
            StartCoroutine(CoCastProjectile());

            IEnumerator CoCastProjectile()
            {
                yield return new WaitForSeconds(.3f);

                if (Controller.IsBeingDestroyed())
                    yield break;

                var laser = Instantiate(Projectile);
                laser.transform.position = Controller.ProjectileSpawnPosition.position;

                var direction = GameManager.Instance.Player.transform.position - Controller.ProjectileSpawnPosition.position;

                laser.Setup(direction.normalized);
            }
        }
    }
}