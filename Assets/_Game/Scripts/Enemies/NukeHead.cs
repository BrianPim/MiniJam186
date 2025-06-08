using UnityEngine;
using System.Collections;

namespace Enemies
{
    public class NukeHead : EnemyBehaviour
    {
        [SerializeField] private float ExplosionCircleCastRadius = 2f;
        [SerializeField] private float ExplosionDamageToFriendlies = 50f;
        
        public override void Awake()
        {
            base.Awake();

            Controller.EnemySpawningComplete();
        }

        public override void Update()
        {
            base.Update();

            Controller.SetOverrideDestination(GameManager.Instance.Player.transform.position);

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

            Controller.BlockActions = true;
            //Controller.Animator.SetTrigger("shoot");
            //Controller.SfxShoot.Play();
        
            StartCoroutine(CoExplode());

            IEnumerator CoExplode()
            {
                yield return new WaitForSeconds(.1f);
                
                RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, ExplosionCircleCastRadius, Vector3.right, 0);

                foreach (var hit in results)
                {
                    if (!hit.collider) continue;

                    hit.transform.gameObject.TryGetComponent<EnemyController>(out var enemy);

                    if (enemy)
                    {
                        var element = Controller.GetElement();
                        enemy.TakeDamage(ExplosionDamageToFriendlies, element, GameManager.Instance.GetElementColor(element),1.5f);
                    }
                    else if (hit.collider.gameObject == GameManager.Instance.Player.gameObject)
                    {
                        GameManager.Instance.Player.TakeDamage(1);
                    }
                }
                
                yield return new WaitForSeconds(.3f);

                if (Controller.IsBeingDestroyed())
                    yield break;

                Destroy(gameObject);
            }
        }
    }
}